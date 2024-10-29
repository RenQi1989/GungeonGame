using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public Rigidbody2D playerRb;
    public float speed = 2.5f;
    public SpriteRenderer playerSprite;
    public static int HP = 3;
    public static Action HPChangeEvent;

    [Header("Weapon Settings")]
    public GameObject weapon;
    public Pistol pistol;

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
        playerRb.velocity = new UnityEngine.Vector2(horizontal, vertical).normalized * speed; // normalized 斜着移动的速度也为 1

        // 主角翻转
        if (horizontal < 0)
        {
            playerSprite.flipX = true; // 水平镜像翻转(所以不使用 flipY)
        }
        else if (horizontal > 0)
        {
            playerSprite.flipX = false;
        }

        // 武器射击和旋转

        // 1. 射击：射击方向是鼠标点击的方向
        var mouseScreenPosition = Input.mousePosition;
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        var shootDirection = (mouseWorldPosition - this.transform.position).normalized;

        // 2. 旋转
        var radius = Mathf.Atan2(shootDirection.y, shootDirection.x); // 鼠标点击方向在 y 轴和 x 轴之间的夹角弧度
        var eulerAngle = radius * Mathf.Rad2Deg; // 把弧度转成欧拉角
        weapon.transform.localRotation = UnityEngine.Quaternion.Euler(0, 0, eulerAngle);// 把欧拉角赋给武器的自转属性

        if (shootDirection.x > 0) // 武器翻转
        {
            weapon.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
        }
        else
        {
            weapon.transform.localScale = new UnityEngine.Vector3(1, -1, 1);
        }

        // 主角射击
        if (Input.GetMouseButtonDown(0))
        {
            pistol.ShootMouseButtonDown(shootDirection);
        }
    }
}
