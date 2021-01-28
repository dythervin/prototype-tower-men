using MyExtensions;
using System.Collections;
using UnityEngine;

public class AttackController : MonoBehaviour {
    [SerializeField] bool turnToTargetImmediately = false;
    CharData charData;
    CharController charController;
    MeshData meshData;
    Animator animator;

    float attackEndTime = 0;
    Coroutine attackTask;
    Coroutine turnTask;
    Coroutine rotationTask;
    public Vector3 Target { get; protected set; }
    bool leftHanded => charController.LeftHanded;

    private void Awake() {
        charController = GetComponent<CharController>();
        charData = GetComponent<CharData>();
        meshData = charController.MeshData;
        animator = charController.Animator;
    }

    public void Hit() {
        charData.Weapon.Attack(ProjectilePos(), Target);
    }
    void Attack() {
        animator.SetTrigger(CharData.AttackHash);
    }
    /// <summary>
    /// Return spawn position for projectile
    /// </summary>
    /// <returns></returns>
    protected Vector3 ProjectilePos() {
        if (charData.Weapon.definition.weaponHandle == WeaponHandle.OneHanded)
            return meshData.ProjectileSpots[leftHanded ? 1 : 0].position + transform.forward * 0.3f;
        else
            return (meshData.ProjectileSpots[0].position + meshData.ProjectileSpots[1].position) / 2 + transform.forward * 0.3f;
    }

    public bool EnemyInVision() {
        foreach (var enemy in SpawnManager.Instance.Enemies) {
            if (Physics.Linecast(charController.Center.position, enemy.Center.position, out RaycastHit hit, ~GameManager.Instance.PlayerLayer)) {
                if (hit.collider.CompareTag(Tags.enemy)) {
                    return true;
                }
            }
        }
        return false;
    }

    public void Stop() {
        if (attackTask != null) {
            StopCoroutine(attackTask);
        }
        if (turnTask != null) {
            StopCoroutine(turnTask);
        }
    }

    IEnumerator Attacking() {
        if (Time.time < attackEndTime)
            yield return new WaitForSeconds(attackEndTime - Time.time);

        do {
            Attack();
            attackEndTime = Time.time + 1 / charData.BaseStats.attacksPerSec + charData.BaseStats.attackDelay;
            yield return charData.AttackDurationWait;
            if (charData.BaseStats.attackDelay > 0)
                yield return charData.AttackDelay;
        } while (Input.GetMouseButton(0));
        attackTask = null;

    }

    IEnumerator TurnToTarget() {
        if (turnToTargetImmediately) {
            RotateImmidiately();
        } else {
            while (!transform.IsFacingTarget(Target, out Vector3 direction, 0.90f)) {
                RotateByStep(direction);
                yield return null;
            }
        }
        if (attackTask == null) {
            attackTask = StartCoroutine(Attacking());
        }
        turnTask = null;
    }
    IEnumerator Rotate() {
        while (Input.GetMouseButton(0)) {
            if (turnToTargetImmediately) {
                RotateImmidiately();
            } else {
                Vector3 direction = (Target - transform.position).normalized;
                RotateByStep(direction);
            }
            yield return null;
        }
        rotationTask = null;
    }

    void RotateByStep(Vector3 direction) {
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, charData.BaseStats.angularSpeed * Time.deltaTime);
    }

    void RotateImmidiately() {
        Vector3 direction = (Target - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void Update() {
        if (charController.CharState != CharStateDef.CanAttack || GameManager.Instance.GameState != GameStateDef.Running)
            return;


        if (Input.GetMouseButtonDown(0)) {
            if (TriggerAttack() && turnTask == null) {
                turnTask = StartCoroutine(TurnToTarget());
            }
        } else if (Input.GetMouseButton(0)) {
            if (TriggerAttack() && rotationTask == null) {
                rotationTask = StartCoroutine(Rotate());
            }
        }
    }

    private bool TriggerAttack() {
        Ray ray = GameManager.Instance.playerCamera.Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            Target = hit.point;
            return true;
        }
        return false;
    }
}





