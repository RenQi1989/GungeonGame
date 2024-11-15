using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP1 : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip pickUpSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            audioSource.Play();
            Player.HP++;
            Player.HPChangeEvent.Invoke();
            Player.Default.DisplayTextOnPlayer("Health + 1", 2);
            Destroy(gameObject);
        }
    }
}
