using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    protected Cell[] cells;
    protected int width;
    protected int height;

    [Tooltip("미로의 셀 묶음")]
    public Cell[] Cells => cells;
    [Tooltip("미로의 가로 길이")]
    public int Width => width;
    [Tooltip("미로의 세로 길이")]
    public int Height => height;

    /// <summary>
    /// 미로를 생성
    /// </summary>
    /// <param name="width">미로의 가로 길이</param>
    /// <param name="height">미로의 세로 길이</param>
    /// <param name="seed">랜덤용 시드. -1 이면 지정된 시드 적용</param>
    public void MakeMaze(int width, int height, int seed = -1)
    {
        this.width = width;
        this.height = height;

        if (seed != -1)
        {
            Random.InitState(seed);
        }

        cells = new Cell[Width * Height];

        OnSpecificAlgorithExcute();    // 각 알로리즘 별 코드 실행

        Debug.Log("미로 만들기 완료");
    }

    /// <summary>
    /// 각 알로기즘 별 override 해야 하는 함수. 미로 생성 알고리즘.
    /// </summary>
    protected virtual void OnSpecificAlgorithExcute()
    {
        // cell을 생성하고 알고리즘 결과에 맞게 세팅
    }

    /// <summary>
    /// 두 셀 사이의 벽을 제거
    /// </summary>
    /// <param name="from">시작 셀</param>
    /// <param name="to">도착 셀</param>
    protected void ConnectPath(Cell from, Cell to)
    {
        Vector2Int dir = new(to.X - from.X, to.Y - from.Y);    // from에서 to로 가능 방향 구하기.

        if (dir.x > 0)
        {
            // 동쪽
            from.MakePath(Direction.Eask);
            to.MakePath(Direction.West);
        }
        else if (dir.x < 0)
        {
            // 서쪽
            from.MakePath(Direction.West);
            to.MakePath(Direction.Eask);
        }
        else if (dir.y > 0)
        {
            // 남쪽
            from.MakePath(Direction.South);
            to.MakePath(Direction.North);
        }
        else if (dir.y < 0)
        {
            // 북쪽
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
    /// 특정 그리드 좌표의 셀을 리턴
    /// </summary>
    /// <param name="x">셀의 그리드 x 위치</param>
    /// <param name="y">셀의 그리드 y 위치</param>
    /// <returns>(x, y) 위치에 있는 셀</returns>
    public Cell GetCell(int x, int y)
    {
        int index = GridToIndex(x, y);

        return cells[index];
    }
}
