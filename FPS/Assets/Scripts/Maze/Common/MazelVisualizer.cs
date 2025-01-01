using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MazelVisualizer : MonoBehaviour
{
    [Tooltip("셀의 프리펩")]
    public GameObject cellPrefab;

    /// <summary>
    /// 파라미터로 받은 미로를 그린다
    /// </summary>
    /// <param name="maze">Maze로 만들어진 미로</param>
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

            Debug.Log("미로 그리기 완");
        }
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

        Debug.Log("미로 비주얼라이저 초기화");
    }

    /// <summary>
    /// 그리드 좌표로 셀의 월드 좌표 구하는 함수
    /// </summary>
    /// <param name="x">x 위치</param>
    /// <param name="y">y 위치</param>
    /// <returns></returns>
    public Vector3 GridToWorld(int x, int y)
    {
        float size = CellVisualizer.cellSize;
        float sizeHalf = size * 0.5f;

        return new(size * x + sizeHalf, size * -y - sizeHalf);
    }
}
