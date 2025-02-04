using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.onGameClear();
        }
    }

    public void SetRandomPosition(int width, int height)
    {
        // 랜덤으로 가장자리 Grid 위치 구하기
        Vector2Int result = new Vector2Int();
        // 0 : 북, 1 : 동, 2 : 남, 3 : 서
        int dir = Random.Range(0, 4);

        switch (dir)
        {
            case 0:
                result.x = Random.Range(0, width);
                result.y = 0;
                break;
            case 1:
                result.x = width - 1;
                result.y = Random.Range(0, height);
                break;
            case 2:
                result.x = Random.Range(0, width);
                result.y = height - 1;
                break;
            case 3:
                result.x = 0;
                result.y = Random.Range(0, height);
                break;
        }

        transform.position = MazelVisualizer.GridToWorld(result.x, result.y);
    }

#if UNITY_EDITOR
    public Vector2Int TestSetRandomPosition(int width, int height)
    {
        // 랜덤으로 가장자리 Grid 위치 구하기
        Vector2Int result = new Vector2Int();
        // 0 : 북, 1 : 동, 2 : 남, 3 : 서
        int dir = Random.Range(0, 4);

        switch (dir)
        {
            case 0:
                result.x = Random.Range(0, width);
                result.y = 0;
                break;
            case 1:
                result.x = width - 1;
                result.y = Random.Range(0, height);
                break;
            case 2:
                result.x = Random.Range(0, width);
                result.y = height - 1;
                break;
            case 3:
                result.x = 0;
                result.y = Random.Range(0, height);
                break;
        }

        return result;
    }
#endif
}
