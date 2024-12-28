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
    // 참조
    // https://weblog.jamisbuck.org/2010/12/27/maze-generation-recursive-backtracking

    protected override void OnSpecificAlgorithExcute()
    {
        // 재구적 백트래킹 알고리즘(Recurcive BackTracking Algorithm)

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[GridToIndex(x, y)] = new BackTrackingCell(x, y);     // 모든 셀 생성(종류에 맞는 셀로 생성)
            }
        }

        int index = Random.Range(0, cells.Length);
        BackTrackingCell start = (BackTrackingCell)cells[index];

        start.visited = true;
        MakeRecursive(start.X, start.Y);

        // 시작지점까지 돌아왔으므로 알고리즘 종료
    }

    /// <summary>
    /// 재귀처리
    /// </summary>
    /// <param name="x">셀의 x 위치</param>
    /// <param name="y">셀의 y 위치</param>
    private void MakeRecursive(int x, int y)
    {
        BackTrackingCell current = (BackTrackingCell)cells[GridToIndex(x, y)];
        Vector2Int[] dirs = { new(0, 1), new(1, 0), new(0, -1), new(-1, 0) };

        Util.Shuffle(dirs);      // 램덤하게 이동할 방향 결정

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int newPos = new(x + dir.x, y + dir.y);

            if (IsInGrid(newPos))     // 미로 안 인지 확인
            {
                BackTrackingCell neighbor = (BackTrackingCell)cells[GridToIndex(newPos)];

                if (!neighbor.visited)     // 방문한 적이 있는지 확인(방문하지 않았어야 함)
                {
                    neighbor.visited = true;     // 방문했다고 표시

                    ConnectPath(current, neighbor);      // 두 셀간에 길을 연결
                    MakeRecursive(neighbor.X, neighbor.Y);
                }
            }
        }

        // 4방향 확인이 끝났다.
    }
}
