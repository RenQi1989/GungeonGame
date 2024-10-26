using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Vector2 direction;
    void Start()
    {

    }

    void Update()
    {
        this.transform.Translate(direction * Time.deltaTime);
    }

    // 碰撞检测
    private void OnCollisionEnter2D(Collision2D other)
    {
        // 每次生成的敌人叫做 Enemy(clone)，所以不能用 name == ""判断
        if (other.gameObject.name.StartsWith("Enemy"))
        {
            GameUI.gameUI.gamePass.SetActive(true); // 当主角的子弹碰到敌人，弹出”游戏通关“
            other.gameObject.SetActive(false); // Enemy失活
            Time.timeScale = 0; // 时间停止
        }
    }
}
