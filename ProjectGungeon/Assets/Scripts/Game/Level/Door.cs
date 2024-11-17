using UnityEngine;
using QFramework;
using Unity.VisualScripting;

namespace QFramework.ProjectGungeon
{
    public partial class Door : ViewController
    {
        public LevelController.DoorDirections direction { get; set; }
        public int X { get; set; } // 门的 XY 坐标
        public int Y { get; set; }
        public AudioSource audioSource;
        public AudioClip openSound;

        public enum DoorStates
        {
            Open,
            Close
        }

        // 门的状态机
        public FSM<DoorStates> doorState = new FSM<DoorStates>();
        private void Awake()
        {
            // 在 Open 状态下
            doorState.State(DoorStates.Open)
            .OnEnter(() =>
            {
                GetComponent<SpriteRenderer>().sprite = DoorOpen; // 把门的贴图设置为 开门图
                GetComponent<BoxCollider2D>().isTrigger = true; // 门开的时候不检测碰撞（不卡人）
            })
            .OnExit(() =>
            {
                audioSource.Play();
            });

            // 在 Close 状态下
            doorState.State(DoorStates.Close)
            .OnEnter(() =>
            {
                GetComponent<SpriteRenderer>().sprite = DoorClose; // 把门的贴图设置为 关门图
                GetComponent<BoxCollider2D>().isTrigger = false;
            })
            .OnExit(() =>
            {
                audioSource.Play();
            });

            doorState.StartState(DoorStates.Open);
        }

    }
}
