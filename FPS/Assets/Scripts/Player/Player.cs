using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("����Ƽ�� �����ϴ� ���ۿ� �ڵ�(�Է�ó���� �Լ��� ��� ���� Ŭ����)")]
    private StarterAssetsInputs starterAssets;
    [Tooltip("�Ѹ� �Կ��ϴ� ī�޶� �ִ� ���� ������Ʈ")]
    private GameObject gunCamera;

    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        gunCamera = transform.GetChild(2).gameObject;
    }

    private void Start()
    {
        starterAssets.onZoom += DisableGunCamera;      // �� �� �� ����� �Լ� ����
    }

    /// <summary>
    /// �� ǥ���ϴ� ī�޶� Ȱ��ȭ ����
    /// </summary>
    /// <param name="enable">t : ��Ȱ��ȭ(���� �Ⱥ��δ�), f : Ȱ��ȭ(���� ���δ�)</param>
    private void DisableGunCamera(bool disable = true)
    {
        gunCamera.SetActive(!disable);
    }
}
