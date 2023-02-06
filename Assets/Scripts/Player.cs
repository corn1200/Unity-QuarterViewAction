using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    public GameObject[] weapons;
    public bool[] hasWeapons;

    float horizonAxis;
    float verticalAxis;

    bool isWalk;
    bool isJump;
    bool isInteraction;
    bool isJumping;
    bool isDodge;

    bool isSwap1;
    bool isSwap2;
    bool isSwap3;
    bool isSwaping;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidBody;
    Animator animator;

    GameObject nearObject;
    GameObject equipWeapon;

    int equipWeaponIndex = -1;

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
        Swap();
        Interaction();
    }

    void GetInput()
    {
        horizonAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        isWalk = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
        isInteraction = Input.GetButtonDown("Interaction");
        isSwap1 = Input.GetButtonDown("Swap1");
        isSwap2 = Input.GetButtonDown("Swap2");
        isSwap3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(horizonAxis, 0, verticalAxis).normalized;

        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        if (isWalk)
        {
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        }
        if (isSwaping)
        {
            moveVec = Vector3.zero;
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
        if (isJump && moveVec == Vector3.zero && !isJumping && !isDodge && !isSwaping)
        {
            rigidBody.AddForce(Vector3.up * 15, ForceMode.Impulse);
            animator.SetBool("isJumping", true);
            animator.SetTrigger("doJump");
            isJumping = true;
        }
    }

    void Dodge()
    {
        if (isJump && moveVec != Vector3.zero && !isJumping && !isDodge && !isSwaping)
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

    void Swap()
    {
        if(isSwap1 && (!hasWeapons[0] || equipWeaponIndex == 0))
        {
            return;
        }
        if (isSwap2 && (!hasWeapons[1] || equipWeaponIndex == 1))
        {
            return;
        }
        if (isSwap3 && (!hasWeapons[2] || equipWeaponIndex == 2))
        {
            return;
        }

        int weaponIndex = -1;
        if (isSwap1)
        {
            weaponIndex = 0;
        }
        if (isSwap2)
        {
            weaponIndex = 1;
        }
        if (isSwap3)
        {
            weaponIndex = 2;
        }

        if ((isSwap1 || isSwap2 || isSwap3) && !isJumping && !isDodge && !isSwaping)
        {
            if (equipWeapon != null)
            {
                equipWeapon.SetActive(false);
            }
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            animator.SetTrigger("doSwap");

            isSwaping = true;

            Invoke("SwapingOut", 0.4f);
        }
    }

    void SwapingOut()
    {
        isSwaping = false;
    }

    void Interaction()
    {
        if (isInteraction && nearObject != null && !isJumping && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJumping", false);
            isJumping = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
}
