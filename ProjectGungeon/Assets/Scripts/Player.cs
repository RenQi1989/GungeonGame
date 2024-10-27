using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerBullet PlayerBulletPrefab;
    public Rigidbody2D playerRb;
    public float speed = 2.0f;

    void Start()
    {

    }

    void Update()
    {
        // 主角移动
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        playerRb.velocity = new Vector2(horizontal, vertical).normalized * speed;


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
