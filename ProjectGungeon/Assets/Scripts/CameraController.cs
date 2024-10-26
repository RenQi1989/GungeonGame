using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Player player;
    void Start()
    {

    }

    void Update()
    {
        if (player) // 如果 player 对象存在摄像机就跟随
        {
            var cameraPosition = player.transform.position + Vector3.up * 0.5f;
            cameraPosition.z = -10;
            transform.position = cameraPosition; // 每一帧都根据玩家的最新位置，重新计算摄像机的位置，从而让摄像机始终跟随玩家移动。
        }
    }
}
