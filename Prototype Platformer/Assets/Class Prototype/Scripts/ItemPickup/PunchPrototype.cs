using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchPrototype : Item
{

    [Header("Attack Properties")]
    public Weapon Attack;

    public override string getName()
    {
        return "fisticuffs";
    }

    public override int getType()
    {
        return Item.Punch;
    }

    public override void use(Transform targetList, Transform user)
    {
        //play sound
        audio.Play("punch");
        Attack.Fire(user, user.GetChild(0).gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(audio==null)
            audio= FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
