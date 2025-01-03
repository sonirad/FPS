using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Test07_DynamicNavSurface : TestBase
{
    [Header("¹Ì·Î")]
    public int width = 10;
    public int height = 10;

    public MazelVisualizer backTracking;
    public MazelVisualizer eller;
    public MazelVisualizer wilson;

    public NavMeshSurface surface;
    private AsyncOperation navAsync;

    public MazeGenerator generator;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        backTracking.Clear();

        BackTracking mazeBackTracking = new BackTracking();

        mazeBackTracking.MakeMaze(width, height, seed);
        backTracking.Draw(mazeBackTracking);

        eller.Clear();

        Eller mazeEller = new Eller();

        mazeEller.MakeMaze(width, height, seed);
        eller.Draw(mazeEller);

        wilson.Clear();

        Wilson mazeWilson = new Wilson();

        mazeWilson.MakeMaze(width, height, seed);
        wilson.Draw(mazeWilson);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        StartCoroutine(UpdateSurface());
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        generator.Generate(width, height);
    }

    private IEnumerator UpdateSurface()
    {
        navAsync = surface.UpdateNavMesh(surface.navMeshData);

        while (!navAsync.isDone)
        {
            yield return null;
        }

        Debug.Log("Nav Surface Udated!");
    }
}