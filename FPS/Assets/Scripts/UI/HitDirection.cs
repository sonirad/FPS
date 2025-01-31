using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitDirection : MonoBehaviour
{
    public float duration = 0.5f;
    private float timeElapsed = 0.0f;
    private float inverseDuration;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        inverseDuration = 1 / duration;
        GameManager.Instance.Player.onAttacked += OnPlayerAttacked;
        timeElapsed = duration;
        image.color = Color.clear;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        float alpha = timeElapsed * inverseDuration;
        // 항상 흰색에서 투명색으로 보간이 일어남
        image.color = Color.Lerp(Color.white, Color.clear, alpha);
    }

    private void OnPlayerAttacked(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
        image.color = Color.white;
        timeElapsed = 0;
    }
}