using UnityEngine;
using QFramework;
using QFramework.ProjectGungeon;

namespace QFramework.Project
{
    public partial class Bow : GunManager
    {
        public override PlayerBullet bulletPrefab => PlayerBullet; // 主角子弹模板
        public override AudioSource audioPlayer => SelfAudioSource; // 音效播放器
        private ShootDuration shootDuration = new ShootDuration(shootDuration: 0.5f, chargeTime: 0.5f);
        private GunClip gunClip = new GunClip(clipCapacity: 30, currentClipCapacity: 30, bulletBackpack: 60);
        float chargeTime = 0; // 蓄力时间
        public override bool IsReloading => gunClip.isReloading;

        // 更新武器装备状态
        public override void IsEquipped()
        {
            if (gunClip.CurrentClipCapacity == 0) // 主角当前弹夹子弹数为0时，显示提示文字
            {
                Player.Default.DisplayTextOnPlayer("No Arrow!", 2);
            }
            gunClip.UpdateUI();
        }

        private void Update()
        {
            gunClip.BulletReload(reloadSound); // 换弹夹
        }

        // Bow的射击方式：鼠标按下蓄力，鼠标松开发射，发射完毕后填一支新箭 
        // 射击逻辑
        public void Shoot(Vector2 shootDirection)
        {
            var playerBullet = Instantiate(bulletPrefab);

            // 子弹发射的位置就是子弹模板的位置（枪口处）
            playerBullet.transform.position = bulletPrefab.transform.position;
            playerBullet.velocity = shootDirection.normalized * 10;
            playerBullet.transform.right = shootDirection; // 调整箭的飞行角度
            playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活
            playerBullet.gunDamage = 1;

            // 播放随机射击音效
            var soundsIndex = Random.Range(0, shootSounds.Count);
            audioPlayer.clip = shootSounds[soundsIndex];
            audioPlayer.Play();

            // 子弹消耗
            gunClip.UseBullet();

            // 更新武器装备状态
            IsEquipped();
        }

        // 按下鼠标：搭箭
        public override void ShootMouseDown(Vector2 shootDirection)
        {
            if (gunClip.CurrentClipCapacity > 0) // 如果还有子弹就显示蓄力箭，没有子弹就不显示
            {
                PrepareArrow.gameObject.SetActive(true);
            }
            else
            {
                PrepareArrow.gameObject.SetActive(false);
            }
        }

        // 按住鼠标：蓄力
        public override void Shooting(Vector2 shootDirection)
        {
            chargeTime += Time.deltaTime; // 累积蓄力时间

            if (gunClip.CurrentClipCapacity > 0)
            {
                PrepareArrow.gameObject.SetActive(true);
            }
            else
            {
                PrepareArrow.gameObject.SetActive(false);
            }
        }

        // 放开鼠标：发射
        public override void ShootMouseUp(Vector2 shootDirection)
        {
            if (gunClip.CurrentClipCapacity > 0 && chargeTime > 0.5f && !IsReloading)
            {
                PrepareArrow.gameObject.SetActive(false); // 隐藏蓄力箭
                Shoot(shootDirection);
                shootDuration.Duration = 0.5f;
                chargeTime = 0f; // 重置蓄力时间
            }
        }
    }
}
