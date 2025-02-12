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
        Idle = 0,      // 대기상태, 제자리에서 가민히 있기
        Wander,      // 배회상태, 주변을 왔다갔다 한다.
        Chase,         // 추척상태, 플레이어가 마지막으로 목격된 장소를 향해 계속 이동한다.
        Find,            // 탐색상태, 추적 도중에 플레이어가 시야에서 사라지면 주변을 찾는다.
        Attack,             // 공격상태, 플레이어가 일정범위 안에 들어오면 일정 주기로 공격한다.
        Dead            // 사망상태, 죽음.(일정 시간 후 재생성)
    }

    /// <summary>
    /// 적이 드랍할 아이템의 종류를 나타내는 enum
    /// </summary>
    public enum ItemTable : byte
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
    [Tooltip("공격 범위 안에 들어왔는지 감지하는 센서")]
    private AttackSensor attackSensor;

    // 탐색 관련
    [Tooltip("탐색 상태에서 배회 상태로 돌아가기까지 걸리는 시간")]
    public float findTime = 5.0f;
    [Tooltip("탐색 진행 시간")]
    private float findTimeElapsed = 0.0f;
    [Tooltip("추적 대상")]
    private Transform chaseTarget = null;

    // 기타
    private NavMeshAgent agent;

    // 눈 색상
    [Tooltip("상태별 적의 눈 색상")]
    [ColorUsage(false, true)]
    public Color[] stateEyeColors;
    [Tooltip("눈의 머터리얼")]
    private Material eyeMaterial;
    [Tooltip("눈 색의 ID")]
    readonly int EyeColorID = Shader.PropertyToID("_Eye_Color");

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
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.radius = sightRange;
        Transform child = transform.GetChild(1);
        attackSensor = child.GetComponent<AttackSensor>();

        attackSensor.onSensorTriggered += (target) =>
        {
            // Attack 상태에서 한번만 실행
            if (attackTarget == null)
            {
                attackTarget = target.GetComponent<Player>();
                // enemy는 리스폰으로 위치만 변경되고 객체가 사라지지 않으니 델리게이트에서 제거할 필요가 없음
                attackTarget.onDie += ReturnWander;
                State = BehaviorState.Attack;
            }
        };

        // root
        child = transform.GetChild(0);
        // head
        child = child.GetChild(0);
        // eye
        child = child.GetChild(0);

        Renderer eyeRenderer = child.GetComponent<Renderer>();

        eyeMaterial = eyeRenderer.material;
        eyeMaterial.SetColor(EyeColorID, stateEyeColors[(int)BehaviorState.Wander]);

        onUpdate = Udate_Idle;
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
        if (other.CompareTag("Player"))
        {
            chaseTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("Out : " + chaseTarget);
            chaseTarget = null;
        }
    }

    /// <summary>
    /// 특정 상태가 되었을 때의 처리를 실행
    /// </summary>
    /// <param name="newState">새 상태</param>
    private void OnStateEnter(BehaviorState newState)
    {
        eyeMaterial.SetColor(EyeColorID, stateEyeColors[(int)newState]);

        switch (newState)
        {
            case BehaviorState.Idle:
                onUpdate = Udate_Idle;
                agent.speed = 0.0f;
                attackSensor.gameObject.SetActive(false);
                // 공격 정지 시키기
                break;
            case BehaviorState.Wander:
                onUpdate = Update_Wander;
                agent.speed = walkSpeed * (1 - speedPenalty);
                agent.SetDestination(GetRandomDestination());
                break;
            case BehaviorState.Chase:
                onUpdate = Update_Chase;
                agent.speed = runSpeed * (1 - speedPenalty);
                break;
            case BehaviorState.Find:
                onUpdate = Update_Find;
                findTimeElapsed = 0.0f;
                agent.speed = walkSpeed * (1 - speedPenalty);
                agent.angularSpeed = 360.0f;

                StartCoroutine(LookAround());
                break;
            case BehaviorState.Attack:
                onUpdate = Update_Atttack;
                break;
            case BehaviorState.Dead:
                DropItem();

                agent.speed = 0.0f;
                agent.velocity = Vector3.zero;
                // 스포너에게 부활 요청용
                onDie?.Invoke(this);
                gameObject.SetActive(false);
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
            case BehaviorState.Idle:
                agent.speed = walkSpeed;
                attackSensor.gameObject.SetActive(true);
                break;
            case BehaviorState.Find:
                agent.angularSpeed = 120.0f;

                StopAllCoroutines();
                break;
            case BehaviorState.Attack:
                attackTarget.onDie -= ReturnWander;
                attackTarget = null;
                break;
            case BehaviorState.Dead:
                gameObject.SetActive(true);
                HP = maxHP;
                speedPenalty = 0.0f;
                attackPowerPenalty = 0.0f;
                break;
            default:
                //case BehaviorState.Wander:
                //case BehaviorState.Chase:
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
        // Debug.Log("플레이어 공격");
        // 피격 방향 표시를 위해 enemy 자체를 넘김
        attackTarget.OnAttacked(this);
    }

    /// <summary>
    /// 공격 상태에서 배회 상태로 전환
    /// </summary>
    private void ReturnWander()
    {
        State = BehaviorState.Wander;
    }

    /// <summary>
    /// 공격 담함을 처리
    /// </summary>
    /// <param name="hit">맞은 부위</param>
    /// <param name="damage">데미지</param>
    public void OnAttacked(HitLocation hit, float damage)
    {
        HP -= damage;

        switch (hit)
        {
            case HitLocation.Body:
                Debug.Log("몸통에 맞았다");
                break;
            case HitLocation.Head:
                hp -= damage;
                Debug.Log("머리에 맞았다");
                break;
            case HitLocation.Arm:
                attackPowerPenalty += 0.1f;
                Debug.Log("팔에 맞았다");
                break;
            case HitLocation.Leg:
                speedPenalty += 0.3f;
                Debug.Log("다리에 맞았다");
                break;
        }

        if (State == BehaviorState.Wander || State == BehaviorState.Find)
        {
            // 맞으면 즉시 추적에 돌입한다
            State = BehaviorState.Chase;
            agent.SetDestination(GameManager.Instance.Player.transform.position);
        }
        else
        {
            agent.speed = runSpeed * (1 - speedPenalty);
        }
    }

    /// <summary>
    /// 플레이어 찾는 시도
    /// </summary>
    /// <returns>t : 플레이어 찾았다, f : 못찾았다.</returns>
    private bool FindPlayer()
    {
        bool result = false;

        // 추적 대상이 존재하고
        if (chaseTarget != null)
        {
            // 시야 범위 안에 있으면 플레이어를 찾은 것
            result = IsPlayerInSight(out _);
        }

        return result;
    }

    /// <summary>
    /// 플레이어가 시야 범위안에 있는지 확인
    /// </summary>
    /// <param name="position">플레이어가 시야 범위 안에 있을 때 플레이어의 위치</param>
    /// <returns>t : 시야 범위 안에 있다, f : 시야 범위 안에 없다.</returns>
    private bool IsPlayerInSight(out Vector3 position)
    {
        bool result = false;
        position = Vector3.zero;

        // 시야 범위 트리거 안에 들어왔는지 확인
        if (chaseTarget != null)
        {
            Vector3 dir = chaseTarget.position - transform.position;
            // 적 눈높이에서 시작되는 레이 생성
            Ray ray = new(transform.position + Vector3.up * 1.9f, dir);

            if (Physics.Raycast(ray, out RaycastHit hit, sightRange, LayerMask.GetMask("Player", "Wall")))
            {
                if (hit.transform == chaseTarget)
                {
                    // 플레이어와 적 사이에 가리는 것이 없다.
                    float angle = Vector3.Angle(transform.forward, dir);

                    if (angle * 2 < sightAngle)
                    {
                        // 플레이어가 시야각 안에 있다.
                        position = chaseTarget.position;
                        result = true;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 주변을 두리번 거리는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator LookAround()
    {
        Vector3[] positions =
        {
            transform.position + transform.forward * 1.5f,    // 앞
            transform.position - transform.forward * 1.5f,    // 뒤
            transform.position + transform.right * 1.5f,    // 오른쪽
            transform.position - transform.right * 1.5f    // 왼쪽
        };

        int current;
        int prev = 0;
        int length = positions.Length;

        while (true)
        {
            do
            {
                current = UnityEngine.Random.Range(0, length);
            }
            while (current == prev);

            agent.SetDestination(positions[current]);
            prev = current;

            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// 적을 리스폰
    /// </summary>
    /// <param name="spawnPosition">리스폰 할 위치</param>
    /// <param name="init">첫 생성 여부(T : 첫번째 리스폰)</param>
    public void Respawn(Vector3 spawnPosition, bool init = false)
    {
        agent.Warp(spawnPosition);

        if (init)
        {
            State = BehaviorState.Idle;
        }
        else
        {
            State = BehaviorState.Wander;
        }
    }

    /// <summary>
    /// 적을 움직이게 시작하게 만듬
    /// </summary>
    public void Play()
    {
        State = BehaviorState.Wander;
    }
    
    /// <summary>
    /// 적을 안 움직이게 만듬
    /// </summary>
    public void Stop()
    {
        State = BehaviorState.Idle;
    }

    /// <summary>
    /// 아이템을 드랍
    /// </summary>
    /// <param name="table">드랍할 아이템</param>
    private void DropItem(ItemTable table = ItemTable.Random)
    {
        ItemTable select = table;

        if (table == ItemTable.Random)
        {
            float random = UnityEngine.Random.value;

            if (random < 0.8f)
            {
                select = ItemTable.Heal;
            }
            else if (random < 0.9f)
            {
                select = ItemTable.AssaultRifle;
            }
            else
            {
                select = ItemTable.Shotgun;
            }
        }

        Factory.Instance.GetDropItem(select, transform.position);
    }

    private void Udate_Idle()
    {

    }

    private void Update_Wander()
    {
        if (FindPlayer())
        {
            // 플레이어를 찾았으면 Chase 상태로 변경
            State = BehaviorState.Chase;
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 목적지에 도착했으면 다시 랜덤 위치로 이동
            agent.SetDestination(GetRandomDestination());
        }
    }

    private void Update_Chase()
    {
        if (IsPlayerInSight(out Vector3 position))
        {
            // 마지막 목격 장소를 목적지로 새로 설정
            agent.SetDestination(position);
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 플레이어가 안보이고 마지막 목격지에 도착했다 => 찾기 상태로 전화
            State = BehaviorState.Find;
        }
    }

    private void Update_Find()
    {
        findTimeElapsed += Time.deltaTime;

        if (findTimeElapsed > findTime)
        {
            // 일정 시간이 지난 때까지 플레이어를 못 찾음 -> 배회 상태로 변경
            State = BehaviorState.Wander;
        }
        else if (FindPlayer())
        {
            // 플레이어 찾았다 -> 추적
            State = BehaviorState.Chase;
        }
    }

    private void Update_Atttack()
    {
        // 한번 공격 상태에 들어가면 끝까지 쫓아온다.
        agent.SetDestination(attackTarget.transform.position);
        
        // 적이 플레이어 바라보게 만들기
        Quaternion target = Quaternion.LookRotation(attackTarget.transform.position - transform.position);
        // 적이 플레이어를 바라보는 회전
        transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime);

        // 적이 공격하기
        attackElapsed += Time.deltaTime;

        if (attackElapsed > attackInterval)
        {
            Attack();

            attackElapsed = 0.0f;
        }
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

    public void Test_StateChange(BehaviorState state)
    {
        State = state;
        agent.speed = 0.0f;
        agent.velocity = Vector3.zero;
    }

    public void Test_EnemyStop()
    {
        agent.speed = 0.0f;
        agent.velocity = Vector3.zero;
    }
#endif
}
