using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchPrototype : Item
{
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
        if(Attack.Fire(user, user.gameObject))
            audio.Play("Punch");
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
