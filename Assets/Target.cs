using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : SnapBreakStop
{
    [Header("Target")]
    public float impactModifier = 2f;
    private Animator animator;

    private void Awake()
    {
        animator = transform.GetComponentInChildren<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<PoolBallController>(out PoolBallController pbc))
        {
            Vector3 additionalImpulse = collision.relativeVelocity * impactModifier; // Triple the impact, so double the existing impact
            if (rb != null)
            {
                rb.AddForce(additionalImpulse, ForceMode.Impulse);
            }
            print("Start animation");
            animator.Play("Skull", -1, 0f);
            animator.speed = 1;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float playTime = 1f;
            if (stateInfo.IsName("Skull"))
            {
                playTime = stateInfo.length * stateInfo.normalizedTime;
            }
            Invoke(nameof(ResetAnimation), playTime);
        }
    }

    private void ResetAnimation()
    {
        animator.Play("Skull", 0, 0f);
        animator.speed = 0; // Stop the animation
    }
}
