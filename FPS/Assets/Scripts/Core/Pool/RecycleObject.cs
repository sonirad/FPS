using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleObject : MonoBehaviour
{
    [Tooltip("재활용 오브젝트가 비활성화 될 때 실행되는 델리게이트")]
    public Action onDisable;

    protected virtual void OnEnable()
    {
        // 리셋 용도(없어도 상관없음)
        StopAllCoroutines();
    }

    protected virtual void OnDisable()
    {
        // 비활성화 되었음을 알림(풀 만들 때 할일 이 등록되어야 함)
        onDisable?.Invoke();
    }

    /// <summary>
    /// 일정 시간 후에 이 게임 오브젝트를 비활성화 시키는 쿠루틴
    /// </summary>
    /// <param name="delay">비활성화 됭 때까지 딜레이 시간</param>
    /// <returns></returns>
    protected IEnumerator LifeOver(float delay = 0.0f)
    {
        // delay 만큼 기다리고
        yield return new WaitForSeconds(delay);

        // 비활성화
        gameObject.SetActive(false);
    }
}
