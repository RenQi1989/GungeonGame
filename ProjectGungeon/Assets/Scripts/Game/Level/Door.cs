using UnityEngine;
using QFramework;

namespace QFramework.ProjectGungeon
{
    public partial class Door : ViewController
    {
        public LevelController.DoorDirections direction { get; set; }
        public int X { get; set; } // 门的 XY 坐标
        public int Y { get; set; }
    }
}
