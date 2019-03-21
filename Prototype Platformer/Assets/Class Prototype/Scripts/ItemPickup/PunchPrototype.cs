﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchPrototype : Item
{
    private static readonly string[] name = { "Bibbs", "Leslie", "Giggles", "Hobbs" };

    public string AnimationTag = "isAttacking";
    public string ItemName = "fisticufs";
    public int itemType = Item.Punch;

    [Header("Attack Properties")]
    public Weapon Attack;

    public override string getName()
    {
        return ItemName;
    }
    public override string getAnimationFlag()
    {
        return AnimationTag;
    }

    public override int getType()
    {
        return itemType;
    }

    public override void use(Transform targetList, Transform user)
    {
        print("Punch Grunt" + name[user.gameObject.GetComponent<CharacterMovement_Physics>().PlayerNumber]);

        if (Attack.Fire(targetList, user.gameObject))
        {

           // audio.Play("Punch" + name[PlayerNumber]);
        }
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
