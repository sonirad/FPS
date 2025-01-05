using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunBase : MonoBehaviour
{
    // 사정 거리
    // 탄창 (탄창 크기, 탄창에 남아있는 총알 수)
    // 데미지
    // 연사 속도
    // 탄 퍼짐
    // 총 반동

    [Tooltip("사정 거리")]
    public float range;
    [Tooltip("탄창 크기")]
    public float clipSize;
    [Tooltip("장전된 총알 수( == 남은 총알 수)")]
    private int bulletCount;
    [Tooltip("총의 데이미(총알 한발당 데미지)")]
    public float damage;
    [Tooltip("총의 연삿 속도")]
    public float fireRate;
    [Tooltip("탄 터짐 각도")]
    public float spread;
    [Tooltip("총 반동")]
    public float recoil;
    [Tooltip("총알이 발사되는 트랜스폼(플레이어의 카메라 위치)")]
    protected Transform fireTransform;
    [Tooltip("남은 총알 갯수가 변경되었음을 알리는 델리게이트(int : 남은 총알 갯수)")]
    public Action<int> onBulletCountChange;
    [Tooltip("총알이 한발 발사되었음을 알리는 델리게이트(float : 반동 정도)")]
    public Action<float> onFire;


    protected int BulletCount
    {
        get => bulletCount;
        set
        {
            bulletCount = value;

            onBulletCountChange?.Invoke(bulletCount);    // 총알 갯수가 변경되었음을 알림
        }
    }

    private void Shoot()
    {

    }

    private void Reload()
    {

    }

    public void Equip()
    {
        fireTransform = GameManager.Instance.Player.FireTransform;
    }

    public void UnEquip()
    {

    }

    private void OnDrawGizmos()
    {
        
    }
}
