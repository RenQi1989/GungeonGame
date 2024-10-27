using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelController : MonoBehaviour
{
    public TileBase groundTile;
    public Tilemap groundTileMap;
    public Player playerPrefab;
    public Enemy enemyPrefab;
    public FinalDoor finalDoorPrefab;

    // 字符串房间列表
    /*

    1 代表 Ground
    @ 代表 主角
    e 代表 敌人
    # 代表 终点传送门

    */

    // 加 { get; set; } 防止序列化（不可在unity里被误修改）
    // 序列化 = public = 在 Unity 里直接显示
    public List<string> initRoom { get; set; } = new List<string>()
    {
        "1111111111",
        "1        1",
        "1        1",
        "1        1",
        "1  @      ",
        "1         ",
        "1        1",
        "1        1",
        "1        1",
        "1111111111",
    };

    public List<string> NormalRoom { get; set; } = new List<string>()
    {
        "1111111111",
        "1        1",
        "1        1",
        "1        1",
        "   e      ",
        "          ",
        "1        1",
        "1      e 1",
        "1        1",
        "1111111111",
    };

    public List<string> FinalRoom { get; set; } = new List<string>()
    {
        "1111111111",
        "1        1",
        "1        1",
        "1        1",
        "     #   1",
        "         1",
        "1        1",
        "1        1",
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
        var currentRoomPosX = 0;
        GenerateRoom(currentRoomPosX, initRoom); // 生成初始房间（初始房间在 00 位置）

        currentRoomPosX += initRoom.First().Length + 2; // 之后的房间生成在初始房间等宽，再加两个格子的右边
        GenerateRoom(currentRoomPosX, NormalRoom); // 生成正常房间

        currentRoomPosX += initRoom.First().Length + 2;
        GenerateRoom(currentRoomPosX, FinalRoom); // 生成BOSS房间

    }

    void GenerateRoom(int startRoomPosX, List<string> roomCode)
    {
        // 双层 for 循环遍历 roomCode 列表
        for (var i = 0; i < roomCode.Count; i++) // 一行一行遍历，roomCode.Count 是列表的行数（地图的高）
        {
            // rowCode 是获取的当前行字符串，rowCode.Length 则表示当前行的长度（地图的宽）
            var rowCode = roomCode[i];

            for (int j = 0; j < rowCode.Length; j++)
            {
                var code = rowCode[j]; // 每个行和列交叉的格子对应的字符
                var x = startRoomPosX + j;
                var y = roomCode.Count - i;

                if (code == '1') // 绘制地块
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
                else if (code == '#') // 绘制终点传送门
                {
                    var finalDoor = Instantiate(finalDoorPrefab);
                    finalDoor.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                    finalDoor.gameObject.SetActive(true);
                }
            }
        }
    }

    void Update()
    {

    }
}
