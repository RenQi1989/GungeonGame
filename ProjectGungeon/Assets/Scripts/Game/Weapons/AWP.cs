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
		private float shootCoolDown = 2f; // 开枪冷却时间
		private float coolDownTimer = 0f; // 计时器

		private void Update()
		{
			if (coolDownTimer > 0)
			{
				coolDownTimer -= Time.deltaTime; // 每帧减少冷却时间
			}
		}

		// AWP的射击方式：鼠标按下后发射一颗强力子弹，有较长冷却时间
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
		}

		public override void ShootMouseDown(Vector2 shootDirection)
		{
			if (coolDownTimer <= 0) // 冷却时间到，可以开枪
			{
				Shoot(shootDirection);
				coolDownTimer = shootCoolDown; // 重置冷却计时器
			}
		}
	}
}
