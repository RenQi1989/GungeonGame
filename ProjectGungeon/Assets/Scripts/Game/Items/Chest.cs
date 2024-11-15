using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip openChestSound;
    public GameObject HP1Prefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            audioSource.Play();
            StartCoroutine(OpenChest());
        }
    }

    private IEnumerator OpenChest()
    {
        yield return new WaitForSeconds(openChestSound.length);  // 等待宝箱音效播放完成

        GameObject hp1 = Instantiate(HP1Prefab, transform.position, Quaternion.identity);
        hp1.SetActive(true); // 生成道具

        Destroy(gameObject); // 销毁宝箱
    }
}
