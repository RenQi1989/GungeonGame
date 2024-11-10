using UnityEngine;
using QFramework;

namespace QFramework.ProjectGungeon
{
	public partial class AWP : GunManager
	{
		public override PlayerBullet bulletPrefab => PlayerBullet; // 主角子弹模板
		public override AudioSource audioPlayer => SelfAudioSource; // 音效播放器
		private ShootDuration shootDuration = new ShootDuration(shootDuration: 1.5f, chargeTime: 0f);
		private GunClip gunClip = new GunClip(clipCapacity: 10, currentClipCapacity: 10, bulletBackpack: 30);
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

		// AWP的射击方式：鼠标按下后发射一颗强力子弹，冷却时间较长
		// 射击逻辑
		public void Shoot(Vector2 shootDirection)
		{
			var playerBullet = Instantiate(bulletPrefab);

			// 子弹发射的位置就是子弹模板的位置（枪口处）
			playerBullet.transform.position = bulletPrefab.transform.position;
			playerBullet.velocity = shootDirection.normalized * 20;
			playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活
			playerBullet.gunDamage = 3;

			// 播放射击音效																				 
			audioPlayer.clip = shootSounds[0];
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
				shootDuration.Duration = 2f; // 重置冷却时间
			}
		}
	}
}
