using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public string player;
    public int walkSpeed;
    public int rollSpeed;

    private Rigidbody rb;
    private bool isGrounded = true;
    private int speed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;

        WaveField.forceMgr.Objects.Add(new WaveField.Movable()
        {
            Body = rb,
            DragCoefficient = 0.7f,
            StopSpeed = 1f
        });
    }

    void FixedUpdate()
    {
        if (rb.position.y < -20)
        {
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            rb.position = new Vector3(0.0f, 1.2f, 0.0f);
        }

        float moveXAxis = Input.GetAxis("L_XAxis_" + player);
        float moveZAxis = Input.GetAxis("L_YAxis_" + player);

        float moveYAxis = 0.0f;
        if(isGrounded)
        {
            if (Input.GetButtonDown("A_" + player))
            {
                moveYAxis = 300.0f;
            }

            if (Input.GetButton("X_" + player))
            {
                speed = rollSpeed;
            }
            else
            {
                speed = walkSpeed;
            }
        }
        else
        {
            if (Input.GetButtonDown("B_" + player))
            {
                moveXAxis = 0.0f;
                moveYAxis = -800.0f;
                moveZAxis = 0.0f;
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);

                WaveField.forceMgr.Regions.Add(new WaveField.RadialWaveForceRegion()
                {
                    AnnularRegion = new global::WaveField.Annulus2()
                    {
                        Center = new Vector2(rb.position.x, rb.position.z),
                        InnerRadius = 1f,
                        OuterRadius = 2f,
                        MaxArc = Mathf.PI * 2f
                    },
                    Center = new Vector2(rb.position.x, rb.position.z),
                    Speed = 25f,
                    UnitAmplitude = 2f,
                    UnitWaveAmplitude = 2f,
                    Wavelength = 2f,
                    Attenuation = 1f,
                    MaxRadius = 10f
                });
            }
        }
        Vector3 movement = new Vector3(moveXAxis * speed, moveYAxis, moveZAxis * speed);

        rb.AddForce(movement);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Terrain")
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            isGrounded = false;
        }
    }
}