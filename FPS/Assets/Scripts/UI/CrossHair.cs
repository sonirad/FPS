using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [Tooltip("복구 속도를 조절하기 위한 커브")]
    [SerializeField] private AnimationCurve recoveryCurve;
    [Tooltip("최대 확장 크기")]
    [SerializeField] private float maxExpend = 100.0f;
    [Tooltip("현재 확장되어 있는 길이(defaultExpend에서 얼마나 더 확장되었나)")]
    [SerializeField] private float current = 0.0f;
    [Tooltip("4방향 크로스헤어 이미지의 트랜스폼들")]
    [SerializeField] private RectTransform[] crossRects;
    [Tooltip("각 크로스헤어 이미지의 이동 방향")]
    [SerializeField] readonly Vector2[] direction = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    [Tooltip("기본적으로 확장되어 있는 길이(부모에서 기본적으로 떨어져 있는 정도")]
    [SerializeField] const float defaultExpend = 10.0f;
    [Tooltip("복구 되기 전에 딜레이 시간")]
    [SerializeField] const float recoveryWaitTime = 0.1f;
    [Tooltip("복구 되기 전에 걸리는 시간")]
    [SerializeField] const float recoveryDuration = 0.5f;
    [Tooltip("나누기를 자주하는 것을 피하기 위해 미리 계산해 놓은 것")]
    const float divPreCompute = 1 / recoveryDuration;

    private void Awake()
    {
        crossRects = new RectTransform[transform.childCount];  // 크로스헤어의 이미지 미리 찾아 놓기

        for (int i = 0; i < transform.childCount; i++)
        {
            crossRects[i] = transform.GetChild(i) as RectTransform;
        }
    }

    /// <summary>
    /// 조준선을 확장
    /// </summary>
    /// <param name="amount">확장 정도</param>
    public void Expend(float amount)
    {
        current = Mathf.Min(current + amount, maxExpend);   // 최대치를 넘지 않게 조절

        for (int i = 0; i < crossRects.Length; i++)
        {
            crossRects[i].anchoredPosition = (defaultExpend + current) * direction[i];   // defaultExpend에서 current 만큼 확장
        }

        StopAllCoroutines();               // 코루틴 중복 실행을 방지하기 위한 코루틴 실행 정지
        StartCoroutine(DelayRecovery(recoveryWaitTime));       // defaultExpend 로 복구시키는 코루틴
    }

    /// <summary>
    /// defaultExpend 로 복구시키는 코루틴
    /// </summary>
    /// <param name="wait">처음에 대기하는 시간</param>
    /// <returns></returns>
    IEnumerator DelayRecovery(float wait)
    {
        yield return new WaitForSeconds(wait);     // 처음에 wait 만큼 대기

        float startExpend = current;      // current 를 이용해서 현재 확장 정도 기록(최대치 설정)
        float curveProcess = 0.0f;       // 현재 커브 진행 정도( 0 ~ 1 )

        while (curveProcess < 1)       // curveProcess 가 1이 될 때까지 계속 진행
        {
            curveProcess += Time.deltaTime / divPreCompute;              // recoveryDuration 기간에 맞춰서 curveProcess 진행
            current = recoveryCurve.Evaluate(curveProcess) * startExpend;      // current 를 계산하기(커브 결과 * 최대치)

            for (int i = 0; i < crossRects.Length; i++)
            {
                crossRects[i].anchoredPosition = (current + defaultExpend) * direction[i];     // 계산된 current 에 맞게 축소
            }

            yield return null;     // 1 프레임 대기
        }

        // deltaTime 은 오차가 있을 수 있으니 깔끔하게 defaultExpend 로 지정
        for (int i = 0; i < crossRects.Length; i++)
        {
            crossRects[i].anchoredPosition = defaultExpend * direction[i];
        }

        current = 0;       // current 클리어
    }
}