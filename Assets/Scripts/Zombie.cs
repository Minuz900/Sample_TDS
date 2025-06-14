using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    Rigidbody2D rb;
    ZombiePartsRandomizer parts;
    private Animator animator;

    [SerializeField]
    LayerMask zombieLayer;

    [SerializeField]
    float baseMoveSpeed = 3f;

    [SerializeField]
    float baseGravityScale = 2f;

    [SerializeField]
    float fastMoveSpeed = 3.5f;

    [SerializeField]
    float heavyGravityScale = 6f;

    [SerializeField]
    float jumpCooldown = 0.1f;

    [SerializeField]
    float waitPushTime = 0.2f;


    [SerializeField]
    float detectionFrontRadius = 0.06f;
    [SerializeField]
    Vector2 frontOffset = new Vector2(-0.33f, 0.1f);

    [SerializeField]
    float detectionJumpRadius = 0.29f;
    [SerializeField]
    Vector2 jumpOffset = new Vector2(-0.35f, 0.98f);

    [SerializeField]
    float detectionBackRadius = 0.47f;
    [SerializeField]
    Vector2 backOffset = new Vector2(0.59f, 0.96f);

    [SerializeField]
    float jumpForce = 6.5f;

    [SerializeField]
    float leftForce = 2.2f;

    [SerializeField]
    Vector2 jumpDashForce = new Vector2(-2f, 0.1f);

    [SerializeField]
    float basePosition = -0.85f;

    bool hasJumped = false;
    bool hasPushed = false;
    bool hasCallPushed = true;
    bool isFalling = false;
    float moveSpeed = 3f;
    float lastJumpTime = -999f;
    int layerIndex = 0;


    private void Awake()
    {
        parts = GetComponent<ZombiePartsRandomizer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void SetAttack(bool isAttacking)
    {
        animator.SetBool("IsAttacking", isAttacking);
    }

    void FixedUpdate()
    {
        if (!hasJumped && !hasPushed && Time.time - lastJumpTime > jumpCooldown)
        {
            Vector2 checkFrontPos = (Vector2)transform.position + frontOffset;
            Collider2D frontHit = Physics2D.OverlapCircle(checkFrontPos, detectionFrontRadius, zombieLayer);

            Vector2 checkJumpPos = (Vector2)transform.position + jumpOffset;
            Collider2D jumpHit = Physics2D.OverlapCircle(checkJumpPos, detectionJumpRadius, zombieLayer);

            Vector2 checkBackPos = (Vector2)transform.position + backOffset;
            Collider2D backHit = Physics2D.OverlapCircle(checkBackPos, detectionBackRadius, zombieLayer);

            if (frontHit != null && frontHit.gameObject != gameObject)
            {
                if (jumpHit == null && backHit == null)
                {
                    Jump();
                }

                if (hasCallPushed)
                    hasCallPushed = false;
            }
            else if (transform.position.x < basePosition)
            {
                if (!hasCallPushed)
                {
                    StartCoroutine(WaitPush());
                }
                return;
            }
            else
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            }
        }
    }

    IEnumerator WaitPush()
    {
        hasCallPushed = true;
        SetHeavyGravity();

        yield return new WaitForSeconds(0.1f);
        isFalling = true;
        SetIsFalling();
        GameManager.Instance.pushEvents[layerIndex].RequestPush(
            () => GameManager.Instance.PushLineConnected(layerIndex));
    }


    void Jump()
    {
        rb.velocity = Vector2.zero;

        RefreshMoveSpeed();

        hasJumped = true;
        lastJumpTime = Time.time;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        StartCoroutine(ApplyLateralForceAfterDelay(0.1f));
    }

    IEnumerator ApplyLateralForceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hasJumped)
        {
            rb.AddForce(Vector2.left * leftForce, ForceMode2D.Impulse);
        }
    }

    void JumpDash()
    {
        hasJumped = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDashForce, ForceMode2D.Impulse);
        moveSpeed = fastMoveSpeed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (hasJumped)
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    if (this.transform.position.x < basePosition)
                    {
                        SetHeavyGravity();

                        if (!hasCallPushed)
                        {
                            Push();
                        }
                    }
                    else
                    {
                        JumpDash();
                    }

                    hasJumped = false;

                    break;
                }
            }
        }
        else if (isFalling)
        {
            if (col.gameObject.layer == LayerCache.GetGroundLayerMask(layerIndex) || col.gameObject.layer == gameObject.layer)
            {
                isFalling = false;
                SetIsFalling();
            }
        }
    }

    void SetIsFalling()
    {
        GameManager.Instance.pushEvents[layerIndex].SetFalling(isFalling);
    }

    void SetHeavyGravity()
    {
        rb.gravityScale = heavyGravityScale;
    }

    void Push()
    {
        if (IsZombieAtLeftMost(GameManager.Instance.GetZombies(layerIndex), basePosition, 0.2f))
        {
            isFalling = true;
            SetIsFalling();
            GameManager.Instance.pushEvents[layerIndex].RequestPush(
                () => GameManager.Instance.PushLineConnected(layerIndex));

            hasCallPushed = true;
        }
    }

    bool IsZombieAtLeftMost(List<Zombie> lineZombies, float landingX, float threshold)
    {
        if (lineZombies == null || lineZombies.Count == 0)
            return false;

        // x 오름차순 정렬.
        var sorted = new List<Zombie>(lineZombies);
        sorted.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        // 제일 왼쪽 좀비와 착지 x좌표 비교.
        float leftMostX = sorted[0].transform.position.x;
        if (Mathf.Abs(landingX - leftMostX) < threshold)
            return true;
        return false;
    }

    public void SetZombieLayer(int layerIndex)
    {
        RandomZombieParts();

        this.layerIndex = layerIndex;

        if (this.layerIndex == 0)
        {
            GameManager.Instance.RegisterZombie(this, 0);
            zombieLayer = 1 << LayerMask.NameToLayer("Zombie_Row1");
            SortingUtils.SetSortingRecursively(this.gameObject, "Zombie");
        }
        else if (this.layerIndex == 1)
        {
            GameManager.Instance.RegisterZombie(this, 1);
            zombieLayer = 1 << LayerMask.NameToLayer("Zombie_Row2");
            SortingUtils.SetSortingRecursively(this.gameObject, "Zombie2");
        }
        else
        {
            GameManager.Instance.RegisterZombie(this, 2);
            zombieLayer = 1 << LayerMask.NameToLayer("Zombie_Row3");
            SortingUtils.SetSortingRecursively(this.gameObject, "Zombie3");
        }
    }

    void RandomZombieParts()
    {
        parts.RandomizeParts();
    }

    void RefreshMoveSpeed()
    {
        if (moveSpeed != baseMoveSpeed)
        {
            moveSpeed = baseMoveSpeed;
        }
    }

    void ResumeMovement()
    {
        StartCoroutine(WaitEndPushed(waitPushTime));
    }

    IEnumerator WaitEndPushed(float duration)
    {
        yield return new WaitForSeconds(duration);
        hasPushed = false;
        RefreshMoveSpeed();
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        rb.gravityScale = baseGravityScale;
    }

    public void SmoothPushRight(float distance, float duration)
    {
        StartCoroutine(PushRoutine(distance, duration));
    }

    IEnumerator PushRoutine(float distance, float duration)
    {
        hasPushed = true;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.right * distance;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        ResumeMovement();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + frontOffset, detectionFrontRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere((Vector2)transform.position + jumpOffset, detectionJumpRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + backOffset, detectionBackRadius);
    }
}
