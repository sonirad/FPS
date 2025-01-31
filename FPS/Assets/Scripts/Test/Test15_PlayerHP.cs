using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test15_PlayerHP : TestBase
{
    [SerializeField] private Enemy enemy;

    private void Start()
    {
        if (enemy == null)
        {
            enemy = FindAnyObjectByType<Enemy>();
        }

        enemy.Respawn(transform.GetChild(0).position);
        StartCoroutine(EnemyStop());
    }

    private IEnumerator EnemyStop()
    {
        yield return new WaitForSeconds(0.1f);

        enemy.Test_EnemyStop();
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // Player HP 감소
        GameManager.Instance.Player.HP -= 10;
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // Player HP 증가
        GameManager.Instance.Player.HP += 10;
    }
}