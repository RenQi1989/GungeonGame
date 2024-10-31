using System.Collections;
using System.Collections.Generic;
using QFramework.ProjectGungeon;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameUI : MonoBehaviour
{
    // 做成单例，方便所有类访问 GameUI
    public static GameUI Default;
    public GameObject gamePass;
    public GameObject gameOver;
    public Text HP;
    public Text WeaponInfo;
    public GunClip gunClip;

    private void Awake()
    {
        Default = this;
    }

    private void OnDestroy()
    {
        Default = null;
        Player.HPChangeEvent -= UpdateHP; // 取消订阅血量更新方法
    }

    void Start()
    {
        // 重新开始：点击 Restart 按钮之后，重新加载场景
        gamePass.transform.Find("BtnRestart").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                Global.RestartData(); // 调用重启游戏的数据
                SceneManager.LoadScene("SampleScene");
            }
            );

        gameOver.transform.Find("BtnRestart").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                Global.RestartData();
                SceneManager.LoadScene("SampleScene");
            }
            );

        // 血量更新
        UpdateHP();
        Player.HPChangeEvent += UpdateHP; // 订阅血量更新方法
    }

    // 血量更新
    public void UpdateHP()
    {
        HP.text = "HP:" + Player.HP;
    }

    // 武器信息更新
    public static void UpdateWeaponInfo(GunClip gunClip)
    {
        if (Default != null && Default.WeaponInfo != null)
        {
            Default.WeaponInfo.text = $"Bullet: {gunClip.CurrentBulletCapacity}/{gunClip.BulletCapacity} (Press R to Reload)";
        }
        else
        {
            Debug.LogWarning("GameUI instance or WeaponInfo text field is not initialized.");
        }
    }
}
