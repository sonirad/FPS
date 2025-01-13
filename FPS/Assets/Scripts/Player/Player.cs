using StarterAssets;
using UnityEngine;
using System;

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
    [Tooltip("기본 총(리볼버)")]
    private GunBase defaultGun;

    [Tooltip("총이 변경되었음을 알리는 델리게이트")]
    public Action<GunBase> onGunChange;

    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        controller = GetComponent <FirstPersonController>();
        gunCamera = transform.GetChild(2).gameObject;
        Transform child = transform.GetChild(3);
        // 모든 총 찾기
        guns = child.GetComponentsInChildren<GunBase>(true);

        // 기본 총
        defaultGun = guns[0];
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
            gun.onAmmoDepleted += () => GunChange(GunType.Revolver);
        }

        // 기본 총 설정
        activeGun = defaultGun;
        // 기본 총 장비
        activeGun.Equip();
        // 총 변경 알림
        onGunChange?.Invoke(activeGun);
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
}
