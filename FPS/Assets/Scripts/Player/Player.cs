using StarterAssets;
using UnityEngine;

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

    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        controller = GetComponent <FirstPersonController>();
        gunCamera = transform.GetChild(2).gameObject;
        Transform child = transform.GetChild(3);
        // 모든 총 찾기
        guns = child.GetComponentsInChildren<GunBase>();

        foreach (GunBase gun in guns)
        {
            gun.onFire += controller.FireRecoil;
        }
    }

    private void Start()
    {
        starterAssets.onZoom += DisableGunCamera;      // 줌 할 떄 실행될 함수 연결
    }

    /// <summary>
    /// 총 표시하는 카메라 활성화 설정
    /// </summary>
    /// <param name="enable">t : 비활성화(총이 안보인다), f : 활성화(총이 보인다)</param>
    private void DisableGunCamera(bool disable = true)
    {
        gunCamera.SetActive(!disable);
    }
}
