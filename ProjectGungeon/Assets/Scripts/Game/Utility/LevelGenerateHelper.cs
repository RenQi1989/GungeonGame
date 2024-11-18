using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public class LevelGenerateHelper
    {
        // 生成下一个房间之前，先判断是否有可用的生成方向
        public static List<LevelController.DoorDirections> GetAvailableDirections(DynaGrid<LevelController.GenerateRoomNode> layoutGrid, int x, int y)
        {
            var availableDirection = new List<LevelController.DoorDirections>();

            // 如果房间的右边是空的，就可以在右边生成房间
            if (layoutGrid[x + 1, y] == null)
            {
                availableDirection.Add(LevelController.DoorDirections.Right);
            }
            if (layoutGrid[x - 1, y] == null)
            {
                availableDirection.Add(LevelController.DoorDirections.Left);
            }
            if (layoutGrid[x, y + 1] == null)
            {
                availableDirection.Add(LevelController.DoorDirections.Up);
            }
            if (layoutGrid[x, y - 1] == null)
            {
                availableDirection.Add(LevelController.DoorDirections.Down);
            }
            return availableDirection;
        }

        // 记录每个方向可能生成多少个房间
        public class DirectionWithCount
        {
            public LevelController.DoorDirections direction;
            public int count;
        }

        // 预测门可能生成的方向
        public static List<DirectionWithCount> PredictDirection(DynaGrid<LevelController.GenerateRoomNode> layoutGrid, int x, int y)
        {
            var availableDirections = GetAvailableDirections(layoutGrid, x, y);

            // 把每个方向对应的数量，添加到 List 里
            var directions = new List<DirectionWithCount>();

            foreach (var availableDirection in availableDirections)
            {
                if (availableDirection == LevelController.DoorDirections.Up)
                {
                    var upNodeAvailableDirection = GetAvailableDirections(layoutGrid, x, y + 1);
                    directions.Add(new DirectionWithCount()
                    {
                        count = upNodeAvailableDirection.Count, // 统计能向 UP 方向生成房间的数量有多少
                        direction = availableDirection
                    });
                }
                if (availableDirection == LevelController.DoorDirections.Down)
                {
                    var downNodeAvailableDirection = GetAvailableDirections(layoutGrid, x, y - 1);
                    directions.Add(new DirectionWithCount()
                    {
                        count = downNodeAvailableDirection.Count,
                        direction = availableDirection
                    });
                }
                if (availableDirection == LevelController.DoorDirections.Left)
                {
                    var leftNodeAvailableDirection = GetAvailableDirections(layoutGrid, x - 1, y);
                    directions.Add(new DirectionWithCount()
                    {
                        count = leftNodeAvailableDirection.Count,
                        direction = availableDirection
                    });

                }
                if (availableDirection == LevelController.DoorDirections.Up)
                {
                    var rightNodeAvailableDirection = GetAvailableDirections(layoutGrid, x + 1, y);
                    directions.Add(new DirectionWithCount()
                    {
                        count = rightNodeAvailableDirection.Count,
                        direction = availableDirection
                    });
                }
            }
            return directions; // 返回存储「每个方向」和「方向对应可以生成房间数量」信息的列表
        }

    }
}
