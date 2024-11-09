using System.Collections;
using System.Collections.Generic;
using QFramework.ProjectGungeon;
using UnityEngine;

public class RoomConfig
{
    public RoomTypes roomTypes;
    public List<string> codes = new List<string>();

    public RoomConfig Type(RoomTypes type) // 设定房间类型
    {
        roomTypes = type;
        return this;
    }

    public RoomConfig DrawLine(string code) // 绘制房间格子
    {
        codes.Add(code);
        return this;
    }
}
