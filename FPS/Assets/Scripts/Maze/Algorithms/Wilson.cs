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
        for (int y = 0; y < Width; y++)
        {
            for (int x = 0; x < Height; x++)
            {
                cells[GridToIndex(x, y)] = new WilsonCell(x, y);
            }
        }

        int[] notInMaxeArray = new int[cells.Length];

        for (int i = 0; i < notInMaxeArray.Length; i++)
        {
            notInMaxeArray[i] = i;
        }

        Util.Shuffle(notInMaxeArray);

        List<int> notInMaze = new List<int>(notInMaxeArray);

        int firstIndex = notInMaze[0];

        notInMaze.RemoveAt(0);

        WilsonCell first = (WilsonCell)cells[firstIndex];

        first.isMazeMember = true;

        while (notInMaze.Count > 0)
        {
            int index = notInMaze[0];

            notInMaze.RemoveAt(0);

            WilsonCell current = (WilsonCell)cells[index];

            do
            {
                WilsonCell neighbor = GetNeighbor(current);

                current.next = neighbor;
                current = neighbor;
            }
            while (!current.isMazeMember);

            WilsonCell path = (WilsonCell)cells[index];

            while (path != current)
            {
                path.isMazeMember = true;
                notInMaze.Remove(GridToIndex(path.X, path.Y));
                ConnectPath(path, path.next);
                path = path.next;
            }
        }
    }

    private WilsonCell GetNeighbor(WilsonCell cell)
    {
        Vector2Int neighborPos;

        do
        {
            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];

            neighborPos = new Vector2Int(cell.X + dir.x, cell.Y + dir.y);
        }
        while (!IsInGrid(neighborPos));

        return (WilsonCell)cells[GridToIndex(neighborPos)];
    }
}
