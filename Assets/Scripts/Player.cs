using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float horizonAxis;
    float verticalAxis;
    bool isWalk;
    bool isJump;
    bool isJumping;
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidBody;
    Animator animator;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }
    
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput()
    {
        horizonAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        isWalk = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(horizonAxis, 0, verticalAxis).normalized;

        if(isDodge)
        {
            moveVec = dodgeVec;
        }
        if (isWalk)
        {
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        }
        else
        {
            transform.position += moveVec * speed * Time.deltaTime;
        }

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", isWalk);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if(isJump && moveVec == Vector3.zero && !isJumping && !isDodge)
        {
            rigidBody.AddForce(Vector3.up * 15, ForceMode.Impulse);
            animator.SetBool("isJumping", true);
            animator.SetTrigger("doJump");
            isJumping = true;
        }
    }

    void Dodge()
    {
        if (isJump && moveVec != Vector3.zero && !isJumping && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;
            animator.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJumping", false);
            isJumping = false;
        }
    }
}
