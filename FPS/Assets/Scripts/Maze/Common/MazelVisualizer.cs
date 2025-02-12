using System.Collections.Generic;
using UnityEngine;

public class MazelVisualizer : MonoBehaviour
{
    [Tooltip("셀의 프리펩")]
    public GameObject cellPrefab;
    [Tooltip("골의 프리펩")]
    public GameObject goalPrefab;
    [Tooltip("마지막으로 비주얼라이저가 그린 미로")]
    private Maze maze = null;
    [Tooltip("이웃의 방향 별 그리드 벡터를 저장하는 딕셔너리")]
    private Dictionary<Direction, Vector2Int> neighborDir;
    [Tooltip("코너 세트 배열(0 : 북서, 1 : 북동, 2 : 남동, 3 : 남서")]
    private (Direction, Direction)[] corners = null;

    private void Awake()
    {
        // 이웃의 방향 미리 결정해 놓기
        neighborDir = new Dictionary<Direction, Vector2Int>(4);
        neighborDir[Direction.North] = new Vector2Int(0, -1);
        neighborDir[Direction.Eask] = new Vector2Int(1, 0);
        neighborDir[Direction.South] = new Vector2Int(0, 1);
        neighborDir[Direction.West] = new Vector2Int(-1, 0);

        // 코너 세트 저장해 놓기
        corners = new (Direction, Direction)[]
        {
            (Direction.North, Direction.West),
            (Direction.North, Direction.Eask),
            (Direction.South, Direction.Eask),
            (Direction.South, Direction.West)
        };
    }

    /// <summary>
    /// 파라미터로 받은 미로를 그린다
    /// </summary>
    /// <param name="maze">Maze로 만들어진 미로</param>
    public void Draw(Maze maze)
    {
        this.maze = maze;
        float size = CellVisualizer.cellSize;

        // 셀의 외형 결정
        foreach (var cell in maze.Cells)
        {
            GameObject obj = Instantiate(cellPrefab, transform);

            obj.transform.Translate(cell.X * size, 0, -cell.Y * size);
            obj.gameObject.name = $"Cell_({cell.X}, {cell.Y})";

            // 기본 벽 처리
            CellVisualizer cellVisualizer = obj.GetComponent<CellVisualizer>();

            cellVisualizer.RefreshWall(cell.Path);

            // 코너 마감
            // ~ 필요한 코너만 남겨놓기(모시리쪽 이웃으로 길이 나 있으면서 이웃이 가진 모서리쪽에 벽이 있다)
            int cornerMask = 0;     // 코너 보일지 여부를 저장하는 마스크 데이터

            for (int i = 0; i < corners.Length; i++)
            {
                // 보여질 코너인지 확인해서
                if (IsCornerVisible(cell, corners[i].Item1, corners[i].Item2))
                {
                    // 마스크에 기록
                    cornerMask |= 1 << i;
                }
            }

            // 마스크 데이터를 기반으로 비주얼라이저에서 리프레시
            cellVisualizer.RefreshCorner(cornerMask);
        }

        // 골 지점 추가
        GameObject goalObj = Instantiate(goalPrefab, transform);
        Goal goal = goalObj.GetComponent<Goal>();

        goal.SetRandomPosition(maze.Width, maze.Height);
        // Debug.Log("미로 비주얼라이저 그리기 완");
    }

    /// <summary>
    /// 모든 셀 제거
    /// </summary>
    public void Clear()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);

            child.SetParent(null);
            Destroy(child.gameObject);
        }

        // Debug.Log("미로 비주얼라이저 초기화");
    }

    /// <summary>
    /// 그리드 좌표로 셀의 월드 좌표 구하는 함수
    /// </summary>
    /// <param name="x">x 위치</param>
    /// <param name="y">y 위치</param>
    /// <returns></returns>
    public static Vector3 GridToWorld(int x, int y)
    {
        float size = CellVisualizer.cellSize;
        float sizeHalf = size * 0.5f;

        return new(size * x + sizeHalf, 0, size * -y - sizeHalf);
    }

    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변환하는 함수
    /// </summary>
    /// <param name="world">월드 좌표</param>
    /// <returns>미로 상의 그리드 좌표</returns>
    public static Vector2Int WorldToGrid(Vector3 world)
    {
        float size = CellVisualizer.cellSize;
        Vector2Int result = new((int)(world.x / size), (int)(-world.z / size));

        return result;
    }

    /// <summary>
    /// 특정 셀의 코너 중 보여질 여부를 판단
    /// </summary>
    /// <param name="cell">확인한 셀</param>
    /// <param name="dir1">코너를 만드는 방향 1</param>
    /// <param name="dir2">코너를 만드는 방향 2</param>
    /// <returns>t : dir1, dir2가 만드는 코너 부분이 보인다. f : 안보인다</returns>
    private bool IsCornerVisible(Cell cell, Direction dir1, Direction dir2)
    {
        bool result = false;

        // dir1, dir2가 코너를 만드는 방향이고 둘 다 길이 있는지 확인
        if (cell.CornerPathCheck(dir1, dir2))
        {
            // dir1 방향의 셀 찾기
            Cell neighborCell_1 = maze.GetCell(cell.X + neighborDir[dir1].x, cell.Y + neighborDir[dir1].y);
            // dir2 방향의 셀 찾기
            Cell neighborCell_2 = maze.GetCell(cell.X + neighborDir[dir2].x, cell.Y + neighborDir[dir2].y);

            // 둘 다 코너쪽에 벽이 있는지 확인
            result = neighborCell_1.IsWall(dir2) && neighborCell_2.IsWall(dir1);
        }

        return result;
    }
}
