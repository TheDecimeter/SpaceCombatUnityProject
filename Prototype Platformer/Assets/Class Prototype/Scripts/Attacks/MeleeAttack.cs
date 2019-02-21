using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Weapon
{
    public GameObject projectilePrefab;

    public float fireRate = 0.25f;

    private bool _canFire = true;

    public override void Fire(Transform attackSpawnPoint, GameObject friendly)
    {
        if (!_canFire) return;

        //print(" firing ");

        Vector3 attackPoint = friendly.transform.position + 2 * friendly.transform.forward;

        GameObject projectile = (GameObject)Instantiate(projectilePrefab, attackPoint, attackSpawnPoint.rotation, null);
        //Physics.IgnoreCollision(attackSpawnPoint.GetComponent<Collider>(), projectile.GetComponent<Collider>());

        projectile.GetComponent<Projectile_Direct>().friend = friendly;
        projectile.GetComponent<DealDamage>().friend = friendly;
        projectile.transform.parent = friendly.transform;

        _canFire = false;

        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(fireRate);

        _canFire = true;
    }
}
