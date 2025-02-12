using System.Collections.Generic;
using UnityEngine;

public class MazelVisualizer : MonoBehaviour
{
    [Tooltip("���� ������")]
    public GameObject cellPrefab;
    [Tooltip("���� ������")]
    public GameObject goalPrefab;
    [Tooltip("���������� ���־�������� �׸� �̷�")]
    private Maze maze = null;
    [Tooltip("�̿��� ���� �� �׸��� ���͸� �����ϴ� ��ųʸ�")]
    private Dictionary<Direction, Vector2Int> neighborDir;
    [Tooltip("�ڳ� ��Ʈ �迭(0 : �ϼ�, 1 : �ϵ�, 2 : ����, 3 : ����")]
    private (Direction, Direction)[] corners = null;

    private void Awake()
    {
        // �̿��� ���� �̸� ������ ����
        neighborDir = new Dictionary<Direction, Vector2Int>(4);
        neighborDir[Direction.North] = new Vector2Int(0, -1);
        neighborDir[Direction.Eask] = new Vector2Int(1, 0);
        neighborDir[Direction.South] = new Vector2Int(0, 1);
        neighborDir[Direction.West] = new Vector2Int(-1, 0);

        // �ڳ� ��Ʈ ������ ����
        corners = new (Direction, Direction)[]
        {
            (Direction.North, Direction.West),
            (Direction.North, Direction.Eask),
            (Direction.South, Direction.Eask),
            (Direction.South, Direction.West)
        };
    }

    /// <summary>
    /// �Ķ���ͷ� ���� �̷θ� �׸���
    /// </summary>
    /// <param name="maze">Maze�� ������� �̷�</param>
    public void Draw(Maze maze)
    {
        this.maze = maze;
        float size = CellVisualizer.cellSize;

        // ���� ���� ����
        foreach (var cell in maze.Cells)
        {
            GameObject obj = Instantiate(cellPrefab, transform);

            obj.transform.Translate(cell.X * size, 0, -cell.Y * size);
            obj.gameObject.name = $"Cell_({cell.X}, {cell.Y})";

            // �⺻ �� ó��
            CellVisualizer cellVisualizer = obj.GetComponent<CellVisualizer>();

            cellVisualizer.RefreshWall(cell.Path);

            // �ڳ� ����
            // ~ �ʿ��� �ڳʸ� ���ܳ���(��ø��� �̿����� ���� �� �����鼭 �̿��� ���� �𼭸��ʿ� ���� �ִ�)
            int cornerMask = 0;     // �ڳ� ������ ���θ� �����ϴ� ����ũ ������

            for (int i = 0; i < corners.Length; i++)
            {
                // ������ �ڳ����� Ȯ���ؼ�
                if (IsCornerVisible(cell, corners[i].Item1, corners[i].Item2))
                {
                    // ����ũ�� ���
                    cornerMask |= 1 << i;
                }
            }

            // ����ũ �����͸� ������� ���־���������� ��������
            cellVisualizer.RefreshCorner(cornerMask);
        }

        // �� ���� �߰�
        GameObject goalObj = Instantiate(goalPrefab, transform);
        Goal goal = goalObj.GetComponent<Goal>();

        goal.SetRandomPosition(maze.Width, maze.Height);
        // Debug.Log("�̷� ���־������ �׸��� ��");
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

        // Debug.Log("�̷� ���־������ �ʱ�ȭ");
    }

    /// <summary>
    /// �׸��� ��ǥ�� ���� ���� ��ǥ ���ϴ� �Լ�
    /// </summary>
    /// <param name="x">x ��ġ</param>
    /// <param name="y">y ��ġ</param>
    /// <returns></returns>
    public static Vector3 GridToWorld(int x, int y)
    {
        float size = CellVisualizer.cellSize;
        float sizeHalf = size * 0.5f;

        return new(size * x + sizeHalf, 0, size * -y - sizeHalf);
    }

    /// <summary>
    /// ���� ��ǥ�� �׸��� ��ǥ�� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <param name="world">���� ��ǥ</param>
    /// <returns>�̷� ���� �׸��� ��ǥ</returns>
    public static Vector2Int WorldToGrid(Vector3 world)
    {
        float size = CellVisualizer.cellSize;
        Vector2Int result = new((int)(world.x / size), (int)(-world.z / size));

        return result;
    }

    /// <summary>
    /// Ư�� ���� �ڳ� �� ������ ���θ� �Ǵ�
    /// </summary>
    /// <param name="cell">Ȯ���� ��</param>
    /// <param name="dir1">�ڳʸ� ����� ���� 1</param>
    /// <param name="dir2">�ڳʸ� ����� ���� 2</param>
    /// <returns>t : dir1, dir2�� ����� �ڳ� �κ��� ���δ�. f : �Ⱥ��δ�</returns>
    private bool IsCornerVisible(Cell cell, Direction dir1, Direction dir2)
    {
        bool result = false;

        // dir1, dir2�� �ڳʸ� ����� �����̰� �� �� ���� �ִ��� Ȯ��
        if (cell.CornerPathCheck(dir1, dir2))
        {
            // dir1 ������ �� ã��
            Cell neighborCell_1 = maze.GetCell(cell.X + neighborDir[dir1].x, cell.Y + neighborDir[dir1].y);
            // dir2 ������ �� ã��
            Cell neighborCell_2 = maze.GetCell(cell.X + neighborDir[dir2].x, cell.Y + neighborDir[dir2].y);

            // �� �� �ڳ��ʿ� ���� �ִ��� Ȯ��
            result = neighborCell_1.IsWall(dir2) && neighborCell_2.IsWall(dir1);
        }

        return result;
    }
}
