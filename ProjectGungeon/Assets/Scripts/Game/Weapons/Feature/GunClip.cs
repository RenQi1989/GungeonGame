using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class GunClip
{
    public int ClipCapacity { get; set; }
    public int CurrentClipCapacity { get; set; }
    public bool isReloading = false;

    // 允许射击的条件：有子弹
    public bool CanShoot => CurrentClipCapacity > 0;

    // 构造器
    public GunClip(int clipCapacity, int currentClipCapacity)
    {
        ClipCapacity = clipCapacity;
        CurrentClipCapacity = currentClipCapacity;
    }

    // 更新UI
    public void UpdateUI()
    {
        GameUI.UpdateWeaponInfo(this);
    }

    // 子弹消耗
    public void UseBullet()
    {
        CurrentClipCapacity--;
        UpdateUI();
    }

    // 计算每次需要 Reload 的子弹数量
    public int GetReloadAmount()
    {
        return ClipCapacity - CurrentClipCapacity;
    }

    // 更换弹夹
    public void BulletReload(AudioClip reloadSound)
    {
        // 只有在按下 R 键时才执行重装逻辑
        if (Input.GetKeyDown(KeyCode.R))
        {

            isReloading = true;
            int amountToReload = GetReloadAmount();

            ActionKit.Sequence()
                .PlaySound(reloadSound) // 播放重装音效
                .Callback(() =>
                {
                    CurrentClipCapacity += amountToReload; // 恢复弹夹容量
                    UpdateUI(); // 更新 UI
                    isReloading = false;
                })
                .StartCurrentScene();

        }
    }
}


