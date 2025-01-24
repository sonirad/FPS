using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test13_Enemy : TestBase
{
    public Enemy enemy;
    public Transform respawn;

    public Enemy.BehaviorState behaviorState = Enemy.BehaviorState.Wander;

    private void Start()
    {
        enemy.Respawn(respawn.position);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Vector3 pos = enemy.Test_GetRandomPosition();
        Debug.Log(pos);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // OnStateEnter¿¡¼­ ´« »ö±ò º¯°æ
        // ¹èÈ¸ : ³ì»ö
        // ÃßÀû : ÁÖÈ²
        // Å½»ö : ÆÄ¶û
        // °ø°Ý : »¡»ó
        // »ç¸Á : °ËÁ¤
        enemy.Test_StateChange(behaviorState);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        enemy.Test_EnemyStop();
    }
}
