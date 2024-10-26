using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyBullet enemyBulletPrefab;
    public enum States
    {
        FollowPlayer,
        Shoot,
    }

    public States state = States.FollowPlayer; // 默认状态：跟随

    void Start()
    {
        Application.targetFrameRate = 60; // 锁定每秒 60 帧
    }

    public float FollowPlayerSeconds = 3.0f; // 敌人跟随玩家时间，默认 3 秒
    public float CurrentSeconds = 0; // 用来计时

    void Update()
    {
        // 敌人状态判断
        if (state == States.FollowPlayer)
        {
            CurrentSeconds += Time.deltaTime; // 开始计时

            // 当怪物跟随玩家超过 3 秒，进入射击状态，计时清零
            if (CurrentSeconds >= FollowPlayerSeconds)
            {
                state = States.Shoot;
                CurrentSeconds = 0;
            }

            if (CameraController.player) // 如果 player 对象存在就跟随
            {
                // 获取怪物到玩家的方向向量
                var directionToPlayer = (CameraController.player.transform.position - this.transform.position).normalized;
                // 让怪物每一帧朝向玩家移动
                this.transform.Translate(directionToPlayer * Time.deltaTime);
            }
        }
        else if (state == States.Shoot)
        {
            // 开始计时
            CurrentSeconds += Time.deltaTime;

            // 开枪冷却（保证每颗子弹之间不能连发，必须有个 0.1s 的间隔）
            if (CurrentSeconds >= 0.1f)
            {
                state = States.FollowPlayer;
                FollowPlayerSeconds = Random.Range(2f, 4f);
                CurrentSeconds = 0;
            }

            // 每秒最多发射 3 颗子弹
            if (Time.frameCount % 20 == 0)
            {
                if (CameraController.player) // 如果 player 对象存在就射击
                {
                    var directionToPlayer = (CameraController.player.transform.position - this.transform.position).normalized;

                    var enemyBullet = Instantiate(enemyBulletPrefab);
                    enemyBullet.direction = directionToPlayer; // 敌人的子弹朝向玩家
                    enemyBullet.transform.position = this.transform.position; // 子弹位置就是敌人位置
                    enemyBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活
                }
            }
        }
    }
}
