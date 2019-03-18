using UnityEngine;

abstract public class Weapon : MonoBehaviour 
{
    public abstract bool Fire(Transform attackSpawnPoint, GameObject friendly);
}
