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
        public TileBase groundTile; // 具体地块
        public Tilemap wallTileMap; // 墙壁地图
        public Tilemap floorTileMap; // 地板地图
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
        public IEnemy enemyPrefab => EnemyB.GetComponent<IEnemy>(); // 把测试的敌人类型改成：敌人B
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

        // 房间节点的信息
        public class GenerateRoomNode
        {
            public RoomNode node;

            // 因为一个房间可能有多个门的方向，所以用 HashSet 保存门的方向
            public HashSet<DoorDirections> doorDirections { get; set; }

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
                    .GenerateNextRoom(RoomTypes.ChestRoom, n =>
                    {
                        n.GenerateNextRoom(RoomTypes.NormalRoom)
                            .GenerateNextRoom(RoomTypes.ChestRoom);
                    })
                    .GenerateNextRoom(RoomTypes.NormalRoom)
                    .GenerateNextRoom(RoomTypes.NormalRoom)
                    .GenerateNextRoom(RoomTypes.ChestRoom, n =>
                    {
                        n.GenerateNextRoom(RoomTypes.NormalRoom)
                            .GenerateNextRoom(RoomTypes.ChestRoom);
                    })
                    .GenerateNextRoom(RoomTypes.FinalRoom);

            // 网格类型的数据结构，用来生成四面房间（每个房间存在网格里，不存在的房间为null）
            var layoutGrid = new DynaGrid<GenerateRoomNode>();

            // 生成房间布局(predictWeight: 0 - 100, 0完全随机生成房间，100最优解生成房间)
            bool GenerateLayoutBFS(RoomNode roomNode, DynaGrid<GenerateRoomNode> layoutGrid, int predictWeight = 0)
            {
                // 遍历网格队列（选择广度优先，深度优先容易进死胡同）
                var queue = new Queue<GenerateRoomNode>();
                queue.Enqueue(new GenerateRoomNode() // 在队列里加入第一个节点
                {
                    x = 0,
                    y = 0,
                    node = roomNode, // 根节点
                    doorDirections = new HashSet<DoorDirections>()
                });

                // 判断房间生成的方向
                while (queue.Count > 0)
                {
                    var generateNode = queue.Dequeue();

                    // 生成失败的情况 1
                    if (layoutGrid[generateNode.x, generateNode.y] == null)
                    {
                        layoutGrid[generateNode.x, generateNode.y] = generateNode;
                    }
                    else
                    {
                        Debug.Log("生成房间发生冲突");
                        return false;
                    }

                    // 获取可生成房间的方向
                    var availableDirection = LevelGenerateHelper.GetAvailableDirections(layoutGrid, generateNode.x, generateNode.y);

                    // 生成失败的情况 2
                    if (generateNode.node.children.Count > availableDirection.Count)
                    {
                        Debug.Log("生成房间发生冲突");
                        return false;
                    }

                    // 获取预测的可生成房间的方向和对应数量
                    var directions = LevelGenerateHelper.PredictDirection(layoutGrid, generateNode.x, generateNode.y);
                    // 将可生成房间的方向对应的数量，从大到小排序
                    directions.Sort((a, b) => b.count - a.count);

                    // 预测值的真假 
                    var predictGenerate = false;
                    if (UnityEngine.Random.Range(0, 100) < predictWeight)
                    {
                        // 按照最优解生成
                        predictGenerate = true;
                    }
                    else
                    {
                        // 完全随机生成
                        predictGenerate = false;
                    }

                    // 遍历子节点，生成房间
                    foreach (var roomNodeChild in generateNode.node.children)
                    {
                        // 下一个房间的方向（如果有预测值真假，选择从最佳解里生成，还是从可用方向里随机生成）
                        var nextRoomDirection = predictGenerate ? directions.First().direction : availableDirection.GetAndRemoveRandomItem();
                        if (predictGenerate) // 如果有预测值
                        {
                            // 把列表里的第一个元素去掉
                            directions.RemoveAt(0);
                        }

                        if (nextRoomDirection == DoorDirections.Right)
                        {
                            generateNode.doorDirections.Add(DoorDirections.Right); // 标记门的朝向

                            queue.Enqueue(new GenerateRoomNode
                            {
                                x = generateNode.x + 1, // 在初始房间右边生成一个房间
                                y = generateNode.y,
                                node = roomNodeChild,

                                doorDirections = new HashSet<DoorDirections>()
                                {
                                    DoorDirections.Left
                                }
                            });
                        }
                        else if (nextRoomDirection == DoorDirections.Left)
                        {
                            generateNode.doorDirections.Add(DoorDirections.Left);

                            queue.Enqueue(new GenerateRoomNode
                            {
                                x = generateNode.x - 1, // 在初始房间左边生成一个房间
                                y = generateNode.y,
                                node = roomNodeChild,
                                doorDirections = new HashSet<DoorDirections>()
                                {
                                    DoorDirections.Right
                                }
                            });
                        }
                        else if (nextRoomDirection == DoorDirections.Up)
                        {
                            generateNode.doorDirections.Add(DoorDirections.Up);

                            queue.Enqueue(new GenerateRoomNode
                            {
                                x = generateNode.x, // 在初始房间上面生成一个房间
                                y = generateNode.y + 1,
                                node = roomNodeChild,
                                doorDirections = new HashSet<DoorDirections>()
                                {
                                    DoorDirections.Down
                                }
                            });
                        }
                        else if (nextRoomDirection == DoorDirections.Down)
                        {
                            generateNode.doorDirections.Add(DoorDirections.Down);

                            queue.Enqueue(new GenerateRoomNode
                            {
                                x = generateNode.x, // 在初始房间下面生成一个房间
                                y = generateNode.y - 1,
                                node = roomNodeChild,
                                doorDirections = new HashSet<DoorDirections>()
                                {
                                    DoorDirections.Up
                                }
                            });
                        }
                    }
                }
                return true; // 生成房间逻辑跑通
            }

            var predictWeight = 0; // 预测房间方向的权重
            // 循环生成房间，失败了就增加一点预测权重，从完全随机生成，逐渐逼近最佳生成
            print("predictWeight:" + predictWeight);
            while (!GenerateLayoutBFS(layout, layoutGrid, predictWeight))
            {
                Debug.Log("因为冲突，所以重新生成了");
                predictWeight++;
                layoutGrid.Clear();
            }

            // 房间网格
            var roomGrid = new DynaGrid<Room>();
            layoutGrid.ForEach((x, y, generateNode) =>
            {
                var room = GenerateRoomByNode(x, y, generateNode);
                roomGrid[x, y] = room;
            });

            // 根据节点生成房间
            Room GenerateRoomByNode(int x, int y, GenerateRoomNode node)
            {
                var roomPosX = x * (Config.initRoom.codes.First().Length + 2);
                var roomPosY = y * (Config.initRoom.codes.Count + 2);

                // 针对一个房间
                if (node.node.roomTypes == RoomTypes.InitRoom)
                {
                    return BuildRoom(roomPosX, roomPosY, Config.initRoom, node);
                }
                else if (node.node.roomTypes == RoomTypes.NormalRoom)
                {
                    return BuildRoom(roomPosX, roomPosY, Config.normalRoomList.GetRandomItem(), node);
                }
                else if (node.node.roomTypes == RoomTypes.ChestRoom)
                {
                    return BuildRoom(roomPosX, roomPosY, Config.chestRoom, node);
                }
                else if (node.node.roomTypes == RoomTypes.FinalRoom)
                {
                    return BuildRoom(roomPosX, roomPosY, Config.finalRoom, node);
                }

                return null;
            }

            // 生成过道
            void GenerateCorridor()
            {
                // 遍历 roomGrid 里所有的房间
                roomGrid.ForEach((x, y, room) =>
                {
                    foreach (var door in room.doors)
                    {
                        // 左过道
                        if (door.direction == DoorDirections.Left)
                        {
                            var targetRoom = roomGrid[x - 1, y];
                            var targetDoor = targetRoom.doors.First(d => d.direction == DoorDirections.Right);

                            // 绘制过道
                            for (int i = door.X; i <= targetDoor.X; i++)
                            {
                                wallTileMap.SetTile(new Vector3Int(i, door.Y + 2, 0), Wall);
                                floorTileMap.SetTile(new Vector3Int(i, door.Y + 1, 0), Floor);
                                floorTileMap.SetTile(new Vector3Int(i, door.Y, 0), Floor);
                                floorTileMap.SetTile(new Vector3Int(i, door.Y - 1, 0), Floor);
                                wallTileMap.SetTile(new Vector3Int(i, door.Y - 2, 0), Wall);
                            }
                        }
                        else if (door.direction == DoorDirections.Right) // 右过道
                        {
                            var targetRoom = roomGrid[x + 1, y];
                            var targetDoor = targetRoom.doors.First(d => d.direction == DoorDirections.Left);

                            for (int i = door.X; i <= targetDoor.X; i++)
                            {
                                wallTileMap.SetTile(new Vector3Int(i, door.Y + 2, 0), Wall);
                                floorTileMap.SetTile(new Vector3Int(i, door.Y + 1, 0), Floor);
                                floorTileMap.SetTile(new Vector3Int(i, door.Y, 0), Floor);
                                floorTileMap.SetTile(new Vector3Int(i, door.Y - 1, 0), Floor);
                                wallTileMap.SetTile(new Vector3Int(i, door.Y - 2, 0), Wall);
                            }
                        }
                        else if (door.direction == DoorDirections.Up) // 上过道
                        {
                            var targetRoom = roomGrid[x, y + 1];
                            var targetDoor = targetRoom.doors.First(d => d.direction == DoorDirections.Down);

                            for (int i = door.Y; i <= targetDoor.Y; i++)
                            {
                                wallTileMap.SetTile(new Vector3Int(door.X + 2, i, 0), Wall);
                                floorTileMap.SetTile(new Vector3Int(door.X + 1, i, 0), Floor);
                                floorTileMap.SetTile(new Vector3Int(door.X, i, 0), Floor);
                                floorTileMap.SetTile(new Vector3Int(door.X - 1, i, 0), Floor);
                                wallTileMap.SetTile(new Vector3Int(door.X - 2, i, 0), Wall);
                            }
                        }
                        else if (door.direction == DoorDirections.Down) // 下过道
                        {
                            var targetRoom = roomGrid[x, y - 1];
                            var targetDoor = targetRoom.doors.First(d => d.direction == DoorDirections.Up);

                            for (int i = door.Y; i <= targetDoor.Y; i++)
                            {
                                wallTileMap.SetTile(new Vector3Int(door.X + 2, i, 0), Wall);
                                floorTileMap.SetTile(new Vector3Int(door.X + 1, i, 0), Floor);
                                floorTileMap.SetTile(new Vector3Int(door.X, i, 0), Floor);
                                floorTileMap.SetTile(new Vector3Int(door.X - 1, i, 0), Floor);
                                wallTileMap.SetTile(new Vector3Int(door.X - 2, i, 0), Wall);
                            }
                        }
                    }
                });
            }
            GenerateCorridor(); // 调用生成过道的方法
        }

        // 构建房间
        Room BuildRoom(int startRoomPosX, int startRoomPosY, RoomConfig roomConfig, GenerateRoomNode node)
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

            roomScript.generateNode = node;

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
                        // 房间中心点到门的距离
                        var doorDistance = new Vector2(x + 0.5f, y + 0.5f) - new Vector2(roomPositionX, roomPositionY);

                        var cacheX = x;
                        var cacheY = y;

                        // 门在左 or 门在右
                        if (doorDistance.x.Abs() > doorDistance.y.Abs())
                        {
                            if (doorDistance.x > 0) // 门在右边
                            {
                                if (node.doorDirections.Contains(DoorDirections.Right)) // 右方向
                                {
                                    // 获得 Door 脚本：Door 是 Room 的子节点
                                    var doorScript = Door.InstantiateWithParent(roomScript)
                                                            .LocalScaleX(3) // 门拉伸到3格
                                                            .Position2D(new Vector3(x + 0.5f, y + 0.5f, 0))
                                                            .Show(); // 默认门是显示的，根据状态切换开合图片
                                    doorScript.LocalRotation(Quaternion.Euler(0, 0, 90)); // 贴图旋转90度
                                    doorScript.X = x;
                                    doorScript.Y = y;
                                    doorScript.direction = DoorDirections.Right; // 记录门的方向
                                    roomScript.AddDoor(doorScript);

                                    ActionKit.NextFrame(() => // 等一帧后执行
                                    {
                                        // 把一格门扩大成三格门（抹去 d 上下两行的砖）
                                        wallTileMap.SetTile(new Vector3Int(cacheX, cacheY - 1, 0), null);
                                        wallTileMap.SetTile(new Vector3Int(cacheX, cacheY + 1, 0), null);
                                    }).Start(this);
                                }
                                else // 没门就绘制墙
                                {
                                    wallTileMap.SetTile(new Vector3Int(x, y, 0), Wall);
                                }
                            }
                            else // 门在左边
                            {
                                if (node.doorDirections.Contains(DoorDirections.Left)) // 左方向
                                {
                                    // 获得 Door 脚本：Door 是 Room 的子节点
                                    var doorScript = Door.InstantiateWithParent(roomScript)
                                                            .LocalScaleX(3)
                                                            .Position2D(new Vector3(x + 0.5f, y + 0.5f, 0))
                                                            .Show();
                                    doorScript.LocalRotation(Quaternion.Euler(0, 0, 90));
                                    doorScript.X = x;
                                    doorScript.Y = y;
                                    doorScript.direction = DoorDirections.Left;
                                    roomScript.AddDoor(doorScript);

                                    ActionKit.NextFrame(() => // 等一帧后执行
                                    {
                                        // 把一格门扩大成三格门（抹去 d 上下两行的砖）
                                        wallTileMap.SetTile(new Vector3Int(cacheX, cacheY - 1, 0), null);
                                        wallTileMap.SetTile(new Vector3Int(cacheX, cacheY + 1, 0), null);
                                    }).Start(this);
                                }
                                else // 没门就绘制墙
                                {
                                    wallTileMap.SetTile(new Vector3Int(x, y, 0), Wall);
                                }
                            }
                        }
                        else // 门在上 or 门在下
                        {
                            if (doorDistance.y > 0) // 门在上
                            {
                                if (node.doorDirections.Contains(DoorDirections.Up)) // 上方向
                                {
                                    // 获得 Door 脚本：Door 是 Room 的子节点
                                    var doorScript = Door.InstantiateWithParent(roomScript)
                                                            .LocalScaleX(3)
                                                            .Position2D(new Vector3(x + 0.5f, y + 0.5f, 0))
                                                            .Show();
                                    doorScript.X = x;
                                    doorScript.Y = y;
                                    doorScript.direction = DoorDirections.Up;
                                    roomScript.AddDoor(doorScript);

                                    ActionKit.NextFrame(() => // 等一帧后执行
                                    {
                                        // 把一格门扩大成三格门（抹去 d 上下两行的砖）
                                        wallTileMap.SetTile(new Vector3Int(cacheX + 1, cacheY, 0), null);
                                        wallTileMap.SetTile(new Vector3Int(cacheX - 1, cacheY, 0), null);
                                    }).Start(this);
                                }
                                else // 没门就绘制墙
                                {
                                    wallTileMap.SetTile(new Vector3Int(x, y, 0), Wall);
                                }
                            }
                            else // 门在下
                            {
                                if (node.doorDirections.Contains(DoorDirections.Down)) // 下方向
                                {
                                    // 获得 Door 脚本：Door 是 Room 的子节点
                                    var doorScript = Door.InstantiateWithParent(roomScript)
                                                            .LocalScaleX(3)
                                                            .Position2D(new Vector3(x + 0.5f, y + 0.5f, 0))
                                                            .Show();
                                    doorScript.X = x;
                                    doorScript.Y = y;
                                    doorScript.direction = DoorDirections.Down;
                                    roomScript.AddDoor(doorScript);

                                    ActionKit.NextFrame(() => // 等一帧后执行
                                    {
                                        // 把一格门扩大成三格门（抹去 d 上下两行的砖）
                                        wallTileMap.SetTile(new Vector3Int(cacheX + 1, cacheY, 0), null);
                                        wallTileMap.SetTile(new Vector3Int(cacheX - 1, cacheY, 0), null);
                                    }).Start(this);
                                }
                                else // 没门就绘制墙
                                {
                                    wallTileMap.SetTile(new Vector3Int(x, y, 0), Wall);
                                }
                            }
                        }
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
            return roomScript;
        }
    }
}
