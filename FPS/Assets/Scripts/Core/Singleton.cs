using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance = null;
    [Tooltip("�� �̱����� �ʱ�ȭ �Ǿ����� Ȯ��")]
    private bool isInitialized = false;
    [Tooltip("�� �̱����� ����ó���� �Ǿ����� Ȯ��")]
    private static bool isShutdown = false;

    public static T Instance
    {
        get
        {
            if (isShutdown)
            {
                Debug.LogWarning("�̱����� �̹� ���� �� �̴�");

                return null;
            }

            if (instance == null)      // ��ü�� ������
            {
                T singleton = FindObjectOfType<T>();    // �ٸ� ���� ������Ʈ�� �ش� �̱����� ������ Ȯ��

                if (singleton == null)    // �ش� �̱����� ������
                {
                    GameObject obj = new GameObject();      // �� ������Ʈ �����
                    singleton = obj.AddComponent<T>();     // �̸��� ����
                    singleton.name = typeof(T).Name;       // �̱��� ������Ʈ ���� �߰�
                }

                instance = singleton;           // �ٸ� ���� ������Ʈ�� �ִ� �̱����̳� ���� ���� �̱��� ����

                DontDestroyOnLoad(instance.gameObject);      // ���� ����� �� ���� ������Ʈ�� �������� �ʵ��� ����
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)        // ���� �̹� ��ġ�� �ٸ� �̱����� ����.
        {
            instance = this as T;       // ù��°�� ����
            DontDestroyOnLoad(instance.gameObject);     // ���� ����� �� ���ӿ�����Ʈ�� �������� �ʵ��� ����
        }
        else
        {
            if (instance != this)        // �̹� ���� �̱����� �ִ�.
            {
                Destroy(this.gameObject);       // // �� �ڽ��� ����
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// ���� �ε�Ǿ��� �� ȣ��
    /// </summary>
    /// <param name="scene">�� ����</param>
    /// <param name="mode">�ε� ���</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isInitialized)
        {
            OnPreInitialize();
        }

        if (mode != LoadSceneMode.Additive)
        {
            OnInitialize();
        }
    }

    /// <summary>
    /// �̱����� ������� �� �ѹ��� ȣ��
    /// </summary>
    protected virtual void OnPreInitialize()
    {
        isInitialized = true;
    }

    /// <summary>
    /// �̱����� ��������� ���� ����� �� ���� ȣ��(additive�� �ȵ�)
    /// </summary>
    protected virtual void OnInitialize()
    {

    }

    private void OnApplicationQuit()
    {
        isShutdown = true;
    }
}
