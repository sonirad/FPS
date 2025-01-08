using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : GunBase
{
    [Tooltip("한번에 발사되는 총알 갯수")]
    public int pellet = 6;

    protected override void FireProcess(bool isFireStart = true)
    {
        if (isFireStart)
        {
            base.FireProcess(isFireStart);

            for (int i = 0; i < pellet; i++)
            {
                // 여러번 hit 처리
                HitProcess();
            }

            FireRecoil();
        }
    }
}
