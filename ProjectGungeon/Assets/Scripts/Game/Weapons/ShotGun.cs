using UnityEngine;
using QFramework;

namespace QFramework.ProjectGungeon
{
	public partial class ShotGun : GunManager
	{

		public override PlayerBullet bulletPrefab => PlayerBullet; // 主角子弹模板
		public override AudioSource audioPlayer => SelfAudioSource; // 音效播放器
		private ShootDuration shootDuration = new ShootDuration(shootDuration: 1.0f, chargeTime: 0f);
		private GunClip gunClip = new GunClip(clipCapacity: 30, currentClipCapacity: 30, bulletBackpack: 60);
		private ShootFire shootFire = new ShootFire();
		public override bool IsReloading => gunClip.isReloading;

		// 更新武器装备状态
		public override void IsEquipped()
		{
			gunClip.UpdateUI();
		}

		private void Update()
		{
			if (shootDuration.Duration > 0)
			{
				shootDuration.Duration -= Time.deltaTime; // 每帧减少冷却时间
			}

			gunClip.BulletReload(reloadSound); // 换弹夹
		}

		// ShotGun的射击方式：鼠标按下后可以同时发射三颗子弹，有较长冷却时间
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
		}

		public override void ShootMouseDown(Vector2 shootDirection)
		{
			if (shootDuration.CanShoot && gunClip.CanShoot && !IsReloading)
			{
				Shoot(shootDirection);

				Vector2 secondBulletDirection = Quaternion.Euler(0, 0, 10) * shootDirection; // 在2D中，X和Y轴代表平面，而Z轴则用于深度或旋转
				Shoot(secondBulletDirection);

				Vector2 thirdBulletDirection = Quaternion.Euler(0, 0, -10) * shootDirection;
				Shoot(thirdBulletDirection);

				shootDuration.Duration = 1.0f;  // 重置冷却计时器

				// 开枪火花
				shootFire.ShowShootFire(bulletPrefab.Position2D(), shootDirection);
			}
		}
	}
}
