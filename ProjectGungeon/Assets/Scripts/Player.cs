using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerBullet PlayerBulletPrefab;
    void Start()
    {

    }

    void Update()
    {
        // 主角移动
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontal, vertical) * Time.deltaTime);

        // 主角射击
        if (Input.GetMouseButtonDown(0))
        {
            var playerBullet = Instantiate(PlayerBulletPrefab);
            playerBullet.direction = Vector2.right;
            playerBullet.transform.position = this.transform.position; // 子弹位置就是主角位置
            playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活
        }
    }
}
