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
        Idle = 0,      // ������, ���ڸ����� ������ �ֱ�
        Wander,      // ��ȸ����, �ֺ��� �Դٰ��� �Ѵ�.
        Chase,         // ��ô����, �÷��̾ ���������� ��ݵ� ��Ҹ� ���� ��� �̵��Ѵ�.
        Find,            // Ž������, ���� ���߿� �÷��̾ �þ߿��� ������� �ֺ��� ã�´�.
        Attack,             // ���ݻ���, �÷��̾ �������� �ȿ� ������ ���� �ֱ�� �����Ѵ�.
        Dead            // �������, ����.(���� �ð� �� �����)
    }

    /// <summary>
    /// ���� ����� �������� ������ ��Ÿ���� enum
    /// </summary>
    public enum ItemTable : byte
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
    [Tooltip("���� ���� �ȿ� ���Դ��� �����ϴ� ����")]
    private AttackSensor attackSensor;

    // Ž�� ����
    [Tooltip("Ž�� ���¿��� ��ȸ ���·� ���ư������ �ɸ��� �ð�")]
    public float findTime = 5.0f;
    [Tooltip("Ž�� ���� �ð�")]
    private float findTimeElapsed = 0.0f;
    [Tooltip("���� ���")]
    private Transform chaseTarget = null;

    // ��Ÿ
    private NavMeshAgent agent;

    // �� ����
    [Tooltip("���º� ���� �� ����")]
    [ColorUsage(false, true)]
    public Color[] stateEyeColors;
    [Tooltip("���� ���͸���")]
    private Material eyeMaterial;
    [Tooltip("�� ���� ID")]
    readonly int EyeColorID = Shader.PropertyToID("_Eye_Color");

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
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.radius = sightRange;
        Transform child = transform.GetChild(1);
        attackSensor = child.GetComponent<AttackSensor>();

        attackSensor.onSensorTriggered += (target) =>
        {
            // Attack ���¿��� �ѹ��� ����
            if (attackTarget == null)
            {
                attackTarget = target.GetComponent<Player>();
                // enemy�� ���������� ��ġ�� ����ǰ� ��ü�� ������� ������ ��������Ʈ���� ������ �ʿ䰡 ����
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
    /// Ư�� ���°� �Ǿ��� ���� ó���� ����
    /// </summary>
    /// <param name="newState">�� ����</param>
    private void OnStateEnter(BehaviorState newState)
    {
        eyeMaterial.SetColor(EyeColorID, stateEyeColors[(int)newState]);

        switch (newState)
        {
            case BehaviorState.Idle:
                onUpdate = Udate_Idle;
                agent.speed = 0.0f;
                attackSensor.gameObject.SetActive(false);
                // ���� ���� ��Ű��
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
                // �����ʿ��� ��Ȱ ��û��
                onDie?.Invoke(this);
                gameObject.SetActive(false);
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
        // Debug.Log("�÷��̾� ����");
        // �ǰ� ���� ǥ�ø� ���� enemy ��ü�� �ѱ�
        attackTarget.OnAttacked(this);
    }

    /// <summary>
    /// ���� ���¿��� ��ȸ ���·� ��ȯ
    /// </summary>
    private void ReturnWander()
    {
        State = BehaviorState.Wander;
    }

    /// <summary>
    /// ���� ������ ó��
    /// </summary>
    /// <param name="hit">���� ����</param>
    /// <param name="damage">������</param>
    public void OnAttacked(HitLocation hit, float damage)
    {
        HP -= damage;

        switch (hit)
        {
            case HitLocation.Body:
                Debug.Log("���뿡 �¾Ҵ�");
                break;
            case HitLocation.Head:
                hp -= damage;
                Debug.Log("�Ӹ��� �¾Ҵ�");
                break;
            case HitLocation.Arm:
                attackPowerPenalty += 0.1f;
                Debug.Log("�ȿ� �¾Ҵ�");
                break;
            case HitLocation.Leg:
                speedPenalty += 0.3f;
                Debug.Log("�ٸ��� �¾Ҵ�");
                break;
        }

        if (State == BehaviorState.Wander || State == BehaviorState.Find)
        {
            // ������ ��� ������ �����Ѵ�
            State = BehaviorState.Chase;
            agent.SetDestination(GameManager.Instance.Player.transform.position);
        }
        else
        {
            agent.speed = runSpeed * (1 - speedPenalty);
        }
    }

    /// <summary>
    /// �÷��̾� ã�� �õ�
    /// </summary>
    /// <returns>t : �÷��̾� ã�Ҵ�, f : ��ã�Ҵ�.</returns>
    private bool FindPlayer()
    {
        bool result = false;

        // ���� ����� �����ϰ�
        if (chaseTarget != null)
        {
            // �þ� ���� �ȿ� ������ �÷��̾ ã�� ��
            result = IsPlayerInSight(out _);
        }

        return result;
    }

    /// <summary>
    /// �÷��̾ �þ� �����ȿ� �ִ��� Ȯ��
    /// </summary>
    /// <param name="position">�÷��̾ �þ� ���� �ȿ� ���� �� �÷��̾��� ��ġ</param>
    /// <returns>t : �þ� ���� �ȿ� �ִ�, f : �þ� ���� �ȿ� ����.</returns>
    private bool IsPlayerInSight(out Vector3 position)
    {
        bool result = false;
        position = Vector3.zero;

        // �þ� ���� Ʈ���� �ȿ� ���Դ��� Ȯ��
        if (chaseTarget != null)
        {
            Vector3 dir = chaseTarget.position - transform.position;
            // �� �����̿��� ���۵Ǵ� ���� ����
            Ray ray = new(transform.position + Vector3.up * 1.9f, dir);

            if (Physics.Raycast(ray, out RaycastHit hit, sightRange, LayerMask.GetMask("Player", "Wall")))
            {
                if (hit.transform == chaseTarget)
                {
                    // �÷��̾�� �� ���̿� ������ ���� ����.
                    float angle = Vector3.Angle(transform.forward, dir);

                    if (angle * 2 < sightAngle)
                    {
                        // �÷��̾ �þ߰� �ȿ� �ִ�.
                        position = chaseTarget.position;
                        result = true;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// �ֺ��� �θ��� �Ÿ��� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    private IEnumerator LookAround()
    {
        Vector3[] positions =
        {
            transform.position + transform.forward * 1.5f,    // ��
            transform.position - transform.forward * 1.5f,    // ��
            transform.position + transform.right * 1.5f,    // ������
            transform.position - transform.right * 1.5f    // ����
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
    /// ���� ������
    /// </summary>
    /// <param name="spawnPosition">������ �� ��ġ</param>
    /// <param name="init">ù ���� ����(T : ù��° ������)</param>
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
    /// ���� �����̰� �����ϰ� ����
    /// </summary>
    public void Play()
    {
        State = BehaviorState.Wander;
    }
    
    /// <summary>
    /// ���� �� �����̰� ����
    /// </summary>
    public void Stop()
    {
        State = BehaviorState.Idle;
    }

    /// <summary>
    /// �������� ���
    /// </summary>
    /// <param name="table">����� ������</param>
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
            // �÷��̾ ã������ Chase ���·� ����
            State = BehaviorState.Chase;
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // �������� ���������� �ٽ� ���� ��ġ�� �̵�
            agent.SetDestination(GetRandomDestination());
        }
    }

    private void Update_Chase()
    {
        if (IsPlayerInSight(out Vector3 position))
        {
            // ������ ��� ��Ҹ� �������� ���� ����
            agent.SetDestination(position);
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // �÷��̾ �Ⱥ��̰� ������ ������� �����ߴ� => ã�� ���·� ��ȭ
            State = BehaviorState.Find;
        }
    }

    private void Update_Find()
    {
        findTimeElapsed += Time.deltaTime;

        if (findTimeElapsed > findTime)
        {
            // ���� �ð��� ���� ������ �÷��̾ �� ã�� -> ��ȸ ���·� ����
            State = BehaviorState.Wander;
        }
        else if (FindPlayer())
        {
            // �÷��̾� ã�Ҵ� -> ����
            State = BehaviorState.Chase;
        }
    }

    private void Update_Atttack()
    {
        // �ѹ� ���� ���¿� ���� ������ �Ѿƿ´�.
        agent.SetDestination(attackTarget.transform.position);
        
        // ���� �÷��̾� �ٶ󺸰� �����
        Quaternion target = Quaternion.LookRotation(attackTarget.transform.position - transform.position);
        // ���� �÷��̾ �ٶ󺸴� ȸ��
        transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime);

        // ���� �����ϱ�
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
