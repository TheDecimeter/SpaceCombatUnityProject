using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile_Physics : Projectile
{
    public float projectileSpeed = 10f;
    public float timeToDie = 5f;

    private Rigidbody _rigidbody;

    public void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody>();

        StartCoroutine(Timout());

        _rigidbody.velocity = this.transform.forward * projectileSpeed;
    }

    void Update()
    {
        _rigidbody.AddForce(this.transform.forward * projectileSpeed * Time.deltaTime, ForceMode.Force);
    }


    IEnumerator Timout()
    {
        yield return new WaitForSeconds(timeToDie);

        Destroy(this.gameObject);
    }

}
