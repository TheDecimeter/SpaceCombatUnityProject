using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPrototype : Item
{


    [Header("Attack Properties")]
    public Weapon Attack;

    public override string getName()
    {
        return "Gun";
    }

    public override int getType()
    {
        return Item.Ranged;
    }

    public override void use(Transform targetList, Transform user)
    {
        Attack.Fire(user, user.GetChild(0).gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
