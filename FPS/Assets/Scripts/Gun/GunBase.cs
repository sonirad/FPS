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
    // ���� �Ÿ�
    // źâ (źâ ũ��, źâ�� �����ִ� �Ѿ� ��)
    // ������
    // ���� �ӵ�
    // ź ����
    // �� �ݵ�

    [Tooltip("���� �Ÿ�")]
    public float range;
    [Tooltip("źâ ũ��")]
    public int clipSize;
    [Tooltip("������ �Ѿ� ��( == ���� �Ѿ� ��)")]
    private int bulletCount;
    [Tooltip("���� ���̹�(�Ѿ� �ѹߴ� ������)")]
    public float damage;
    [Tooltip("���� ���� �ӵ�(1�ʴ� �߻� ��)")]
    public float fireRate;
    [Tooltip("ź ���� ������ ����")]
    public float spread;
    [Tooltip("�� �ݵ�")]
    public float recoil;
    [Tooltip("���� �߻� �������� ���� Ȯ��")]
    protected bool isFireReady = true;
    [Tooltip("muzzle ����Ʈ")]
    private VisualEffect muzzleEffect;

    [Tooltip("�Ѿ��� �߻�Ǵ� Ʈ������(�÷��̾��� ī�޶� ��ġ)")]
    protected Transform fireTransform;
    [Tooltip("���� �Ѿ� ������ ����Ǿ����� �˸��� ��������Ʈ(int : ���� �Ѿ� ����)")]
    public Action<int> onAmmoCountChange;
    [Tooltip("�Ѿ��� �ѹ� �߻�Ǿ����� �˸��� ��������Ʈ(float : �ݵ� ����)")]
    public Action<float> onFire;
    [Tooltip("�Ѿ��� �� ���������� �˸��� ��������Ʈ")]
    public Action onAmmoDepleted;

    [Tooltip("muzzle ����Ʈ �ߵ���")]
    readonly int onFireID = Shader.PropertyToID("OnFire");


    protected int BulletCount
    {
        get => bulletCount;
        set
        {
            bulletCount = value;

            onAmmoCountChange?.Invoke(bulletCount);    // �Ѿ� ������ ����Ǿ����� �˸�

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
    /// �ʱ�ȭ
    /// </summary>
    private void Initialize()
    {
        // �Ѿ��� ������ ������ ���·� �ʱ�ȭ
        BulletCount = clipSize;
        // �߻� �������� �ʱ�ȭ
        isFireReady = true;
    }

    /// <summary>
    /// �� �߻�
    /// </summary>
    public void Fire(bool isFireStart = true)
    {
        // �߻� �����ϰ� �Ѿ��� ����������
        if (isFireReady && BulletCount > 0)
        {
            // �� �߻�
            FireProcess(isFireStart);
        }
    }

    /// <summary>
    /// �߻簡 �������� �� �ǻ��� ��ɵ�
    /// </summary>
    /// <param name="isFireStart">�߻� �Է��� ������ : t, ������ : f</param>
    protected virtual void FireProcess(bool isFireStart = true)
    {
        // ��� �߻簡 ���� �ʰ� ����
        isFireReady = false;

        // ���� ����Ʈ ������
        MuzzleEffectOn();

        // ���� �ð� �� �ڵ����� �߻� �����ϰ� ����
        StartCoroutine(FireReady());

        // �Ѿ� ���� ����
        BulletCount--;
    }

    /// <summary>
    /// ���� �ð� �Ŀ� isFireReady�� true�� ���� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    private IEnumerator FireReady()
    {
        // fireLRate�� ���� ��ٸ��� �ð�
        yield return new WaitForSeconds(1 / fireRate);

        isFireReady = true;
    }

    /// <summary>
    /// muzzle ����Ʈ ����
    /// </summary>
    protected void MuzzleEffectOn()
    {
        muzzleEffect.SendEvent(onFireID);
    }

    /// <summary>
    /// ���� �ε�ģ ���� ���� ó��
    /// </summary>
    protected void HitProcess()
    {
        // ���� �����
        Ray ray = new (fireTransform.position, GetFireDirection());

        // int i = ~LayerMask.GetMask("Default");       Default ���̾� ���� üũ
        if (Physics.Raycast(ray, out RaycastHit hitInfo, range, ~LayerMask.NameToLayer("Default")))      // ����ĳ��Ʈ
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Enemy target = hitInfo.collider.GetComponentInParent<Enemy>();

                // ���� ������ ������ �Ѱ��ֱ�
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

                // �Ѿ� ���� ������ ����, ������ ��ġ, ������ ���� ���, �ݻ���� ����
                Factory.Instance.GetBulletHole(hitInfo.point, hitInfo.normal, reflect);
            }
        }
    }

    /// <summary>
    /// �ݵ� �˸�
    /// </summary>
    protected void FireRecoil()
    {
        onFire?.Invoke(recoil);
    }

    /// <summary>
    /// ���� ����� �� ó��
    /// </summary>
    public void Equip()
    {
        fireTransform = GameManager.Instance.Player.FireTransform;

        Initialize();
    }

    /// <summary>
    /// ���� ��� ���� �ɶ� ó��
    /// </summary>
    public void UnEquip()
    {
        StopAllCoroutines();

        isFireReady = true;
    }

    /// <summary>
    /// �߻� �� ������ ������ �߻� ���� ���ϱ�
    /// </summary>
    /// <returns>�Ѿ��� �߻��� ����</returns>
    protected Vector3 GetFireDirection()
    {
        Vector3 result = fireTransform.forward;

        // �� �Ʒ��� -spread ~ spread ��ŭ ȸ��(x�� �������� ȸ��)
        result = Quaternion.AngleAxis(UnityEngine.Random.Range(-spread, spread), fireTransform.right) * result;
        // fireTransform.forward�� ������ ��� 0 ~ 360�� ȸ��
        result = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), fireTransform.forward) * result;

        return result;
    }

#if UNITY_EDITOR
    public void Test_Fire(bool isFireStart = true)
    {
        if (fireTransform == null)
        {
            Equip();
            Debug.Log("�߻� �غ�");
        }

        Fire(isFireStart);
        Debug.Log("�߻�");
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
