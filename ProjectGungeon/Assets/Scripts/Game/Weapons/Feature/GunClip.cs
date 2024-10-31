using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunClip
{
    public int BulletCapacity { get; set; }
    public int CurrentBulletCapacity { get; set; }

    // 允许射击的条件：有子弹
    public bool CanShoot => CurrentBulletCapacity > 0;

    // 构造器
    public GunClip(int bulletCapacity, int currentBulletCapacity)
    {
        BulletCapacity = bulletCapacity;
        CurrentBulletCapacity = currentBulletCapacity;
    }

    public void UpdateUI()
    {
        GameUI.UpdateWeaponInfo(this);
    }

    // 子弹消耗
    public void UseBullet()
    {
        CurrentBulletCapacity--;
        UpdateUI();
    }

    // 更换弹夹
    public void BulletReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CurrentBulletCapacity = BulletCapacity;
            UpdateUI();
        }
    }
}
