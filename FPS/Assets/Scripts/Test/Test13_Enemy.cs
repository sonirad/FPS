using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
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

#if UNITY_EDITOR
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Vector3 pos = enemy.Test_GetRandomPosition();
        Debug.Log(pos);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // OnStateEnter���� �� ���� ����
        // ��ȸ : ���
        // ���� : ��Ȳ
        // Ž�� : �Ķ�
        // ���� : ����
        // ��� : ����
        enemy.Test_StateChange(behaviorState);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        enemy.Test_EnemyStop();
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        Factory.Instance.GetDropItem(Enemy.ItemTable.Shotgun, respawn.position);
    }
#endif
}
