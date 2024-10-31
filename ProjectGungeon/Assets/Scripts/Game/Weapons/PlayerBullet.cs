using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 15.0f;
    public Rigidbody2D playerBulletRb;

    private void Awake()
    {
        playerBulletRb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        playerBulletRb.velocity = direction * speed;
    }

    // 碰撞检测
    private void OnCollisionEnter2D(Collision2D other)
    {
        var enemy = other.gameObject.GetComponent<Enemy>(); // 使用挂载的脚本是不是Enemy判断
        if (enemy)
        {
            enemy.Hurt(1);
            Destroy(gameObject); // 销毁主角子弹
        }
        else // 碰到其他东西(比如墙体)，销毁主角子弹
        {
            Destroy(gameObject);
        }
    }
}
