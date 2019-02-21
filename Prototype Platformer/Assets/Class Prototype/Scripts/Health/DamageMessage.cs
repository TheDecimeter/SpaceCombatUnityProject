using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMessage
{
    public GameObject friend;
    public int damage;

    public DamageMessage(int damageAmmount, GameObject friend)
    {
        this.friend = friend;
        this.damage = damageAmmount;
    }
}
