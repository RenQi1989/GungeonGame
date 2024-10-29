using UnityEngine;
using QFramework;

namespace QFramework.ProjectGungeon
{
	public partial class ShotGun : GunManager
	{

		public override PlayerBullet bulletPrefab => PlayerBullet; // 主角子弹模板
		public override AudioSource audioPlayer => SelfAudioSource; // 音效播放器
		private float shootCoolDown = 1.0f; // 开枪冷却时间
		private float coolDownTimer = 0f; // 计时器

		private void Update()
		{
			if (coolDownTimer > 0)
			{
				coolDownTimer -= Time.deltaTime; // 每帧减少冷却时间
			}
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

			// 播放随机射击音效
			var soundsIndex = Random.Range(0, shootSounds.Count);
			audioPlayer.clip = shootSounds[soundsIndex];
			audioPlayer.Play();
		}

		public override void ShootMouseDown(Vector2 shootDirection)
		{
			if (coolDownTimer <= 0) // 冷却时间到，可以开枪
			{
				Shoot(shootDirection);

				Vector2 secondBulletDirection = Quaternion.Euler(0, 0, 10) * shootDirection; // 在2D中，X和Y轴代表平面，而Z轴则用于深度或旋转
				Shoot(secondBulletDirection);

				Vector2 thirdBulletDirection = Quaternion.Euler(0, 0, -10) * shootDirection;
				Shoot(thirdBulletDirection);

				coolDownTimer = shootCoolDown; // 重置冷却计时器
			}
		}
	}
}
