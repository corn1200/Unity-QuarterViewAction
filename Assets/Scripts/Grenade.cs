using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, 
            15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit hit in raycastHits)
        {
            hit.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}
