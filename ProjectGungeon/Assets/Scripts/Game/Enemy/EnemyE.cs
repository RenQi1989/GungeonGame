using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public class EnemyE : MonoBehaviour, IEnemy // 一秒发射5颗子弹
    {
        [Header("Enemy Settings")]
        public EnemyBullet enemyBulletPrefab;
        public SpriteRenderer enemySprite;
        public Rigidbody2D enemyRb;
        public float speed = 1.0f;
        public int HP = 3;

        [Header("Shoot Settings")]
        private float fireTimer = 0; // 子弹发射计时器
        private float fireInterval = 0.2f; // 每颗子弹的时间间隔
        private int bulletsFired = 0; // 当前轮次已发射的子弹数量
        private int bulletsPerRound = 5; // 每次发射的子弹数量
        private float roundCoolDown; // 每轮次之间的随机间隔时间

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
            var shootStateDuration = Random.Range(1, 6);

            state.State(States.Shoot)
                .OnEnter(() =>
                {
                    Debug.Log("进入射击状态");
                    enemyRb.velocity = Vector2.zero; // 敌人射击时禁止位移

                    fireTimer = 0; // 重置发射计时器
                    bulletsFired = 0; // 重置发射数量
                    roundCoolDown = Random.Range(3f, 6f); // 设置随机轮次间隔

                })
                .OnUpdate(() =>
                {

                    fireTimer += Time.deltaTime;

                    // 每轮次发射 5 颗子弹，轮次之间的间隔 3 - 6 秒随机
                    if (bulletsFired < bulletsPerRound && fireTimer >= fireInterval)
                    {
                        fireTimer = 0; // 重置计时器
                        bulletsFired++; // 增加已发射子弹数量

                        if (CameraController.player)
                        {
                            var directionToPlayer = (CameraController.player.transform.position - this.transform.position).normalized;

                            // 发射子弹
                            var enemyBullet = Instantiate(enemyBulletPrefab);
                            enemyBullet.velocity = directionToPlayer * 5; // 5 是射击速度
                            enemyBullet.transform.position = this.transform.position;
                            enemyBullet.gameObject.SetActive(true);

                            // 播放随机射击音效
                            var soundsIndex = Random.Range(0, shootSounds.Count);
                            shootSoundPlayer.clip = shootSounds[soundsIndex];
                            shootSoundPlayer.Play();
                        }
                    }

                    // 射击状态持续随机秒后切换回跟随
                    if (bulletsFired >= bulletsPerRound && state.SecondsOfCurrentState >= roundCoolDown)
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
