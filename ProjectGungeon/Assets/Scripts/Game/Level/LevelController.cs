using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using QFramework.ProjectGungeon;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace QFramework.ProjectGungeon
{
    public partial class LevelController : ViewController
    {
        [Header("Tile Settings")]
        public TileBase groundTile; // 管理具体地块
        public Tilemap wallTileMap; // 管理墙壁地图
        public Tilemap floorTileMap; // 管理地板地图
        public TileBase wall0;
        public TileBase wall1;
        public TileBase wall2;
        public TileBase wall3;
        public TileBase floor0;
        public TileBase floor1;
        public TileBase floor2;
        public TileBase floor3;

        [Header("Character Settings")]
        public Player playerPrefab;
        public Enemy enemyPrefab;
        public FinalDoor finalDoorPrefab;

        [Header("Level Settings")]
        public static LevelController Default;

        // 随机铺墙壁
        public TileBase Wall
        {
            get
            {
                var wallIndex = UnityEngine.Random.Range(0, 3 + 1);
                if (wallIndex == 0)
                {
                    return wall0;
                }
                if (wallIndex == 1)
                {
                    return wall1;
                }
                if (wallIndex == 2)
                {
                    return wall2;
                }
                if (wallIndex == 3)
                {
                    return wall3;
                }
                return wall0;
            }
        }

        // 随机铺地板
        public TileBase Floor
        {
            get
            {
                var floorIndex = UnityEngine.Random.Range(0, 3 + 1);
                if (floorIndex == 0)
                {
                    return floor0;
                }
                if (floorIndex == 1)
                {
                    return floor1;
                }
                if (floorIndex == 2)
                {
                    return floor2;
                }
                if (floorIndex == 3)
                {
                    return floor3;
                }
                return floor0;
            }
        }

        private void Awake()
        {
            Default = this; // 设置单例

            // 开局之前隐藏掉主角和敌人的模板
            playerPrefab.gameObject.SetActive(false);
            enemyPrefab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Default = null; // 销毁单例
        }

        void Start()
        {
            Room.Hide(); // 隐藏模板房间

            // 生成初始房间（初始房间在 00 位置）
            var currentRoomPosX = 0;
            GenerateRoom(currentRoomPosX, Config.initRoom);

            // 生成正常房间（之后的房间生成在初始房间等宽，再加两个格子的右边）
            currentRoomPosX += Config.initRoom.codes.First().Length + 2;
            GenerateRoom(currentRoomPosX, Config.normalRoomList.GetRandomItem());

            currentRoomPosX += Config.initRoom.codes.First().Length + 2;
            GenerateRoom(currentRoomPosX, Config.normalRoomList.GetRandomItem());

            currentRoomPosX += Config.initRoom.codes.First().Length + 2;
            GenerateRoom(currentRoomPosX, Config.normalRoomList.GetRandomItem());

            // 生成 BOSS 房间
            currentRoomPosX += Config.initRoom.codes.First().Length + 2;
            GenerateRoom(currentRoomPosX, Config.finalRoom);

            // 生成过道(先找到门的位置，再向右遍历两个 tile 的位置)
            var roomWidth = Config.initRoom.codes.First().Length;
            var roomHeight = Config.initRoom.codes.Count;

            // 循环铺多个房间的过道
            for (int roomIndex = 0; roomIndex < 4; roomIndex++)
            {
                currentRoomPosX = roomIndex * (roomWidth + 2);

                var doorStartX = currentRoomPosX + roomWidth - 1;
                var doorStartY = 0 + roomHeight / 2 + 1; // (int)(roomHeight * 0.5 - 1);

                // 铺一个房间的过道
                for (int i = 0; i < 2; i++) // 一共遍历 2 次，每次遍历从上到下，铺 6 块砖
                {
                    wallTileMap.SetTile(new Vector3Int(doorStartX + i + 1, doorStartY + 2, 0), Wall);
                    floorTileMap.SetTile(new Vector3Int(doorStartX + i + 1, doorStartY + 1, 0), Floor);
                    floorTileMap.SetTile(new Vector3Int(doorStartX + i + 1, doorStartY, 0), Floor);
                    floorTileMap.SetTile(new Vector3Int(doorStartX + i + 1, doorStartY - 1, 0), Floor);
                    floorTileMap.SetTile(new Vector3Int(doorStartX + i + 1, doorStartY - 2, 0), Floor);
                    wallTileMap.SetTile(new Vector3Int(doorStartX + i + 1, doorStartY - 3, 0), Wall);
                }
            }
        }

        void GenerateRoom(int startRoomPosX, RoomConfig roomConfig)
        {
            var roomCode = roomConfig.codes;
            var roomWidth = roomCode[0].Length; // rowCode.Length 则表示当前行的长度（地图的宽）
            var roomHeight = roomCode.Count(); // roomCode.Count 是列表的行数（地图的高） 

            // 房间坐标（去掉墙体，可供移动的区域）
            var roomPositionX = startRoomPosX + roomWidth * 0.5f;
            var roomPositionY = 1.0f + roomHeight * 0.5f;

            // 获得 Room脚本：Room 是 LevelController 的子节点
            var roomScript = Room.InstantiateWithParent(this)
                            .Position(roomPositionX, roomPositionY)
                            .WithConfig(roomConfig) // 给创建的房间加入配置信息
                            .Show();

            // 设置房间感应区域，要完全覆盖住可供移动的区域（-2是为了保证主角完全进入房间）    
            roomScript.SelfBoxCollider2D.size = new Vector2(roomWidth - 2, roomHeight - 2);

            // 双层 for 循环遍历 roomCode 列表
            for (var i = 0; i < roomCode.Count; i++) // 一行一行遍历
            {
                // rowCode 是获取的当前行字符串
                var rowCode = roomCode[i];

                for (int j = 0; j < rowCode.Length; j++)
                {
                    var code = rowCode[j]; // 每个行和列交叉的格子对应的字符
                    var x = startRoomPosX + j;
                    var y = roomCode.Count - i;

                    // 根据房间的字符串布局，绘制对应 tile                   
                    floorTileMap.SetTile(new Vector3Int(x, y, 0), Floor); // 绘制地板（所有格子都要绘制地板）

                    if (code == '1') // 绘制墙壁
                    {
                        wallTileMap.SetTile(new Vector3Int(x, y, 0), Wall);
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
                        var enemyGeneratePosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                        roomScript.AddEnemyPosition(enemyGeneratePosition);
                    }
                    else if (code == 'd') // 绘制门
                    {
                        // 获得 Door 脚本：Door 是 Room 的子节点
                        var doorScript = Door.InstantiateWithParent(roomScript)
                                                .Position2D(new Vector3(x + 0.5f, y + 0.5f, 0))
                                                .Hide(); // 默认门是隐藏的
                        roomScript.AddDoor(doorScript);
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
    }
}
