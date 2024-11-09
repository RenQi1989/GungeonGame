using UnityEngine;
using QFramework;
using System.Collections.Generic;

namespace QFramework.ProjectGungeon
{
    public partial class Room : ViewController
    {
        List<Vector3> enemyGeneratePositionsList = new List<Vector3>(); // 敌人位置列表
        private List<Door> doors = new List<Door>(); // 门列表

        public RoomConfig roomConfig { get; private set; } // 房间配置信息

        private HashSet<Enemy> enemiesRecords = new HashSet<Enemy>(); // 记录已经生成的敌人( HashSet 可以快速检查列表是否为null，不用像 List 一样逐个遍历)

        public RoomStates roomStates { get; set; } = RoomStates.Locked; // 房间状态默认是关闭

        // 房间状态：防止玩家消灭了所有敌人后，退回原房间，门再次关上
        public enum RoomStates
        {
            Locked,
            PlayerInside,
            Unlocked
        }

        private void Update()
        {
            // 每 30 帧判断一次房间里的敌人是否被清空
            if (Time.frameCount % 30 == 0)
            {
                if (enemiesRecords.Count == 0 && roomStates == RoomStates.PlayerInside)
                {
                    roomStates = RoomStates.Unlocked;
                    foreach (var door in doors)
                    {
                        door.Hide();
                    }
                }
                // 从 enemiesRecords 中【批量删除】所有 null 或已销毁的对象
                enemiesRecords.RemoveWhere(e => !e);
            }
        }

        // 把 房间配置信息 赋给 房间
        public Room WithConfig(RoomConfig roomConfig)
        {
            this.roomConfig = roomConfig;
            return this;
        }

        // 将每个敌人的位置添加到敌人位置列表里
        public void AddEnemyPosition(Vector3 enemyGeneratePosition)
        {
            enemyGeneratePositionsList.Add(enemyGeneratePosition);
        }

        // 房间感应检测
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) // 当玩家进入房间（即房间感应到玩家）
            {
                // 只有房间类型为 Normal 且 房间状态是关闭 时，才会生成敌人和门
                if (roomConfig.roomTypes == RoomTypes.NormalRoom && roomStates == RoomStates.Locked)
                {
                    roomStates = RoomStates.PlayerInside;

                    // 生成敌人
                    foreach (var enemyGeneratePosition in enemyGeneratePositionsList)
                    {
                        var enemy = Instantiate(LevelController.Default.enemyPrefab);
                        enemy.transform.position = enemyGeneratePosition; // 敌人出现的位置就是之前生成好的位置
                        enemy.gameObject.SetActive(true);

                        enemiesRecords.Add(enemy);
                    }
                    // 显示门
                    foreach (var door in doors)
                    {
                        door.Show();
                    }
                }
            }
        }

        public void AddDoor(Door door)
        {
            doors.Add(door);
        }
    }
}
