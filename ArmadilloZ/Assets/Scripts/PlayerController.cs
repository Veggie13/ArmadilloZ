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
            StopSpeed = 0.05f
        });
    }

    void FixedUpdate()
    {
        if (rb.position.y < -20)
        {
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            rb.position = new Vector3(0.0f, 1.2f, 0.0f);
        }
        else if (rb.position.y > 2)
        {
            isGrounded = false;
        }

        float moveXAxis = Input.GetAxis("L_XAxis_" + player);
        float moveZAxis = Input.GetAxis("L_YAxis_" + player);

        float moveYAxis = 0.0f;
        if(isGrounded)
        {
            if (Input.GetButton("A_" + player))
            {
                moveYAxis = 300.0f;
                isGrounded = false;
            }

            if (Input.GetButton("X_" + player))
            {
                speed = rollSpeed;
                Vector2 planeVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
                float bearing = Mathf.Atan2(planeVelocity.y, planeVelocity.x);
                float wakeBearing1 = bearing + 1.5f * Mathf.PI / 2f;
                float wakeBearing2 = bearing + 2.5f * Mathf.PI / 2f;

                WaveField.forceMgr.Regions.Add(new WaveField.RadialWaveForceRegion()
                {
                    AnnularRegion = new global::WaveField.Annulus2()
                    {
                        Center = new Vector2(rb.position.x, rb.position.z),
                        InnerRadius = 0.25f,
                        OuterRadius = 0.5f,
                        MinArc = wakeBearing1 - 0.1f,
                        MaxArc = wakeBearing1 + 0.1f
                    },
                    Center = new Vector2(rb.position.x, rb.position.z),
                    Speed = 2f,
                    UnitAmplitude = 1f,
                    UnitWaveAmplitude = 0.25f,
                    Wavelength = 0.5f,
                    Attenuation = 0.7f,
                    MaxRadius = 1f
                });
                WaveField.forceMgr.Regions.Add(new WaveField.RadialWaveForceRegion()
                {
                    AnnularRegion = new global::WaveField.Annulus2()
                    {
                        Center = new Vector2(rb.position.x, rb.position.z),
                        InnerRadius = 0.25f,
                        OuterRadius = 0.5f,
                        MinArc = wakeBearing2 - 0.1f,
                        MaxArc = wakeBearing2 + 0.1f
                    },
                    Center = new Vector2(rb.position.x, rb.position.z),
                    Speed = 2f,
                    UnitAmplitude = 1f,
                    UnitWaveAmplitude = 0.25f,
                    Wavelength = 0.5f,
                    Attenuation = 0.7f,
                    MaxRadius = 1f
                });
            }
            else
            {
                speed = walkSpeed;
            }

            if (Input.GetButton("Y_" + player))
            {
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
        else
        {
            if (Input.GetButton("B_" + player))
            {
                moveXAxis = 0.0f;
                moveYAxis = -300.0f;
                moveZAxis = 0.0f;
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);

                WaveField.forceMgr.Regions.Add(new WaveField.RadialWaveForceRegion()
                {
                    AnnularRegion = new global::WaveField.Annulus2()
                    {
                        Center = new Vector2(rb.position.x, rb.position.z),
                        InnerRadius = 0.25f,
                        OuterRadius = 0.5f,
                        MaxArc = Mathf.PI * 2f
                    },
                    Center = new Vector2(rb.position.x, rb.position.z),
                    Speed = 6f,
                    UnitAmplitude = 2f,
                    UnitWaveAmplitude = 0.5f,
                    Wavelength = 0.5f,
                    Attenuation = 1f,
                    MaxRadius = 2.5f
                });
                isGrounded = true;
            }
        }
        Vector2 targetPlaneVelocity = new Vector2(moveXAxis, moveZAxis) * speed;
        Vector2 currPlaneVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
        Vector2 diffPlaneVelocity = targetPlaneVelocity - currPlaneVelocity;
        Vector3 force = new Vector3(diffPlaneVelocity.x * 4f, moveYAxis, diffPlaneVelocity.y * 4f);

        rb.AddForce(force, ForceMode.Acceleration);
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