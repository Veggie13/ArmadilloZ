using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public string player = "1";
    public int walkSpeed;
    public int rollSpeed;
    public Text countText;
    public Text winText;

    private Rigidbody rb;
    private int count;
    private bool isGrounded = true;
    private int speed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";
        speed = walkSpeed;
    }
    
    void FixedUpdate()
    {
        if (rb.position.y < -20)
        {
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            rb.position = new Vector3(0.0f, 1.0f, 0.0f);
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

            if (Input.GetButton("Y_" + player))
            {
                count+=30;
                SetCountText();
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
            }
        }
        Vector3 movement = new Vector3(moveXAxis * speed, moveYAxis, moveZAxis * speed);

        rb.AddForce(movement);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Ground")
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            isGrounded = false;
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if(count >= 9000)
        {
            winText.text = "Player " + player + " is Super Saiyan!";
        }
    }
}