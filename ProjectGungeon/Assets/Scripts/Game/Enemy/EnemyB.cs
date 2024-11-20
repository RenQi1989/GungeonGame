using System.Collections;
using System.Collections.Generic;
using QFramework;
using QFramework.ProjectGungeon;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public class EnemyB : MonoBehaviour, IEnemy
    {
        [Header("Enemy Settings")]
        public EnemyBullet enemyBulletPrefab;
        public SpriteRenderer enemySprite;
        public Rigidbody2D enemyRb;
        public float speed = 1.0f;
        public int HP = 3;

        [Header("Audio Settings")]
        public List<AudioClip> shootSounds = new List<AudioClip>();
        public AudioSource shootSoundPlayer;

        // 敌人状态枚举
        public enum States
        {
            FollowPlayer,
            Shoot,
        }

        // 敌人状态机
        public FSM<States> state = new FSM<States>();
        public float FollowPlayerSeconds = 3.0f; // 敌人跟随玩家时间，默认 2 秒
        public float CurrentSeconds = 0; // 用来计时

        // 实现接口的属性
        public GameObject GameObject => gameObject;
        public Room room { get; set; }

        private void Awake()
        {
            // 当状态是 跟随玩家 时
            state.State(States.FollowPlayer)
                .OnEnter(() =>
                {
                    FollowPlayerSeconds = Random.Range(0.5f, 3f); // 动态调整跟随时间
                })
                .OnUpdate(() =>
                {
                    Debug.Log("进入追踪状态");

                    // 当敌人跟随玩家超过 2 秒，进入射击状态，计时清零
                    if (state.SecondsOfCurrentState >= FollowPlayerSeconds)
                    {
                        state.ChangeState(States.Shoot);
                    }

                    if (CameraController.player) // 如果 player 对象存在就跟随
                    {
                        // 获取敌人到玩家的方向向量
                        var directionToPlayer = (CameraController.player.transform.position - this.transform.position).normalized;
                        // 敌人移动
                        enemyRb.velocity = directionToPlayer * speed;

                        // 敌人翻转
                        if (directionToPlayer.x > 0)
                        {
                            enemySprite.flipX = false;
                        }
                        else if (directionToPlayer.x < 0)
                        {
                            enemySprite.flipX = true;
                        }
                    }
                });

            // 当状态是 射击 时    
            state.State(States.Shoot)
                .OnEnter(() =>
                {
                    Debug.Log("进入射击状态");
                    enemyRb.velocity = Vector2.zero; // 敌人射击时禁止位移
                    CurrentSeconds = 0; // 初始化计时器
                })
                .OnUpdate(() =>
                {
                    CurrentSeconds += Time.deltaTime; // 时间累积

                    // 每 1 秒发射子弹
                    if (CurrentSeconds >= 3.0f)
                    {
                        CurrentSeconds = 0;

                        if (CameraController.player)
                        {
                            var directionToPlayer = (CameraController.player.transform.position - this.transform.position).normalized;

                            var count = 3; // 每次发射子弹数量 3
                            var durationAngle = 15; // 子弹之间的夹角 15 度

                            // 主弹道角度：发射向主角的三维向量转成二维向量，再转换成欧拉角
                            var mainAngle = directionToPlayer.ToVector2().ToAngle();

                            for (int i = 0; i < count; i++)
                            {
                                Debug.Log("我要发射3颗子弹");

                                var angle = mainAngle + i * durationAngle - count * durationAngle * 0.5f;
                                var shootDirection = angle.AngleToDirection2D(); // 角度转方向
                                                                                 // 发射子弹
                                var shootPosition = transform.Position2D() + 0.5f * shootDirection;

                                var enemyBullet = Instantiate(enemyBulletPrefab);
                                enemyBullet.velocity = shootDirection * 5; // 3 是射击速度
                                enemyBullet.transform.position = shootPosition;
                                enemyBullet.gameObject.SetActive(true);

                            }

                            // 播放随机射击音效
                            var soundsIndex = Random.Range(0, shootSounds.Count);
                            shootSoundPlayer.clip = shootSounds[soundsIndex];
                            shootSoundPlayer.Play();
                        }
                    }
                    // 射击状态持续 1 秒后切换回跟随
                    if (state.SecondsOfCurrentState >= 6.0f)
                    {
                        state.ChangeState(States.FollowPlayer);
                        FollowPlayerSeconds = Random.Range(2f, 5f); // 动态调整跟随时间
                    }
                });
            state.StartState(States.FollowPlayer); // 默认状态是 跟随主角
        }

        void Start()
        {
            Application.targetFrameRate = 60; // 锁定每秒 60 帧
        }

        void Update()
        {
            state.Update(); // 让状态机逐帧更新
        }

        // 敌人受伤
        public void Hurt(int damage)
        {
            HP -= damage;
            enemyRb.velocity = Vector2.zero;

            if (HP <= 0)
            {
                Destroy(gameObject); // 销毁敌人
            }
        }
        void IEnemy.Hurt(int damage)
        {
            Hurt(damage); // 调用类内部的公共 Hurt 方法
        }

        // 销毁敌人
        private void OnDestroy()
        {
            room.RemoveEnemy(this); // 调用 Room 类的 RemoveEnemy 方法
        }
    }
}
