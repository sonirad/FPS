using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    // HP ����
    [SerializeField] private float hp = 30.0f;             // �׽�Ʈ��
    [SerializeField] private float maxHP = 30.0f;          // ������ �߱��Ѵٸ� �����. ��ü�������δ� �� �ùٸ�

    private BehaviorState state = BehaviorState.Dead;
    [Tooltip("�� ���°� �Ǿ��� �� ���º� ������Ʈ �Լ��� �����ϴ� ��������Ʈ(�Լ������� ��Ȱ)")]
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
