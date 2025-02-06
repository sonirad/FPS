using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test16_Goal : TestBase
{
    private void Start()
    {
        // Goal goal = FindAnyObjectByType<Goal>();

        // goal.SetRandomPosition(GameManager.Instance.MazeWidth, GameManager.Instance.MazeHeight);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Enemy enemy = FindAnyObjectByType<Enemy>();

        enemy.HP -= 10000;
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Goal goal = FindAnyObjectByType<Goal>();
        int size = GameManager.Instance.MazeWidth * GameManager.Instance.MazeHeight;
        int[] counter = new int[size];

        for (int i = 0; i < 10000000; i++)
        {
            Vector2Int result = goal.TestSetRandomPosition(GameManager.Instance.MazeWidth, GameManager.Instance.MazeHeight);

            if (result.x == 1 && result.y == 1)
            {
                Debug.Log("Not Valid");
            }

            int index = result.x + result.y * GameManager.Instance.MazeWidth;
            counter[index]++;
        }

        Debug.Log("Check complete");

        for (int i = 0; i < size; i++)
        {
            Debug.Log($"{i} : {counter[i]}");
        }
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        Goal goal = FindAnyObjectByType<Goal>();

        goal.SetRandomPosition(GameManager.Instance.MazeWidth, GameManager.Instance.MazeHeight);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        GameManager.Instance.onGameEnd += (_) => Debug.Log("Goal In");
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        GameManager.Instance.GameStart();
    }
}
