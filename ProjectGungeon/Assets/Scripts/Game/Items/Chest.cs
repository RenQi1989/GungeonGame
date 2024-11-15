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
            Player.Default.DisplayTextOnPlayer("Open the chest", 2);
            GameObject hp1 = Instantiate(HP1Prefab, transform.position, Quaternion.identity);
            hp1.SetActive(true);
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}
