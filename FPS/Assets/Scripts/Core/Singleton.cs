using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance = null;
    [Tooltip("이 싱글톤이 초기화 되었는지 확인")]
    private bool isInitialized = false;
    [Tooltip("이 싱글톤이 종료처리가 되었는지 확인")]
    private static bool isShutdown = false;

    public static T Instance
    {
        get
        {
            if (isShutdown)
            {
                Debug.LogWarning("싱글톤은 이미 삭제 중 이다");

                return null;
            }

            if (instance == null)      // 객체가 없으면
            {
                T singleton = FindObjectOfType<T>();    // 다른 게임 오브젝트에 해당 싱글톤이 없는지 확인

                if (singleton == null)    // 해당 싱글톤이 없으면
                {
                    GameObject obj = new GameObject();      // 빈 오브젝트 만들고
                    singleton = obj.AddComponent<T>();     // 이름을 지정
                    singleton.name = typeof(T).Name;       // 싱글톤 컴포넌트 만들어서 추가
                }

                instance = singleton;           // 다른 게임 오브젝트에 있는 싱글톤이나 새로 만든 싱글톤 저장

                DontDestroyOnLoad(instance.gameObject);      // 씬이 사라질 때 게임 오브젝트가 삭제되지 않도록 설정
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)        // 씬에 이미 배치된 다른 싱글톤이 없다.
        {
            instance = this as T;       // 첫번째를 저장
            DontDestroyOnLoad(instance.gameObject);     // 씬이 사라질 때 게임오브젝트가 삭제되지 않도록 설정
        }
        else
        {
            if (instance != this)        // 이미 씬에 싱글톤이 있다.
            {
                Destroy(this.gameObject);       // // 나 자신을 삭제
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
    /// 씬이 로드되었을 때 호출
    /// </summary>
    /// <param name="scene">씬 정보</param>
    /// <param name="mode">로딩 모드</param>
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
    /// 싱글톤이 만들어질 때 한번만 호출
    /// </summary>
    protected virtual void OnPreInitialize()
    {
        isInitialized = true;
    }

    /// <summary>
    /// 싱글톤이 만들어지고 씬이 변경될 때 마다 호출(additive는 안됨)
    /// </summary>
    protected virtual void OnInitialize()
    {

    }

    private void OnApplicationQuit()
    {
        isShutdown = true;
    }
}
