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

    public int BulletBackpack { get; set; } // 背包里的子弹数量

    // 构造器
    public GunClip(int clipCapacity, int currentClipCapacity, int bulletBackpack)
    {
        ClipCapacity = clipCapacity;
        CurrentClipCapacity = currentClipCapacity;
        BulletBackpack = bulletBackpack;
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

    // 计算需要 Reload 的子弹数量
    public int GetReloadAmount()
    {
        return ClipCapacity - CurrentClipCapacity;
    }

    // 判断弹夹是否满了
    public bool Full => CurrentClipCapacity == ClipCapacity;

    // 更换弹夹
    public void BulletReload(AudioClip reloadSound)
    {
        // 只有在按下 R 键时才执行重装逻辑
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (BulletBackpack > 0)
            {
                isReloading = true;
                int amountToReload = GetReloadAmount();
                int bulletsToTake = Mathf.Min(amountToReload, BulletBackpack);

                // 更新弹夹容量和背包子弹数量，确保不会超过 ClipCapacity
                CurrentClipCapacity = Mathf.Min(CurrentClipCapacity + bulletsToTake, ClipCapacity);
                BulletBackpack -= bulletsToTake;

                ActionKit.Sequence()
                    .PlaySound(reloadSound) // 播放重装音效
                    .Callback(() =>
                    {
                        UpdateUI(); // 更新 UI
                        isReloading = false;
                    })
                    .StartCurrentScene();
            }
        }
    }
}


