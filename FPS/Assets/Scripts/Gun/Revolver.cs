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

        // 명중 처리
        HitProcess();
        // 총기 반동 신호 보내기
        FireRecoil();
    }

    /// <summary>
    /// 리로드 처리
    /// </summary>
    public void ReLoad()
    {
        // 리로딩 중 아닐 때만 실행
        if (!isReloading)
        {
            // FireProcess에서 실행시키기는 코루틴으로 isFireReady가 true가 되는 것 방지
            StopAllCoroutines();
            // 리로딩 중이라고 표시
            isReloading = true;
            // 리로딩 중 총을 발사하는 것을 방지
            isFireReady = false;

            // 리로드 코루틴 실행
            StartCoroutine(ReloadCoroutine());
        }
    }

    /// <summary>
    /// 리로딩 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadCoroutine()
    {
        // 리로딩 시간만큼 기다린 후
        yield return new WaitForSeconds(reloadDuration);

        // 총 발사 가능하게 설정
        isFireReady = true;
        // 탄창 크기만큼 재장전
        BulletCount = clipSize;
        // 리로딩 종표 표시
        isReloading = false;
    }
}
