using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShivPrototype : Item
{

    public string AnimationTag = "isAttacking";
    public string ItemName = "Shiv";
    public int itemType = Item.Stab;
    public GameObject pickupHud;
    public GameObject inUseHud;

    public UnityEvent onUse;

    [Header("Attack Properties")]
    public Weapon Attack;
    

    public override GameObject getInUseHUD()
    {
        return inUseHud;
    }

    public override GameObject getPickUpHUD()
    {
        return pickupHud;
    }
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

        if (Attack.Fire(targetList, user.gameObject))
        {
            onUse.Invoke();
            audio.Play("shiv");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ItemName = ItemName.Replace("\\n", "\n");
        Attack.effect = effect;
        if (audio == null)
            audio = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
