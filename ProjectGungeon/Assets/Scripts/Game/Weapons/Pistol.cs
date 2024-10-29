using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace QFramework.ProjectGungeon
{
    public partial class Pistol : GunManager
    {
        public override PlayerBullet bulletPrefab => PlayerBullet;
        public override AudioSource audioPlayer => SelfAudioSource;


    }
}
