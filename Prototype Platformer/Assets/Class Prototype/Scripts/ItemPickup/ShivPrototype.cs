using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShivPrototype : Item
{

    public string AnimationTag = "isAttacking";
    public string ItemName = "Shiv";
    public int itemType = Item.Stab;

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


        //sound calls can go here
        audio.Play("shiv");

        Attack.Fire(user, user.GetChild(0).gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (audio == null)
            audio = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
