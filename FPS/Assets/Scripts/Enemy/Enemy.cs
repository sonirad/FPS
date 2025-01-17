using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    // HP 관련
    [SerializeField] private float hp = 30.0f;             // 테스트용
    [SerializeField] private float maxHP = 30.0f;          // 성능을 추구한다면 비관장. 객체지향으로는 더 올바름

    private BehaviorState state = BehaviorState.Dead;
    [Tooltip("각 상태가 되었을 때 상태별 업데이트 함수를 저장하는 델리게이트(함수포인터 역활)")]
    private Action onUpdate = null;

    private BehaviorState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                OnStateExit(value);
                state = value;
                OnStateEnter(value);
            }
        }
    }

    public float HP
    {
        get => hp;
        set
        {
            hp = value;

            if (hp <= 0)
            {
                State = BehaviorState.Dead;
            }
        }
    }

    private void OnStateEnter(BehaviorState newState)
    {

    }

    private void OnStateExit(BehaviorState newState)
    {

    }
}
