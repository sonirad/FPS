using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : GunBase
{
    public float reloadDuration = 1.0f;
    private bool isReloading = false;

    protected override void FireProcess(bool isFireStart = true)
    {
        base.FireProcess();

        // ���� ó��
        HitProcess();
        // �ѱ� �ݵ� ��ȣ ������
        FireRecoil();
    }

    /// <summary>
    /// ���ε� ó��
    /// </summary>
    public void ReLoad()
    {
        // ���ε� �� �ƴ� ���� ����
        if (!isReloading)
        {
            // FireProcess���� �����Ű��� �ڷ�ƾ���� isFireReady�� true�� �Ǵ� �� ����
            StopAllCoroutines();
            // ���ε� ���̶�� ǥ��
            isReloading = true;
            // ���ε� �� ���� �߻��ϴ� ���� ����
            isFireReady = false;

            // ���ε� �ڷ�ƾ ����
            StartCoroutine(ReloadCoroutine());
        }
    }

    /// <summary>
    /// ���ε� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadCoroutine()
    {
        // ���ε� �ð���ŭ ��ٸ� ��
        yield return new WaitForSeconds(reloadDuration);

        // �� �߻� �����ϰ� ����
        isFireReady = true;
        // źâ ũ�⸸ŭ ������
        BulletCount = clipSize;
        // ���ε� ��ǥ ǥ��
        isReloading = false;
    }
}
