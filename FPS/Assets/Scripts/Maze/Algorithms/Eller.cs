using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class EllerCell : Cell
{
    public int setGroup;
    private const int notSet = -1;

    public EllerCell(int x, int y) : base(x, y)
    {
        setGroup = notSet;
    }
}

public class Eller : Maze
{
    [Tooltip("고유한 집합을 설정하기 위한 시리얼 넘버")]
    private int serial = 0;
    [Tooltip("일반적으로 이웃 셀과 합쳐질 확률")]
    [Range(0f, 1f)]
    public float mergeChance = 0.7f;
    [Tooltip("집합당 아래쪽으로 벽을 제거할 확률")]
    [Range(0f, 1f)]
    public float southOpenChange = 0.5f;

    protected override void OnSpecificAlgorithExcute()
    {
        int h = height - 1;     // 미리 계산해 놓기
        EllerCell[] prevLIne = null;      // 이전 줄을 저장하는 변수

        for (int y = 0; y < h; y++)    // 첫줄부터 마지막 줄까지 만드는 용도
        {
            // 한 줄 만들기
            EllerCell[] line = MakeLine(prevLIne);
            // 옆칸끼리 합치기
            MergeAdjacent(line, mergeChance);
            // 아래쪽 벽 제거하기
            RemoveSouthWall(line);
            // 만든 줄 기록하기
            WriteLine(line);
            // 만든 줄을 이전 줄로 설정
            prevLIne = line;
            // 한줄 완료

        }

        // 마지막 줄 만들기
        EllerCell[] lastLine = MakeLine(prevLIne);
        // 마지막 줄에서 이웃 셀과 합쳐질 확률(무조건 합쳐져야 함)
        const float lastMergeChange = 1.1f;

        MergeAdjacent(lastLine, lastMergeChange);
        WriteLine(lastLine);
    }

    /// <summary>
    /// 한 줄 제작
    /// </summary>
    /// <param name="prev">이전 줄</param>
    /// <returns>새롭게 만들어진 한 줄</returns>
    EllerCell[] MakeLine(EllerCell[] prev)
    {
        int row = (prev != null) ? (prev[0].Y + 1) : 0;
        EllerCell[] line = new EllerCell[Width];

        for (int x = 0; x < Width; x++)
        {
            // 새 셀 만들기
            line[x] = new EllerCell(x, row);

            if (prev != null && prev[x].IsPath(Direction.South))     // 위쪽 줄이 있고, 위쪽 줄에 남쪽 벽이 없다. => 위쪽 셀의 집합과 같은 집합
            {
                // 위쪽 셀과 같은 집합에 속하게 만들기
                line[x].setGroup = prev[x].setGroup;
                // 위쪽으로 길을 만들기(위쪽 셀이 남쪽으로 길이 있기 때문)
                line[x].MakePath(Direction.North);
            }
            else        // 위쪽 줄이 없거나, 위쪽 줄에 남쪽 벽이 있다. => 유니크한 집합에 포함
            {
                // 고유한 집합에 속하게 만들기
                line[x].setGroup = serial;
                // 다음 고유한 값 만들기
                serial++;
            }
        }

        return line;
    }

    /// <summary>
    /// 이웃 셀까지 합체
    /// </summary>
    /// <param name="line">합치는 작업을 할 줄</param>
    /// <param name="chance">합쳐질 확률</param>
    private void MergeAdjacent(EllerCell[] line, float chance)
    {
        // 옆칸끼리 합치기
        // 서로 집합이 다르면 랜덤하게 벽을 제거하고 같은 집합으로 만든다.(같은 줄에 있는 같은 종류의 셀이 한번에 바뀐다.)
        // 소러 같은 집합이면 패스

        int count = 1;          // 한줄이 모두 같은 집합에 속하는 것을 방지하기 위한 카운터
        int w = Width - 1;        // 미리 계산

        for (int x = 0; x < w; x++)
        {
            if (count < w && line[x].setGroup != line[x + 1].setGroup && Random.value < chance)
            {
                // count가 width보다 작다 = 한줄이 모두 같은 집합에 속하지 않는다.
                // x와 x + 1번째의 셀이 같은 그룹에 속하지 않는다.
                // 설정한 확률을 통과했다.

                line[x].MakePath(Direction.Eask);      // 서로 같은 길 만들기
                line[x + 1].MakePath(Direction.West);

                int targetGroup = line[x + 1].setGroup;       // x + 1번째의 집합을 저장해 놓기

                line[x + 1].setGroup = line[x].setGroup;     // x + 1번째를 x번째의 집합에 속하게 만들기

                for (int i = x + 2; i < Width; i++)
                {
                    if (line[i].setGroup == targetGroup)        // x + 1번째와 같은 집합에 속한 셀들을 x번째의 집합에 속하게 만들기
                    {
                        line[i].setGroup = line[x].setGroup;
                    }
                }

                count++;       // 카운트 증가
            }
        }
    }

    /// <summary>
    /// 각 집합별로 램덤하게 하나 이상의 남쪽벽 제거
    /// </summary>
    /// <param name="line">작업 처리를 할 줄</param>
    private void RemoveSouthWall(EllerCell[] line)
    {
        // 집합별로 리스트 만들기

        // 키 : 집합번호, 값 : 이 줄의 셀 중 키 값에 해당하는 집합에 포함되는 셀의 x 좌표
        Dictionary<int, List<int>> setListDic = new Dictionary<int, List<int>>();

        for (int x = 0; x < Width; x++)
        {
            int key = line[x].setGroup;

            if (!setListDic.ContainsKey(key))
            {
                setListDic[key] = new List<int>();
            }

            setListDic[key].Add(x);
        }

        // 집합별 리스트를 배열로 변환하고, 집합별로 남쪽에 길 만들기
        foreach (int key in setListDic.Keys)
        {
            int[] array = setListDic[key].ToArray();      // 배열로 변환하고

            Util.Shuffle(array);            // 순서 섞기(랜덤하게 길 만들기 위해)

            int index = array[0];          // 첫번째는 무조건 아래쪽으로 길 만들기

            line[index].MakePath(Direction.South);

            int length = array.Length;

            for (int i = 1; i < length; i++)         // 남은 것들은 확률에 따라 아래쪽에 길 만들기
            {
                if (Random.value < southOpenChange)
                {
                    line[array[i]].MakePath(Direction.South);
                }
            }
        }
    }

    /// <summary>
    /// 한 줄을 Maze.cells에 저장
    /// </summary>
    /// <param name="line">저장할 줄</param>
    private void WriteLine(EllerCell[] line)
    {
        int index = GridToIndex(0, line[0].Y);

        for (int x = 0; x < Width; x++)
        {
            cells[index + x] = line[x];
        }
    }
}
