using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public enum CharTypeDef { Enemy, Player }
public class CharData : MonoBehaviour, IAttackable {
    public CharTypeDef CharType;
    [SerializeField] CharacterAnimationProperties_SO animProperties = null;
    [SerializeField] Weapon weaponPrefab = null;
    CharUI charUI = null;
    public CharController CharController { get; private set; }
    NavMeshAgent agent;
    Animator animator;
    MeshData meshData;
    bool leftHanded;

    #region ANIMATOR HASHES
    private static readonly int attackSpeedHash = Animator.StringToHash("attackSpeed");
    private static readonly int attackIdHash = Animator.StringToHash("attackId");
    public static readonly int idleIdHash = Animator.StringToHash("idleId");
    public static readonly int getUpIdHash = Animator.StringToHash("getUpId");

    public static readonly int moveSpeedHash = Animator.StringToHash("moveSpeed");
    public static readonly int moveAnimSpeedHash = Animator.StringToHash("moveAnimSpeed");

    public static readonly int AttackHash = Animator.StringToHash("Attack");
    public static readonly int MovingHash = Animator.StringToHash("Moving");
    public static readonly int LeftHandedHash = Animator.StringToHash("LeftHanded");

    public static readonly int SpellHash = Animator.StringToHash("Spell");
    public static readonly int TakeHitHash = Animator.StringToHash("TakeHit");
    public static readonly int GetUpHash = Animator.StringToHash("GetUp");
    public static readonly int RandomIdleHash = Animator.StringToHash("RandomIdle");
    #endregion

    public Weapon Weapon { get; private set; }
    #region MAIN STATS
    [SerializeField] MainStats baseStats = null;
    public MainStats BaseStats => baseStats;
    public float Health { get; private set; }
    public float HealthPerc => Health / baseStats.maxHealth;
    public WaitForSeconds AttackDurationWait { get; private set; }
    public WaitForSeconds AttackDelay { get; private set; }
    public float AnimMaxMoveSpeed { get; private set; }
    public float AnimWalkThreshHold { get; private set; }
    #endregion

    #region INITIALIZATIONS
    public void Init(CharController charController, bool leftHanded, CharUI healthBar) {
        CharController = charController;
        agent = CharController.Agent;
        animator = charController.Animator;
        meshData = charController.MeshData;
        this.leftHanded = leftHanded;
        charUI = healthBar;
        ChangeWeapon(weaponPrefab);

        Health = baseStats.maxHealth;
        AttackDelay = new WaitForSeconds(baseStats.attackDelay);

        AnimWalkThreshHold = animProperties.walk / animProperties.run;
        charUI.Init(meshData.Center, HealthPerc);
        OnAttackSpeedChange(0);
        OnMoveSpeedChange(0);
        OnAngularSpeedChange(0);
    }
    #endregion

    #region HEALTH
    public bool ApplyHealth(float healthAmount, bool healTo = false, bool percent = false) {
        if (healthAmount < 0) {
            MyDebug.LogWarning("Negative apply health");
            return false;
        }

        if (Health == baseStats.maxHealth)
            return false;
        else if (percent && healthAmount > 1)
            return false;

        bool healed;
        if (healTo) {
            if (percent && healthAmount > HealthPerc)
                healed = true;
            else if (!percent && healthAmount > Health)
                healed = true;
            else
                healed = false;
        } else
            healed = true;

        if (healed) {

            Health = Mathf.Clamp(healTo ? 0 : Health + healthAmount * (percent ? baseStats.maxHealth : 1), 0, baseStats.maxHealth);
            charUI.HealthBar.HealthPercent = HealthPerc;
        }
        return healed;

    }
    public void TakeHit(Attack attack) {
        Health = Mathf.Clamp(Health - attack.damage, baseStats.immortal ? 1 : 0, baseStats.maxHealth);
        charUI.HealthBar.HealthPercent = HealthPerc;
    }
    #endregion

    #region STAT MODIFIERS

    private void OnAttackSpeedChange(float amount) {
        AttackDurationWait = new WaitForSeconds(1 / (amount + baseStats.attacksPerSec));
        animator.SetFloat(attackSpeedHash, amount + baseStats.attacksPerSec);
    }
    private void OnMoveSpeedChange(float amount) {
        agent.speed = amount + baseStats.moveSpeed;
        AnimMaxMoveSpeed = agent.speed / animProperties.run;
        animator.SetFloat(moveAnimSpeedHash, AnimMaxMoveSpeed);
    }

    private void OnAngularSpeedChange(float amount) {
        agent.angularSpeed = amount + baseStats.angularSpeed;
    }

    #endregion

    #region WEAPON
    public void ChangeWeapon(Weapon weaponPick) {
        Transform weaponSlot;
        weaponSlot = meshData.ProjectileSpots[leftHanded ? 1 : 0];
        //weaponSlot = MeshData.WeaponSlot[leftHanded ? 1 : 0];

        UnEquipWeapon();
        EquipWeapon(weaponPick, weaponSlot);
    }

    public void EquipWeapon(Weapon weaponToEquip, Transform weaponSlot) {
        Weapon = Instantiate(weaponToEquip, weaponSlot);
        animator.SetFloat(attackIdHash, (int)Weapon.definition.attackAnimation);
        Weapon.Init(this);
    }

    public void UnEquipWeapon() {
        if (Weapon != null) {
            Weapon.Destroy();
            Weapon = null;
        }
    }
    #endregion
}
