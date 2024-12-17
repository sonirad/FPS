using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [Tooltip("���� �ӵ��� �����ϱ� ���� Ŀ��")]
    [SerializeField] private AnimationCurve recoveryCurve;
    [Tooltip("�ִ� Ȯ�� ũ��")]
    [SerializeField] private float maxExpend = 100.0f;
    [Tooltip("���� Ȯ��Ǿ� �ִ� ����(defaultExpend���� �󸶳� �� Ȯ��Ǿ���)")]
    [SerializeField] private float current = 0.0f;
    [Tooltip("4���� ũ�ν���� �̹����� Ʈ��������")]
    [SerializeField] private RectTransform[] crossRects;
    [Tooltip("�� ũ�ν���� �̹����� �̵� ����")]
    [SerializeField] readonly Vector2[] direction = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    [Tooltip("�⺻������ Ȯ��Ǿ� �ִ� ����(�θ𿡼� �⺻������ ������ �ִ� ����")]
    [SerializeField] const float defaultExpend = 10.0f;
    [Tooltip("���� �Ǳ� ���� ������ �ð�")]
    [SerializeField] const float recoveryWaitTime = 0.1f;
    [Tooltip("���� �Ǳ� ���� �ɸ��� �ð�")]
    [SerializeField] const float recoveryDuration = 0.5f;
    [Tooltip("�����⸦ �����ϴ� ���� ���ϱ� ���� �̸� ����� ���� ��")]
    const float divPreCompute = 1 / recoveryDuration;

    private void Awake()
    {
        crossRects = new RectTransform[transform.childCount];  // ũ�ν������ �̹��� �̸� ã�� ����

        for (int i = 0; i < transform.childCount; i++)
        {
            crossRects[i] = transform.GetChild(i) as RectTransform;
        }
    }

    /// <summary>
    /// ���ؼ��� Ȯ��
    /// </summary>
    /// <param name="amount">Ȯ�� ����</param>
    public void Expend(float amount)
    {
        current = Mathf.Min(current + amount, maxExpend);   // �ִ�ġ�� ���� �ʰ� ����

        for (int i = 0; i < crossRects.Length; i++)
        {
            crossRects[i].anchoredPosition = (defaultExpend + current) * direction[i];   // defaultExpend���� current ��ŭ Ȯ��
        }

        StopAllCoroutines();               // �ڷ�ƾ �ߺ� ������ �����ϱ� ���� �ڷ�ƾ ���� ����
        StartCoroutine(DelayRecovery(recoveryWaitTime));       // defaultExpend �� ������Ű�� �ڷ�ƾ
    }

    /// <summary>
    /// defaultExpend �� ������Ű�� �ڷ�ƾ
    /// </summary>
    /// <param name="wait">ó���� ����ϴ� �ð�</param>
    /// <returns></returns>
    IEnumerator DelayRecovery(float wait)
    {
        yield return new WaitForSeconds(wait);     // ó���� wait ��ŭ ���

        float startExpend = current;      // current �� �̿��ؼ� ���� Ȯ�� ���� ���(�ִ�ġ ����)
        float curveProcess = 0.0f;       // ���� Ŀ�� ���� ����( 0 ~ 1 )

        while (curveProcess < 1)       // curveProcess �� 1�� �� ������ ��� ����
        {
            curveProcess += Time.deltaTime / divPreCompute;              // recoveryDuration �Ⱓ�� ���缭 curveProcess ����
            current = recoveryCurve.Evaluate(curveProcess) * startExpend;      // current �� ����ϱ�(Ŀ�� ��� * �ִ�ġ)

            for (int i = 0; i < crossRects.Length; i++)
            {
                crossRects[i].anchoredPosition = (current + defaultExpend) * direction[i];     // ���� current �� �°� ���
            }

            yield return null;     // 1 ������ ���
        }

        // deltaTime �� ������ ���� �� ������ ����ϰ� defaultExpend �� ����
        for (int i = 0; i < crossRects.Length; i++)
        {
            crossRects[i].anchoredPosition = defaultExpend * direction[i];
        }

        current = 0;       // current Ŭ����
    }
}