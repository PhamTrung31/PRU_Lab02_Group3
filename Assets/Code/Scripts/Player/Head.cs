using UnityEngine;

public class Head : MonoBehaviour
{
    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player Died: Hit head on ground");

        // Add death logic here (animation, disable movement, game over, etc.)
    }

    // This will be called by the HeadTrigger script
    public void OnHeadHitGround()
    {
        Die();
    }
}
