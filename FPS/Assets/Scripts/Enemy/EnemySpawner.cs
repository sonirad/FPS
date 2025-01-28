using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int enemyCount = 50;
    public GameObject enemyPrefab;
    private int mazeWidth;
    private int mazeHeigth;

    private void Start()
    {
        // 적 생성
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            obj.name = $"Enemy_{i}";
            Enemy enemy = obj.GetComponent<Enemy>();

            enemy.onDie += (target) =>
            {
                StartCoroutine(Respawn(target));
            };

            // 미로 크기 가져오기
            mazeWidth = GameManager.Instance.MazeWidth;
            mazeHeigth = GameManager.Instance.MazeHeight;
        }
    }

    /// <summary>
    /// 플레이어 주변의 랜덤한 스폰 위치를 구함
    /// </summary>
    /// <returns>스폰 위치(미로 한칸의 셀의 가온데 지점)</returns>
    private Vector3 GetRandomSpawnPosition()
    {
        return Vector3.zero;
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
