using UnityEngine;
using Fusion;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private float speed = 3f;

    // Networked property to sync the target point index
    [Networked] private int targetID { get; set; } = 0;

    private Vector2[] points = new Vector2[2];

    public override void Spawned()
    {
        points[0] = new Vector2(transform.position.x + 5f, transform.position.y);
        points[1] = new Vector2(transform.position.x - 5f, transform.position.y);
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            Vector2 targetPos = points[targetID];

            // Move towards the target
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Runner.DeltaTime);

            // Check if enemy reached the point
            if (Vector2.Distance(transform.position, targetPos) < 0.1f)
            {
                // Switch target index (0 to 1, or 1 to 0)
                targetID = (targetID == 0) ? 1 : 0;
            }
        }
    }
}
