using UnityEngine;
using QFramework;
using QFramework.ProjectGungeon;

namespace QFramework.Project
{
	public partial class Bow : GunManager
	{
		public override PlayerBullet bulletPrefab => PlayerBullet; // 主角子弹模板
		public override AudioSource audioPlayer => SelfAudioSource; // 音效播放器

		// Bow的射击方式：鼠标按下蓄力，鼠标松开发射，发射完毕后填一支新箭 
		// 射击逻辑
		public void Shoot(Vector2 shootDirection)
		{
			var playerBullet = Instantiate(bulletPrefab);

			// 子弹发射的位置就是子弹模板的位置（枪口处）
			playerBullet.transform.position = bulletPrefab.transform.position;
			playerBullet.direction = shootDirection;
			playerBullet.transform.right = shootDirection; // 调整箭的飞行角度
			playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活

			// 播放随机射击音效
			var soundsIndex = Random.Range(0, shootSounds.Count);
			audioPlayer.clip = shootSounds[soundsIndex];
			audioPlayer.Play();
		}

		// 按下鼠标：搭箭
		public override void ShootMouseDown(Vector2 shootDirection)
		{
			PrepareArrow.gameObject.SetActive(true); // 显示蓄力箭		
		}

		float chargeTime = 0; // 蓄力时间

		// 按住鼠标：蓄力
		public override void Shooting(Vector2 shootDirection)
		{
			chargeTime += Time.deltaTime;
			PrepareArrow.gameObject.SetActive(true); // 持续显示蓄力箭
		}

		// 放开鼠标：发射
		public override void ShootMouseUp(Vector2 shootDirection)
		{
			if (chargeTime > 0.5f)
			{
				PrepareArrow.gameObject.SetActive(false); // 隐藏蓄力箭
				Shoot(shootDirection);
				chargeTime = 0f; // 重置蓄力时间
			}
		}
	}
}
