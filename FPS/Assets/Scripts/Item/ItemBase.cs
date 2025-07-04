using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : RecycleObject
{
    [Tooltip("1초에 회전하는 속도")]
    public float spinSpeed = 360.0f;
    [Tooltip("회전시킬 메시의 트랜스폼")]
    private Transform meshTransform;

    private void Awake()
    {
        meshTransform = transform.GetChild(0);
    }

    private void Update()
    {
        meshTransform.Rotate(Time.deltaTime * spinSpeed * Vector3.up, Space.World);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 30초 후에 자동으로 사라지기
        StartCoroutine(LifeOver(30));
    }

    private void OnTriggerEnter(Collider other)
    {
        // OnItemConsum 실행
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                OnItemConsum(player);
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 아이템을 먹었을 때 실행되는 함수
    /// </summary>
    /// <param name="player">아이템을 먹은 플레이어</param>
    protected virtual void OnItemConsum(Player player)
    {

    }
}