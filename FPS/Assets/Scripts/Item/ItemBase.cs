using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : RecycleObject
{
    [Tooltip("1�ʿ� ȸ���ϴ� �ӵ�")]
    public float spinSpeed = 360.0f;
    [Tooltip("ȸ����ų �޽��� Ʈ������")]
    private Transform meshTransform;

    private void Awake()
    {
        meshTransform = transform.GetChild(0);
    }

    private void Update()
    {
        meshTransform.Rotate(Time.deltaTime * spinSpeed * Vector3.up, Space.World);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 30�� �Ŀ� �ڵ����� �������
        StartCoroutine(LifeOver(30));
    }

    private void OnTriggerEnter(Collider other)
    {
        // OnItemConsum ����
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                OnItemConsum(player);
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// �������� �Ծ��� �� ����Ǵ� �Լ�
    /// </summary>
    /// <param name="player">�������� ���� �÷��̾�</param>
    protected virtual void OnItemConsum(Player player)
    {

    }
}