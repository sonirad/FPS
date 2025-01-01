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
    [Tooltip("������ ������ �����ϱ� ���� �ø��� �ѹ�")]
    private int serial = 0;
    [Tooltip("�Ϲ������� �̿� ���� ������ Ȯ��")]
    [Range(0f, 1f)]
    public float mergeChance = 0.7f;
    [Tooltip("���մ� �Ʒ������� ���� ������ Ȯ��")]
    [Range(0f, 1f)]
    public float southOpenChange = 0.5f;

    protected override void OnSpecificAlgorithExcute()
    {
        int h = height - 1;     // �̸� ����� ����
        EllerCell[] prevLIne = null;      // ���� ���� �����ϴ� ����

        for (int y = 0; y < h; y++)    // ù�ٺ��� ������ �ٱ��� ����� �뵵
        {
            // �� �� �����
            EllerCell[] line = MakeLine(prevLIne);
            // ��ĭ���� ��ġ��
            MergeAdjacent(line, mergeChance);
            // �Ʒ��� �� �����ϱ�
            RemoveSouthWall(line);
            // ���� �� ����ϱ�
            WriteLine(line);
            // ���� ���� ���� �ٷ� ����
            prevLIne = line;
            // ���� �Ϸ�

        }

        // ������ �� �����
        EllerCell[] lastLine = MakeLine(prevLIne);
        // ������ �ٿ��� �̿� ���� ������ Ȯ��(������ �������� ��)
        const float lastMergeChange = 1.1f;

        MergeAdjacent(lastLine, lastMergeChange);
        WriteLine(lastLine);
    }

    /// <summary>
    /// �� �� ����
    /// </summary>
    /// <param name="prev">���� ��</param>
    /// <returns>���Ӱ� ������� �� ��</returns>
    EllerCell[] MakeLine(EllerCell[] prev)
    {
        int row = (prev != null) ? (prev[0].Y + 1) : 0;
        EllerCell[] line = new EllerCell[Width];

        for (int x = 0; x < Width; x++)
        {
            // �� �� �����
            line[x] = new EllerCell(x, row);

            if (prev != null && prev[x].IsPath(Direction.South))     // ���� ���� �ְ�, ���� �ٿ� ���� ���� ����. => ���� ���� ���հ� ���� ����
            {
                // ���� ���� ���� ���տ� ���ϰ� �����
                line[x].setGroup = prev[x].setGroup;
                // �������� ���� �����(���� ���� �������� ���� �ֱ� ����)
                line[x].MakePath(Direction.North);
            }
            else        // ���� ���� ���ų�, ���� �ٿ� ���� ���� �ִ�. => ����ũ�� ���տ� ����
            {
                // ������ ���տ� ���ϰ� �����
                line[x].setGroup = serial;
                // ���� ������ �� �����
                serial++;
            }
        }

        return line;
    }

    /// <summary>
    /// �̿� ������ ��ü
    /// </summary>
    /// <param name="line">��ġ�� �۾��� �� ��</param>
    /// <param name="chance">������ Ȯ��</param>
    private void MergeAdjacent(EllerCell[] line, float chance)
    {
        // ��ĭ���� ��ġ��
        // ���� ������ �ٸ��� �����ϰ� ���� �����ϰ� ���� �������� �����.(���� �ٿ� �ִ� ���� ������ ���� �ѹ��� �ٲ��.)
        // �ҷ� ���� �����̸� �н�

        int count = 1;          // ������ ��� ���� ���տ� ���ϴ� ���� �����ϱ� ���� ī����
        int w = Width - 1;        // �̸� ���

        for (int x = 0; x < w; x++)
        {
            if (count < w && line[x].setGroup != line[x + 1].setGroup && Random.value < chance)
            {
                // count�� width���� �۴� = ������ ��� ���� ���տ� ������ �ʴ´�.
                // x�� x + 1��°�� ���� ���� �׷쿡 ������ �ʴ´�.
                // ������ Ȯ���� ����ߴ�.

                line[x].MakePath(Direction.Eask);      // ���� ���� �� �����
                line[x + 1].MakePath(Direction.West);

                int targetGroup = line[x + 1].setGroup;       // x + 1��°�� ������ ������ ����

                line[x + 1].setGroup = line[x].setGroup;     // x + 1��°�� x��°�� ���տ� ���ϰ� �����

                for (int i = x + 2; i < Width; i++)
                {
                    if (line[i].setGroup == targetGroup)        // x + 1��°�� ���� ���տ� ���� ������ x��°�� ���տ� ���ϰ� �����
                    {
                        line[i].setGroup = line[x].setGroup;
                    }
                }

                count++;       // ī��Ʈ ����
            }
        }
    }

    /// <summary>
    /// �� ���պ��� �����ϰ� �ϳ� �̻��� ���ʺ� ����
    /// </summary>
    /// <param name="line">�۾� ó���� �� ��</param>
    private void RemoveSouthWall(EllerCell[] line)
    {
        // ���պ��� ����Ʈ �����

        // Ű : ���չ�ȣ, �� : �� ���� �� �� Ű ���� �ش��ϴ� ���տ� ���ԵǴ� ���� x ��ǥ
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

        // ���պ� ����Ʈ�� �迭�� ��ȯ�ϰ�, ���պ��� ���ʿ� �� �����
        foreach (int key in setListDic.Keys)
        {
            int[] array = setListDic[key].ToArray();      // �迭�� ��ȯ�ϰ�

            Util.Shuffle(array);            // ���� ����(�����ϰ� �� ����� ����)

            int index = array[0];          // ù��°�� ������ �Ʒ������� �� �����

            line[index].MakePath(Direction.South);

            int length = array.Length;

            for (int i = 1; i < length; i++)         // ���� �͵��� Ȯ���� ���� �Ʒ��ʿ� �� �����
            {
                if (Random.value < southOpenChange)
                {
                    line[array[i]].MakePath(Direction.South);
                }
            }
        }
    }

    /// <summary>
    /// �� ���� Maze.cells�� ����
    /// </summary>
    /// <param name="line">������ ��</param>
    private void WriteLine(EllerCell[] line)
    {
        int index = GridToIndex(0, line[0].Y);

        for (int x = 0; x < Width; x++)
        {
            cells[index + x] = line[x];
        }
    }
}
