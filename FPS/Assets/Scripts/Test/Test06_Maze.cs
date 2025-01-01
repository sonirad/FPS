using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test06_Maze : TestBase
{
    [Header("¼¿")]
    public Direction pathData;
    public CellVisualizer cellVisualizer;
    [Header("¹Ì·Î")]
    public int width = 5;
    public int height = 5;

    public MazelVisualizer backTracking;
    public MazelVisualizer eller;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        cellVisualizer.RefreshWall((byte)pathData);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Debug.Log(cellVisualizer.GetPath());
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        backTracking.Clear();

        BackTracking maze = new BackTracking();

        maze.MakeMaze(width, height, seed);
        backTracking.Draw(maze);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        eller.Clear();

        Eller maze = new Eller();

        maze.MakeMaze(width, height, seed);
        eller.Draw(maze);
    }
}
