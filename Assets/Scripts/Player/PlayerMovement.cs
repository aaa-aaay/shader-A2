using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController cc;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3.0f;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        Jump();
        if (cc.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;


        }
        velocity.y += gravity * Time.deltaTime;


        cc.Move((velocity + move * speed) * Time.deltaTime); // Apply movement



    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && cc.isGrounded)
        {
            Debug.Log("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }


}
