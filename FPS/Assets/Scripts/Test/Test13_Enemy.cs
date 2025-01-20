using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test13_Enemy : TestBase
{
    public Enemy enemy;
    public Transform respawn;

    private void Awake()
    {
        enemy.Respawn(respawn.position);
    }
}
