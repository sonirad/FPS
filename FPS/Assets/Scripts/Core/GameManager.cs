using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    private CinemachineVirtualCamera followCamaer;
    private Player player;
    [Tooltip("미로 가로 길이")]
    [SerializeField] private int mazeWidth = 20;
    [Tooltip("미로 세로 길이")]
    [SerializeField] private int mazeHeight = 20;
    [Tooltip("미로 생성기")]
    private MazeGenerator mazeGenerator;
    [Tooltip("킬 카운트")]
    private int killCount = 0;
    [Tooltip("플레이 타임")]
    private float playTime = 0.0f;
    [Tooltip("적 스포너")]
    private EnemySpawner spawner;

    [Tooltip("플레이어를 따라다니는 카메라")]
    public CinemachineVirtualCamera FollowCamera => followCamaer;
    [Tooltip("플레이어")]
    public Player Player => player;
    [Tooltip("미로 가로 길이 프로퍼티")]
    public int MazeWidth => mazeWidth;
    [Tooltip("미로 세로 길이 프로퍼티")]
    public int MazeHeight => mazeHeight;
    [Tooltip("미로 확인용 피로퍼티")]
    public Maze Maze => mazeGenerator.Maze;
    public EnemySpawner Spawner => spawner;

    [Tooltip("게임 시작을 알리는 델리게이트")]
    public Action onGameStart;
    [Tooltip("게임 종료을 알리는 델리게이트")]
    public Action<bool> onGameEnd;

    private void Update()
    {
        playTime += Time.deltaTime;
    }

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        Vector3 centerPos = MazelVisualizer.GridToWorld(MazeWidth / 2, MazeHeight / 2);
        // 플레이어를 미로의 가온데 위치로 옮기기
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
                // 적 스폰
                spawner?.EnemyAll_Spawn();

                // 플레이 시간 초기화
                playTime = 0;
                // 킬 카운트 초기화
                killCount = 0;
            };
        }

        ResultPanel resultPanel = FindAnyObjectByType<ResultPanel>();

        resultPanel.gameObject.SetActive(false);

        onGameEnd += (isClear) =>
        {
            // Time.timeSinceLevelLoad : 씬이 로딩되고 지난 시간
            CrossHair crosshair = FindAnyObjectByType<CrossHair>();
            // 크로스 헤어 안 보이게 만들기
            crosshair.gameObject.SetActive(false);
            // 입력 막고
            resultPanel.Open(true, killCount, playTime);
        };

        // 커서 안 보이게 만들기
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IncreaseKillCount()
    {
        killCount++;
    }

    /// <summary>
    /// 게임 시작 시 실행
    /// </summary>
    public void GameStart()
    {
        onGameStart?.Invoke();
    }

    /// <summary>
    /// 게임 클리어 시 실행
    /// </summary>
    public void GameClear()
    {
        onGameEnd?.Invoke(true);
    }

    /// <summary>
    /// 게임 오버 시 실행
    /// </summary>
    public void GameOver()
    {
        onGameEnd?.Invoke(false);
    }
}
