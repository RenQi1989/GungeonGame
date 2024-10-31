using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using QFramework;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public partial class Pistol : GunManager
    {
        public override PlayerBullet bulletPrefab => PlayerBullet;
        public override AudioSource audioPlayer => SelfAudioSource;
        private ShootDuration shootDuration;
        private GunClip gunClip;

        private void Start()
        {
            shootDuration = new ShootDuration(shootDuration: 0.5f, chargeTime: 0f);
            gunClip = new GunClip(bulletCapacity: 30, currentBulletCapacity: 30);

            gunClip.UpdateUI();
        }

        private void Update()
        {
            // 射击冷却判断
            if (shootDuration.Duration > 0)
            {
                shootDuration.Duration -= Time.deltaTime; // 每帧减少冷却时间
            }

            gunClip.BulletReload(); // 换弹夹
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

            // 播放随机射击音效
            var soundsIndex = Random.Range(0, shootSounds.Count);
            audioPlayer.clip = shootSounds[soundsIndex];
            audioPlayer.Play();

            // 子弹消耗
            gunClip.UseBullet();
        }

        public override void ShootMouseDown(Vector2 shootDirection)
        {
            if (shootDuration.CanShoot && gunClip.CanShoot)
            {
                Shoot(shootDirection);
                shootDuration.Duration = 0.25f; // 重置冷却时间
            }
        }
    }
}
