using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadilloAnimation : MonoBehaviour
{
    public string player = "1";

    private Animator animator;
    private Rigidbody rb;

    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetAxis("L_XAxis_" + player) < -0.2)
        {
            if (Input.GetAxis("L_YAxis_" + player) > 0.2)
            {
                // Top right
                animator.SetInteger("DirectionState", 1);
            }
            else if (Input.GetAxis("L_YAxis_" + player) < -0.2)
            {
                // Bottom right
                animator.SetInteger("DirectionState", 7);
            }
            else
            {
                // Right
                animator.SetInteger("DirectionState", 0);
            }
        }
        else if (Input.GetAxis("L_XAxis_" + player) > 0.2)
        {
            if (Input.GetAxis("L_YAxis_" + player) > 0.2)
            {
                // Top left
                animator.SetInteger("DirectionState", 3);
            }
            else if (Input.GetAxis("L_YAxis_" + player) < -0.2)
            {
                // Bottom left
                animator.SetInteger("DirectionState", 5);
            }
            else
            {
                // Left
                animator.SetInteger("DirectionState", 4);
            }
        }
        else
        {
            if (Input.GetAxis("L_YAxis_" + player) > 0.2)
            {
                // Top
                animator.SetInteger("DirectionState", 2);
            }
            else if (Input.GetAxis("L_YAxis_" + player) < -0.2)
            {
                // Bottom
                animator.SetInteger("DirectionState", 6);
            }
        }

        if (Input.GetButton("X_" + player))
        {
            animator.SetBool("IsRolling", true);
        }
        else
        {
            animator.SetBool("IsRolling", false);
        }

        if (rb.velocity.magnitude < 0.5)
        {
            animator.SetBool("IsIdle", true);
        }
        else
        {
            animator.SetBool("IsIdle", false);
        }
	}
}
