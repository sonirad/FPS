using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test14_EnemySpawner : TestBase
{
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Enemy enemy = FindAnyObjectByType<Enemy>();

        enemy.OnAttacked(HitLocation.Body, 100000);
    }
}
