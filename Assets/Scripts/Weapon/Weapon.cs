using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Weapon : MonoBehaviour
{
    #region SET IN EDITOR
    public ItemWeapon_SO definition;
    public Rigidbody rb;
    public Collider[] colliders;
    public ParticleSystem particle;
    #endregion
    private ParticleSystem.EmissionModule emission;
    protected CharData charData;

    protected virtual void Awake()
    {
        SetColliders(false);
        if (particle != null)
            emission = particle.emission;
    }
    public virtual void Init(CharData charData)
    {
        this.charData = charData;
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    public void ParticleEmission(bool active)
    {
        if (particle != null)
            emission.enabled = active;
    }
    public abstract void Attack(Vector3 projectilePoint, Vector3 target);
    public virtual void OnDeath()
    {
        //Weapon falls on the ground on death and destroys in 5 s.
        transform.parent = null;
        SetColliders(true);
        rb.isKinematic = false;
        rb.useGravity = true;
        Destroy(gameObject, 5);
    }

    private void SetColliders(bool active)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = active;
        }
    }
    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

}

public enum BasicAttackDefinitions
{
    _1HSimple, _1HSplash, _1HUp,
    _2HArea01, _2HArea02,
    _2HAttack01, _2HAttack02, _2HAttack03Long, _2HAttack04Long, _2HAttack05
}

public enum WeaponHandle { OneHanded, TwoHanded }

