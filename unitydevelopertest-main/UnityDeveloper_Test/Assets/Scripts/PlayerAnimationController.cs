using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

   public void RunningAnim(float move)
    {
        anim.SetFloat("Move", move);
    }
    public void JumpingAnim(bool isJumping)
    {
        anim.SetBool("IsFalling", isJumping);
    }
}
