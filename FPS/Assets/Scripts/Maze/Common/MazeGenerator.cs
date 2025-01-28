using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System;

[RequireComponent(typeof(MazelVisualizer))]
[RequireComponent(typeof(NavMeshSurface))]
public class MazeGenerator : MonoBehaviour
{
    public enum MazeAlgorithm
    {
        RecursiveBackTracking = 0,
        Eller,
        Wilson
    }

    public int seed = -1;
    public MazeAlgorithm mazeAlgorithm = MazeAlgorithm.Wilson;
    MazelVisualizer visualizer;
    NavMeshSurface navMeshSurface;
    AsyncOperation navAsync;
    private Maze maze = null;

    [Tooltip("생성한 미로")]
    public Maze Maze => maze;

    [Tooltip("미로 생성이 끝났음을 알리는 델리게이트")]
    public Action onMazeGenerated;

    private void Awake()
    {
        visualizer = GetComponent<MazelVisualizer>();
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void Update()
    {
        Debug.Log("Update");
    }

    public void Generate(int width, int height)
    {
        Maze maze = null;

        switch (mazeAlgorithm)
        {
            case MazeAlgorithm.RecursiveBackTracking:
                maze = new BackTracking();
                break;
            case MazeAlgorithm.Eller:
                maze = new Eller();
                break;
            case MazeAlgorithm.Wilson:
                maze = new Wilson();
                break;
        }

        maze.MakeMaze(width, height, seed);
        visualizer.Clear();
        visualizer.Draw(maze);

        StartCoroutine(UpdateSurface());
    }

    private IEnumerator UpdateSurface()
    {
        navAsync = navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);

        while (!navAsync.isDone)
        {
            yield return null;
        }

        Debug.Log("Nav Surface Update!");

        // 오클루전 컬링도 새로 베이크 필요
        onMazeGenerated?.Invoke();       // 미로 생성이 끝났음을 알림
    }
}
