using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour {
    [SerializeField] Projectile_SO definition = null;
    [SerializeField]
    private Rigidbody rb = null;

    //[SerializeField]
    //private Collider Collider = null;

    [SerializeField]
    private Light lightC = null;

    [SerializeField]
    private ParticleSystem particleS = null;
    private ParticleSystem.MainModule particleMain;
    private ParticleSystem.EmissionModule emission;

    private Coroutine projectileLifetime;
    private WaitForSeconds waitLifetime;
    private WaitForSeconds waitForLastParticle;
    private Attack attack;
    private bool active = false;
    private float rateOverDistance;

    private void Awake() {
        waitLifetime = new WaitForSeconds(definition.lifetime);
        waitForLastParticle = new WaitForSeconds(particleS.main.startLifetime.constantMax);

        particleMain = particleS.main;
        emission = particleS.emission;
        rateOverDistance = emission.rateOverDistanceMultiplier;

    }

    public void Launch(Vector3 target, Attack attack, bool lookAtTarget = true) {

        this.attack = attack;
        active = true;
        if (lookAtTarget)
            transform.rotation = Quaternion.LookRotation((target - transform.position).normalized, Vector3.up);

        projectileLifetime = StartCoroutine(DisableDelayed());
        rb.angularVelocity = Vector3.zero;
        emission.enabled = true;
        lightC.enabled = true;

        particleMain.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;
        rb.velocity = transform.forward.normalized * definition.launchForce;

    }

    public void OnSlowMotion() {

        emission.rateOverDistanceMultiplier = rateOverDistance * (1 / Time.timeScale);
    }

    private IEnumerator DisableDelayed() {
        yield return waitLifetime;
        StartCoroutine(Disable());
        projectileLifetime = null;
    }

    private IEnumerator Disable() {
        emission.enabled = false;
        lightC.enabled = false;
        yield return waitForLastParticle;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision) {
        if (!active)
            return;

        active = false;
        if (collision.collider.CompareTag(Tags.enemy)) {
            CharController target = collision.gameObject.GetComponentInParent<CharController>();
            attack.rigidbody = collision.rigidbody;
            target.OnHitTake(attack);
        }
        if (projectileLifetime != null)
            StopCoroutine(projectileLifetime);
        StartCoroutine(Disable());
    }

}
