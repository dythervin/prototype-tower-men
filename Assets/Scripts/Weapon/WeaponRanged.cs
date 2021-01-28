using UnityEngine;

public class WeaponRanged : Weapon
{
    [SerializeField]
    private ProjectileController projectilePrefab = null;

    [SerializeField]
    private int amountToPool = 2;

    private ObjectPooler<ProjectileController> pooledProjectiles;

    private void Start()
    {
        pooledProjectiles = new ObjectPooler<ProjectileController>(projectilePrefab, amountToPool: amountToPool);
    }

    public override void Attack(Vector3 projectilePos, Vector3 target)
    {
        ProjectileController projectile = pooledProjectiles.GetPooledObject(projectilePos, true);
        projectile.Launch(target, definition.CreateAttack(charData, target));
    }

    private void OnDestroy() {
        pooledProjectiles.Destroy();
    }

}
