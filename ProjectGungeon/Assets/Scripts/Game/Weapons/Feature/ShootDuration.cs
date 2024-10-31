using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public class ShootDuration // 抽象所有武器的共同属性
    {
        public float Duration { get; set; }
        public float ChargeTime { get; set; }

        // 允许射击的条件：冷却完毕
        public bool CanShoot => Duration <= 0;

        // 构造器
        public ShootDuration(float shootDuration, float chargeTime)
        {
            Duration = shootDuration;
            ChargeTime = chargeTime;
        }
    }
}