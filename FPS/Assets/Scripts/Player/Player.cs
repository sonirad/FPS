using StarterAssets;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

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
    [Tooltip("�ִ� HP")]
    public float MaxHP = 100.0f;
    [Tooltip("���� HP")]
    private float hp;
    [Tooltip("PlayerInput ������Ʈ")]
    private PlayerInput playerInput;

    [Tooltip("���� ����Ǿ����� �˸��� ��������Ʈ")]
    public Action<GunBase> onGunChange;
    [Tooltip("�÷��̾ �׾��� �� ����� ��������Ʈ")]
    public Action onDie;
    [Tooltip("������ �޾��� �� ����� ��������Ʈ(float : ���� ���� ����. �÷��̾� forward�� ������ ���� ���� ���� ������ ����. �ð����)")]
    public Action<float> onAttacked;
    [Tooltip("HP�� ����Ǿ��� �� ����� ��������Ʈ(float : ���� HP)")]
    public Action<float> onHPChange;
    [Tooltip("�÷��̾ ���� ��� ��ġ�Ǿ��� �� ���� �� ��������Ʈ")]
    public Action onSpawn;

    [Tooltip("���� HP Ȯ�� �� ������ ������Ƽ")]
    public float HP
    {
        get => hp;
        set
        {
            hp = value;

            if (hp <= 0)
            {
                // HP�� 0 ���ϸ� ���
                Die();
            }

            // HP �ִ� �ּ� �� ����� �����
            hp = Mathf.Clamp(hp, 0, MaxHP);

            Debug.Log($"HP : {hp}");

            // HP ��ȭ �˸���
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
        // ��� �� ã��
        guns = child.GetComponentsInChildren<GunBase>(true);
    }

    private void Start()
    {
        starterAssets.onZoom += DisableGunCamera;      // �� �� �� ����� �Լ� ����

        CrossHair crossHair = FindAnyObjectByType<CrossHair>();

        foreach (GunBase gun in guns)
        {
            // ȭ�� ƨ��� ȿ��
            gun.onFire += controller.FireRecoil;
            // ���ؼ� Ȯ�� ȿ��
            gun.onFire += (expend) => crossHair.Expend(expend * 10);
            // �Ѿ��� �� �������� �⺻ ������ ����
            gun.onAmmoDepleted += () =>
            {
                if (!(activeGun is Revolver))
                {
                    // �Ѿ��� �� �������� �⺻������ ����
                    GunChange(GunType.Revolver);
                }
            };
        }

        // �⺻ �� ����
        activeGun = guns[0];
        // �⺻ �� ���
        activeGun.Equip();
        // �� ���� �˸�
        onGunChange?.Invoke(activeGun);

        HP = MaxHP;
        // ������ Ŭ����Ǹ� �Է� ����
        GameManager.Instance.onGameEnd += (_) => InputDisable();

        Spawn();
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
        activeGun.UnEquip();
        // ���� �� ��Ȱ��ȭ�ϰ� ��� ��ü
        activeGun.gameObject.SetActive(false);

        // �� �� ����ϰ� Ȱ��ȭ
        activeGun = guns[(int)gunType];
        activeGun.Equip();
        activeGun.gameObject.SetActive(true);
        // �� ���� �˸�
        onGunChange?.Invoke(activeGun);
    }

    /// <summary>
    /// ��� ���� ���� �߻�.
    /// </summary>
    /// <param name="isFireStart">t : �߻��ư ������, f : �߻��ư ����.</param>
    public void GunFire(bool isFireStart)
    {
        activeGun.Fire(isFireStart);
    }

    /// <summary>
    /// �������� ������
    /// </summary>
    public void RevolverReload()
    {
        Revolver revolver = activeGun as Revolver;

        // activeGun�� ������ �϶��� ������
        if (revolver != null)
        {
            revolver.ReLoad();
        }
    }

    /// <summary>
    /// �Ѿ� ������ ����� �� ����Ǵ� ��������Ʈ�� �ݹ� �߰�
    /// </summary>
    /// <param name="callback">�߰��� �ݹ� �Լ�</param>
    public void AddAmmoCountChangeDelegate(Action<int> callback)
    {
        foreach (var gun in guns)
        {
            gun.onAmmoCountChange += callback;
        }
    }

    /// <summary>
    /// ������ �޾��� �� ����
    /// </summary>
    /// <param name="enemy">������ �� ��</param>
    public void OnAttacked(Enemy enemy)
    {
        Vector3 dir = enemy.transform.position - transform.position;
        // ���� ���� ����(�ð����)
        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        onAttacked?.Invoke(-angle);
        HP -= enemy.attackPower;
    }

    public void Spawn()
    {
        GameManager gameManager = GameManager.Instance;
        Vector3 centerPos = MazelVisualizer.GridToWorld(gameManager.MazeWidth / 2, gameManager.MazeHeight / 2);
        // �÷��̾ �̷��� ���µ� ��ġ�� �ű��
        transform.position = centerPos;

        onSpawn?.Invoke();
    }

    /// <summary>
    /// �÷��̾� ��� ó����
    /// </summary>
    private void Die()
    {
        // �׾����� �˸�
        onDie?.Invoke();
        // �÷��̾� ������Ʈ ��Ȱ��ȭ
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �Է��� ���� �Լ�
    /// </summary>
    private void InputDisable()
    {
        // �׼Ǹ��� 1���� �ֱ� ������ �׳� ó��
        playerInput.actions.actionMaps[0].Disable();
    }
}