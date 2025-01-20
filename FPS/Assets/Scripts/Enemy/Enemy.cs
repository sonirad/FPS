using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/// <summary>
/// ���� ���� �� �ִ� ����
/// </summary>
public enum HitLocation : byte
{
    Body,
    Head,
    Arm,
    Leg
}

public class Enemy : MonoBehaviour
{
    public enum BehaviorState : byte
    {
        Wander = 0,      // ��ȸ����, �ֺ��� �Դٰ��� �Ѵ�.
        Chase,         // ��ô����, �÷��̾ ���������� ��ݵ� ��Ҹ� ���� ��� �̵��Ѵ�.
        Find,            // Ž������, ���� ���߿� �÷��̾ �þ߿��� ������� �ֺ��� ã�´�.
        Attack,             // ���ݻ���, �÷��̾ �������� �ȿ� ������ ���� �ֱ�� �����Ѵ�.
        Dead            // �������, ����.(���� �ð� �� �����)
    }

    /// <summary>
    /// ���� ����� �������� ������ ��Ÿ���� enum
    /// </summary>
    private enum ItemTable : byte
    {
        Heal,      // �� ������
        AssaultRifle,      // ���ݼ���
        Shotgun,      // ����
        Random        // ����
    }

    // HP ����
    [Tooltip("���� HP")]
    [SerializeField] private float hp = 30.0f;             // �׽�Ʈ��
    [Tooltip("�ִ� HP")]
    [SerializeField] private float maxHP = 30.0f;          // ������ �߱��Ѵٸ� �����. ��ü�������δ� �� �ùٸ�

    // �̵� ����
    [Tooltip("�̵� �ӵ�(��ȸ �� ã�� ���¿��� ���)")]
    public float walkSpeed = 2.0f;
    [Tooltip("�̵� �ӵ�(���� �� ���� ���¿��� ���)")]
    public float runSpeed = 7.0f;
    [Tooltip("�̵� �г�Ƽ(�ٸ��� ������ ����)")]
    private float speedPenalty = 0;

    // �þ� ����
    [Tooltip("���� �þ� ��")]
    public float sightAngle = 90.0f;
    [Tooltip("���� �þ� ����")]
    public float sightRange = 20.0f;

    // ���� ����
    [Tooltip("���� ���")]
    private Player attackTarget = null;
    [Tooltip("���ݷ�")]
    public float attackPower = 10.0f;
    [Tooltip("���� �ð� ����")]
    public float attackInterval = 1.0f;
    [Tooltip("���� �ð� ������")]
    private float attackElapsed = 0;
    [Tooltip("���� �г�Ƽ ����")]
    private float attackPowerPenalty = 0;

    // Ž�� ����
    [Tooltip("Ž�� ���¿��� ��ȸ ���·� ���ư������ �ɸ��� �ð�")]
    public float findTime = 5.0f;
    [Tooltip("Ž�� ���� �ð�")]
    private float findTimeElapsed = 0.0f;

    // ��Ÿ
    private NavMeshAgent agent;

    [Tooltip("���� ���� ����")]
    private BehaviorState state = BehaviorState.Dead;
    [Tooltip("�� ���°� �Ǿ��� �� ���º� ������Ʈ �Լ��� �����ϴ� ��������Ʈ(�Լ������� ��Ȱ)")]
    private Action onUpdate = null;
    [Tooltip("��� �� ����� ��������Ʈ")]
    public Action<Enemy> onDie;

    /// <summary>
    /// ���� ���� Ȯ�� �� ������ ������Ƽ
    /// </summary>
    private BehaviorState State
    {
        get => state;
        set
        {
            // ���°� �޶�����
            if (state != value)
            {
                // ���� ���¿��� ������ ó�� ����
                OnStateExit(state);
                state = value;
                // �� ���¿� ���� ����
                OnStateEnter(state);
            }
        }
    }

    /// <summary>
    /// HP ���� �� Ȯ�ο� ������Ƽ
    /// </summary>
    public float HP
    {
        get => hp;
        set
        {
            hp = value;

            if (hp <= 0)
            {
                // HP�� 0 ���ϸ� ���
                State = BehaviorState.Dead;
            }
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        onUpdate();
    }

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    /// <summary>
    /// Ư�� ���°� �Ǿ��� ���� ó���� ����
    /// </summary>
    /// <param name="newState">�� ����</param>
    private void OnStateEnter(BehaviorState newState)
    {
        switch (newState)
        {
            case BehaviorState.Wander:
                onUpdate = Update_Wander;
                agent.speed = walkSpeed;
                agent.SetDestination(GetRandomDestination());
                break;
            case BehaviorState.Chase:
                break;
            case BehaviorState.Find:
                break;
            case BehaviorState.Attack:
                break;
            case BehaviorState.Dead:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Ư�� ���¿��� ���� ���� ó���� ����
    /// </summary>
    /// <param name="newState">�� ����</param>
    private void OnStateExit(BehaviorState oldState)
    {
        switch (oldState)
        {
            case BehaviorState.Wander:
                break;
            case BehaviorState.Chase:
                break;
            case BehaviorState.Find:
                break;
            case BehaviorState.Attack:
                break;
            case BehaviorState.Dead:
                gameObject.SetActive(true);
                HP = maxHP;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ��ȸ�ϱ� ���� ������ ��ġ�� ������.
    /// </summary>
    /// <returns>������ ��ȸ�� ������</returns>
    private Vector3 GetRandomDestination()
    {
        int range = 3;
        Vector2Int current = MazelVisualizer.WorldToGrid(transform.position);
        int x = UnityEngine.Random.Range(current.x - range, current.x + range + 1);
        int y = UnityEngine.Random.Range(current.y - range, current.y + range + 1);

        return MazelVisualizer.GridToWorld(x, y);
    }

    /// <summary>
    /// �÷��̾ ����
    /// </summary>
    private void Attack()
    {

    }

    /// <summary>
    /// ���� ������ ó��
    /// </summary>
    /// <param name="hit">���� ����</param>
    /// <param name="damage">������</param>
    public void OnAttacked(HitLocation hit, float damage)
    {
        // ������ ��� ������ �����Ѵ�.
    }

    /// <summary>
    /// �÷��̾� ã�� �õ�
    /// </summary>
    /// <returns>t : �÷��̾� ã�Ҵ�, f : ��ã�Ҵ�.</returns>
    private bool FindPlayer()
    {
        return false;
    }

    /// <summary>
    /// �÷��̾ �þ� �����ȿ� �ִ��� Ȯ��
    /// </summary>
    /// <param name="position">�÷��̾ �þ� ���� �ȿ� ���� �� �÷��̾��� ��ġ</param>
    /// <returns>t : �þ� ���� �ȿ� �ִ�, f : �þ� ���� �ȿ� ����.</returns>
    private bool IsPlayerInSight(out Vector3 position)
    {
        position = Vector3.zero;

        return false;
    }

    /// <summary>
    /// �ֺ��� �θ��� �Ÿ��� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    private IEnumerator LookAround()
    {
        yield return null;
    }

    /// <summary>
    /// ���� ������
    /// </summary>
    /// <param name="spawnPosition">������ �� ��ġ</param>
    public void Respawn(Vector3 spawnPosition)
    {
        agent.Warp(spawnPosition);

        State = BehaviorState.Wander;
    }

    /// <summary>
    /// �������� ���
    /// </summary>
    /// <param name="table">����� ������</param>
    private void DropItem(ItemTable table = ItemTable.Random)
    {

    }

    private void Update_Wander()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(GetRandomDestination());
        }
    }

    private void Update_Chase()
    {

    }

    private void Update_Find()
    {

    }

    private void Update_Atttack()
    {

    }

    private void Update_Dead()
    {

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
    }

    public Vector3 Test_GetRandomPosition()
    {
        return GetRandomDestination();
    }
#endif
}
