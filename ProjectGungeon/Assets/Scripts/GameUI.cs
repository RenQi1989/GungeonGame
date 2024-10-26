using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    // 做成单例，方便所有类访问 GameUI
    public static GameUI gameUI;
    public GameObject gamePass;
    public GameObject gameOver;

    private void Awake()
    {
        gameUI = this;
    }

    private void OnDestroy()
    {
        gameUI = null;
    }

    void Start()
    {
        // 重新开始：点击 Restart 按钮之后，重新加载场景
        gamePass.transform.Find("BtnRestart").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                SceneManager.LoadScene("SampleScene");
                Time.timeScale = 1; // 时间继续流逝
            }
            );

        gameOver.transform.Find("BtnRestart").GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                SceneManager.LoadScene("SampleScene");
                Time.timeScale = 1; // 时间继续流逝
            }
            );
    }

    void Update()
    {

    }
}
