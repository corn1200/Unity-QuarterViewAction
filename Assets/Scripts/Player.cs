using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObj;
    public Camera followCamera;

    public int ammo;
    public int coin;
    public int headlth;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

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

    bool isFire;
    bool isFireReady = true;
    bool isGrenade;
    bool isReload;
    bool isReloading;
    bool isBorder;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigidBody;
    Animator animator;

    GameObject nearObject;
    Weapon equipWeapon;

    int equipWeaponIndex = -1;

    float fireDelay;

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
        Grenade();
        Attack();
        Reload();
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
        isFire = Input.GetButton("Fire1");
        isGrenade = Input.GetButton("Fire2");
        isReload = Input.GetButtonDown("Reload");
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
        if (isSwaping || isReloading || !isFireReady)
        {
            moveVec = Vector3.zero;
        }
        if (!isBorder)
        {
            if (isWalk)
            {
                transform.position += moveVec * speed * 0.3f * Time.deltaTime;
            }
            else
            {
                transform.position += moveVec * speed * Time.deltaTime;
            }
        }

        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", isWalk);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

        if (isFire)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 100))
            {
                Vector3 nextVec = raycastHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
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

    void Grenade()
    {
        if (hasGrenades == 0)
        {
            return;
        }
        if (isGrenade && !isReload && !isSwaping)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 100))
            {
                Vector3 nextVec = raycastHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody grenadeRigidbody = instantGrenade.GetComponent<Rigidbody>();
                grenadeRigidbody.AddForce(nextVec, ForceMode.Impulse);
                grenadeRigidbody.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
        {
            return;
        }
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (isFire && isFireReady && !isDodge && !isSwaping)
        {
            equipWeapon.Use();
            animator.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null)
        {
            return;
        }
        if (equipWeapon.type == Weapon.Type.Melee)
        {
            return;
        }
        if (ammo == 0)
        {
            return;
        }
        if (isReload && !isJump && !isDodge && !isSwaping && isFireReady)
        {
            animator.SetTrigger("doReload");
            isReloading = true;

            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.currentAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
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
        if (isSwap1 && (!hasWeapons[0] || equipWeaponIndex == 0))
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
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

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

    void FreezeRotation()
    {
        rigidBody.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            animator.SetBool("isJumping", false);
            isJumping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                    {
                        ammo = maxAmmo;
                    }
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                    {
                        coin = maxCoin;
                    }
                    break;
                case Item.Type.Heart:
                    headlth += item.value;
                    if (headlth > maxHealth)
                    {
                        headlth = maxHealth;
                    }
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                    {
                        hasGrenades = maxHasGrenades;
                    }
                    break;
            }
            Destroy(other.gameObject);
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
