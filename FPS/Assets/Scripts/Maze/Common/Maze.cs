using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    protected Cell[] cells;
    protected int width;
    protected int height;

    [Tooltip("�̷��� �� ����")]
    public Cell[] Cells => cells;
    [Tooltip("�̷��� ���� ����")]
    public int Width => width;
    [Tooltip("�̷��� ���� ����")]
    public int Height => height;

    /// <summary>
    /// �̷θ� ����
    /// </summary>
    /// <param name="width">�̷��� ���� ����</param>
    /// <param name="height">�̷��� ���� ����</param>
    /// <param name="seed">������ �õ�. -1 �̸� ������ �õ� ����</param>
    public void MakeMaze(int width, int height, int seed = -1)
    {
        this.width = width;
        this.height = height;

        if (seed != -1)
        {
            Random.InitState(seed);
        }

        cells = new Cell[Width * Height];

        OnSpecificAlgorithExcute();    // �� �˷θ��� �� �ڵ� ����

        Debug.Log("�̷� ����� �Ϸ�");
    }

    /// <summary>
    /// �� �˷α��� �� override �ؾ� �ϴ� �Լ�. �̷� ���� �˰���.
    /// </summary>
    protected virtual void OnSpecificAlgorithExcute()
    {
        // cell�� �����ϰ� �˰��� ����� �°� ����
    }

    /// <summary>
    /// �� �� ������ ���� ����
    /// </summary>
    /// <param name="from">���� ��</param>
    /// <param name="to">���� ��</param>
    protected void ConnectPath(Cell from, Cell to)
    {
        Vector2Int dir = new(to.X - from.X, to.Y - from.Y);    // from���� to�� ���� ���� ���ϱ�.

        if (dir.x > 0)
        {
            // ����
            from.MakePath(Direction.Eask);
            to.MakePath(Direction.West);
        }
        else if (dir.x < 0)
        {
            // ����
            from.MakePath(Direction.West);
            to.MakePath(Direction.Eask);
        }
        else if (dir.y > 0)
        {
            // ����
            from.MakePath(Direction.South);
            to.MakePath(Direction.North);
        }
        else if (dir.y < 0)
        {
            // ����
            from.MakePath(Direction.North);
            to.MakePath(Direction.South);
        }
    }

    protected bool IsInGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
    
    protected bool IsInGrid(Vector2Int grid)
    {
        return grid.x >= 0 && grid.y >= 0 && grid.x < width && grid.y < height;
    }

    protected Vector2Int IndexToGrid(int index)
    {
        return new(index % width, index / width);
    }

    protected int GridToIndex(int x, int y)
    {
        return x + y * width;
    }

    protected int GridToIndex(Vector2Int grid)
    {
        return grid.x + grid.y * width;
    }

    /// <summary>
    /// Ư�� �׸��� ��ǥ�� ���� ����
    /// </summary>
    /// <param name="x">���� �׸��� x ��ġ</param>
    /// <param name="y">���� �׸��� y ��ġ</param>
    /// <returns>(x, y) ��ġ�� �ִ� ��</returns>
    public Cell GetCell(int x, int y)
    {
        int index = GridToIndex(x, y);

        return cells[index];
    }
}
