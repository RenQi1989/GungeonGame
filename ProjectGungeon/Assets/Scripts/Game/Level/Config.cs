using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public enum RoomTypes
    {
        InitRoom,
        NormalRoom,
        ChestRoom,
        FinalRoom
    }

    public class EnemyWaveConfig { }

    public class Config
    {
        // 字符串房间列表
        /*

        1 代表 Ground
        @ 代表 主角
        e 代表 敌人
        d 代表 门
        # 代表 终点传送门

        */

        // 加 { get; set; } 防止序列化（不可在unity里被误修改）
        // 序列化 = public = 在 Unity 里直接显示

        public static RoomConfig initRoom = new RoomConfig()
                                            .Type(RoomTypes.InitRoom) // 使用 RoomConfig 类的 Type 方法

            // 使用 RoomConfig 类的 DrawLine 方法
            .DrawLine("11111111ddd11111111")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1            1    1")
            .DrawLine("1             1   1")
            .DrawLine("1   11            1")
            .DrawLine("d    1            d")
            .DrawLine("d  @              d")
            .DrawLine("d        1        d")
            .DrawLine("1       1         1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("11111111ddd11111111");

        // 正常房间列表
        public static List<RoomConfig> normalRoomList = new List<RoomConfig>()
        {
            // 样式一
            new RoomConfig().Type(RoomTypes.NormalRoom)

            .DrawLine("11111111ddd11111111")
            .DrawLine("1                 1")
            .DrawLine("1        e        1")
            .DrawLine("1      1   1      1")
            .DrawLine("1        1        1")
            .DrawLine("1        e        1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("d        1        d")
            .DrawLine("d       e e       d")
            .DrawLine("d        1        d")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1        e        1")
            .DrawLine("1        1        1")
            .DrawLine("1      1   1      1")
            .DrawLine("1        e        1")
            .DrawLine("1                 1")
            .DrawLine("11111111ddd11111111"),

            // 样式二
            new RoomConfig().Type(RoomTypes.NormalRoom)

            .DrawLine("11111111ddd11111111")
            .DrawLine("1                 1")
            .DrawLine("1        e        1")
            .DrawLine("1    1       1    1")
            .DrawLine("1    e       e    1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("d      e 1        d")
            .DrawLine("d     e 1 1       d")
            .DrawLine("d      e 1        d")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1    e       e    1")
            .DrawLine("1    1       1    1")
            .DrawLine("1        e        1")
            .DrawLine("1                 1")
            .DrawLine("11111111ddd11111111"),

            // 样式三
            new RoomConfig().Type(RoomTypes.NormalRoom)

            .DrawLine("11111111ddd11111111")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1  e  11          1")
            .DrawLine("1      1          1")
            .DrawLine("1      11  e      1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("d        e        d")
            .DrawLine("d      e 1 e      d")
            .DrawLine("d        e        d")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1           11  e 1")
            .DrawLine("1           1     1")
            .DrawLine("1       e  11     1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("11111111ddd11111111"),
        };

        public static RoomConfig finalRoom = new RoomConfig()
                                            .Type(RoomTypes.FinalRoom)

            .DrawLine("11111111ddd11111111")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("d                 d")
            .DrawLine("d        #        d")
            .DrawLine("d                 d")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("11111111ddd11111111");

        public static RoomConfig chestRoom = new RoomConfig()
                                    .Type(RoomTypes.ChestRoom)

            .DrawLine("11111111ddd11111111")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("d                 d")
            .DrawLine("d        c        d")
            .DrawLine("d                 d")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("1                 1")
            .DrawLine("11111111ddd11111111");
    }
}
