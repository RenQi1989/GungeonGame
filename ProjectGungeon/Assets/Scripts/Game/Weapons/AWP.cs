using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace QFramework.ProjectGungeon
{
	public partial class AWP : GunManager
	{
		public override PlayerBullet bulletPrefab => PlayerBullet; // 主角子弹模板
		public override AudioSource audioPlayer => SelfAudioSource; // 音效播放器
		private ShootDuration shootDuration;
		private GunClip gunClip;

		private void Start()
		{
			shootDuration = new ShootDuration(shootDuration: 1.5f, chargeTime: 0f);
			gunClip = new GunClip(bulletCapacity: 20, currentBulletCapacity: 20);

			gunClip.UpdateUI();
		}
		private void Update()
		{
			if (shootDuration.Duration > 0)
			{
				shootDuration.Duration -= Time.deltaTime; // 每帧减少冷却时间
			}

			gunClip.BulletReload(); // 换弹夹
		}

		// AWP的射击方式：鼠标按下后发射一颗强力子弹，冷却时间较长
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
			audioPlayer.Play();

			// 子弹消耗
			gunClip.UseBullet();
		}

		public override void ShootMouseDown(Vector2 shootDirection)
		{
			if (shootDuration.CanShoot && gunClip.CanShoot)
			{
				Shoot(shootDirection);
				shootDuration.Duration = 2f; // 重置冷却时间
			}
		}
	}
}
