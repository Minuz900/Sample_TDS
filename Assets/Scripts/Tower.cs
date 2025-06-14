using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public List<Zombie> blockingZombies = new List<Zombie>();

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Zombie"))
        {
            Zombie zombie = col.GetComponent<Zombie>();
            if (zombie != null && !blockingZombies.Contains(zombie))
            {
                blockingZombies.Add(zombie);
                GameManager.Instance.UpdateSpeedByZombieCount(blockingZombies.Count);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Zombie"))
        {
            Zombie zombie = col.GetComponent<Zombie>();
            if (zombie != null && blockingZombies.Contains(zombie))
            {
                blockingZombies.Remove(zombie);
                GameManager.Instance.UpdateSpeedByZombieCount(blockingZombies.Count);
            }
        }
    }
}
