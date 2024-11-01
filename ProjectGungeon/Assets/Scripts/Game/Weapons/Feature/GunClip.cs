using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class GunClip
{
    public int BulletCapacity { get; set; }
    public int CurrentBulletCapacity { get; set; }
    public bool isReloading = false;

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
    public void BulletReload(AudioClip reloadSound)
    {
        // 只有在按下 R 键时才执行重装逻辑
        if (Input.GetKeyDown(KeyCode.R))
        {
            isReloading = true;
            ActionKit.Sequence()
                .PlaySound(reloadSound) // 播放重装音效
                .Callback(() =>
                {
                    CurrentBulletCapacity = BulletCapacity; // 恢复弹夹容量
                    UpdateUI(); // 更新 UI
                    isReloading = false;
                })
                .StartCurrentScene();
        }
    }

}
