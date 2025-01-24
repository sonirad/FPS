using UnityEngine;

public class Factory : Singleton<Factory>
{
    private BulletHolePool bulletHolePool;
    private AssaultRiflePool assaultRiflePool;
    private ShotgunPool shotgunPool;
    private HealPackPool healPackPool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        bulletHolePool = GetComponentInChildren<BulletHolePool>();
        bulletHolePool?.Initialze();

        assaultRiflePool = GetComponentInChildren<AssaultRiflePool>();
        assaultRiflePool?.Initialze();

        shotgunPool = GetComponentInChildren<ShotgunPool>();
        shotgunPool?.Initialze();

        healPackPool = GetComponentInChildren<HealPackPool>();
        healPackPool?.Initialze();
    }

    public BulletHole GetBulletHole()
    {
        return bulletHolePool?.GetObject();
    }

    public BulletHole GetBulletHole(Vector3 position, Vector3 normal, Vector3 reflect)
    {
        BulletHole hole = bulletHolePool?.GetObject();

        hole.Initialize(position, normal, reflect);

        return hole;
    }

    public GunItem GetAssaultRifleItem(Vector3 position)
    {
        GunItem item = assaultRiflePool?.GetObject();

        item.transform.position = position;

        return item;
    }
    
    public GunItem GetShotgunItem(Vector3 position)
    {
        GunItem item = shotgunPool?.GetObject();

        item.transform.position = position;

        return item;
    }

    public HealItem GetHealPackItem(Vector3 position)
    {
        HealItem item = healPackPool?.GetObject();

        item.transform.position = position;

        return item;
    }

    public ItemBase GetDropItem(Enemy.ItemTable table, Vector3 position)
    {
        ItemBase item = null;

        switch (table)
        {
            case Enemy.ItemTable.AssaultRifle:
                item = GetAssaultRifleItem(position);
                break;
            case Enemy.ItemTable.Shotgun:
                item = GetShotgunItem(position);
                break;
            case Enemy.ItemTable.Heal:
                item = GetHealPackItem(position);
                break;
        }

        return item;
    }
}
