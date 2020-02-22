using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashDamage : MonoBehaviour
{
    public GameObject PlayerArray;
    public float maxDamage;
    public float range;


    public void Fire(Transform attackSpawnPoint, GameObject friendly, float range, float maxDamage, string effect = "")
    {

        //from list of players, see if any are within range
        foreach (Transform child in PlayerArray.transform) {
            float dist = distance(attackSpawnPoint.position, child.position);
            if (dist < range)
                if (child.gameObject != friendly)
                {
                    child.gameObject.GetComponent<PlayerHealth>().DealDamage(new DamageMessage((int)(1 + maxDamage * (1 - (dist / range))), effect, friendly));
                    //print("splashDamage " + (int)(1 + maxDamage * (1 - (dist / range))));
                }
        }

    }

    public void Fire()
    {
        Projectile_Direct p = GetComponent<Projectile_Direct>();
        if (p)
        {
            GameObject friend = p.friend;
            Fire(transform, friend, range, maxDamage);
        }
    }

    private void OnDestroy()
    {
        Fire();
    }

    private static float distance(Vector3 from, Vector3 to)
    {
        return Mathf.Sqrt(Mathf.Pow(from.x - to.x, 2) + Mathf.Pow(from.y - to.y, 2));
    }
}
