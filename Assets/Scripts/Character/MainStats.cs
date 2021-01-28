using UnityEngine;

[CreateAssetMenu(fileName = "CharStats", menuName = "Character/Stats")]
public class MainStats : ScriptableObject {
    [Min(10)] public float maxHealth;
    public bool immortal = false;
    [Space]

    [Min(0.01f)] public float moveSpeed;
    [Min(120)] public float angularSpeed;
    [Space]

    [Min(0)] public float damage;
    [Min(0.01f), Tooltip("Without considering attack delay")] public float attacksPerSec;
    [Min(0), Tooltip("Delay between attacks")] public float attackDelay;
}




