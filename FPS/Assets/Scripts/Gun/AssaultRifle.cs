using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : GunBase
{
    protected override void FireProcess(bool isFireStart = true)
    {
        if (isFireStart)
        {
            // 입력이 들어왔을 때 발사 시작
            StartCoroutine(FireRepeat());
        }
        else
        {
            // 입력이 끝났을 때 발사 종료
            StopAllCoroutines();

            isFireReady = true;
        }
    }

    private IEnumerator FireRepeat()
    {
        // 총알이 남아있는 동안 계속 반복
        while (BulletCount > 0)
        {
            // 머즐 이팩트 켜고
            MuzzleEffectOn();

            // 총알 갯수 하나 줄이기
            BulletCount--;

            // 명중 처리
            HitProcess();
            // 반동 주기
            FireRecoil();

            // 발사속도 만큼 대기
            yield return new WaitForSeconds(1 / fireRate);
        }

        isFireReady = true;
    }
}
