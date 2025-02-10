using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Direction : byte
{
    None = 0,
    North = 1,
    Eask = 2,
    South = 4,
    West = 8,
}

public class Cell
{
    [Tooltip("�� ���� ����� ���� ����ϴ� ����(�ϵ����� ������� �ش��ϴ� ��Ʈ�� 1�� ���õǾ� ������ ���� �ִ�. 0�� ���õǾ� ������ ���� ����.(���� �ִ�)")]
    private byte path;
    [Tooltip("�̷� �׸��� �󿡼��� x ��ǥ(���� -> ������")]
    protected int x;
    [Tooltip("�̷� �׸��� �󿡼��� y ��ǥ(�� -> �Ʒ�")]
    protected int y;

    [Tooltip("������ ���� Ȯ���ϱ� ���� ������Ƽ")]
    public byte Path => path;
    public int X => x;
    public int Y => y;

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="x">���� ��ġ</param>
    /// <param name="y">���� ��ġ</param>
    public Cell(int x, int y)
    {
        this.path = (byte)Direction.None;
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// �� ���� ���� �߰�
    /// </summary>
    /// <param name="pathDirection">���� ���� ���� ����</param>
    public void MakePath(Direction pathDirection)
    {
        path |= (byte)pathDirection;
    }

    /// <summary>
    /// Ư�� ������ ������ Ȯ��
    /// </summary>
    /// <param name="pathDirection">Ȯ���� ����</param>
    /// <returns>t : ��, f : ��</returns>
    public bool IsPath(Direction pathDirection)
    {
        return (path & (byte)pathDirection) != 0;
    }

    /// <summary>
    /// Ư�� ������ ������ Ȯ��
    /// </summary>
    /// <param name="pathDirection">Ȯ���� ����</param>
    /// <returns>t : ��, f : ��</returns>
    public bool IsWall(Direction pathDirection)
    {
        return (path & (byte)pathDirection) == 0;
    }

    /// <summary>
    /// �ڳ� üũ��(�� ���⿡ ��� ���� �־�� true)
    /// </summary>
    /// <param name="dir1"></param>
    /// <param name="dir2"></param>
    /// <returns>dir!�� dir2�� �ڳʸ� ����� �����̰� �� �� ���� ���� ���� true</returns>
    public bool CornerPathCheck(Direction dir1, Direction dir2)
    {
        bool result = false;
        Direction corner = dir1 | dir2;

        if (corner == (Direction.North | Direction.Eask)
            || corner == (Direction.North | Direction.West)
            || corner == (Direction.South | Direction.Eask)
            || corner == (Direction.South | Direction.West))      // �ڳ����� Ȯ��
        {
            // ���� �� ���� �ִ��� Ȯ��
            result = IsPath(dir1) && IsPath(dir2);
        }

        return result;
    }
}