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
    private int killCount = 0;
    private float playTime = 0.0f;

    public CinemachineVirtualCamera FollowCamera => followCamaer;
    public Player Player => player;
    [Tooltip("�̷� ���� ���� ������Ƽ")]
    public int MazeWidth => mazeWidth;
    [Tooltip("�̷� ���� ���� ������Ƽ")]
    public int MazeHeight => mazeHeight;
    [Tooltip("�̷� Ȯ�ο� �Ƿ���Ƽ")]
    public Maze Maze => mazeGenerator.Maze;

    private void Update()
    {
        playTime += Time.deltaTime;
    }

    protected override void OnInitialize()
    {
        CrossHair crosshair = FindAnyObjectByType<CrossHair>();
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
                // �÷��� �ð� �ʱ�ȭ
                playTime = 0;
                // ų ī��Ʈ �ʱ�ȭ
                killCount = 0;
            };
        }

        ResultPanel resultPanel = FindAnyObjectByType<ResultPanel>();

        resultPanel.gameObject.SetActive(false);

        Goal goal = FindAnyObjectByType<Goal>();

        goal.onGameClear += () =>
        {
            // Time.timeSinceLevelLoad : ���� �ε��ǰ� ���� �ð�
            crosshair.gameObject.SetActive(false);     // ũ�ν� ��� �� ���̰� �����
            // �Է� ����
            player.InputDisable();
            resultPanel.Open(true, killCount, playTime);
        };

        // Ŀ�� �� ���̰� �����
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IncreaseKillCount()
    {
        killCount++;
    }
}
