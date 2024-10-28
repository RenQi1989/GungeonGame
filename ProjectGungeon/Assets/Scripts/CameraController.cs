using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            // 相机滑动跟随
            var targetPosition = new Vector2(player.transform.position.x, player.transform.position.y); // 相机的目标位置是 player 的位置
            Vector3 currentPosition = this.transform.position; // 相机当前的位置

            currentPosition = Vector2.Lerp(currentPosition, targetPosition, (1.0f - MathF.Exp(-Time.deltaTime * 5)));

            currentPosition.z = -10; // 让相机后退一点

            transform.position = currentPosition; // 每一帧都根据玩家的最新位置，重新计算摄像机的位置，从而让摄像机始终跟随玩家移动。

        }
    }
}
