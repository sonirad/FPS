using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MazelVisualizer : MonoBehaviour
{
    [Tooltip("���� ������")]
    public GameObject cellPrefab;

    /// <summary>
    /// �Ķ���ͷ� ���� �̷θ� �׸���
    /// </summary>
    /// <param name="maze">Maze�� ������� �̷�</param>
    public void Draw(Maze maze)
    {
        float size = CellVisualizer.cellSize;

        foreach (var cell in maze.Cells)
        {
            GameObject obj = Instantiate(cellPrefab, transform);

            obj.transform.Translate(cell.X * size, 0, -cell.Y * size);
            obj.gameObject.name = $"Cell_({cell.X}, {cell.Y})";

            CellVisualizer cellVisualizer = obj.GetComponent<CellVisualizer>();

            cellVisualizer.RefreshWall(cell.Path);

            Debug.Log("�̷� �׸��� ��");
        }
    }

    /// <summary>
    /// ��� �� ����
    /// </summary>
    public void Clear()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);

            child.SetParent(null);
            Destroy(child.gameObject);
        }

        Debug.Log("�̷� ���־������ �ʱ�ȭ");
    }

    /// <summary>
    /// �׸��� ��ǥ�� ���� ���� ��ǥ ���ϴ� �Լ�
    /// </summary>
    /// <param name="x">x ��ġ</param>
    /// <param name="y">y ��ġ</param>
    /// <returns></returns>
    public Vector3 GridToWorld(int x, int y)
    {
        float size = CellVisualizer.cellSize;
        float sizeHalf = size * 0.5f;

        return new(size * x + sizeHalf, size * -y - sizeHalf);
    }
}
