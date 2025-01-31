using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private CinemachineVirtualCamera followCamaer;
    private Player player;
    [Tooltip("미로 가로 길이")]
    public int mazeWidth = 20;
    [Tooltip("미로 세로 길이")]
    public int mazeHeight = 20;
    [Tooltip("미로 생성기")]
    private MazeGenerator mazeGenerator;

    public CinemachineVirtualCamera FollowCamera => followCamaer;
    public Player Player => player;
    [Tooltip("미로 가로 길이 프로퍼티")]
    public int MazeWidth => mazeWidth;
    [Tooltip("미로 세로 길이 프로퍼티")]
    public int MazeHeight => mazeHeight;
    [Tooltip("미로 확인용 피로퍼티")]
    public Maze Maze => mazeGenerator.Maze;

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        GameObject obj = GameObject.FindWithTag("Follow_Camera");

        if (obj != null)
        {
            followCamaer = obj.GetComponent<CinemachineVirtualCamera>();
        }

        mazeGenerator = FindAnyObjectByType<MazeGenerator>();

        if (mazeGenerator != null)
        {
            mazeGenerator.Generate(mazeWidth, mazeHeight);

            mazeGenerator.onMazeGenerated += () =>
            {
                // 플레이어를 미로의 가온데 위치로 옮기기
                Vector3 centerPos = MazelVisualizer.GridToWorld(mazeWidth / 2, mazeHeight / 2);
                player.transform.position = centerPos;
            };
        }
    }
}
