using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// where T : RecycleObject - T Ÿ���� RecycleObject�̰ų� RecycleObject�� ��ӹ��� Ŭ������ ����
public class ObjectPool<T> : MonoBehaviour where T : RecycleObject
{
    [Tooltip("Ǯ���� ���� �� ������Ʈ�� ������")]
    public GameObject originalPrefab;

    [Tooltip("Ǯ�� ũ��. ó���� �����ϴ� ������Ʈ�� ����. ��� ������ 2^n�� ��� ���� ����.")]
    public int poolSize = 64;

    [Tooltip("TŸ������ ������ ������Ʈ�� �迭. ������ ��� ������Ʈ�� �ִ� �迭.")]
    private T[] pool;

    [Tooltip("���� ��� ������(��Ȱ��ȭ �Ǿ��ִ�) ������Ʈ���� �����ϴ� ť")]
    private Queue<T> readyQueue;

    public void Initialze()
    {
        // Ǯ�� ���� ��������� �ʴ� ���
        if (pool == null)
        {
            // �迭�� ũ�⸸ŭ new
            pool = new T[poolSize];
            // ����ť�� ����� capacity�� poolSize�� ����
            readyQueue = new Queue<T>(poolSize);

            GenerateObjects(0, poolSize, pool);
        }
        // Ǯ�� �̹� ������� �ִ� ���(ex: ���� �߰��� �ε� or ���� �ٽ� ����)
        else
        {
            // foreach : Ư�� �÷��� �ȿ� �ִ� ��� ��Ҹ� �ѹ��� ó���ؾ� �� ���� ���� �� ���
            foreach (T obj in pool)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Ǯ���� ������� �ʴ� ������Ʈ�� �ϳ� ���� �� �����ϴ� �Լ�
    /// </summary>
    /// <param name="position">��ġ�� ��ġ(���� ��ǥ)</param>
    /// <param name="eulerAngle">��ġ�� ���� ����</param>
    /// <returns>Ǯ���� ���� ������Ʈ(Ȱ��ȭ��)</returns>
    public T GetObject(Vector3? position = null, Vector3? eulerAngle = null)
    {
        // ����ť�� ������Ʈ�� �����ִ��� Ȯ��
        if (readyQueue.Count > 0)
        {
            // ���������� �ϳ� ������
            T comp = readyQueue.Dequeue();

            // ������ ��ġ�� �̵�
            comp.transform.position = position.GetValueOrDefault();
            // ������ ������ ȸ��
            comp.transform.rotation = Quaternion.Euler(eulerAngle.GetValueOrDefault());
            // Ȱ��ȭ
            comp.gameObject.SetActive(true);
            // ������Ʈ �� �߰� ó��
            OnGerateObject(comp);

            return comp;
        }
        // ����ť�� ����ִ� == �����ִ� ������Ʈ�� ����
        else
        {
            // Ǯ�� �ι�� Ȯ��
            ExpandPool();

            // ���� �ϳ� ������
            return GetObject(position, eulerAngle);
        }
    }

    /// <summary>
    /// �� ������Ʈ ���� Ư���� ó���ؾ� �� ���� ���� ��� �����ϴ� �Լ�
    /// </summary>
    /// <param name="component"></param>
    protected virtual void OnGetObject(T component)
    {

    }

    /// <summary>
    /// Ǯ�� �ι�� Ȯ��
    /// </summary>
    private void ExpandPool()
    {
        // �ִ��� �Ͼ�� �ȵǴ� ���̴ϱ� ��� ǥ��
        Debug.LogWarning($"{gameObject.name} Ǯ ������ ����. {poolSize} -> {poolSize * 2}");

        // ���ο� Ǯ�� ũ�� ����
        int newSize = poolSize * 2;
        // ���ο� Ǯ ����
        T[] newPool = new T[newSize];

        // ���� Ǯ�� �ִ� ������ �� Ǯ�� ����
        for (int i = 0; i < poolSize; i++)
        {
            newPool[i] = pool[i];
        }

        // �� Ǯ�� ���� �κп� ������Ʈ ���� �� �߰�
        GenerateObjects(poolSize, newSize, newPool);

        // �� Ǯ ������ ����
        pool = newPool;
        // �� Ǯ�� Ǯ�� ������Ʈ
        poolSize = newSize;
    }

    /// <summary>
    /// Ǯ���� ����� ������Ʈ�� ����
    /// </summary>
    /// <param name="start">���� ���� ������ �ε���</param>
    /// <param name="end">���� ������ ������ �ε��� + 1</param>
    /// <param name="result">������ ������Ʈ�� �� �迭</param>
    private void GenerateObjects(int start, int end, T[] result)
    {
        for (int i = start; i < end; i++)
        {
            // ������ ����
            GameObject obj = Instantiate(originalPrefab, transform);
            // �̸� ����
            obj.name = $"{originalPrefab.name}_{i}";

            T comp = obj.GetComponent<T>();
            // ��Ȱ�� ������Ʈ�� ��Ȱ��ȭ �Ǹ� ����ť�� �ǵ�����
            comp.onDisable += () => readyQueue.Enqueue(comp);
            // readyQueue.Enqueue(comp);             // ����ť�� �߰��ϰ� (���� ��������Ʈ ����� �� ������ �Ʒ����� ��Ȱ��ȭ �ϸ� �ڵ����� ó��)

            OnGerateObject(comp);

            // �迭�� ����
            result[i] = comp;
            // ��Ȱ����
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// �� TŸ�Ժ��� ���� �� ���Ŀ� �ʿ��� �߰� �۾��� ó��
    /// </summary>
    /// <param name="comp"></param>
    protected virtual void OnGerateObject(T comp)
    {

    }
}
