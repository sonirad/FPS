using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletCount : MonoBehaviour
{
    private TextMeshProUGUI current;
    private TextMeshProUGUI max;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        current = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        max = child.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;

        player.AddAmmoCountChangeDelegate(OnAmmoCountChane);

        player.onGunChange += OnGunChange;
    }

    /// <summary>
    /// �Ѿ� ���� ���� �� ����
    /// </summary>
    /// <param name="count"></param>
    private void OnAmmoCountChane(int count)
    {
        current.text = count.ToString();
    }

    /// <summary>
    /// ���� ����� �� ����
    /// </summary>
    /// <param name="gun"></param>
    private void OnGunChange(GunBase gun)
    {
        max.text = gun.clipSize.ToString();
    }
}
