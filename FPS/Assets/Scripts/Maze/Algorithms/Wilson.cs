using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WilsonCell : Cell
{
    [Tooltip("��ΰ� ����� ���� �� ���� ���� ����")]
    public WilsonCell next;
    [Tooltip("�� ���� �̷ο� ���ԵǾ� �ִ��� �����ϰ� Ȯ��")]
    public bool isMazeMember;

    public WilsonCell(int x, int y) : base(x, y)
    {
        next = null;
        isMazeMember = false;
    }
}

public class Wilson : Maze
{
    readonly Vector2Int[] dirs = { new(0, 1), new(1, 0), new(0, -1), new(-1, 0), };

    protected override void OnSpecificAlgorithExcute()
    {
        // ��� �� �����
        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                cells[GridToIndex(x, y)] = new WilsonCell(x, y);
            }
        }

        // �̷ο� ���Ե��� �ʴ� ������ ����� ����Ʈ �����
        int[] notInMaxeArray = new int[cells.Length];          // ���� ���ؼ� �迭 �����

        for (int i = 0; i < notInMaxeArray.Length; i++)
        {
            // �ε��� ����ϱ�
            notInMaxeArray[i] = i;
        }

        // �迭 ����
        Util.Shuffle(notInMaxeArray);

        // �迭�� ������� ����Ʈ �����
        List<int> notInMaze = new List<int>(notInMaxeArray);

        // �ʵ��� �� ���� �������� �̷ο� �߰�
        int firstIndex = notInMaze[0];        // �̷ο� ���Ե��� ���� �� �߿��� �ϳ� ������

        notInMaze.RemoveAt(0);

        WilsonCell first = (WilsonCell)cells[firstIndex];

        // ���� ���� �̷ο� ����
        first.isMazeMember = true;

        // notInMaze�� ���Ե� �������� ������ 0 ���� ũ�� == ���� �̷ο� ���Ե��� ���� ���� �����ִ�.
        while (notInMaze.Count > 0)
        {
            // �̷ο� ���Ե��� ���� �� �� �ϳ��� �������� ����(A��)
            int index = notInMaze[0];

            notInMaze.RemoveAt(0);

            WilsonCell current = (WilsonCell)cells[index];

            // A���� ��ġ���� �������� �� ĭ �����δ�. (�̵��� ���� ��ϵǾ�� ��)
            do
            {
                // �̿� ���� ���ϰ�
                WilsonCell neighbor = GetNeighbor(current);

                // ���� �̵��ϴ��� ���
                current.next = neighbor;
                // current ����
                current = neighbor;
            }
            while (!current.isMazeMember);    // �̷ο� ���Ե� ���� ������ �������� ��θ� �̷ο� ���Խ�Ų��. (��ο� ���� ���� ����)

            WilsonCell path = (WilsonCell)cells[index];

            // ���ۺ��� current�� ������ �� ���� ����
            while (path != current)
            {
                // �� ���� �̷ο� ����
                path.isMazeMember = true;
                // �̷ο� ���ԵǾ� ���� ���� ������ ��Ͽ��� ����
                notInMaze.Remove(GridToIndex(path.X, path.Y));
                // ���� ���� �� ����
                ConnectPath(path, path.next);
                // ���� ���� �Ѿ
                path = path.next;
            }
        }     // ��� ���� �̷ο� ���Ե� ������ 2������ ���ư� �ݺ�
    }

    /// <summary>
    /// �Ķ���ͷ� ���� ���� �̿� �� �ϳ��� ����
    /// </summary>
    /// <param name="cell">�̿��� ã�� ��</param>
    /// <returns>�Ķ������ �̿� �� �ϳ�</returns>
    private WilsonCell GetNeighbor(WilsonCell cell)
    {
        Vector2Int neighborPos;

        do
        {
            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];

            neighborPos = new Vector2Int(cell.X + dir.x, cell.Y + dir.y);
        }
        while (!IsInGrid(neighborPos));      // �׸��� ���� �ȿ� �ִ� ��ġ�� �� ������ �ݺ�

        return (WilsonCell)cells[GridToIndex(neighborPos)];
    }
}
