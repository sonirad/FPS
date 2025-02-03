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
    private int killCount = 0;
    private float playTime = 0.0f;

    public CinemachineVirtualCamera FollowCamera => followCamaer;
    public Player Player => player;
    [Tooltip("미로 가로 길이 프로퍼티")]
    public int MazeWidth => mazeWidth;
    [Tooltip("미로 세로 길이 프로퍼티")]
    public int MazeHeight => mazeHeight;
    [Tooltip("미로 확인용 피로퍼티")]
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
                // 플레이어를 미로의 가온데 위치로 옮기기
                Vector3 centerPos = MazelVisualizer.GridToWorld(mazeWidth / 2, mazeHeight / 2);
                player.transform.position = centerPos;
                // 플레이 시간 초기화
                playTime = 0;
                // 킬 카운트 초기화
                killCount = 0;
            };
        }

        ResultPanel resultPanel = FindAnyObjectByType<ResultPanel>();

        resultPanel.gameObject.SetActive(false);

        Goal goal = FindAnyObjectByType<Goal>();

        goal.onGameClear += () =>
        {
            // Time.timeSinceLevelLoad : 씬이 로딩되고 지난 시간
            crosshair.gameObject.SetActive(false);     // 크로스 헤어 안 보이게 만들기
            // 입력 막고
            player.InputDisable();
            resultPanel.Open(true, killCount, playTime);
        };

        // 커서 안 보이게 만들기
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IncreaseKillCount()
    {
        killCount++;
    }
}
