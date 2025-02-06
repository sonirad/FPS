using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    private CinemachineVirtualCamera followCamaer;
    private Player player;
    [Tooltip("�̷� ���� ����")]
    [SerializeField] private int mazeWidth = 20;
    [Tooltip("�̷� ���� ����")]
    [SerializeField] private int mazeHeight = 20;
    [Tooltip("�̷� ������")]
    private MazeGenerator mazeGenerator;
    [Tooltip("ų ī��Ʈ")]
    private int killCount = 0;
    [Tooltip("�÷��� Ÿ��")]
    private float playTime = 0.0f;
    [Tooltip("�� ������")]
    private EnemySpawner spawner;

    [Tooltip("�÷��̾ ����ٴϴ� ī�޶�")]
    public CinemachineVirtualCamera FollowCamera => followCamaer;
    [Tooltip("�÷��̾�")]
    public Player Player => player;
    [Tooltip("�̷� ���� ���� ������Ƽ")]
    public int MazeWidth => mazeWidth;
    [Tooltip("�̷� ���� ���� ������Ƽ")]
    public int MazeHeight => mazeHeight;
    [Tooltip("�̷� Ȯ�ο� �Ƿ���Ƽ")]
    public Maze Maze => mazeGenerator.Maze;
    public EnemySpawner Spawner => spawner;

    [Tooltip("���� ������ �˸��� ��������Ʈ")]
    public Action onGameStart;
    [Tooltip("���� ������ �˸��� ��������Ʈ")]
    public Action<bool> onGameEnd;

    private void Update()
    {
        playTime += Time.deltaTime;
    }

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        Vector3 centerPos = MazelVisualizer.GridToWorld(MazeWidth / 2, MazeHeight / 2);
        // �÷��̾ �̷��� ���µ� ��ġ�� �ű��
        player.transform.position = centerPos;
        player.onDie += GameOver;
        GameObject obj = GameObject.FindWithTag("Follow_Camera");

        if (obj != null)
        {
            followCamaer = obj.GetComponent<CinemachineVirtualCamera>();
        }

        spawner = FindAnyObjectByType<EnemySpawner>();
        mazeGenerator = FindAnyObjectByType<MazeGenerator>();

        if (mazeGenerator != null)
        {
            mazeGenerator.Generate(mazeWidth, mazeHeight);

            mazeGenerator.onMazeGenerated += () =>
            {
                // �� ����
                spawner?.EnemyAll_Spawn();

                // �÷��� �ð� �ʱ�ȭ
                playTime = 0;
                // ų ī��Ʈ �ʱ�ȭ
                killCount = 0;
            };
        }

        ResultPanel resultPanel = FindAnyObjectByType<ResultPanel>();

        resultPanel.gameObject.SetActive(false);

        onGameEnd += (isClear) =>
        {
            // Time.timeSinceLevelLoad : ���� �ε��ǰ� ���� �ð�
            CrossHair crosshair = FindAnyObjectByType<CrossHair>();
            // ũ�ν� ��� �� ���̰� �����
            crosshair.gameObject.SetActive(false);
            // �Է� ����
            resultPanel.Open(true, killCount, playTime);
        };

        // Ŀ�� �� ���̰� �����
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IncreaseKillCount()
    {
        killCount++;
    }

    /// <summary>
    /// ���� ���� �� ����
    /// </summary>
    public void GameStart()
    {
        onGameStart?.Invoke();
    }

    /// <summary>
    /// ���� Ŭ���� �� ����
    /// </summary>
    public void GameClear()
    {
        onGameEnd?.Invoke(true);
    }

    /// <summary>
    /// ���� ���� �� ����
    /// </summary>
    public void GameOver()
    {
        onGameEnd?.Invoke(false);
    }
}
