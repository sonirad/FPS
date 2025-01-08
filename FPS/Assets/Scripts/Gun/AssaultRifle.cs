using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : GunBase
{
    protected override void FireProcess(bool isFireStart = true)
    {
        if (isFireStart)
        {
            // �Է��� ������ �� �߻� ����
            StartCoroutine(FireRepeat());
        }
        else
        {
            // �Է��� ������ �� �߻� ����
            StopAllCoroutines();

            isFireReady = true;
        }
    }

    private IEnumerator FireRepeat()
    {
        // �Ѿ��� �����ִ� ���� ��� �ݺ�
        while (BulletCount > 0)
        {
            // ���� ����Ʈ �Ѱ�
            MuzzleEffectOn();

            // �Ѿ� ���� �ϳ� ���̱�
            BulletCount--;

            // ���� ó��
            HitProcess();
            // �ݵ� �ֱ�
            FireRecoil();

            // �߻�ӵ� ��ŭ ���
            yield return new WaitForSeconds(1 / fireRate);
        }

        isFireReady = true;
    }
}
