using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using QFramework;
using QFramework.ProjectGungeon;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public Rigidbody2D playerRb;
    public float speed = 7.0f;
    public SpriteRenderer playerSprite;
    public static int HP = 3;
    public static Action HPChangeEvent;
    public static Player Default; // 单例
    public Text playerText; // 跟随人物的文字

    [Header("Weapon Settings")]
    public GunManager currentGun;
    private List<GunManager> gunList = new List<GunManager>();
    public SpriteRenderer weaponFire;

    [Header("Audio Settings")]
    public AudioSource audioSource;  // 添加一个 AudioSource
    public AudioClip switchWeaponSound;  // 切枪音效

    private void Awake()
    {
        Default = this; // 设置单例

        gunList.Add(Instantiate(pistolPrefab, transform).GetComponent<GunManager>());
        gunList.Add(Instantiate(AWPPrefab, transform).GetComponent<GunManager>());
        gunList.Add(Instantiate(bowPrefab, transform).GetComponent<GunManager>());
        gunList.Add(Instantiate(MP5Prefab, transform).GetComponent<GunManager>());
        gunList.Add(Instantiate(rocketGunPrefab, transform).GetComponent<GunManager>());
        gunList.Add(Instantiate(shotGunPrefab, transform).GetComponent<GunManager>());

        Default.playerText.gameObject.SetActive(false); // 隐藏跟随人物的文本
        UseWeapon(0, false); // 默认武器
    }

    private void OnDestroy()
    {
        Default = null; // 销毁单例
    }

    // 播放跟随主角的文字
    public void DisplayTextOnPlayer(string text, float duration)
    {
        StartCoroutine(ShowText(text, duration));
    }
    IEnumerator ShowText(string text, float duration)
    {
        Default.playerText.text = text;
        Default.playerText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        Default.playerText.gameObject.SetActive(false);
    }

    // 使用武器
    public void UseWeapon(int index, bool playSwitchWeaponSound = true)
    {
        currentGun.gameObject.SetActive(false); // 上一把武器隐藏
        currentGun = gunList[index];
        currentGun.gameObject.SetActive(true); // 当前武器显示

        currentGun.IsEquipped(); // 更新武器装备状态

        if (playSwitchWeaponSound)
        {
            audioSource.PlayOneShot(switchWeaponSound); // 播放切枪音效

        }
    }

    // 主角受伤
    public void PlayerHurt(int damage)
    {
        HP -= damage;
        HPChangeEvent();

        if (HP <= 0)
        {
            HP = 0;
            GameUI.Default.gameOver.SetActive(true);
            Time.timeScale = 0;
        }
    }

    void Update()
    {
        // 主角移动
        var horizontal = Input.GetAxisRaw("Horizontal"); // horizontal大于0是向右走，小于0是向左走
        var vertical = Input.GetAxisRaw("Vertical"); // vertical大于0是向上走，小于0是向下走
        playerRb.velocity = new UnityEngine.Vector2(horizontal, vertical).normalized * speed; // normalized 斜着移动的速度也为 1

        /* 主角翻转
		if (horizontal < 0)
		{
			playerSprite.flipX = true; // 水平镜像翻转(所以不使用 flipY)
		}
		else if (horizontal > 0)
		{
			playerSprite.flipX = false;
		} */

        // 武器射击和枪口旋转
        // 1. 射击：射击方向是鼠标点击的方向
        var mouseScreenPosition = Input.mousePosition;
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        var shootDirection = (mouseWorldPosition - this.transform.position).normalized;

        // 2. 枪口旋转
        var radius = Mathf.Atan2(shootDirection.y, shootDirection.x); // 鼠标点击方向在 y 轴和 x 轴之间的夹角弧度
        var eulerAngle = radius * Mathf.Rad2Deg; // 把弧度转成欧拉角
        currentGun.gameObject.transform.localRotation = UnityEngine.Quaternion.Euler(0, 0, eulerAngle);// 把欧拉角赋给武器的自转属性

        // 武器翻转（主角面向跟随枪口方向）
        if (shootDirection.x > 0)
        {
            currentGun.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            playerSprite.flipX = false;
        }
        else if (shootDirection.x < 0)
        {
            currentGun.transform.localScale = new UnityEngine.Vector3(1, -1, 1);
            playerSprite.flipX = true;
        }

        // 主角射击
        if (Input.GetMouseButtonDown(0)) // 点击
        {
            currentGun.ShootMouseDown(shootDirection);
        }
        if (Input.GetMouseButton(0)) // 按住
        {
            currentGun.Shooting(shootDirection);
        }
        if (Input.GetMouseButtonUp(0)) // 抬起
        {
            currentGun.ShootMouseUp(shootDirection);
        }

        // 切换武器
        if (Input.GetKeyDown(KeyCode.Q)) // 切换上一把
        {
            if (!currentGun.IsReloading) // 非 Reload 时间才可以切换武器
            {
                var index = gunList.FindIndex(gun => gun == currentGun);
                index--;

                if (index < 0)
                {
                    index = gunList.Count - 1;
                }
                UseWeapon(index);
            }
        }
        if (Input.GetKeyDown(KeyCode.E)) // 切换到下一把武器
        {
            if (!currentGun.IsReloading)
            {
                var index = gunList.FindIndex(gun => gun == currentGun);
                index++;

                if (index > gunList.Count - 1)
                {
                    index = 0;
                }
                UseWeapon(index);
            }
        }
    }
}
