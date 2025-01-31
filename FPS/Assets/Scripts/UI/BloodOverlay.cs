using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class BloodOverlay : MonoBehaviour
{
    public AnimationCurve curve;
    public Color color = Color.clear;
    private Image image;
    private float inverseMaxHP;
    private float targetAlpha = 0;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = color;
    }

    private void Start()
    {
        GameManager.Instance.Player.onHPChange += OnHPChange;

        // ���� ����� �� / ��� *�� ó���ϱ� ���� �̸� ����� ����
        inverseMaxHP = 1 / GameManager.Instance.Player.MaxHP;
    }

    private void Update()
    {
        color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime);
        image.color = color;
    }

    private void OnHPChange(float health)
    {
        targetAlpha = curve.Evaluate(1 - (health * inverseMaxHP));
    }
}