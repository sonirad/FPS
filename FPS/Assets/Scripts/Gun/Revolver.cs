using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : GunBase
{
    public float reloadDuration = 1.0f;

    protected override void FireProcess()
    {
        base.FireProcess();

        // ���� ó��
        HitProcess();
        // �ѱ� �ݵ� ��ȣ ������
        FireRecoil();
    }

    public void ReLoad()
    {

    }
}
