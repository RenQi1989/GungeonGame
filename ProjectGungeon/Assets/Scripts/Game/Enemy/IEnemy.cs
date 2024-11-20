using System.Collections;
using System.Collections.Generic;
using QFramework.ProjectGungeon;
using UnityEngine;

public interface IEnemy // 管理所有敌人的接口
{
    GameObject gameObject
    {
        get => this.gameObject; // 返回当前对象的 GameObject
        set => throw new System.NotImplementedException(); // 一般不需要设置，可以保留抛出异常
    }

    void Hurt(int damage);

    Room room { get; set; }
}
