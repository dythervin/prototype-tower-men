using UnityEngine;

public class MeshData : MonoBehaviour, IParalyzable, IDestructible
{
    [SerializeField] Transform[] projectileSpots = null;
    [SerializeField] Transform[] weaponSlot = null;
    [SerializeField] Transform[] foots = null;
    [SerializeField] Transform pelvis = null;
    [SerializeField] Rigidbody ragdollCore = null;
    [SerializeField] Rigidbody[] rigidbodies = null;
    [SerializeField] Renderer[] renderers = null;
    public Renderer[] Renderers => renderers;
    public Transform[] ProjectileSpots => projectileSpots;
    public Transform[] WeaponSlot => weaponSlot;
    public Transform Center { get; private set; }
    public Vector3 PelvisPosition => pelvis.position;
    public Vector3 FootPosition => Vector3.Lerp(foots[0].position, foots[1].position, 0.5f);

    private void Awake()
    {
        Center = ragdollCore.transform;
    }

    private void Start()
    {
        Kinematic(true);
    }

    public void ApplyForce(Vector3 force)
    {
        ragdollCore.AddForce(force, ForceMode.Impulse);
    }

    public bool IsFacingUp()
    {
        float dotProd = Vector3.Dot(pelvis.up, Vector3.down);
        if (dotProd >= 0)
            return true;
        return false;
    }

    public void Paralyze(Attack attack)
    {
        Vector3 direction = attack.direction;
        //direction.y += 1;
        Kinematic(false);
        attack.rigidbody.AddForce(direction * attack.knockbackForce, ForceMode.Impulse);
    }

    public void GetUp()
    {
        Kinematic(true);
    }

    public void Kinematic(bool set)
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = set;
        }
    }

    public void OnGotUp()
    {
        //None
    }

    public void OnDestruction(Attack attack) {
        Paralyze(attack);
    }
}
