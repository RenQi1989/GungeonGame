using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public enum RoomTypes
    {
        initRoom,
        NormalRoom,
        FinalRoom
    }

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
                                            .Type(RoomTypes.initRoom) // 使用 RoomConfig 类的 Type 方法

            // 使用 RoomConfig 类的 DrawLine 方法
            .DrawLine("111111111111111111")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1            1   1")
            .DrawLine("1             1  1")
            .DrawLine("1   11           d")
            .DrawLine("1    1           d")
            .DrawLine("1  @             d")
            .DrawLine("1        1       d")
            .DrawLine("1       1        1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("111111111111111111");

        public static RoomConfig normalRoom = new RoomConfig()
                                            .Type(RoomTypes.NormalRoom)

            .DrawLine("111111111111111111")
            .DrawLine("1                1")
            .DrawLine("1        e       1")
            .DrawLine("1       e1       1")
            .DrawLine("1        e       1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("d      1         d")
            .DrawLine("d        e1      d")
            .DrawLine("d        e1      d")
            .DrawLine("d      1         d")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1        e       1")
            .DrawLine("1       e1       1")
            .DrawLine("1        e       1")
            .DrawLine("1                1")
            .DrawLine("111111111111111111");

        public static RoomConfig finalRoom = new RoomConfig()
                                            .Type(RoomTypes.FinalRoom)

            .DrawLine("111111111111111111")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("d                1")
            .DrawLine("d                1")
            .DrawLine("d           #    1")
            .DrawLine("d                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("1                1")
            .DrawLine("111111111111111111");
    }
}
