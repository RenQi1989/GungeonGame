using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    public Vector2 direction;
    void Start()
    {

    }

    void Update()
    {
        this.transform.Translate(direction * Time.deltaTime);
    }

    // 碰撞检测（敌人子弹碰撞到主角）
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Player>()) // 使用挂载的脚本是不是Player判断
        {
            other.gameObject.GetComponent<Player>().PlayerHurt(1); // 调用 Player 类的 PlayerHurt 方法
            Destroy(gameObject); // 销毁敌人子弹
        }
    }
}
