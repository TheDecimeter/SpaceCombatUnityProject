using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunPrototype : Item
{


    public string AnimationTag = "isShooting";
    public string ItemName = "Gun";
    public int itemType = Item.Ranged;
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
    

    public override void use(Transform attackSpawnPoint, Transform user)
    {

        //sound calls can go here

        if (Attack.Fire(attackSpawnPoint, user.gameObject))
        {
            onUse.Invoke();
            //audio.Play("rifle");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ItemName = ItemName.Replace("\\n","\n");
        Attack.effect = effect;
        if (audio == null)
            audio = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
