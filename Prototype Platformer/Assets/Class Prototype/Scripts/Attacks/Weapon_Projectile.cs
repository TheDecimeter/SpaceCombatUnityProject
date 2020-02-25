using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Projectile : Weapon {

    public GameObject projectilePrefab;

    public float fireRate = 0.25f;

    private bool _canFire = true;
    

    public override bool Fire (Transform attackSpawnPoint, GameObject friendly)
    {
        if (!_canFire) return false;

        //print(" firing ");

        //Vector3 attackPoint = friendly.transform.position + 1 * friendly.transform.forward;

        GameObject projectile = (GameObject)Instantiate(projectilePrefab, attackSpawnPoint.position, attackSpawnPoint.rotation, null);
        //Physics.IgnoreCollision(attackSpawnPoint.GetComponent<Collider>(), projectile.GetComponent<Collider>());

        projectile.SetActive(true);
        
        //Physics.IgnoreCollision(friendly.transform.Find("Collision/Foot Collider").gameObject.GetComponent<SphereCollider>(),
        //    projectile.GetComponent<Collider>());
        Physics.IgnoreCollision(friendly.transform.Find("Collision/Body Collider").gameObject.GetComponent<CapsuleCollider>(),
            projectile.GetComponent<Collider>());

        projectile.GetComponent<Projectile_Direct>().friend = friendly;
        projectile.GetComponent<DealDamage>().friend = friendly;
        projectile.GetComponent<DealDamage>().effect=effect;

        //projectile.transform.parent = null;

        _canFire = false;

        StartCoroutine(AttackCooldown());

        return true;
    }

    IEnumerator AttackCooldown ()
    {
        yield return new WaitForSeconds(fireRate);

        _canFire = true;
    }
}
