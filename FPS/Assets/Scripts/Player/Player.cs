using StarterAssets;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Tooltip("유니티가 제공하는 시작용 코드(입력처리용 함수를 모아 놓은 클래스)")]
    private StarterAssetsInputs starterAssets;
    [Tooltip("유니티가 제공하는 입력 처리용 코드")]
    private FirstPersonController controller;
    [Tooltip("총만 촬영하는 카메라가 있는 게임 오브젝트")]
    private GameObject gunCamera;
    [Tooltip("총은 카메라 기준으로 발사")]
    public Transform FireTransform => transform.GetChild(0);    // 카메라 루트
    [Tooltip("플레이어가 장비할 수 있는 모든 총")]
    private GunBase[] guns;
    [Tooltip("현재 장비하고 있는 총")]
    private GunBase activeGun;
    [Tooltip("최대 HP")]
    public float MaxHP = 100.0f;
    [Tooltip("현재 HP")]
    private float hp;
    [Tooltip("PlayerInput 컴포넌트")]
    private PlayerInput playerInput;

    [Tooltip("총이 변경되었음을 알리는 델리게이트")]
    public Action<GunBase> onGunChange;
    [Tooltip("플레이어가 죽었을 때 실행될 델리게이트")]
    public Action onDie;
    [Tooltip("공격을 받았을 때 실행될 델리게이트(float : 공격 받은 각도. 플레이어 forward와 적으로 가는 방향 벡터 사이의 각도. 시계방향)")]
    public Action<float> onAttacked;
    [Tooltip("HP가 변경되었을 때 실행될 델리게이트(float : 현재 HP)")]
    public Action<float> onHPChange;
    [Tooltip("플레이어가 맵의 가운데 배치되었을 때 실행 될 델리게이트")]
    public Action onSpawn;

    [Tooltip("현재 HP 확인 및 설정용 프로퍼티")]
    public float HP
    {
        get => hp;
        set
        {
            hp = value;

            if (hp <= 0)
            {
                // HP가 0 이하면 사망
                Die();
            }

            // HP 최대 최소 안 벗어나게 만들기
            hp = Mathf.Clamp(hp, 0, MaxHP);

            Debug.Log($"HP : {hp}");

            // HP 변화 알리기
            onHPChange?.Invoke(hp);
        }
    }

    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        controller = GetComponent <FirstPersonController>();
        playerInput = GetComponent<PlayerInput>();
        gunCamera = transform.GetChild(2).gameObject;
        Transform child = transform.GetChild(3);
        // 모든 총 찾기
        guns = child.GetComponentsInChildren<GunBase>(true);
    }

    private void Start()
    {
        starterAssets.onZoom += DisableGunCamera;      // 줌 할 떄 실행될 함수 연결

        CrossHair crossHair = FindAnyObjectByType<CrossHair>();

        foreach (GunBase gun in guns)
        {
            // 화면 튕기는 효과
            gun.onFire += controller.FireRecoil;
            // 조준선 확장 효과
            gun.onFire += (expend) => crossHair.Expend(expend * 10);
            // 총알이 다 떨어지면 기본 총으로 변경
            gun.onAmmoDepleted += () =>
            {
                if (!(activeGun is Revolver))
                {
                    // 총알이 다 떨어지면 기본총으로 변경
                    GunChange(GunType.Revolver);
                }
            };
        }

        // 기본 총 설정
        activeGun = guns[0];
        // 기본 총 장비
        activeGun.Equip();
        // 총 변경 알림
        onGunChange?.Invoke(activeGun);

        HP = MaxHP;
        // 게임이 클리어되면 입력 막기
        GameManager.Instance.onGameEnd += (_) => InputDisable();

        Spawn();
    }

    /// <summary>
    /// 총 표시하는 카메라 활성화 설정
    /// </summary>
    /// <param name="enable">t : 비활성화(총이 안보인다), f : 활성화(총이 보인다)</param>
    private void DisableGunCamera(bool disable = true)
    {
        gunCamera.SetActive(!disable);
    }

    /// <summary>
    /// 장비 중인 총을 변경
    /// </summary>
    /// <param name="gunType"></param>
    public void GunChange(GunType gunType)
    {
        activeGun.UnEquip();
        // 이전 총 비활성화하고 장비 해체
        activeGun.gameObject.SetActive(false);

        // 새 총 장비하고 활성화
        activeGun = guns[(int)gunType];
        activeGun.Equip();
        activeGun.gameObject.SetActive(true);
        // 총 변경 알림
        onGunChange?.Invoke(activeGun);
    }

    /// <summary>
    /// 장비 중인 총을 발사.
    /// </summary>
    /// <param name="isFireStart">t : 발사버튼 눌렀다, f : 발사버튼 땠다.</param>
    public void GunFire(bool isFireStart)
    {
        activeGun.Fire(isFireStart);
    }

    /// <summary>
    /// 리볼러를 재장전
    /// </summary>
    public void RevolverReload()
    {
        Revolver revolver = activeGun as Revolver;

        // activeGun이 리볼버 일때만 재장전
        if (revolver != null)
        {
            revolver.ReLoad();
        }
    }

    /// <summary>
    /// 총알 갯수가 변경될 때 실행되는 델리게이트에 콜백 추가
    /// </summary>
    /// <param name="callback">추가할 콜백 함수</param>
    public void AddAmmoCountChangeDelegate(Action<int> callback)
    {
        foreach (var gun in guns)
        {
            gun.onAmmoCountChange += callback;
        }
    }

    /// <summary>
    /// 공격을 받았을 때 실행
    /// </summary>
    /// <param name="enemy">공격을 한 적</param>
    public void OnAttacked(Enemy enemy)
    {
        Vector3 dir = enemy.transform.position - transform.position;
        // 공격 당한 각도(시계방향)
        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        onAttacked?.Invoke(-angle);
        HP -= enemy.attackPower;
    }

    public void Spawn()
    {
        GameManager gameManager = GameManager.Instance;
        Vector3 centerPos = MazelVisualizer.GridToWorld(gameManager.MazeWidth / 2, gameManager.MazeHeight / 2);
        // 플레이어를 미로의 가온데 위치로 옮기기
        transform.position = centerPos;

        onSpawn?.Invoke();
    }

    /// <summary>
    /// 플레이어 사망 처리용
    /// </summary>
    private void Die()
    {
        // 죽었음을 알림
        onDie?.Invoke();
        // 플레이어 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 입력을 막는 함수
    /// </summary>
    private void InputDisable()
    {
        // 액션맵이 1개만 있기 때문에 그냥 처리
        playerInput.actions.actionMaps[0].Disable();
    }
}