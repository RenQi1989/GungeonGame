using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerBullet PlayerBulletPrefab;
    public Rigidbody2D playerRb;
    public float speed = 2.5f;
    public SpriteRenderer playerSprite;
    public static int HP = 3;
    public static Action HPChangeEvent;

    void Start()
    {

    }

    // 主角受伤
    public void PlayerHurt(int damage)
    {
        HP -= damage;
        HPChangeEvent();

        if (HP <= 0)
        {
            HP = 0;
            GameUI.gameUI.gameOver.SetActive(true);
            Time.timeScale = 0;
        }
    }

    void Update()
    {
        // 主角移动
        var horizontal = Input.GetAxisRaw("Horizontal"); // horizontal大于0是向右走，小于0是向左走
        var vertical = Input.GetAxisRaw("Vertical"); // vertical大于0是向上走，小于0是向下走
        playerRb.velocity = new Vector2(horizontal, vertical).normalized * speed; // normalized 斜着移动的速度也为 1

        // 主角翻转
        if (horizontal < 0)
        {
            playerSprite.flipX = true; // 水平镜像翻转(所以不使用 flipY)
        }
        else if (horizontal > 0)
        {
            playerSprite.flipX = false;
        }

        // 主角射击
        if (Input.GetMouseButtonDown(0))
        {
            // 射击方向是鼠标点击的方向
            var mouseScreenPosition = Input.mousePosition;
            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            var shootDirection = (mouseWorldPosition - this.transform.position).normalized;

            var playerBullet = Instantiate(PlayerBulletPrefab);

            playerBullet.direction = shootDirection;

            // 子弹位置就是主角位置
            playerBullet.transform.position = this.transform.position;

            playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活
        }
    }
}
