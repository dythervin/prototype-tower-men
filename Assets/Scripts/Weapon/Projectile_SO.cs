using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new projectile", menuName = "Projectiles/new projectile")]
public class Projectile_SO : ScriptableObject
{
    [Min(1)]
    public float launchForce = 20;

    [Range(1, 15)]
    public float lifetime = 3;
}
