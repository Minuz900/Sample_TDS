using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushEventController : MonoBehaviour
{
    public Queue<System.Action> pushEventQueue = new Queue<System.Action>();
    private bool isPushing = false;

    bool isFalling = false;

    public void SetFalling(bool falling)
    {
        isFalling = falling;
    }

    public void RequestPush(System.Action pushAction)
    {
        if (isPushing)
        {
            pushEventQueue.Enqueue(pushAction);
        }
        else
        {
            StartCoroutine(DoPush(pushAction));
        }
    }

    private IEnumerator DoPush(System.Action pushAction)
    {
        isPushing = true;
        pushAction.Invoke();

        yield return new WaitUntil(() => isFalling == false);

        isPushing = false;

        if (pushEventQueue.Count > 0)
        {
            var nextAction = pushEventQueue.Dequeue();
            StartCoroutine(DoPush(nextAction));
        }
    }
}
