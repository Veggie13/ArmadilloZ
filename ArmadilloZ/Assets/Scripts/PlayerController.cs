using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public string player;
    public int walkSpeed;
    public int rollSpeed;

    private AudioSource feetAudio;
    private AudioSource rollAudio;
    private AudioSource poundAudio;
    private AudioSource blastAudio;
    private Rigidbody rb;
    private bool isGrounded = true;
    private bool isPounding = false;
    private bool isRolling = false;
    private bool isWalking = false;
    private int speed;
    private Vector2 lastFacing;
    private WaveField.Movable movable;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        feetAudio = GameObject.Find("Feet" + player).GetComponent<AudioSource>();
        rollAudio = GameObject.Find("Roll" + player).GetComponent<AudioSource>();
        poundAudio = GameObject.Find("Pound" + player).GetComponent<AudioSource>();
        blastAudio = GameObject.Find("Blast" + player).GetComponent<AudioSource>();
        speed = walkSpeed;
        lastFacing = new Vector2(0f, -1f);
        movable = new WaveField.Movable()
        {
            Body = rb,
            DragCoefficient = 0.5f,
            StopSpeed = 2f
        };

        WaveField.forceMgr.Objects.Add(movable);
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
        Vector2 moveVector = new Vector2(moveXAxis, moveZAxis);
        bool moveAudio = false;
        if (moveVector.magnitude > 0.2)
        {
            lastFacing = moveVector.normalized;
            movable.DragActive = false;
            moveAudio = true;
        }
        else
        {
            movable.DragActive = true;
        }

        float moveYAxis = 0.0f;
        if(isGrounded)
        {
            if (Input.GetButton("A_" + player))
            {
                moveYAxis = 220.0f;
                isGrounded = false;
            }

            if (Input.GetButton("X_" + player) && moveVector.magnitude > 0.2)
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
                    UnitAmplitude = 2f,
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
                    UnitAmplitude = 2f,
                    UnitWaveAmplitude = 0.25f,
                    Wavelength = 0.5f,
                    Attenuation = 0.7f,
                    MaxRadius = 1f
                });

                rollAudio.enabled = true;
                feetAudio.enabled = false;
            }
            else
            {
                speed = walkSpeed;

                if (Input.GetButtonDown("B_" + player))
                {
                    float blastBearing = Mathf.Atan2(lastFacing.y, lastFacing.x);
                    WaveField.forceMgr.Regions.Add(new WaveField.RadialWaveForceRegion()
                    {
                        AnnularRegion = new global::WaveField.Annulus2()
                        {
                            Center = new Vector2(rb.position.x, rb.position.z),
                            MinRadius = 0.25f,
                            InnerRadius = -0.5f,
                            OuterRadius = 0.5f,
                            MinArc = blastBearing - 0.3f,
                            MaxArc = blastBearing + 0.3f
                        },
                        Center = new Vector2(rb.position.x, rb.position.z),
                        Speed = 4f,
                        UnitAmplitude = 4f,
                        UnitWaveAmplitude = 0.25f,
                        Wavelength = 0.25f,
                        Attenuation = 0.5f,
                        MaxRadius = 5f
                    });
                    blastAudio.Play();
                }

                rollAudio.enabled = false;
                feetAudio.enabled = moveAudio;
            }

            if (Input.GetButton("Y_" + player))
            {
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
        else
        {
            if (Input.GetButtonDown("B_" + player))
            {
                moveXAxis = 0.0f;
                moveYAxis = -500.0f;
                moveZAxis = 0.0f;
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                isPounding = true;
            }

            rollAudio.enabled = false;
            feetAudio.enabled = false;
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
            if (isPounding)
            {
                var velocity = rb.velocity;
                velocity.y = 0f;
                rb.velocity = velocity;
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
                    UnitAmplitude = 6f,
                    UnitWaveAmplitude = 0.5f,
                    Wavelength = 0.5f,
                    Attenuation = 1f,
                    MaxRadius = 2.5f
                });
                isPounding = false;
                poundAudio.Play();
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            isGrounded = false;
            isPounding = false;
        }
    }
}