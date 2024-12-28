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

    }

    /// <summary>
    /// �� ���պ��� �����ϰ� �ϳ� �̻��� ���ʺ� ����
    /// </summary>
    /// <param name="line">�۾� ó���� �� ��</param>
    private void RemoveSouthWall(EllerCell[] line)
    {

    }

    /// <summary>
    /// �� ���� Maze.cells�� ����
    /// </summary>
    /// <param name="line">������ ��</param>
    private void WriteLine(EllerCell[] line)
    {

    }
}
