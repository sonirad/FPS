using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private CinemachineVirtualCamera followCamaer;
    private Player player;
    [Tooltip("�̷� ���� ����")]
    public int mazeWidth = 20;
    [Tooltip("�̷� ���� ����")]
    public int mazeHeight = 20;
    [Tooltip("�̷� ������")]
    private MazeGenerator mazeGenerator;

    public CinemachineVirtualCamera FollowCamera => followCamaer;
    public Player Player => player;
    [Tooltip("�̷� ���� ���� ������Ƽ")]
    public int MazeWidth => mazeWidth;
    [Tooltip("�̷� ���� ���� ������Ƽ")]
    public int MazeHeight => mazeHeight;
    [Tooltip("�̷� Ȯ�ο� �Ƿ���Ƽ")]
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
                // �÷��̾ �̷��� ���µ� ��ġ�� �ű��
                Vector3 centerPos = MazelVisualizer.GridToWorld(mazeWidth / 2, mazeHeight / 2);
                player.transform.position = centerPos;
            };
        }
    }
}
