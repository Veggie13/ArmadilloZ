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
        Vector2 facing = new Vector2(Input.GetAxis("L_XAxis_" + player), Input.GetAxis("L_YAxis_" + player));
        bool rolling = Input.GetButton("X_" + player);

        float faceBearingGrad = (Mathf.Atan2(facing.y, facing.x) / Mathf.PI + 2f) % 2f;

        animator.SetBool("IsRolling", rolling);

        if (facing.magnitude > 0.1f)
        {
            int faceDirection;
            if (rolling)
            {
                var dirs = new[] { 0, 7, 5, 4, 7, 5 };
                faceDirection = dirs[Mathf.RoundToInt(3f * faceBearingGrad + 6f) % 6];
            }
            else
            {
                var dirs = new[] { 3, 2, 1, 7, 6, 5 };
                faceDirection = dirs[(Mathf.RoundToInt(3f * (faceBearingGrad - 0.5f) + 6f) + 1) % 6];
            }
            animator.SetInteger("DirectionState", faceDirection);
        }

        if (facing.magnitude < 0.2)
        {
            animator.SetBool("IsIdle", true);
        }
        else
        {
            animator.SetBool("IsIdle", false);
        }
	}
}
