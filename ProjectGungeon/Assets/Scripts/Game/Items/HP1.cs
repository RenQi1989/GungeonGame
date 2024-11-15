using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP1 : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip pickUpSound;
    private SpriteRenderer spriteRenderer;

    private void OnTriggerEnter2D(Collider2D other)

    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (other.gameObject.GetComponent<Player>())
        {
            audioSource.Play();
            spriteRenderer.enabled = false;
            Player.HP++;
            Player.HPChangeEvent.Invoke();
            Player.Default.DisplayTextOnPlayer("Health + 1", 2);
            StartCoroutine(GetHP1());
        }
    }
    private IEnumerator GetHP1()
    {
        yield return new WaitForSeconds(pickUpSound.length);
        Destroy(gameObject);
    }

}
