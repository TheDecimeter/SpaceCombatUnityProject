using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPrototype : Item
{


    public string AnimationTag = "isShooting";
    public string ItemName = "Gun";
    public int itemType = Item.Ranged;

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

        if(Attack.Fire(user, user.gameObject)) 
            audio.Play("rifle");
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
