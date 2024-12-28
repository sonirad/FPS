using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackTrackingCell : Cell
{
    public bool visited;

    public BackTrackingCell(int x, int y) : base(x, y)
    {
        visited = false;
    }
}

public class BackTracking : Maze
{
    // ����
    // https://weblog.jamisbuck.org/2010/12/27/maze-generation-recursive-backtracking

    protected override void OnSpecificAlgorithExcute()
    {
        // �籸�� ��Ʈ��ŷ �˰���(Recurcive BackTracking Algorithm)

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[GridToIndex(x, y)] = new BackTrackingCell(x, y);     // ��� �� ����(������ �´� ���� ����)
            }
        }

        int index = Random.Range(0, cells.Length);
        BackTrackingCell start = (BackTrackingCell)cells[index];

        start.visited = true;
        MakeRecursive(start.X, start.Y);

        // ������������ ���ƿ����Ƿ� �˰��� ����
    }

    /// <summary>
    /// ���ó��
    /// </summary>
    /// <param name="x">���� x ��ġ</param>
    /// <param name="y">���� y ��ġ</param>
    private void MakeRecursive(int x, int y)
    {
        BackTrackingCell current = (BackTrackingCell)cells[GridToIndex(x, y)];
        Vector2Int[] dirs = { new(0, 1), new(1, 0), new(0, -1), new(-1, 0) };

        Util.Shuffle(dirs);      // �����ϰ� �̵��� ���� ����

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int newPos = new(x + dir.x, y + dir.y);

            if (IsInGrid(newPos))     // �̷� �� ���� Ȯ��
            {
                BackTrackingCell neighbor = (BackTrackingCell)cells[GridToIndex(newPos)];

                if (!neighbor.visited)     // �湮�� ���� �ִ��� Ȯ��(�湮���� �ʾҾ�� ��)
                {
                    neighbor.visited = true;     // �湮�ߴٰ� ǥ��

                    ConnectPath(current, neighbor);      // �� ������ ���� ����
                    MakeRecursive(neighbor.X, neighbor.Y);
                }
            }
        }

        // 4���� Ȯ���� ������.
    }
}
