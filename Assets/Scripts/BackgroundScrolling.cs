using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    public enum BackgroundType
    {
        None,
        Type1,
        Type2,
    }
    public float scrollSpeed = 2f;
    public float backgroundWidth = 20f;
    public Transform[] backgrounds;


    public BackgroundType backgroundType = BackgroundType.None;

    void Update()
    {
        if (backgroundType == BackgroundType.Type1)
        {
            scrollSpeed = GameManager.Instance.currentBackgroundSpeed;
        }
        else if (backgroundType == BackgroundType.Type2)
        {
            scrollSpeed = GameManager.Instance.currentBackgroundSecSpeed;
        }

        foreach (var bg in backgrounds)
        {
            bg.position += Vector3.left * scrollSpeed * Time.deltaTime;

            if (bg.position.x <= -backgroundWidth)
            {
                float rightmost = GetRightmostBackgroundX();
                bg.position = new Vector3(rightmost + backgroundWidth, bg.position.y, bg.position.z);
            }
        }
    }

    float GetRightmostBackgroundX()
    {
        float maxX = float.MinValue;
        foreach (var bg in backgrounds)
        {
            if (bg.position.x > maxX)
                maxX = bg.position.x;
        }
        return maxX;
    }
}

