using UnityEngine;

public class Attack {
    public readonly CharData attacker;
    public readonly float damage;
    public readonly bool critical;
    public readonly Vector3 direction;

    public readonly float knockbackForce;
    public readonly bool paralyze;
    public readonly float paralyzeDuration;
    public Rigidbody rigidbody;

    public Attack(CharData attacker, float damage, bool critical, Vector3 direction, float knockbackForce, bool paralyze, float paralyzeDuration)
    {
        this.attacker = attacker;
        this.damage = damage;
        this.critical = critical;
        this.direction = direction;
        this.knockbackForce = knockbackForce;
        this.paralyze = paralyze;
        this.paralyzeDuration = paralyzeDuration;
    }    
}