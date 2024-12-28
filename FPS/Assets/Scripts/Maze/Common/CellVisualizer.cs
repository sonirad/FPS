using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellVisualizer : MonoBehaviour
{
    [Tooltip("셀 한변의 크기")]
    public const float cellSize = 10f;

    GameObject[] walls;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        walls = new GameObject[child.childCount];

        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = child.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// 입력받은 데이터에 맞게 벽의 활성화 여부 재설정
    /// </summary>
    /// <param name="data"></param>
    public void RefreshWall(byte data)
    {
        // data : 북동남서 순서대로 1이 세팅되어 있으면 길( = 벽이 없다). 0이 세팅되어 있으면 벽.

        for (int i = 0; i < walls.Length; i++)
        {
            int mask = 1 << i;

            walls[i].SetActive(!((data & mask) != 0));     // 순서대로 마스크를 생성한 후 & 연산으로 결과 확인
        }
    }

    /// <summary>
    /// 현재 셀의 상태를 확인해서 벽 활성화 정보를 리턴
    /// </summary>
    /// <returns>4bit 북동남서 순으로 벽 : 0, 길 : 1 로 세팅</returns>
    public Direction GetPath()
    {
        int mask = 0;       // << 연산의 결과는 int이기 때문에 int로 설정

        for (int i = 0; i < walls.Length; i++)
        {
            if (!walls[i].activeSelf)      // 활성화 되어 있는지 확인해서
            {
                mask |= 1 << i;       // 비활성화 되어 있다면 1로 세팅
            }
        }

        return (Direction)mask;
    }
}
