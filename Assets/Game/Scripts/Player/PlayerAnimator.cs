using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerAnimator : NetworkBehaviour
{
    private PlayerState state;
    public Animator anim;

    private bool isJumping;
    private bool isTeleporting;

    // Use this for initialization
    void Start()
    {
        state = GetComponent<PlayerState>();
        anim = transform.Find("boi10").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (isLocalPlayer) return;

        if (state.GetState("jumping") && !anim.GetCurrentAnimatorStateInfo(0).IsName("jump!"))
        {
            if (isJumping) return;

            anim.SetTrigger("jump!");
            isJumping = true;
            return;
        }

        if (state.GetState("teleporting") && !anim.GetCurrentAnimatorStateInfo(0).IsName("teleport!"))
        {
            if (isTeleporting) return;

            anim.SetTrigger("teleport!");
            isTeleporting = true;
            return;
        }

        isJumping = false;
        isTeleporting = false;

        anim.SetBool("attacking", state.GetState("attacking"));
        anim.SetBool("moving", state.GetState("moving"));
    }
}
