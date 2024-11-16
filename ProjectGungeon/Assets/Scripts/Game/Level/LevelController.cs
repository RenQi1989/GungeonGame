using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QFramework;
using QFramework.ProjectGungeon;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Tilemaps;
using static RoomConfig;

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
        public Chest chestPrefab;

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

        // 生成房间节点
        public class GenerateRoomNode
        {
            public RoomNode node { get; set; }
            public int x { get; set; } // 房间节点的 XY 坐标
            public int y { get; set; }
        }

        // 门的生成方向
        public enum DoorDirections
        {
            Up,
            Down,
            Left,
            Right
        }

        void Start()
        {
            Room.Hide(); // 隐藏模板房间

            // 带有 Node 信息的房间布局
            var layout = new RoomNode(RoomTypes.InitRoom);

            layout.GenerateNextRoom(RoomTypes.NormalRoom)
                    .GenerateNextRoom(RoomTypes.NormalRoom)
                    .GenerateNextRoom(RoomTypes.ChestRoom)
                    .GenerateNextRoom(RoomTypes.NormalRoom)
                    .GenerateNextRoom(RoomTypes.NormalRoom)
                    .GenerateNextRoom(RoomTypes.FinalRoom);

            // 网格类型的数据结构，用来生成四面房间（每个房间存在网格里，不存在的房间为null）
            var layoutGrid = new DynaGrid<GenerateRoomNode>();

            // 生成房间布局
            void GenerateLayout(RoomNode roomNode, DynaGrid<GenerateRoomNode> layoutGrid)
            {

                // 遍历网格队列（选择广度优先，深度优先容易进死胡同）
                var queue = new Queue<GenerateRoomNode>();
                queue.Enqueue(new GenerateRoomNode() // 在队列里加入第一个节点
                {
                    x = 0,
                    y = 0,
                    node = roomNode // 根节点
                });

                // 判断房间生成的方向
                while (queue.Count > 0)
                {
                    var generateNode = queue.Dequeue();
                    layoutGrid[generateNode.x, generateNode.y] = generateNode;

                    var availableDirection = new List<DoorDirections>();

                    // 如果房间的右边是空的，就可以在右边生成房间
                    if (layoutGrid[generateNode.x + 1, generateNode.y] == null)
                    {
                        availableDirection.Add(DoorDirections.Right);
                    }
                    if (layoutGrid[generateNode.x - 1, generateNode.y] == null)
                    {
                        availableDirection.Add(DoorDirections.Left);
                    }
                    if (layoutGrid[generateNode.x, generateNode.y + 1] == null)
                    {
                        availableDirection.Add(DoorDirections.Up);
                    }
                    if (layoutGrid[generateNode.x, generateNode.y - 1] == null)
                    {
                        availableDirection.Add(DoorDirections.Down);
                    }

                    // 遍历子节点，生成房间
                    foreach (var roomNodeChild in generateNode.node.children)
                    {
                        // 下一个房间的方向：从可用的门方向里随机选一个
                        var nextRoomDirection = availableDirection.GetRandomItem();

                        if (nextRoomDirection == DoorDirections.Right)
                        {
                            queue.Enqueue(new GenerateRoomNode
                            {
                                x = generateNode.x + 1, // 在初始房间右边生成一个房间
                                y = generateNode.y,
                                node = roomNodeChild
                            });
                        }
                        else if (nextRoomDirection == DoorDirections.Left)
                        {
                            queue.Enqueue(new GenerateRoomNode
                            {
                                x = generateNode.x - 1, // 在初始房间左边生成一个房间
                                y = generateNode.y,
                                node = roomNodeChild
                            });
                        }
                        else if (nextRoomDirection == DoorDirections.Up)
                        {
                            queue.Enqueue(new GenerateRoomNode
                            {
                                x = generateNode.x, // 在初始房间上面生成一个房间
                                y = generateNode.y + 1,
                                node = roomNodeChild
                            });
                        }
                        else if (nextRoomDirection == DoorDirections.Down)
                        {
                            queue.Enqueue(new GenerateRoomNode
                            {
                                x = generateNode.x, // 在初始房间下面生成一个房间
                                y = generateNode.y - 1,
                                node = roomNodeChild
                            });
                        }
                    }
                }
            }

            GenerateLayout(layout, layoutGrid);

            layoutGrid.ForEach((x, y, generateNode) =>
            {
                GenerateRoomByNode(x, y, generateNode.node);
            });


            // 生成初始房间（初始房间在 0,0 位置）
            var currentRoomPosX = 0;

            // 根据节点生成房间
            void GenerateRoomByNode(int x, int y, RoomNode node)
            {
                var roomPosX = x * (Config.initRoom.codes.First().Length + 2);
                var roomPosY = y * (Config.initRoom.codes.Count + 2);

                // 针对一个房间
                if (node.roomTypes == RoomTypes.InitRoom)
                {
                    BuildRoom(roomPosX, roomPosY, Config.initRoom);
                }
                else if (node.roomTypes == RoomTypes.NormalRoom)
                {
                    BuildRoom(roomPosX, roomPosY, Config.normalRoomList.GetRandomItem());
                }
                else if (node.roomTypes == RoomTypes.ChestRoom)
                {
                    BuildRoom(roomPosX, roomPosY, Config.chestRoom);
                }
                else if (node.roomTypes == RoomTypes.FinalRoom)
                {
                    BuildRoom(roomPosX, roomPosY, Config.finalRoom);
                }
            }

            // 生成过道
            void GenerateCorridor(int roomCount) // 参数是房间总数量
            {
                // 先找到门的位置，再向右遍历两个 tile 的位置
                var roomWidth = Config.initRoom.codes.First().Length;
                var roomHeight = Config.initRoom.codes.Count;

                // 循环铺多个房间的过道
                for (int roomIndex = 0; roomIndex < roomCount - 1; roomIndex++)
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

            //GenerateCorridor(7); // 生成房间的数量
        }

        void BuildRoom(int startRoomPosX, int startRoomPosY, RoomConfig roomConfig)
        {
            var roomCode = roomConfig.codes;
            var roomWidth = roomCode[0].Length; // rowCode.Length 则表示当前行的长度（地图的宽）
            var roomHeight = roomCode.Count(); // roomCode.Count 是列表的行数（地图的高） 

            // 房间坐标（去掉墙体，可供移动的区域）
            var roomPositionX = startRoomPosX + roomWidth * 0.5f;
            var roomPositionY = startRoomPosY + 1.0f + roomHeight * 0.5f;

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
                    var y = startRoomPosY + roomCode.Count - i;

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
                    else if (code == 'c') // 绘制宝箱
                    {
                        var chest = Instantiate(chestPrefab);
                        chest.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                        chest.gameObject.SetActive(true);
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
