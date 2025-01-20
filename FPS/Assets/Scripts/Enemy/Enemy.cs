using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/// <summary>
/// 적이 맞을 수 있는 부위
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
        Wander = 0,      // 배회상태, 주변을 왔다갔다 한다.
        Chase,         // 추척상태, 플레이어가 마지막으로 목격된 장소를 향해 계속 이동한다.
        Find,            // 탐색상태, 추적 도중에 플레이어가 시야에서 사라지면 주변을 찾는다.
        Attack,             // 공격상태, 플레이어가 일정범위 안에 들어오면 일정 주기로 공격한다.
        Dead            // 사망상태, 죽음.(일정 시간 후 재생성)
    }

    /// <summary>
    /// 적이 드랍할 아이템의 종류를 나타내는 enum
    /// </summary>
    private enum ItemTable : byte
    {
        Heal,      // 힐 아이템
        AssaultRifle,      // 돌격소총
        Shotgun,      // 샷건
        Random        // 랜덤
    }

    // HP 관련
    [Tooltip("현재 HP")]
    [SerializeField] private float hp = 30.0f;             // 테스트용
    [Tooltip("최대 HP")]
    [SerializeField] private float maxHP = 30.0f;          // 성능을 추구한다면 비관장. 객체지향으로는 더 올바름

    // 이동 관련
    [Tooltip("이동 속도(배회 및 찾기 상태에서 사용)")]
    public float walkSpeed = 2.0f;
    [Tooltip("이동 속도(추적 및 공격 상태에서 사용)")]
    public float runSpeed = 7.0f;
    [Tooltip("이동 패널티(다리를 맞으면 증가)")]
    private float speedPenalty = 0;

    // 시야 관련
    [Tooltip("적의 시야 각")]
    public float sightAngle = 90.0f;
    [Tooltip("적의 시야 범위")]
    public float sightRange = 20.0f;

    // 공격 관련
    [Tooltip("공격 대상")]
    private Player attackTarget = null;
    [Tooltip("공격력")]
    public float attackPower = 10.0f;
    [Tooltip("공격 시간 간격")]
    public float attackInterval = 1.0f;
    [Tooltip("공격 시간 측정용")]
    private float attackElapsed = 0;
    [Tooltip("공격 패널티 정도")]
    private float attackPowerPenalty = 0;

    // 탐색 관련
    [Tooltip("탐색 상태에서 배회 상태로 돌아가기까지 걸리는 시간")]
    public float findTime = 5.0f;
    [Tooltip("탐색 진행 시간")]
    private float findTimeElapsed = 0.0f;

    // 기타
    private NavMeshAgent agent;

    [Tooltip("적의 현재 상태")]
    private BehaviorState state = BehaviorState.Dead;
    [Tooltip("각 상태가 되었을 때 상태별 업데이트 함수를 저장하는 델리게이트(함수포인터 역활)")]
    private Action onUpdate = null;
    [Tooltip("사망 시 실행될 델리게이트")]
    public Action<Enemy> onDie;

    /// <summary>
    /// 적의 상태 확인 및 설정용 프로퍼티
    /// </summary>
    private BehaviorState State
    {
        get => state;
        set
        {
            // 상태가 달라지면
            if (state != value)
            {
                // 이전 상태에서 나가기 처리 실행
                OnStateExit(state);
                state = value;
                // 새 상태에 들어가기 실행
                OnStateEnter(state);
            }
        }
    }

    /// <summary>
    /// HP 설정 및 확인용 프로퍼티
    /// </summary>
    public float HP
    {
        get => hp;
        set
        {
            hp = value;

            if (hp <= 0)
            {
                // HP가 0 이하면 사망
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
    /// 특정 상태가 되었을 때의 처리를 실행
    /// </summary>
    /// <param name="newState">새 상태</param>
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
    /// 특정 상태에서 나갈 때의 처리를 실행
    /// </summary>
    /// <param name="newState">새 상태</param>
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
    /// 배회하기 위해 랜덤한 위치를 돌려줌.
    /// </summary>
    /// <returns>랜덤한 배회용 목적지</returns>
    private Vector3 GetRandomDestination()
    {
        int range = 3;
        Vector2Int current = MazelVisualizer.WorldToGrid(transform.position);
        int x = UnityEngine.Random.Range(current.x - range, current.x + range + 1);
        int y = UnityEngine.Random.Range(current.y - range, current.y + range + 1);

        return MazelVisualizer.GridToWorld(x, y);
    }

    /// <summary>
    /// 플레이어를 공격
    /// </summary>
    private void Attack()
    {

    }

    /// <summary>
    /// 공격 담함을 처리
    /// </summary>
    /// <param name="hit">맞은 부위</param>
    /// <param name="damage">데미지</param>
    public void OnAttacked(HitLocation hit, float damage)
    {
        // 맞으면 즉시 추적에 돌입한다.
    }

    /// <summary>
    /// 플레이어 찾는 시도
    /// </summary>
    /// <returns>t : 플레이어 찾았다, f : 못찾았다.</returns>
    private bool FindPlayer()
    {
        return false;
    }

    /// <summary>
    /// 플레이어가 시야 범위안에 있는지 확인
    /// </summary>
    /// <param name="position">플레이어가 시야 범위 안에 있을 때 플레이어의 위치</param>
    /// <returns>t : 시야 범위 안에 있다, f : 시야 범위 안에 없다.</returns>
    private bool IsPlayerInSight(out Vector3 position)
    {
        position = Vector3.zero;

        return false;
    }

    /// <summary>
    /// 주변을 두리번 거리는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator LookAround()
    {
        yield return null;
    }

    /// <summary>
    /// 적을 리스폰
    /// </summary>
    /// <param name="spawnPosition">리스폰 할 위치</param>
    public void Respawn(Vector3 spawnPosition)
    {
        agent.Warp(spawnPosition);

        State = BehaviorState.Wander;
    }

    /// <summary>
    /// 아이템을 드랍
    /// </summary>
    /// <param name="table">드랍할 아이템</param>
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
