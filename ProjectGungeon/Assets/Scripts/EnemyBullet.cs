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

    private void OnCollisionEnter2D(Collision2D other)
    {
        // 每次生成的敌人叫做 Player(clone)，所以不能用 name == ""判断
        if (other.gameObject.name.StartsWith("Player"))
        {
            GameUI.gameUI.gameOver.SetActive(true);
            other.gameObject.SetActive(false); // Player失活
            Time.timeScale = 0; // 时间停止
        }
    }
}
