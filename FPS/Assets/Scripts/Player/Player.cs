using StarterAssets;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("����Ƽ�� �����ϴ� ���ۿ� �ڵ�(�Է�ó���� �Լ��� ��� ���� Ŭ����)")]
    private StarterAssetsInputs starterAssets;
    [Tooltip("����Ƽ�� �����ϴ� �Է� ó���� �ڵ�")]
    private FirstPersonController controller;
    [Tooltip("�Ѹ� �Կ��ϴ� ī�޶� �ִ� ���� ������Ʈ")]
    private GameObject gunCamera;
    [Tooltip("���� ī�޶� �������� �߻�")]
    public Transform FireTransform => transform.GetChild(0);    // ī�޶� ��Ʈ
    [Tooltip("�÷��̾ ����� �� �ִ� ��� ��")]
    private GunBase[] guns;

    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        controller = GetComponent <FirstPersonController>();
        gunCamera = transform.GetChild(2).gameObject;
        Transform child = transform.GetChild(3);
        // ��� �� ã��
        guns = child.GetComponentsInChildren<GunBase>();

        foreach (GunBase gun in guns)
        {
            gun.onFire += controller.FireRecoil;
        }
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
