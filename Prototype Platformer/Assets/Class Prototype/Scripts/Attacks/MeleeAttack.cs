using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Weapon
{
    public GameObject projectilePrefab;

    public float fireRate = 0.25f;
    public float range = 5;
    public int damage = 3;

    public string effect = "blur";

    [Header("Where are players stored:")]
    public GameObject PlayerArray;

    private bool _canFire = true;

    public override bool Fire(Transform attackSpawnPoint, GameObject friendly)
    {
        if (!_canFire) return false;

        bool ret = false;
        //print(" firing ");

        //from list of players, see if any are within range
        foreach (Transform child in PlayerArray.transform)
            if (distance(attackSpawnPoint.position, child.position) < range)
                if (child.gameObject != friendly)
                {
                    child.gameObject.GetComponent<PlayerHealth>().DealDamage(new DamageMessage(damage, effect, friendly));
                    ret = true;


                }

        /*
        Vector3 attackPoint = friendly.transform.position + 2 * friendly.transform.forward;

        GameObject projectile = (GameObject)Instantiate(projectilePrefab, attackPoint, attackSpawnPoint.rotation, null);
        //Physics.IgnoreCollision(attackSpawnPoint.GetComponent<Collider>(), projectile.GetComponent<Collider>());

        projectile.GetComponent<Projectile_Direct>().friend = friendly;
        projectile.GetComponent<DealDamage>().friend = friendly;
        projectile.transform.parent = friendly.transform;
        */
        _canFire = false;

        StartCoroutine(AttackCooldown());
        return ret;
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(fireRate);

        _canFire = true;
    }

    private static float distance(Vector3 from, Vector3 to)
    {
        return Mathf.Sqrt(Mathf.Pow(from.x - to.x, 2) + Mathf.Pow(from.y - to.y, 2));
    }
}
