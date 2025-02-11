using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{
    public int enemyCount = 50;
    public GameObject enemyPrefab;
    private int mazeWidth;
    private int mazeHeigth;
    private Player player;
    private Enemy[] enemies;

    public Action onSpawnCompleted;

    private void Awake()
    {
        enemies = new Enemy[enemyCount];
    }

    private void Start()
    {
        // 미로 크기 가져오기
        mazeWidth = GameManager.Instance.MazeWidth;
        mazeHeigth = GameManager.Instance.MazeHeight;

        player = GameManager.Instance.Player;

        GameManager.Instance.onGameStart += EnemyAll_Play;
        GameManager.Instance.onGameEnd += (_) => EnemyAll_Stop();
    }

    public void EnemyAll_Spawn()
    {
        // 적 생성
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            obj.name = $"Enemy_{i}";
            Enemy enemy = obj.GetComponent<Enemy>();
            enemies[i] = enemy;

            enemy.onDie += (target) =>
            {
                GameManager.Instance.IncreaseKillCount();
                StartCoroutine(Respawn(target));
            };

            enemy.Respawn(GetRandomSpawnPosition(true), true);
        }

        // 스폰이 완료되면 알림
        onSpawnCompleted?.Invoke();
    }

    /// <summary>
    /// 모든 적 움직이게 만들기
    /// </summary>
    private void EnemyAll_Play()
    {
        foreach (var enemy in enemies)
        {
            // wander 상태로 변경
            enemy.Play();
        }
    }

    /// <summary>
    /// 모든 적을 일시정지 시키기(이동, 공격)
    /// </summary>
    private void EnemyAll_Stop()
    {
        foreach (var enemy in enemies)
        {
            // 대기상태로 변경
            enemy.Stop();
        }
    }

    /// <summary>
    /// 플레이어 주변의 랜덤한 스폰 위치를 구함
    /// </summary>
    /// <returns>스폰 위치(미로 한칸의 셀의 가온데 지점)</returns>
    private Vector3 GetRandomSpawnPosition(bool init = false)
    {
        // 플레이어의 그리드 위치
        Vector2Int playerPosition;

        if (init)
        {
            // 플레이어가 정상적으로 있다는 보장이 없는 경우 그냥 미로의 가온데 위치
            playerPosition = new(mazeWidth / 2, mazeHeigth / 2);
        }
        else
        {
            // 일반 플레이 중에는 플레이어의 그리드 위치
            playerPosition = MazelVisualizer.WorldToGrid(player.transform.position);
        }

        int x;
        int y;
        int limit = 100;
        float halfSize = Mathf.Min(mazeWidth, mazeHeigth) * 0.5f;

        do
        {
            // 플레이어 위치에서 +-5 범위 안이 걸릴 때까지 랜덤 돌리기
            int index = UnityEngine.Random.Range(0, mazeHeigth * mazeWidth);       // 미로 밖은 선택되지 않게 하기
            x = index / mazeWidth;
            y = index % mazeHeigth;

            limit--;

            // 최대 100번만 시도하기
            if (limit < 1)
            {
                break;
            }
        }
        while (!(x < playerPosition.x + halfSize && x > playerPosition.x - halfSize 
        && y < playerPosition.y + halfSize
        && y > playerPosition.y - halfSize));

        Vector3 world = MazelVisualizer.GridToWorld(x, y);

        return world;
    }

    /// <summary>
    /// 일정 시간 후에 target을 리스폰 시키는 코루틴
    /// </summary>
    /// <param name="target">리스폰 대상</param>
    /// <returns></returns>
    private IEnumerator Respawn(Enemy target)
    {
        yield return new WaitForSeconds(3);

        target.Respawn(GetRandomSpawnPosition());
    }
}
