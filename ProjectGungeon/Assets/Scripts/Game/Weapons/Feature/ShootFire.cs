using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public class ShootFire
    {
        public void ShowShootFire(Vector2 bulletPrefabPosition, Vector2 shootDirection)
        {
            Player.Default.weaponFire.Position2D(bulletPrefabPosition); // 火花位置就是子弹位置
            Player.Default.weaponFire.transform.right = shootDirection;
            Player.Default.weaponFire.Show();

            ActionKit.DelayFrame(3, () => // 在当前场景里延迟 3 帧执行隐藏火花图
            {
                Player.Default.weaponFire.Hide();
            }).StartCurrentScene();
        }
    }
}
