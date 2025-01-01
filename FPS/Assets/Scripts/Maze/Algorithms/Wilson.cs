using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WilsonCell : Cell
{
    [Tooltip("경로가 만들어 졌을 때 다음 셀의 참조")]
    public WilsonCell next;
    [Tooltip("이 셀의 미로에 포함되어 있는지 설정하고 확인")]
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
        // 모든 셀 만들기
        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                cells[GridToIndex(x, y)] = new WilsonCell(x, y);
            }
        }

        // 미로에 포함되지 않는 셀들을 기록한 리스트 만들기
        int[] notInMaxeArray = new int[cells.Length];          // 섞기 위해서 배열 만들기

        for (int i = 0; i < notInMaxeArray.Length; i++)
        {
            // 인덱스 기록하기
            notInMaxeArray[i] = i;
        }

        // 배열 섞기
        Util.Shuffle(notInMaxeArray);

        // 배열을 기반으로 리스트 만들기
        List<int> notInMaze = new List<int>(notInMaxeArray);

        // 필드의 한 곳을 랜덤으로 미로에 추가
        int firstIndex = notInMaze[0];        // 미로에 포함되지 않은 셀 중에서 하나 꺼내기

        notInMaze.RemoveAt(0);

        WilsonCell first = (WilsonCell)cells[firstIndex];

        // 꺼낸 셀을 미로에 포함
        first.isMazeMember = true;

        // notInMaze에 포함된 아이템의 갯수가 0 보다 크다 == 아직 미로에 포함되지 않은 셀이 남아있다.
        while (notInMaze.Count > 0)
        {
            // 미로에 포함되지 않은 셀 중 하나를 랜덤으로 선택(A셀)
            int index = notInMaze[0];

            notInMaze.RemoveAt(0);

            WilsonCell current = (WilsonCell)cells[index];

            // A셀의 위치에서 랜덤으로 한 칸 움직인다. (이동한 셀이 기록되어야 함)
            do
            {
                // 이웃 셀을 구하고
                WilsonCell neighbor = GetNeighbor(current);

                // 어디로 이동하는지 기록
                current.next = neighbor;
                // current 변경
                current = neighbor;
            }
            while (!current.isMazeMember);    // 미로에 포함된 셀에 도착할 때까지의 경로를 미로에 포함시킨다. (경로에 따라 벽도 제거)

            WilsonCell path = (WilsonCell)cells[index];

            // 시작부터 current에 도착할 때 까지 돌기
            while (path != current)
            {
                // 이 셀을 미로에 포함
                path.isMazeMember = true;
                // 미로에 포함되어 있지 않은 셀들의 목록에서 제거
                notInMaze.Remove(GridToIndex(path.X, path.Y));
                // 다음 셀과 길 연결
                ConnectPath(path, path.next);
                // 다음 셀로 넘어감
                path = path.next;
            }
        }     // 모든 셀이 미로에 포함될 때까지 2번으로 돌아가 반복
    }

    /// <summary>
    /// 파라메터로 받은 셀의 이웃 중 하나를 리턴
    /// </summary>
    /// <param name="cell">이웃을 찾은 셀</param>
    /// <returns>파라메터의 이웃 중 하나</returns>
    private WilsonCell GetNeighbor(WilsonCell cell)
    {
        Vector2Int neighborPos;

        do
        {
            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];

            neighborPos = new Vector2Int(cell.X + dir.x, cell.Y + dir.y);
        }
        while (!IsInGrid(neighborPos));      // 그리드 영역 안에 있는 위치를 고를 때까지 반복

        return (WilsonCell)cells[GridToIndex(neighborPos)];
    }
}
