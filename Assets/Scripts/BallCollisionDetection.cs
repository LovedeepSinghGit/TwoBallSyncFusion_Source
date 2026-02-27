using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BallCollisionDetection : NetworkBehaviour
{
    [SerializeField] private Vector2 resetPosition = new Vector2(0, -5f);
    [SerializeField] private Rigidbody2D ballRb;
    [SerializeField] private Ball ballScript;


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Only the Host should handle logic that changes networked state
        if (Object != null && Object.HasStateAuthority)
        {
            if (other.CompareTag("BottomBoundery"))
            {
                TeleportBall();
            }

            if (other.CompareTag("FinishLine"))
            {
                Ball ballScript = GetComponent<Ball>();

                if (ballScript.Winner == PlayerRef.None)
                {
                    ballScript.Winner = Object.InputAuthority;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Object != null && Object.HasStateAuthority)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                Vector2 hitNormal = collision.GetContact(0).normal;

                if (hitNormal.y > 0.7f)
                {
                    NetworkObject enemyNetObj = collision.gameObject.GetComponent<NetworkObject>();
                    if (enemyNetObj != null)
                    {
                        Runner.Despawn(enemyNetObj);
                    }
                }
                else
                {
                    TeleportBall();
                }
            }
        }
    }

    private void TeleportBall()
    {
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        ballRb.position = resetPosition;
    }
}
