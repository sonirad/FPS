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
    [Tooltip("���� ����ϰ� �ִ� ��")]
    private GunBase activeGun;
    [Tooltip("�⺻ ��(������)")]
    private GunBase defaultGun;

    private void Awake()
    {
        starterAssets = GetComponent<StarterAssetsInputs>();
        controller = GetComponent <FirstPersonController>();
        gunCamera = transform.GetChild(2).gameObject;
        Transform child = transform.GetChild(3);
        // ��� �� ã��
        guns = child.GetComponentsInChildren<GunBase>(true);

        foreach (GunBase gun in guns)
        {
            gun.onFire += controller.FireRecoil;
        }

        // �⺻ ��
        defaultGun = guns[0];
    }

    private void Start()
    {
        starterAssets.onZoom += DisableGunCamera;      // �� �� �� ����� �Լ� ����

        // �⺻ �� ����
        activeGun = defaultGun;
        // �⺻ �� ���
        activeGun.Equip();
    }

    /// <summary>
    /// �� ǥ���ϴ� ī�޶� Ȱ��ȭ ����
    /// </summary>
    /// <param name="enable">t : ��Ȱ��ȭ(���� �Ⱥ��δ�), f : Ȱ��ȭ(���� ���δ�)</param>
    private void DisableGunCamera(bool disable = true)
    {
        gunCamera.SetActive(!disable);
    }

    /// <summary>
    /// ��� ���� ���� ����
    /// </summary>
    /// <param name="gunType"></param>
    public void GunChange(GunType gunType)
    {
        // ���� �� ��Ȱ��ȭ�ϰ� ��� ��ü
        activeGun.gameObject.SetActive(false);
        activeGun.UnEquip();

        // �� �� ����ϰ� Ȱ��ȭ
        activeGun = guns[(int)gunType];
        activeGun.Equip();
        activeGun.gameObject.SetActive(true);
    }

    /// <summary>
    /// ��� ���� ���� �߻�. ���� �̿ϼ�
    /// </summary>
    /// <param name="isFireStart">t : �߻��ư ������, f : �߻��ư ����.</param>
    public void GunFire(bool isFireStart)
    {
        activeGun.Fire(isFireStart);
    }

    public void RevolverReload()
    {
        Revolver revolver = activeGun as Revolver;

        if (revolver != null)
        {
            revolver.ReLoad();
        }
    }
}
