using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelController : MonoBehaviour
{
    public TileBase groundTile;
    public Tilemap groundTileMap;
    public Player playerPrefab;
    public Enemy enemyPrefab;

    // 字符串房间列表
    /*

    1 代表 Ground
    @ 代表 主角
    e 代表 敌人

    */

    // 加 { get; set; } 防止序列化（不可在unity里被误修改）
    // 序列化 = public = 在 Unity 里直接显示
    public List<string> initRoom { get; set; } = new List<string>()
    {
        "1111111111",
        "1        1",
        "1        1",
        "1        1",
        "1  @     1",
        "1        1",
        "1        1",
        "1      e 1",
        "1        1",
        "1111111111",
    };

    private void Awake()
    {
        // 开局之前隐藏掉主角和敌人的模板
        playerPrefab.gameObject.SetActive(false);
        enemyPrefab.gameObject.SetActive(false);
    }

    void Start()
    {
        // 遍历 initRoom 列表
        for (var i = 0; i < initRoom.Count; i++) // 一行一行遍历，initRoom.Count 是列表的行数（地图的高）
        {
            // rowCode 是从 InitRoom 中获取的当前行字符串，rowCode.Length 则表示当前行的长度（地图的宽）
            var rowCode = initRoom[i];

            for (int j = 0; j < rowCode.Length; j++)
            {
                var code = rowCode[j]; // 每个行和列交叉的格子对应的字符
                var x = j;
                var y = initRoom.Count - i;

                if (code == '1') // 如果 code 对应的 char 是 1，就绘制地块
                {
                    groundTileMap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
                else if (code == '@') // 绘制主角
                {
                    var player = Instantiate(playerPrefab);
                    // xy是地块方格的左下角, 加 0.5f 之后才能让主角居中
                    player.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                    player.gameObject.SetActive(true);
                    CameraController.player = player; // 让相机跟随新生成的 player 对象
                }
                else if (code == 'e') // 绘制敌人
                {
                    var enemy = Instantiate(enemyPrefab);
                    enemy.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                    enemy.gameObject.SetActive(true);
                }
            }
        }
    }

    void Update()
    {

    }
}
