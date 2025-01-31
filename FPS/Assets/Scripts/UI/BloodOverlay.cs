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

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = color;
    }

    private void Start()
    {
        GameManager.Instance.Player.onHPChange += OnHPChange;

        // 비율 계산할 때 / 대신 *로 처리하기 위해 미리 계산해 놓기
        inverseMaxHP = 1 / GameManager.Instance.Player.MaxHP;
    }

    private void OnHPChange(float health)
    {
        color.a = curve.Evaluate(1 - (health * inverseMaxHP));
        image.color = color;
    }
}