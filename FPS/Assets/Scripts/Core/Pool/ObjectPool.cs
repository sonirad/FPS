using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// where T : RecycleObject - T 타입은 RecycleObject이거나 RecycleObject를 상속받은 클래스만 가능
public class ObjectPool<T> : MonoBehaviour where T : RecycleObject
{
    [Tooltip("풀에서 관리 할 오브젝트의 프리펩")]
    public GameObject originalPrefab;

    [Tooltip("풀의 크기. 처음에 생성하는 오브젝트의 갯수. 모든 갯수는 2^n로 잡는 것이 좋다.")]
    public int poolSize = 64;

    [Tooltip("T타입으로 지정된 오브젝트의 배열. 생성된 모든 오브젝트가 있는 배열.")]
    private T[] pool;

    [Tooltip("현재 사용 가능한(비활성화 되어있는) 오브젝트들을 관리하는 큐")]
    private Queue<T> readyQueue;

    public void Initialze()
    {
        // 풀이 아직 만들어지지 않는 경우
        if (pool == null)
        {
            // 배열의 크기만큼 new
            pool = new T[poolSize];
            // 레디큐를 만들고 capacity를 poolSize로 지정
            readyQueue = new Queue<T>(poolSize);

            GenerateObjects(0, poolSize, pool);
        }
        // 풀이 이미 만들어져 있는 경우(ex: 씬이 추가로 로딩 or 씬이 다시 시작)
        else
        {
            // foreach : 특정 컬렉션 안에 있는 모든 요소를 한번씩 처리해야 할 일이 일을 때 사용
            foreach (T obj in pool)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 풀에서 사용하지 않는 오브젝트를 하나 꺼낸 후 리턴하는 함수
    /// </summary>
    /// <param name="position">배치될 위치(월드 좌표)</param>
    /// <param name="eulerAngle">배치될 때의 각도</param>
    /// <returns>풀에서 꺼낸 오브젝트(활성화됨)</returns>
    public T GetObject(Vector3? position = null, Vector3? eulerAngle = null)
    {
        // 레디큐에 오브젝트가 남아있는지 확인
        if (readyQueue.Count > 0)
        {
            // 남아있으면 하나 꺼내고
            T comp = readyQueue.Dequeue();

            // 지정된 위치로 이동
            comp.transform.position = position.GetValueOrDefault();
            // 지정된 각도로 회전
            comp.transform.rotation = Quaternion.Euler(eulerAngle.GetValueOrDefault());
            // 활성화
            comp.gameObject.SetActive(true);
            // 오브젝트 별 추가 처리
            OnGerateObject(comp);

            return comp;
        }
        // 레디큐가 비어있다 == 남아있는 오브젝트가 없다
        else
        {
            // 풀을 두배로 확장
            ExpandPool();

            // 새로 하나 꺼낸다
            return GetObject(position, eulerAngle);
        }
    }

    /// <summary>
    /// 각 오브젝트 별로 특별히 처리해야 할 일이 있을 경우 실행하는 함수
    /// </summary>
    /// <param name="component"></param>
    protected virtual void OnGetObject(T component)
    {

    }

    /// <summary>
    /// 풀을 두배로 확장
    /// </summary>
    private void ExpandPool()
    {
        // 최대한 일어나면 안되는 일이니까 경고 표시
        Debug.LogWarning($"{gameObject.name} 풀 사이즈 증가. {poolSize} -> {poolSize * 2}");

        // 새로운 풀의 크기 지정
        int newSize = poolSize * 2;
        // 새로운 풀 생성
        T[] newPool = new T[newSize];

        // 이전 풀에 있던 내용을 새 풀에 복사
        for (int i = 0; i < poolSize; i++)
        {
            newPool[i] = pool[i];
        }

        // 새 풀의 남은 부분에 오브젝트 생성 후 추가
        GenerateObjects(poolSize, newSize, newPool);

        // 새 풀 사이즈 설정
        pool = newPool;
        // 새 풀을 풀로 업데이트
        poolSize = newSize;
    }

    /// <summary>
    /// 풀에서 사용할 오브젝트르 생성
    /// </summary>
    /// <param name="start">새로 생성 시작할 인덱스</param>
    /// <param name="end">새로 생성이 끝나는 인덱스 + 1</param>
    /// <param name="result">새생된 오브잭트가 들어갈 배열</param>
    private void GenerateObjects(int start, int end, T[] result)
    {
        for (int i = start; i < end; i++)
        {
            // 프리팹 생성
            GameObject obj = Instantiate(originalPrefab, transform);
            // 이름 변경
            obj.name = $"{originalPrefab.name}_{i}";

            T comp = obj.GetComponent<T>();
            // 재활용 오브젝트가 비활성화 되면 레이큐로 되돌려라
            comp.onDisable += () => readyQueue.Enqueue(comp);
            // readyQueue.Enqueue(comp);             // 레이큐에 추가하고 (위에 델리게이트 등록한 것 때문에 아래에서 비활성화 하면 자동으로 처리)

            OnGerateObject(comp);

            // 배열에 저장
            result[i] = comp;
            // 비활성하
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// 각 T타입별로 생성 후 직후에 필요한 추가 작업을 처리
    /// </summary>
    /// <param name="comp"></param>
    protected virtual void OnGerateObject(T comp)
    {

    }
}
