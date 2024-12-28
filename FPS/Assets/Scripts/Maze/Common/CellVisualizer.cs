using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellVisualizer : MonoBehaviour
{
    [Tooltip("�� �Ѻ��� ũ��")]
    public const float cellSize = 10f;

    GameObject[] walls;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        walls = new GameObject[child.childCount];

        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = child.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// �Է¹��� �����Ϳ� �°� ���� Ȱ��ȭ ���� �缳��
    /// </summary>
    /// <param name="data"></param>
    public void RefreshWall(byte data)
    {
        // data : �ϵ����� ������� 1�� ���õǾ� ������ ��( = ���� ����). 0�� ���õǾ� ������ ��.

        for (int i = 0; i < walls.Length; i++)
        {
            int mask = 1 << i;

            walls[i].SetActive(!((data & mask) != 0));     // ������� ����ũ�� ������ �� & �������� ��� Ȯ��
        }
    }

    /// <summary>
    /// ���� ���� ���¸� Ȯ���ؼ� �� Ȱ��ȭ ������ ����
    /// </summary>
    /// <returns>4bit �ϵ����� ������ �� : 0, �� : 1 �� ����</returns>
    public Direction GetPath()
    {
        int mask = 0;       // << ������ ����� int�̱� ������ int�� ����

        for (int i = 0; i < walls.Length; i++)
        {
            if (!walls[i].activeSelf)      // Ȱ��ȭ �Ǿ� �ִ��� Ȯ���ؼ�
            {
                mask |= 1 << i;       // ��Ȱ��ȭ �Ǿ� �ִٸ� 1�� ����
            }
        }

        return (Direction)mask;
    }
}
