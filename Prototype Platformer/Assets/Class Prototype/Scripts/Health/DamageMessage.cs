using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMessage
{
    private const string NoEffect = "none";
    public GameObject friend;
    public int damage;
    public string effect;
    public bool residualDamage;

    public DamageMessage(int damageAmmount, GameObject friend)
    {
        this.friend = friend;
        this.damage = damageAmmount;
        this.effect = NoEffect;
        this.residualDamage = false;
    }
    public DamageMessage(int damageAmmount, string effect, GameObject friend)
    {
        this.friend = friend;
        this.damage = damageAmmount;
        this.effect = effect;
        this.residualDamage = false;
    }
    public DamageMessage(int damageAmmount, GameObject friend, bool residual)
    {
        this.friend = friend;
        this.damage = damageAmmount;
        this.effect = NoEffect;
        this.residualDamage = residual;
    }
}
