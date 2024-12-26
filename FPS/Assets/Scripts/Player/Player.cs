using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("유니티가 제공하는 시작용 코드(입력처리용 함수를 모아 놓은 클래스)")]
    private StarterAssetsInputs starterAssets;
    [Tooltip("총만 촬영하는 카메라가 있는 게임 오브젝트")]
    private GameObject gunCamera;

    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        gunCamera = transform.GetChild(2).gameObject;
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
