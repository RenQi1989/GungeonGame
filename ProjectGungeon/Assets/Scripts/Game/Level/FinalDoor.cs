using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            GameUI.Default.gamePass.SetActive(true);
            Time.timeScale = 0; // 时间停止
        }
    }
}
