using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    Rigidbody rigidbody;
    BoxCollider boxCollider;
    Material material;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        currentHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            material.color = Color.white;
        }
        else
        {
            material.color = Color.gray;
            gameObject.layer = 12;

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigidbody.freezeRotation = false;
                rigidbody.AddForce(reactVec * 5, ForceMode.Impulse);
                rigidbody.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigidbody.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            Destroy(gameObject, 4);
        }
    }
}
