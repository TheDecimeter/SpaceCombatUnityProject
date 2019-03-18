using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMessage
{
    public GameObject friend;
    public int damage;
    public string effect;

    public DamageMessage(int damageAmmount, GameObject friend)
    {
        this.friend = friend;
        this.damage = damageAmmount;
        this.effect = "none";
    }
    public DamageMessage(int damageAmmount, string effect, GameObject friend)
    {
        this.friend = friend;
        this.damage = damageAmmount;
        this.effect = effect;
    }
}
