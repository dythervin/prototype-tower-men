
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum CharStateDef { Idle, Moving, CanAttack, Dead, Disabled }

[RequireComponent(typeof(NavMeshAgent))]
public class CharController : MonoBehaviour {
    #region SET IN EDITOR
    [SerializeField] protected CharData charData = null;
    [SerializeField] protected MeshData meshData = null;
    [SerializeField] protected Animator animator = null;
    //[SerializeField] protected Rigidbody charRb;
    [SerializeField] protected NavMeshAgent agent = null;
    #endregion    
    [ReadOnly, SerializeField] CharStateDef charState = CharStateDef.Disabled;
    #region REPORTERS
    public bool Alive => charData.Health > 0;
    public MeshData MeshData => meshData;
    public Animator Animator => animator;
    public NavMeshAgent Agent => agent;
    public Transform Center => meshData.Center;
    protected Weapon Weapon => charData.Weapon;
    public CharStateDef CharState => charState;
    #endregion
    public bool LeftHanded { get; private set; }

    IAttackable[] attackables;
    IDestructible[] destructibles;
    IParalyzable[] paralyzables;

    protected Coroutine taskIdle;
    protected CharUI charUI;
    private float paralyzeEndTime;

    private void Start() {
        Init();
    }
    public void Init() {
        LeftHanded = Random.value <= 0.5f;
        charUI = UIManager.Instance.GetCharUI();
        animator.SetBool(CharData.LeftHandedHash, LeftHanded);
        charData.Init(this, LeftHanded, charUI);
        agent.enabled = true;
        agent.stoppingDistance = 0;
        UpdateCharState(CharStateDef.Idle);
        if (charData.CharType == CharTypeDef.Enemy)
            SpawnManager.Instance.Enemies.Add(this);
    }

    public void DestroyChar() {
        UIManager.Instance.DestroyCharUI(charUI);
        Destroy(gameObject);
    }

    protected void Awake() {
        attackables = GetComponentsInChildren<IAttackable>();
        destructibles = GetComponentsInChildren<IDestructible>();
        paralyzables = GetComponentsInChildren<IParalyzable>();
    }


    protected virtual IEnumerator Idle() {
        WaitForSeconds delay = new WaitForSeconds(15f);
        yield return new WaitForSeconds(Random.value * 15f);
        while (true) {
            Animator.SetFloat(CharData.idleIdHash, Random.Range(0, 3));
            Animator.SetTrigger(CharData.RandomIdleHash);
            yield return delay;
        }
    }

    public void UpdateCharState(CharStateDef target) {
        if (target == charState)//if same state do nothing
            return;

        switch (charState) //previous state
        {
            case CharStateDef.Idle:
                if (taskIdle != null)
                    StopCoroutine(taskIdle);
                break;
            case CharStateDef.CanAttack:
                Weapon.ParticleEmission(true);
                break;
            case CharStateDef.Moving:
                animator.SetFloat(CharData.moveSpeedHash, 0);
                animator.SetBool(CharData.MovingHash, false);
                break;
        }
        charState = target;
        switch (target)// target state
        {
            case CharStateDef.Idle:
                taskIdle = StartCoroutine(Idle());
                break;
            case CharStateDef.CanAttack:
                Weapon.ParticleEmission(false);
                break;
            case CharStateDef.Moving:
                animator.SetBool(CharData.MovingHash, true);
                break;
            case CharStateDef.Disabled:
                //attackController.Stop();
                break;
        }
    }
    #region ON HIT
    public void OnHitTake(Attack attack) {
        if (Alive) {
            foreach (IAttackable attackable in attackables) {
                attackable.TakeHit(attack);
            }

            if (Alive) {
                if (attack.paralyze) {
                    UpdateCharState(CharStateDef.Disabled);
                    paralyzeEndTime = Time.time + attack.paralyzeDuration;
                    StartCoroutine(ParalyzeTask(attack));
                }
            } else {
                agent.enabled = false;
                animator.enabled = false;
                foreach (IDestructible d in destructibles) {
                    d.OnDestruction(attack);
                }
                charUI.Disable();
            }
        } else
            meshData.Paralyze(attack);
    }

    public IEnumerator ParalyzeTask(Attack attack) {
        Paralyze(attack);
        while (Time.time < paralyzeEndTime)
            yield return new WaitForSeconds(paralyzeEndTime - Time.time);

        if (Alive) {
            GetUp();
            foreach (IParalyzable item in paralyzables) {
                item.GetUp();
            }
        }
    }

    protected void Paralyze(Attack attack) {
        animator.enabled = false;
        agent.enabled = false;
        foreach (IParalyzable item in paralyzables) {
            item.Paralyze(attack);
        }
    }

    protected void GetUp() {
        Vector3 footPos = meshData.FootPosition;
        Vector3 pelvisPos = meshData.PelvisPosition;
        footPos.y = pelvisPos.y;

        int getUpId;
        Quaternion rotation;
        if (meshData.IsFacingUp()) {
            getUpId = Random.Range(1, 4);
            rotation = Quaternion.LookRotation((footPos - pelvisPos), Vector3.up);
        } else {
            getUpId = 0;
            rotation = Quaternion.LookRotation((pelvisPos - footPos), Vector3.up);
        }

        Vector3 position;
        if (getUpId == 2)  //char position is under foots
            position = footPos;
        else
            position = pelvisPos; //char position is under pelvis

        position.y = 0;
        transform.position = position;
        transform.rotation = rotation;

        animator.enabled = true;
        animator.SetInteger(CharData.getUpIdHash, getUpId);
        animator.SetTrigger(CharData.GetUpHash);
    }

    private void OnGetUpComplete()  // Called in animator
    {
        if (!Alive)
            return;

        agent.enabled = true;
        if (!agent.isOnNavMesh) {
            MyDebug.LogError("Not on navmesh");
        }
        UpdateCharState(CharStateDef.Idle);
        foreach (IParalyzable item in paralyzables) {
            item.OnGotUp();
        }
    }

    public void Warp(Vector3 position) {
        Weapon.gameObject.SetActive(false);
        agent.Warp(position);
        Weapon.gameObject.SetActive(true);
    }
    #endregion
}