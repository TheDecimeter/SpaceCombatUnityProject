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
    public PostProcessLayer blurComponent;
    public PostProcessVolume camEffects;
    private PostProcessVolume defaultCamEffects;
    public Text health;
    

    private int framesDamage = 0, damageRate = 0;
    private int secondsCounter = 0;

    private CharacterMovement_Physics character;
    private bool deathQuip = true;
    public bool isDead{get; protected set;}

    public void Start ()
    {


        MotionBlur mb;
        if (camEffects.profile.TryGetSettings(out mb))
            mb.enabled.value = false;
        DepthOfField df;
        if (camEffects.profile.TryGetSettings(out df))
            df.enabled.value = false;

        isDead = false;
        _currentHealth = startingHealth;
        audio = FindObjectOfType<AudioManager>();
        PlayerNumber = GetComponent<CharacterMovement_Physics>().PlayerNumber;
        AnimState = GetComponent<CharacterMovement_Physics>().AnimState;

        character = GetComponent<CharacterMovement_Physics>();
    }

    void FixedUpdate()
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
                DealDamage(new DamageMessage(damageRate, null));
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
                if(!MaxHealthQuip())
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
        health.text =" "+ _currentHealth;

        damageEvent.Invoke();

        if (_currentHealth <= 0)
        {
            KilledQuip();
            PlayerDeath();
            _currentHealth = 0;
        }
    }

    public int getHealth()
    {
        return _currentHealth;
    }

    private void startBlur()
    {
        FloatParameter sf = new FloatParameter { value = 360f };
        FloatParameter af = new FloatParameter { value = 24f };
        IntParameter si = new IntParameter { value = 32 };
        //print("start blur");
        blurComponent.gameObject.GetComponent<CameraBob>().Bob=true;
        //blurComponent.enabled = true;
        //camEffects.profile.AddSettings<MotionBlur>().shutterAngle = sf;
        //camEffects.profile.AddSettings<MotionBlur>().sampleCount = si;
        //camEffects.profile.AddSettings<DepthOfField>().aperture= af;

        MotionBlur mb;
        camEffects.profile.TryGetSettings(out mb);
        mb.enabled.value = true;
        DepthOfField df;
        camEffects.profile.TryGetSettings(out df);
        df.enabled.value = true;

        frameCounter = 0;
    }
    private void endBlur()
    {
        FloatParameter sf = new FloatParameter { value = 4f };
        FloatParameter af = new FloatParameter { value = 32f };
        IntParameter si = new IntParameter { value = 0 };
        //print("end blur");
        blurComponent.gameObject.GetComponent<CameraBob>().Bob = false;
        //blurComponent.enabled = false;
        //camEffects.profile.AddSettings<MotionBlur>().shutterAngle = sf;
        //camEffects.profile.AddSettings<MotionBlur>().sampleCount = si;
        //camEffects.profile.AddSettings<DepthOfField>().aperture = af;


        MotionBlur mb;
        if(camEffects.profile.TryGetSettings(out mb))
            mb.enabled.value = false;
        DepthOfField df;
        if(camEffects.profile.TryGetSettings(out df))
            df.enabled.value = false;
    }

    private void KilledQuip()
    {
        deathQuip = true;
        switch (Random.Range(0, 11))
        {
            case 0:
                health.text = " : (";
                info.say("Why didn't\nI choose love", -1);
                break;
            case 1:
                health.text = " :'{";
                info.say("KAAHHHHNN", -1);
                break;
            case 3:
                health.text = " : P";
                info.say("REKT??", -1);
                break;
            case 4:
                health.text = " :'{";
                info.say("Momma was right\nabout you", -1);
                break;
            case 5:
                health.text = " :'{";
                info.say("Et tu, Brute?", -1);
                break;
            default:
                health.text = "0";
                info.say("", -1);
                break;
        }
    }

    public void PlayerDeath ()
    {
        if (!deathQuip)
            AsteroidQuip();

        if (AnimState == null)
            AnimState = GetComponent<CharacterMovement_Physics>().AnimState;
        AnimState.startDie();
        GetComponent<Rigidbody>().useGravity = false;

        framesDamage = 0;

        transform.Find("Collision/Foot Collider").gameObject.GetComponent<SphereCollider>().enabled = false;
        transform.Find("Collision/Body Collider").gameObject.GetComponent<CapsuleCollider>().enabled = false;


        isDead = true;
        //death sounds can go here
        audio.Play("Death" + name[PlayerNumber]);
        //print("Death" + name[PlayerNumber]);

        
        deathEvent.Invoke();
        _canTakeDamage = false;
    }

    private void AsteroidQuip()
    {
        switch (Random.Range(0, 8))
        {
            case 0:
                health.text = " : (";
                info.say("But paper was\nsupposed to beat rock.", -1);
                break;
            case 1:
                health.text = " :'{";
                info.say("KAAHHHHNN", -1);
                break;
            case 3:
                health.text = " : P";
                info.say("Long Live Rock!", -1);
                break;
            case 4:
                health.text = " :'{";
                info.say("Huh...", -1);
                break;
            case 5:
                health.text = " :'{";
                info.say("Speak of this\nTo no one!", -1);
                break;
            default:
                health.text = "0";
                info.say("", -1);
                break;
        }
    }

    private bool MaxHealthQuip()
    {
        switch (Random.Range(0, 20))
        {
            case 0:
                HealthParticle.Create(transform, "No loitering!", Color.yellow, true);
                return true;
            case 1:
                HealthParticle.Create(transform, "Kill something!", Color.yellow, true);
                return true;
            case 3:
                HealthParticle.Create(transform, "Healed!\nIn the name of\nCthulhu", Color.yellow, true);
                return true;
            case 4:
                HealthParticle.Create(transform, "Seek Rehab", Color.yellow, true);
                return true;
        }
        return false;
    }
}
