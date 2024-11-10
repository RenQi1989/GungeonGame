using UnityEngine;
using QFramework;

namespace QFramework.ProjectGungeon
{
	public partial class RocketGun : GunManager
	{
		public override PlayerBullet bulletPrefab => PlayerBullet; // 主角子弹模板
		public override AudioSource audioPlayer => SelfAudioSource; // 音效播放器
		private ShootDuration shootDuration = new ShootDuration(shootDuration: 2.0f, chargeTime: 0f);
		private GunClip gunClip = new GunClip(clipCapacity: 15, currentClipCapacity: 15, bulletBackpack: 30);
		public override bool IsReloading => gunClip.isReloading;

		// 更新武器装备状态
		public override void IsEquipped()
		{
			gunClip.UpdateUI();
		}
		//private void Start()
		//{
		//shootDuration = new ShootDuration(shootDuration: 2.0f, chargeTime: 0f);
		//gunClip = new GunClip(bulletCapacity: 15, currentBulletCapacity: 15);

		//gunClip.UpdateUI();
		//}

		private void Update()
		{
			if (shootDuration.Duration > 0)
			{
				shootDuration.Duration -= Time.deltaTime; // 每帧减少冷却时间
			}

			gunClip.BulletReload(reloadSound); // 换弹夹
		}

		// RocketGun的射击方式：鼠标按下后发射一颗火箭子弹，有较长冷却时间
		// 射击逻辑
		public void Shoot(Vector2 shootDirection)
		{
			var playerBullet = Instantiate(bulletPrefab);

			// 子弹发射的位置就是子弹模板的位置（枪口处）
			playerBullet.transform.position = bulletPrefab.transform.position;
			playerBullet.velocity = shootDirection.normalized * 10;
			playerBullet.transform.right = shootDirection; // 调整火箭子弹的飞行角度
			playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活
			playerBullet.gunDamage = 3;

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
				shootDuration.Duration = 2f; // 重置冷却时间
			}
		}
	}
}
