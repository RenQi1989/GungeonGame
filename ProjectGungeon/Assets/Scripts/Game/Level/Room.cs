using UnityEngine;
using QFramework;
using System.Collections.Generic;
using System.Collections;

namespace QFramework.ProjectGungeon
{
    public partial class Room : ViewController
    {
        List<Vector3> enemyGeneratePositionsList = new List<Vector3>(); // 敌人位置列表
        private List<Door> doors = new List<Door>(); // 门列表
        public RoomConfig roomConfig { get; private set; } // 房间配置信息
        private HashSet<Enemy> enemiesRecords = new HashSet<Enemy>(); // 记录已经生成的敌人( HashSet 可以快速检查列表是否为null，不用像 List 一样逐个遍历)
        public RoomStates roomStates { get; set; } = RoomStates.Locked; // 房间状态默认是关闭

        // 敌人波次列表
        private List<EnemyWaveConfig> enemyWaves = new List<EnemyWaveConfig>()
        {
            new EnemyWaveConfig(),
            new EnemyWaveConfig()
        };

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
                // 敌人记录数量为0 且 房间状态为主角已进入
                if (enemiesRecords.Count == 0 && roomStates == RoomStates.PlayerInside)
                {
                    if (enemyWaves.Count > 0) // 还有剩余敌人波次
                    {
                        GenerateEnemies();
                    }
                    else // 没有敌人剩余波次
                    {
                        // 门打开
                        roomStates = RoomStates.Unlocked;
                        foreach (var door in doors)
                        {
                            door.Hide();
                        }
                    }
                }
                // 从 enemiesRecords 中【批量删除】所有 null 或已销毁的对象
                enemiesRecords.RemoveWhere(e => !e);
            }
        }

        // 生成敌人(有敌人剩余波次时才生成)
        public void GenerateEnemies()
        {
            enemyWaves.RemoveAt(0); // 移除上一波敌人(移除列表里的第一个元素)

            // Shuffle方法：随机生成敌人位置和数量
            var enemyCount = Random.Range(3, 6 + 1);

            Shuffle(enemyGeneratePositionsList);

            for (int i = 0; i < enemyCount && i < enemyGeneratePositionsList.Count; i++)
            {
                var enemy = Instantiate(LevelController.Default.enemyPrefab);
                enemy.transform.position = enemyGeneratePositionsList[i]; // 敌人出现的位置按随机分配
                enemy.gameObject.SetActive(true);

                enemiesRecords.Add(enemy);
            }
        }

        // Shuffle API
        private void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
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
                // 只有房间类型为 Normal 且 房间状态是关闭 时，才会生成敌人 & 门
                if (roomConfig.roomTypes == RoomTypes.NormalRoom && roomStates == RoomStates.Locked)
                {
                    roomStates = RoomStates.PlayerInside;

                    GenerateEnemies();
                    StartCoroutine(ShowDoorsWithDelay()); // 关门延迟协程
                }
            }
        }

        // 关门延迟协程
        private IEnumerator ShowDoorsWithDelay()
        {
            yield return new WaitForSeconds(0.5f); // 延迟0.5秒
            foreach (var door in doors)
            {
                door.Show();
            }
        }

        public void AddDoor(Door door)
        {
            doors.Add(door);
        }

    }
}
