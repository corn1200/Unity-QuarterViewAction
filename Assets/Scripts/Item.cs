using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }
    public Type type;
    public int value;

    Rigidbody rigidbody;
    SphereCollider sphereCollider;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigidbody.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
