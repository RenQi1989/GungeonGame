using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public PlayerBullet playerBulletPrefab;
    public List<AudioClip> shootSounds = new List<AudioClip>(); // 射击音效列表
    public AudioSource shootSoundPlayer;

    // 点击鼠标攻击
    public void ShootMouseButtonDown(Vector2 shootDirection)
    {
        var playerBullet = Instantiate(playerBulletPrefab);

        // 子弹发射的位置就是子弹模板的位置（枪口处）
        playerBullet.transform.position = playerBulletPrefab.transform.position;
        playerBullet.direction = shootDirection;
        playerBullet.gameObject.SetActive(true); // 把在Inspector失活的子弹重新激活

        // 播放随机射击音效
        var soundsIndex = Random.Range(0, shootSounds.Count);
        shootSoundPlayer.clip = shootSounds[soundsIndex];
        shootSoundPlayer.Play();

    }

    public void ShootMouseButtonHold(Vector2 shootingDirection)
    {

    }

    public void ShootMouseButtonUp(Vector2 shootingDirection)
    {

    }
}
