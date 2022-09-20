using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private int[] states = new int[3];


    void Start()
    {
        animator = GetComponent<Animator>();

        states[(int)CharacterMovement.Animations.RUN] = Animator.StringToHash("Run");
        states[(int)CharacterMovement.Animations.WALK] = Animator.StringToHash("Walk");
        states[(int)CharacterMovement.Animations.JUMP] = Animator.StringToHash("Jump");
    }

    public void Change(CharacterMovement.Animations state)
    {
        switch (state)
        {
            case CharacterMovement.Animations.IDLE:

                animator.SetBool(states[(int)CharacterMovement.Animations.WALK], false);
                animator.SetBool(states[(int)CharacterMovement.Animations.RUN], false);
                break;

            case CharacterMovement.Animations.JUMP:

                animator.SetTrigger(states[(int)CharacterMovement.Animations.JUMP]);
                break;

            case CharacterMovement.Animations.RUN:

                animator.SetBool(states[(int)CharacterMovement.Animations.WALK], true);
                animator.SetBool(states[(int)CharacterMovement.Animations.RUN], true);
                break;

            case CharacterMovement.Animations.WALK:

                animator.SetBool(states[(int)CharacterMovement.Animations.WALK], true);
                animator.SetBool(states[(int)CharacterMovement.Animations.RUN], false);
                break;


        }
    }

}
