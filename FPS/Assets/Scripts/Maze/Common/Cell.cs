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
    [Tooltip("이 셀에 연결된 길을 기록하는 변수(북동남서 순서대로 해당하는 비트에 1이 세팅되어 있으면 길이 있다. 0이 세팅되어 있으면 길이 없다.(벽이 있다)")]
    private byte path;
    [Tooltip("미로 그리드 상에서의 x 좌표(왼쪽 -> 오른쪽")]
    protected int x;
    [Tooltip("미로 그리드 상에서의 y 좌표(위 -> 아래")]
    protected int y;

    [Tooltip("연결한 길을 확인하기 위한 프로퍼티")]
    public byte Path => path;
    public int X => x;
    public int Y => y;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="x">셀의 위치</param>
    /// <param name="y">셀의 위치</param>
    public Cell(int x, int y)
    {
        this.path = (byte)Direction.None;
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// 이 셀에 길을 추가
    /// </summary>
    /// <param name="pathDirection">새로 길을 만들 방향</param>
    public void MakePath(Direction pathDirection)
    {
        path |= (byte)pathDirection;
    }

    /// <summary>
    /// 특정 방향이 길인지 확인
    /// </summary>
    /// <param name="pathDirection">확인할 방향</param>
    /// <returns>t : 길, f : 벽</returns>
    public bool IsPath(Direction pathDirection)
    {
        return (path & (byte)pathDirection) != 0;
    }

    /// <summary>
    /// 특정 방향이 벽인지 확인
    /// </summary>
    /// <param name="pathDirection">확인할 방향</param>
    /// <returns>t : 길, f : 벽</returns>
    public bool IsWall(Direction pathDirection)
    {
        return (path & (byte)pathDirection) == 0;
    }

    /// <summary>
    /// 코너 체크용(양 방향에 모두 길이 있어야 true)
    /// </summary>
    /// <param name="dir1"></param>
    /// <param name="dir2"></param>
    /// <returns>dir!과 dir2가 코너를 만드는 방향이고 둘 다 같이 있을 때만 true</returns>
    public bool CornerPathCheck(Direction dir1, Direction dir2)
    {
        bool result = false;
        Direction corner = dir1 | dir2;

        if (corner == (Direction.North | Direction.Eask)
            || corner == (Direction.North | Direction.West)
            || corner == (Direction.South | Direction.Eask)
            || corner == (Direction.South | Direction.West))      // 코너인지 확인
        {
            // 양쪽 다 갈이 있는지 확인
            result = IsPath(dir1) && IsPath(dir2);
        }

        return result;
    }
}