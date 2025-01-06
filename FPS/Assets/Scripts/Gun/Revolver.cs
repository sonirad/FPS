using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : GunBase
{
    public float reloadDuration = 1.0f;

    protected override void FireProcess()
    {
        base.FireProcess();

        // 명중 처리
        HitProcess();
        // 총기 반동 신호 보내기
        FireRecoil();
    }

    public void ReLoad()
    {

    }
}
