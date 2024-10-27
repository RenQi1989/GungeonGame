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
        if (other.gameObject.GetComponent<Player>()) // 使用挂载的脚本是不是Player判断
        {
            other.gameObject.SetActive(false); // Player失活
        }
    }
}
