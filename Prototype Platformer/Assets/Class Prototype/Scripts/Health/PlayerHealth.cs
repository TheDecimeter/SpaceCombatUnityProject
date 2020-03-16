using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    private static readonly string[] name = { "Bibbs", "Leslie", "Giggles", "Hobbs" };
    private AudioManager audio;
    private int PlayerNumber;

    public int startingHealth = 100;
    public int blurFrames = 500;

    public UnityEvent damageEvent;
    public UnityEvent deathEvent;

    private int _currentHealth, frameCounter, maxHealth=100;
    private bool _canTakeDamage = true;
    public GameObject colliders;

    public TextManager info;
    private AnimationStates AnimState;
    public Camera blurComponent;
    public RenderTexture blurScreen;
    public RawImage blurLayer;

    private PostProcessVolume defaultCamEffects;
    //public Text health;
    public HUD hud;
    
    public GameObject KilledBy { get; protected set;}
    private GameObject alive, poisonedBy;
    private int healthLoiterCount = 0, healthLoiterChase = 0;
    private const int ScoldEvery = 5, ChaseEvery = 20;

    private int framesDamage = 0, damageRate = 0;
    private int secondsCounter = 0;

    private CharacterMovement_Physics character;
    private bool deathQuip = true;
    public bool isDead{get; protected set;}

    private bool isHuman = false;

    public void Start ()
    {
        
        //MotionBlur mb;
        //if (camEffects.profile.TryGetSettings(out mb))
        //    mb.enabled.value = false;
        //DepthOfField df;
        //if (camEffects.profile.TryGetSettings(out df))
        //    df.enabled.value = false;

        isDead = false;
        _currentHealth = startingHealth;
        audio = FindObjectOfType<AudioManager>();
        PlayerNumber = GetComponent<CharacterMovement_Physics>().PlayerNumber;
        AnimState = GetComponent<CharacterMovement_Physics>().AnimState;

        character = GetComponent<CharacterMovement_Physics>();

        int players = GetPlayers();

        isHuman = (character.PlayerNumber + 1 <= players)||players==0;

        alive = new GameObject("Alive flag for "+character.name[PlayerNumber]);
        KilledBy = alive;
    }

    void FixedUpdate()
    {
        if (isHuman)
        {
            if (frameCounter < blurFrames)
            {
                frameCounter++;
                if (frameCounter == blurFrames)
                    endBlur();
            }
            if (framesDamage > 0)
            {
                secondsCounter++;
                if (secondsCounter == 50)
                {
                    secondsCounter = 0;
                    framesDamage--;
                    HealthParticle.Create(transform, "poison", new Color(0, .8f, 0), false);
                    DealDamage(new DamageMessage(damageRate, poisonedBy, true));
                }
            }
        }
    }

    public void DealDamage(DamageMessage message)
    {
        //if (message.friend==gameObject||message.friend == colliders)
            //return;

        if (!_canTakeDamage) return;

        if (message.damage < 0)
        {
            //heal poison
            if (framesDamage != 0)
            {
                HealthParticle.Create(transform, "Poison\nCured", Color.green, true);
                framesDamage = 0;
            }
            if (_currentHealth > maxHealth)
                return;
            _currentHealth -= message.damage;
            if (_currentHealth > maxHealth)
            {
                healthLoiterCount++;
                healthLoiterChase++;
                if (healthLoiterCount > ScoldEvery)
                {
                    healthLoiterCount = 0;
                    MaxHealthQuip();
                }
                if (healthLoiterChase > ChaseEvery)
                {
                    healthLoiterChase = 0;
                    ChaseAway();
                }
                HealthParticle.Create(transform, "MAX HP", Color.cyan, true);

                _currentHealth = maxHealth;
            }
            else
            {
                HealthParticle.Create(transform, -message.damage);
            }
            info.say("HP: " + _currentHealth, 15);
            return;
        }

        GetComponent<ShowDamage>().animateDamage();

        audio.Play("Hurt" + name[PlayerNumber]);
        if (AnimState == null)
            AnimState = GetComponent<CharacterMovement_Physics>().AnimState;
        AnimState.updateAnimationState(AnimationStates.Tag.damage, true);

        //print("deal damage " + message.damage);

        if (message.effect == "blur")
        {
            startBlur();
        }
        if (message.effect.Contains("poison"))
        {
            poisonedBy = message.friend;
            string[] tokens = message.effect.Split(',');
            int.TryParse(tokens[1], out damageRate);
            int.TryParse(tokens[2], out framesDamage);
        }

        int damage;
        if (character.damageFactor != 1)
        {
            damage = (int)(message.damage * character.damageFactor);
            HealthParticle.Create(transform, "Resist\n"+(message.damage-damage), Color.cyan, true);
        }
        else
            damage = message.damage;

        _currentHealth -= damage;

        //print("PLAYER HEALTH: " + _currentHealth);
        info.say("HP: " + _currentHealth, 15);
        HealthParticle.Create(transform, -damage);
        hud.Link.Health[PlayerNumber].text =" "+ _currentHealth;

        damageEvent.Invoke();

        if (_currentHealth <= 0)
        {
            if (KilledByPersonInst(message))
                KilledByPersonDirectQuip(message.friend);
            else if (KilledByPersonResidual(message))
                KilledByPlayerResidual(message.friend);
            else
                KilledByEnvironmentQuip(message.friend);
            PlayerDeath();
            _currentHealth = 0;
        }
    }

    private bool KilledByPersonInst(DamageMessage message)
    {
        if (message==null)
            return false;
        if (message.residualDamage)
            return false;
        KilledBy = message.friend;
        if (message.friend == null)
            return false;
        
        return message.friend.GetComponent<PlayerHealth>();
    }
    private bool KilledByPersonResidual(DamageMessage message)
    {
        if (message == null)
            return false;

        KilledBy = message.friend;
        if (message.friend == null)
            return false;

        if (!message.friend.GetComponent<PlayerHealth>())
            return false;
        return message.residualDamage;
            
    }

    public int getHealth()
    {
        return _currentHealth;
    }

    private void startBlur()
    {
        if (!isHuman)
            return;
        blurComponent.gameObject.GetComponent<CameraBob>().Bob=true;
        blurComponent.targetTexture = blurScreen;
        //blurComponent.depthTextureMode = DepthTextureMode.None;
        blurLayer.gameObject.SetActive(true);


        frameCounter = 0;
    }
    private void endBlur()
    {
        blurComponent.gameObject.GetComponent<CameraBob>().Bob = false;
        //blurComponent.depthTextureMode = DepthTextureMode.Depth;
        blurComponent.targetTexture = null;
        blurLayer.gameObject.SetActive(false);
    }

    private void KilledByEnvironmentQuip(GameObject killer)
    {
        if (!isHuman)
            return;
        deathQuip = true;
        switch (Random.Range(0, 3))
        {
            case 0:
                hud.Link.Health[PlayerNumber].text = " : (";
                info.say("YEAH!\nTook it like a\nBOSS!", -1);
                break;
            case 1:
                hud.Link.Health[PlayerNumber].text = " :'{";
                info.say("YEAH!\nHow tough am I!", -1);
                break;
            case 2:
                hud.Link.Health[PlayerNumber].text = " : P";
                info.say("What doesn't kill you\nmakes you stronger.\nNormally\nit <b>kills</b> you though.", -1);
                break;
            default:
                hud.Link.Health[PlayerNumber].text = "0";
                info.say("", -1);
                break;
        }
    }
    private void KilledByPlayerResidual(GameObject killer)
    {
        if (!isHuman)
            return;
        deathQuip = true;
        switch (Random.Range(0, 6))
        {
            case 0:
                hud.Link.Health[PlayerNumber].text = " : (";
                info.say("Coward!", -1);
                break;
            case 1:
                hud.Link.Health[PlayerNumber].text = " :'{";
                info.say("Where was the body armor?", -1);
                break;
            case 2:
                hud.Link.Health[PlayerNumber].text = " :'{";
                info.say("In a fair fight\nI'd kill you!\n \n ", -1);
                killer.GetComponent<PlayerHealth>().info.say("That’s no incentive for me \nto fight fair, then is it?", 500);
                break;
            default:
                hud.Link.Health[PlayerNumber].text = "0";
                info.say("", -1);
                break;
        }
    }


    private void KilledByPersonDirectQuip(GameObject killer)
    {
        if (!isHuman)
            return;
        deathQuip = true;
        switch (Random.Range(0, 12))
        {
            case 0:
                hud.Link.Health[PlayerNumber].text = " : (";
                info.say("Why didn't\nI choose peace", -1);
                break;
            case 1:
                hud.Link.Health[PlayerNumber].text = " :'{";
                info.say("<b><size=110%>KAA<size=100%>HN<size=90%>NN<size=80%>N<size=70%>N<size=60%>N</b>!", -1);
                break;
            case 3:
                hud.Link.Health[PlayerNumber].text = " : P";
                info.say("REKT??", -1);
                break;
            case 4:
                hud.Link.Health[PlayerNumber].text = " :{";
                info.say("Hello\nMy name is "+name[character.PlayerNumber]+"\nYou killed my father\n<b>Prepare to</b>...", -1);
                break;
            case 5:
                hud.Link.Health[PlayerNumber].text = " : {";
                info.say("Et tu, Brute?", -1);
                break;
            case 6:
                hud.Link.Health[PlayerNumber].text = " =[";
                info.say("denial\nanger\nbargaining\ndepression\n<size=120%><b>REVENGE</b></size>", -1);
                break;
            case 7:
                hud.Link.Health[PlayerNumber].text = " =(";
                info.say("Avenge ME!\n ", -1);
                killer.GetComponent<PlayerHealth>().info.say("No Thanks", 120);
                break;
            case 8:
                hud.Link.Health[PlayerNumber].text = " :'(";
                info.say("You have died\nof dysentery", -1);
                break;
            case 9:
                hud.Link.Health[PlayerNumber].text = " ={";
                info.say("Where is the love?", -1);
                break;
            default:
                hud.Link.Health[PlayerNumber].text = "0";
                info.say("", -1);
                break;
        }
    }

    public void PlayerDeath ()
    {

        if (KilledBy == alive)
            InitiateAsteroidDeath();
        else
        {
            if (AnimState == null)
                AnimState = GetComponent<CharacterMovement_Physics>().AnimState;
            AnimState.startDie();
        }
        


        framesDamage = 0;

        transform.Find("Collision/Body Collider").gameObject.AddComponent<DontCollideWithPlayer>();

        isDead = true;
        //death sounds can go here
        audio.Play("Death" + name[PlayerNumber]);
        //print("Death" + name[PlayerNumber]);

        
        deathEvent.Invoke();
        _canTakeDamage = false;
    }

    private void InitiateAsteroidDeath()
    {
        AsteroidQuip();
        if (AnimState == null)
            AnimState = character.AnimState;
        AnimState.stopAnimating();

        gameObject.GetComponent<CameraLocator>().CameraLocation.parent.gameObject.GetComponent<PlatformerCameraFollow>().lookAhead = false;

        float rot=30f;
        float xythrust = 1f;
        float zthrust = 10f;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = 0;
        rb.AddForce(new Vector3(Random.Range(-xythrust, xythrust), Random.Range(-xythrust, xythrust),zthrust));
        rb.AddTorque(new Vector3(Random.Range(-rot, rot), Random.Range(-rot, rot), Random.Range(-rot, rot)));
    }

    private void AsteroidQuip()
    {
        if (!isHuman)
            return;
        switch (Random.Range(0, 11))
        {
            case 0:
                hud.Link.Health[PlayerNumber].text = " : (";
                info.say("But paper was\nsupposed to beat rock.", -1);
                break;
            case 1:
                hud.Link.Health[PlayerNumber].text = " :'{";
                info.say("How was I\nsupposed to know\nasteroids were bad?", -1);
                break;
            case 3:
                hud.Link.Health[PlayerNumber].text = " : P";
                info.say("Long Live Rock!", -1);
                break;
            case 4:
                hud.Link.Health[PlayerNumber].text = " :'{";
                info.say("So...\nthat\nhappened...", -1);
                break;
            case 5:
                hud.Link.Health[PlayerNumber].text = " :'{";
                info.say("Well, of all the\nways to go...\nCould be worse", -1);
                break;
            case 6:
                hud.Link.Health[PlayerNumber].text = " :'{";
                info.say("You promised me\nI wouldn't die\nlike this!", -1);
                break;
            default:
                hud.Link.Health[PlayerNumber].text = "0";
                info.say("", -1);
                break;
        }
    }

    private bool MaxHealthQuip()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                HealthParticle.Create(transform, "No loitering!", Color.yellow, true);
                return true;
            case 1:
                HealthParticle.Create(transform, "Kill something!", Color.yellow, true);
                return true;
            case 2:
                HealthParticle.Create(transform, "Healed!\nIn the name of\nCthulhu", Color.yellow, true);
                return true;
            default:
                HealthParticle.Create(transform, "Seek Rehab", Color.yellow, true);
                return true;
        }
    }

    private void ChaseAway()
    {
        string msg = "They're camping\nover here";
        switch (Random.Range(0, 3))
        {
            case 0:
                msg = "They're camping\nover here";
                break;
            case 1:
                msg = "Hope no one\nfinds me here";
                break;
            case 2:
                Item w = character._currentItem;
                if(w.Rank()>5)
                    msg = "I've got \n"+ w.Aweapon() + "\nwith your name on it";
                else
                    msg= "They're over here\nThey've only got a\n" + w.Aweapon() + "";
                break;
            default:
                msg = "Come At Me Bro";
                break;
        }
        float scale = 5;
        HealthParticle.Create(transform, Vector3.up * scale, 5, msg, Color.yellow, true);
        HealthParticle.Create(transform, Vector3.down * scale, 5, msg, Color.yellow, true);
        HealthParticle.Create(transform, Vector3.left * scale, 5, msg, Color.yellow, true);
        HealthParticle.Create(transform, Vector3.right * scale, 5, msg, Color.yellow, true);

        HealthParticle.Create(transform, (Vector3.up + Vector3.left).normalized * scale, 5, msg, Color.yellow, true);
        HealthParticle.Create(transform, (Vector3.up + Vector3.right).normalized * scale, 5, msg, Color.yellow, true);
        HealthParticle.Create(transform, (Vector3.down + Vector3.left).normalized * scale, 5, msg, Color.yellow, true);
        HealthParticle.Create(transform, (Vector3.down + Vector3.right).normalized * scale, 5, msg, Color.yellow, true);
    }


    private int GetPlayers()
    {
        int r = 0;
        FindObjectOfType<UndestroyableData>().GetPlayers((x) => { r = x; });
        return r;
    }
}
