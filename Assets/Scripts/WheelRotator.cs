using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    public float rotateSpeed = 360f;

    void Update()
    {
        rotateSpeed = GameManager.Instance.currentWheelSpeed;
        transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
    }
}
