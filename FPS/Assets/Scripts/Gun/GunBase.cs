using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.VFX;
using Unity.VisualScripting;

public enum GunType : byte
{
    Revolver = 0,
    ShotGun,
    AssaultRifle
}

public class GunBase : MonoBehaviour
{
    // 사정 거리
    // 탄창 (탄창 크기, 탄창에 남아있는 총알 수)
    // 데미지
    // 연사 속도
    // 탄 퍼짐
    // 총 반동

    [Tooltip("사정 거리")]
    public float range;
    [Tooltip("탄창 크기")]
    public int clipSize;
    [Tooltip("장전된 총알 수( == 남은 총알 수)")]
    private int bulletCount;
    [Tooltip("총의 데이미(총알 한발당 데미지)")]
    public float damage;
    [Tooltip("총의 연삿 속도(1초당 발사 수)")]
    public float fireRate;
    [Tooltip("탄 터짐 각도의 절반")]
    public float spread;
    [Tooltip("총 반동")]
    public float recoil;
    [Tooltip("현재 발사 가능한지 여부 확인")]
    protected bool isFireReady = true;
    [Tooltip("muzzle 이펙트")]
    private VisualEffect muzzleEffect;

    [Tooltip("총알이 발사되는 트랜스폼(플레이어의 카메라 위치)")]
    protected Transform fireTransform;
    [Tooltip("남은 총알 갯수가 변경되었음을 알리는 델리게이트(int : 남은 총알 갯수)")]
    public Action<int> onAmmoCountChange;
    [Tooltip("총알이 한발 발사되었음을 알리는 델리게이트(float : 반동 정도)")]
    public Action<float> onFire;
    [Tooltip("총알이 다 떨어졌음을 알리는 델리게이트")]
    public Action onAmmoDepleted;

    [Tooltip("muzzle 이펙트 발동용")]
    readonly int onFireID = Shader.PropertyToID("OnFire");


    protected int BulletCount
    {
        get => bulletCount;
        set
        {
            bulletCount = value;

            onAmmoCountChange?.Invoke(bulletCount);    // 총알 갯수가 변경되었음을 알림

            if (bulletCount < 1)
            {
                onAmmoDepleted?.Invoke();
            }
        }
    }

    private void Awake()
    {
        muzzleEffect = GetComponentInChildren<VisualEffect>();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    private void Initialize()
    {
        // 총알을 완전히 장전된 상태로 초기화
        BulletCount = clipSize;
        // 발사 가능으로 초기화
        isFireReady = true;
    }

    /// <summary>
    /// 총 발사
    /// </summary>
    public void Fire(bool isFireStart = true)
    {
        // 발사 가능하고 총알이 남아있으면
        if (isFireReady && BulletCount > 0)
        {
            // 총 발사
            FireProcess(isFireStart);
        }
    }

    /// <summary>
    /// 발사가 성공했을 때 실생할 기능들
    /// </summary>
    /// <param name="isFireStart">발사 입력이 들어오면 : t, 끝나면 : f</param>
    protected virtual void FireProcess(bool isFireStart = true)
    {
        // 계속 발사가 되지 않게 막기
        isFireReady = false;

        // 머즐 이팩트 보여줌
        MuzzleEffectOn();

        // 일정 시간 후 자동으로 발사 가능하게 설정
        StartCoroutine(FireReady());

        // 총알 갯수 감소
        BulletCount--;
    }

    /// <summary>
    /// 일정 시간 후에 isFireReady를 true로 변경 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator FireReady()
    {
        // fireLRate에 따라 기다리는 시간
        yield return new WaitForSeconds(1 / fireRate);

        isFireReady = true;
    }

    /// <summary>
    /// muzzle 이팩트 실행
    /// </summary>
    protected void MuzzleEffectOn()
    {
        muzzleEffect.SendEvent(onFireID);
    }

    /// <summary>
    /// 총이 부딪친 곳에 따른 처리
    /// </summary>
    protected void HitProcess()
    {
        // 레이 만들기
        Ray ray = new (fireTransform.position, GetFireDirection());

        // int i = ~LayerMask.GetMask("Default");       Default 레이어 빼고 체크
        if (Physics.Raycast(ray, out RaycastHit hitInfo, range, ~LayerMask.NameToLayer("Default")))      // 레이캐스트
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Enemy target = hitInfo.collider.GetComponentInParent<Enemy>();

                // 맞은 부위와 데미지 넘겨주기
                if (hitInfo.collider.CompareTag("Head"))
                {
                    target.OnAttacked(HitLocation.Head, damage);
                }
                else if (hitInfo.collider.CompareTag("Arm"))
                {
                    target.OnAttacked(HitLocation.Arm, damage);
                }
                else if (hitInfo.collider.CompareTag("Leg"))
                {
                    target.OnAttacked(HitLocation.Leg, damage);
                }
                else if (hitInfo.collider.CompareTag("Body"))
                {
                    target.OnAttacked(HitLocation.Body, damage);
                }
            }
            else
            {
                Vector3 reflect = Vector3.Reflect(ray.direction, hitInfo.normal);

                // 총알 구명 생성을 위해, 생성될 위치, 생성될 면의 노멀, 반사방향 전달
                Factory.Instance.GetBulletHole(hitInfo.point, hitInfo.normal, reflect);
            }
        }
    }

    /// <summary>
    /// 반동 알림
    /// </summary>
    protected void FireRecoil()
    {
        onFire?.Invoke(recoil);
    }

    /// <summary>
    /// 총을 장비할 때 처리
    /// </summary>
    public void Equip()
    {
        fireTransform = GameManager.Instance.Player.FireTransform;

        Initialize();
    }

    /// <summary>
    /// 총이 장비 해제 될때 처리
    /// </summary>
    public void UnEquip()
    {
        StopAllCoroutines();

        isFireReady = true;
    }

    /// <summary>
    /// 발사 각 안으로 랜덤한 발사 방향 구하기
    /// </summary>
    /// <returns>총알을 발사할 방향</returns>
    protected Vector3 GetFireDirection()
    {
        Vector3 result = fireTransform.forward;

        // 위 아래로 -spread ~ spread 만큼 회전(x축 기준으로 회전)
        result = Quaternion.AngleAxis(UnityEngine.Random.Range(-spread, spread), fireTransform.right) * result;
        // fireTransform.forward를 축으로 삼아 0 ~ 360도 회전
        result = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), fireTransform.forward) * result;

        return result;
    }

#if UNITY_EDITOR
    public void Test_Fire(bool isFireStart = true)
    {
        if (fireTransform == null)
        {
            Equip();
            Debug.Log("발사 준비");
        }

        Fire(isFireStart);
        Debug.Log("발사");
    }

    private void OnDrawGizmos()
    {
        if (fireTransform != null)
        {
            Gizmos.color = Color.white;

            Gizmos.DrawLine(fireTransform.position, fireTransform.position + fireTransform.forward * range);
        }
    }

#endif
}
