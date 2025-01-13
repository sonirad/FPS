using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [Tooltip("1�ʿ� ȸ���ϴ� �ӵ�")]
    public float spinSpeed = 360.0f;
    [Tooltip("ȸ����ų �޽��� Ʈ������")]
    private Transform meshTransform;

    private void OnTriggerEnter(Collider other)
    {
        // OnItemConsum ����
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                OnItemConsum(player);
                Destroy(this.gameObject);
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