using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleObject : MonoBehaviour
{
    [Tooltip("��Ȱ�� ������Ʈ�� ��Ȱ��ȭ �� �� ����Ǵ� ��������Ʈ")]
    public Action onDisable;

    protected virtual void OnEnable()
    {
        // ���� �뵵(��� �������)
        StopAllCoroutines();
    }

    protected virtual void OnDisable()
    {
        // ��Ȱ��ȭ �Ǿ����� �˸�(Ǯ ���� �� ���� �� ��ϵǾ�� ��)
        onDisable?.Invoke();
    }

    /// <summary>
    /// ���� �ð� �Ŀ� �� ���� ������Ʈ�� ��Ȱ��ȭ ��Ű�� ���ƾ
    /// </summary>
    /// <param name="delay">��Ȱ��ȭ �� ������ ������ �ð�</param>
    /// <returns></returns>
    protected IEnumerator LifeOver(float delay = 0.0f)
    {
        // delay ��ŭ ��ٸ���
        yield return new WaitForSeconds(delay);

        // ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
