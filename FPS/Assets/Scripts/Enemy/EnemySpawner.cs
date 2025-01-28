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
        // �� ����
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject obj = Instantiate(enemyPrefab, transform);
            obj.name = $"Enemy_{i}";
            Enemy enemy = obj.GetComponent<Enemy>();

            enemy.onDie += (target) =>
            {
                StartCoroutine(Respawn(target));
            };

            // �̷� ũ�� ��������
            mazeWidth = GameManager.Instance.MazeWidth;
            mazeHeigth = GameManager.Instance.MazeHeight;
        }
    }

    /// <summary>
    /// �÷��̾� �ֺ��� ������ ���� ��ġ�� ����
    /// </summary>
    /// <returns>���� ��ġ(�̷� ��ĭ�� ���� ���µ� ����)</returns>
    private Vector3 GetRandomSpawnPosition()
    {
        return Vector3.zero;
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
