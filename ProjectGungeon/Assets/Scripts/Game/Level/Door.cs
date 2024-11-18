using UnityEngine;
using QFramework;
using Unity.VisualScripting;
using MoonSharp.Interpreter.IO;

namespace QFramework.ProjectGungeon
{
    public partial class Door : ViewController
    {
        public LevelController.DoorDirections direction { get; set; }
        public int X { get; set; } // 门的 XY 坐标
        public int Y { get; set; }
        public AudioSource audioSource;
        public AudioClip doorSound;

        public enum DoorStates
        {
            DefaultOpen, // 默认开启（仅初始房间）
            Open,
            DefaultClose, // 默认关闭
            BattleClose // 主角进入房间战斗时关闭
        }

        // 门的状态机
        public FSM<DoorStates> doorState = new FSM<DoorStates>();
        private void Awake()
        {
            // 在 DefaultOpen 状态下
            doorState.State(DoorStates.DefaultOpen)
            .OnEnter(() =>
            {
                GetComponent<SpriteRenderer>().sprite = DoorOpen; // 把门的贴图设置为 开门图
                GetComponent<BoxCollider2D>().Disable(); // 碰撞器失效
            });

            // 在 Open 状态下
            doorState.State(DoorStates.Open)
            .OnEnter(() =>
            {
                GetComponent<SpriteRenderer>().sprite = DoorOpen; // 把门的贴图设置为 开门图
                GetComponent<BoxCollider2D>().Disable(); // 碰撞器失效
                audioSource.Play();
            });

            // 在 DefaultClose 状态下
            doorState.State(DoorStates.DefaultClose)
            .OnEnter(() =>
            {
                GetComponent<SpriteRenderer>().sprite = DoorClose; // 把门的贴图设置为 关门图
                GetComponent<BoxCollider2D>().isTrigger = true; // 执行碰撞逻辑
            });

            // 在 BattleClose 状态下
            doorState.State(DoorStates.BattleClose)
            .OnEnter(() =>
            {
                GetComponent<SpriteRenderer>().sprite = DoorClose; // 把门的贴图设置为 关门图
                GetComponent<BoxCollider2D>().Enable(); // 碰撞器有效
                GetComponent<BoxCollider2D>().isTrigger = false; // 不执行碰撞逻辑
                audioSource.Play();
            });

            doorState.StartState(DoorStates.DefaultClose); // 所有非 InitRoom 门的默认状态：默认关闭
        }

        // 门和主角的碰撞逻辑
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 门关闭时，如果主角碰撞门，则门开
            if (other.gameObject.GetComponent<Player>() && doorState.CurrentStateId == DoorStates.DefaultClose)
            {
                doorState.ChangeState(DoorStates.Open);
            }
        }

    }
}
