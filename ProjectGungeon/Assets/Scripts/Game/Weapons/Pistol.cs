using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using QFramework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public partial class Pistol : GunManager
    {
        public override PlayerBullet bulletPrefab => PlayerBullet;
        public override AudioSource audioPlayer => SelfAudioSource;
        private ShootDuration shootDuration = new ShootDuration(shootDuration: 0.5f, chargeTime: 0f);
        private GunClip gunClip = new GunClip(clipCapacity: 30, currentClipCapacity: 30, bulletBackpack: 100);
        private ShootFire shootFire = new ShootFire();
        public override bool IsReloading => gunClip.isReloading;

        // 更新武器装备状态
        public override void IsEquipped()
        {
            gunClip.UpdateUI();
        }

        private void Update()
        {
            // 射击冷却判断
            if (shootDuration.Duration > 0)
            {
                shootDuration.Duration -= Time.deltaTime; // 每帧减少冷却时间
            }

            gunClip.BulletReload(reloadSound); // 换弹夹
        }

        // Pistol的射击方式：鼠标按下后发射一颗普通子弹，较短冷却时间
        // 射击逻辑
        public void Shoot(Vector2 shootDirection)
        {
            var playerBullet = Instantiate(bulletPrefab);

            // 子弹发射的位置就是子弹模板的位置（枪口处）
            playerBullet.transform.position = bulletPrefab.transform.position;
            playerBullet.direction = shootDirection;
            playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活
            playerBullet.gunDamage = 1;

            // 播放随机射击音效
            var soundsIndex = Random.Range(0, shootSounds.Count);
            audioPlayer.clip = shootSounds[soundsIndex];
            audioPlayer.Play();

            // 子弹消耗
            gunClip.UseBullet();

            // 开枪火花
            shootFire.ShowShootFire(bulletPrefab.Position2D(), shootDirection);
        }

        public override void ShootMouseDown(Vector2 shootDirection)
        {
            if (shootDuration.CanShoot && gunClip.CanShoot && !IsReloading)
            {
                Shoot(shootDirection);
                shootDuration.Duration = 0.25f; // 重置冷却时间
            }
        }
    }
}
