using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private CinemachineVirtualCamera followCamaer;
    private Player player;

    public CinemachineVirtualCamera FollowCamera => followCamaer;
    public Player Player => player;

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        GameObject obj = GameObject.FindWithTag("Follow_Camera");

        if (obj != null)
        {
            followCamaer = obj.GetComponent<CinemachineVirtualCamera>();
        }
    }
}
