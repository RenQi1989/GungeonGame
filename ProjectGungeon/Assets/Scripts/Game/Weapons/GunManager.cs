using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    [ViewControllerChild]
    public abstract class GunManager : ViewController
    {
        public abstract PlayerBullet bulletPrefab { get; } // 主角子弹模板
        public abstract AudioSource audioPlayer { get; } // 音效播放器
        public List<AudioClip> shootSounds = new List<AudioClip>(); // 主角射击音效列表


        // 点击鼠标
        public virtual void ShootMouseDown(Vector2 shootDirection)
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

        }

        // 按住鼠标
        public virtual void Shooting(Vector2 shootingDirection)
        {

        }

        // 鼠标抬起
        public virtual void ShootMouseUp(Vector2 shootingDirection)
        {

        }
    }
}

