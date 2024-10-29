using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 15.0f;

    void Start()
    {

    }

    void Update()
    {
        this.transform.Translate(direction * Time.deltaTime * speed);
    }

    // 碰撞检测
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Enemy>()) // 使用挂载的脚本是不是Enemy判断
        {
            other.gameObject.SetActive(false); // Enemy失活
            Destroy(gameObject); // 销毁主角子弹
        }
        else // 碰到其他东西(比如墙体)，子弹自己隐藏
        {
            Destroy(gameObject);
        }
    }
}
