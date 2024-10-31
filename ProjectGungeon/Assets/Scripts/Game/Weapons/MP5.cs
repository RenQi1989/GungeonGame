using UnityEngine;
using QFramework;
using System.Collections.Generic;

namespace QFramework.ProjectGungeon
{
	public partial class MP5 : GunManager
	{
		public override PlayerBullet bulletPrefab => PlayerBullet;
		public override AudioSource audioPlayer => SelfAudioSource;
		private float currentSecond = 0f; // 连续射击
		private ShootDuration shootDuration;
		private GunClip gunClip;

		private void Start()
		{
			shootDuration = new ShootDuration(shootDuration: 0f, chargeTime: 0f);
			gunClip = new GunClip(bulletCapacity: 80, currentBulletCapacity: 80);

			gunClip.UpdateUI();
		}

		private void Update()
		{
			gunClip.BulletReload(); // 换弹夹
		}

		// MP5的射击方式：鼠标按下后可以连续开枪
		// 射击逻辑
		public void Shoot(Vector2 shootDirection)
		{
			var playerBullet = Instantiate(bulletPrefab);

			// 子弹发射的位置就是子弹模板的位置（枪口处）
			playerBullet.transform.position = bulletPrefab.transform.position;
			playerBullet.direction = shootDirection;
			playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活

			// 播放射击音效																				 
			audioPlayer.clip = shootSounds[0];
			audioPlayer.loop = true; // 循环
			audioPlayer.Play();

			// 子弹消耗
			gunClip.UseBullet();
		}

		// 开始射击
		public override void ShootMouseDown(Vector2 shootDirection)
		{
			if (shootDuration.CanShoot && gunClip.CanShoot)
			{
				Shoot(shootDirection);
			}
		}

		public override void Shooting(Vector2 shootDirection)
		{
			if (shootDuration.CanShoot && gunClip.CanShoot && currentSecond >= 0.1f)
			{
				Shoot(shootDirection);
				currentSecond = 0f;
			}
			currentSecond += Time.deltaTime;
		}

		// 停止射击
		public override void ShootMouseUp(Vector2 shootingDirection)
		{
			audioPlayer.Stop();
		}
	}
}
