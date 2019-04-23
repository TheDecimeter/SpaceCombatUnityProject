using UnityEngine;

abstract public class Weapon : MonoBehaviour 
{
    [HideInInspector]
    public string effect;
    public abstract bool Fire(Transform attackSpawnPoint, GameObject friendly);
}
