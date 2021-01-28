using MyExtensions;
using UnityEngine;
[CreateAssetMenu(fileName = "new weapon", menuName = "Items/new weapon", order = 1)]
public class ItemWeapon_SO : ScriptableObject
{
    public WeaponHandle weaponHandle;

    [Range(0, 1)]
    public float damageVariance;

    [Range(1, 10)]
    public float criticalMultiplier = 1.5f;

    [Range(0, 1)]
    public float criticalChance = 0.3f;

    [Range(0, 1f)]
    public float paralyzeChance = 0.0f;
    
    [Range(0, 2f)]
    public float paralyzeDuration = 1f;

    [Min(0)]
    public float knockbackForce = 30f;

    [Range(0.01f, 10)]
    public float attackSpeedMultiplier = 1;

    public BasicAttackDefinitions attackAnimation;

    public Attack CreateAttack(CharData attacterData, Vector3 target, bool manualCrit = false, bool manualParalyze = false)
    {
        float coreDamage = attacterData.BaseStats.damage * MyRandom.Gaussian(1, damageVariance);
        bool isCritical = Random.value < criticalChance;
        if (isCritical || manualCrit)
            coreDamage *= criticalMultiplier;
        bool isParalyzable = Random.value < paralyzeChance || manualParalyze;

        Vector3 direction = (target - attacterData.CharController.Center.position).normalized;
        return new Attack(attacterData, coreDamage, isCritical, direction, knockbackForce, isParalyzable, paralyzeDuration);
    }
}


