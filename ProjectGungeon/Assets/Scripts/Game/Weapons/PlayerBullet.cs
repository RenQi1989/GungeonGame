using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Vector2 velocity; // velocity = direction * speed;
    public Rigidbody2D playerBulletRb;
    public int gunDamage { get; set; } // 主角子弹伤害

    private void Awake()
    {
        playerBulletRb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        playerBulletRb.velocity = velocity;
    }

    /// 碰撞检测
    private void OnCollisionEnter2D(Collision2D other)
    {
        var enemy = other.gameObject.GetComponent<IEnemy>();
        if (enemy != null)
        {
            enemy.Hurt(gunDamage);
        }
        // 确保子弹立即销毁
        Destroy(gameObject);
    }
}
