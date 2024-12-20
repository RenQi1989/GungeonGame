using System;
using System.Collections;
using System.Collections.Generic;
using QFramework.ProjectGungeon;
using UnityEngine;

public class RoomConfig // 房间配置信息
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

    public class RoomNode // 管理房间生成的节点
    {
        public RoomTypes roomTypes = RoomTypes.InitRoom; // 默认是初始房间
        public List<RoomNode> children = new List<RoomNode>();

        public RoomNode(RoomTypes type) // 构造器
        {
            roomTypes = type;
        }

        // 生成下一个房间(根据房间分支)
        public RoomNode GenerateNextRoom(RoomTypes type, Action<RoomNode> branch = null)
        {
            var roomNode = new RoomNode(type);

            children.Add(roomNode);
            branch?.Invoke(roomNode); // 将新增的 roomNode 传给 branch
            return roomNode;
        }
    }
}
