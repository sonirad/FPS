using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int enemyCount = 50;
    public GameObject enemyPrefab;
    private int mazeWidth;
    private int mazeHeigth;
    private Player player;
    private Enemy[] enemies;

    private void Awake()
    {
        enemies = new Enemy[enemyCount];
    }

    private void Start()
    {
        // �̷� ũ�� ��������
        mazeWidth = GameManager.Instance.MazeWidth;
        mazeHeigth = GameManager.Instance.MazeHeight;

        player = GameManager.Instance.Player;

        GameManager.Instance.onGameStart += EnemyAll_Play;
        GameManager.Instance.onGameClear += EnemyAll_Stop;
    }

    public void EnemyAll_Spawn()
    {
        // �� ����
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

            enemy.Respawn(GetRandomSpawnPosition(true));
        }
    }

    private void EnemyAll_Play()
    {
        foreach (var enemy in enemies)
        {
            enemy.Play();
        }
    }

    private void EnemyAll_Stop()
    {
        foreach (var enemy in enemies)
        {
            enemy.Stop();
        }
    }

    /// <summary>
    /// �÷��̾� �ֺ��� ������ ���� ��ġ�� ����
    /// </summary>
    /// <returns>���� ��ġ(�̷� ��ĭ�� ���� ���µ� ����)</returns>
    private Vector3 GetRandomSpawnPosition(bool init = false)
    {
        // �÷��̾��� �׸��� ��ġ
        Vector2Int playerPosition;

        if (init)
        {
            // �÷��̾ ���������� �ִٴ� ������ ���� ��� �׳� �̷��� ���µ� ��ġ
            playerPosition = new(mazeWidth / 2, mazeHeigth / 2);
        }
        else
        {
            // �Ϲ� �÷��� �߿��� �÷��̾��� �׸��� ��ġ
            playerPosition = MazelVisualizer.WorldToGrid(player.transform.position);
        }

        int x;
        int y;
        int limit = 100;

        do
        {
            // �÷��̾� ��ġ���� +-5 ���� ���� �ɸ� ������ ���� ������
            int index = Random.Range(0, mazeHeigth * mazeWidth);       // �̷� ���� ���õ��� �ʰ� �ϱ�
            x = index / mazeWidth;
            y = index % mazeHeigth;

            limit--;

            // �ִ� 100���� �õ��ϱ�
            if (limit < 1)
            {
                break;
            }
        }
        while (!(x < playerPosition.x + 5 && x > playerPosition.x - 5 && y < playerPosition.y + 5 && y > playerPosition.y - 5));

        Vector3 world = MazelVisualizer.GridToWorld(x, y);

        return world;
    }

    /// <summary>
    /// ���� �ð� �Ŀ� target�� ������ ��Ű�� �ڷ�ƾ
    /// </summary>
    /// <param name="target">������ ���</param>
    /// <returns></returns>
    private IEnumerator Respawn(Enemy target)
    {
        yield return new WaitForSeconds(3);

        target.Respawn(GetRandomSpawnPosition());
    }
}
