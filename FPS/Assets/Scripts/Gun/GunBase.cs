using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public float clipSize;
    [Tooltip("������ �Ѿ� ��( == ���� �Ѿ� ��)")]
    private int bulletCount;
    [Tooltip("���� ���̹�(�Ѿ� �ѹߴ� ������)")]
    public float damage;
    [Tooltip("���� ���� �ӵ�")]
    public float fireRate;
    [Tooltip("ź ���� ����")]
    public float spread;
    [Tooltip("�� �ݵ�")]
    public float recoil;
    [Tooltip("�Ѿ��� �߻�Ǵ� Ʈ������(�÷��̾��� ī�޶� ��ġ)")]
    protected Transform fireTransform;
    [Tooltip("���� �Ѿ� ������ ����Ǿ����� �˸��� ��������Ʈ(int : ���� �Ѿ� ����)")]
    public Action<int> onBulletCountChange;
    [Tooltip("�Ѿ��� �ѹ� �߻�Ǿ����� �˸��� ��������Ʈ(float : �ݵ� ����)")]
    public Action<float> onFire;


    protected int BulletCount
    {
        get => bulletCount;
        set
        {
            bulletCount = value;

            onBulletCountChange?.Invoke(bulletCount);    // �Ѿ� ������ ����Ǿ����� �˸�
        }
    }

    private void Shoot()
    {

    }

    private void Reload()
    {

    }

    public void Equip()
    {
        fireTransform = GameManager.Instance.Player.FireTransform;
    }

    public void UnEquip()
    {

    }

    private void OnDrawGizmos()
    {
        
    }
}
