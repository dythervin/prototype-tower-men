using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager> {
    [SerializeField] CharController playerPrefab = null;
    [SerializeField] CharController enemyPrefab = null;
    [SerializeField] int amountOfEnemyToSpawn = 2;
    [SerializeField] Vector2 xZRange = Vector2.one * 5f;
    [SerializeField] float spawnRadius = 5;

    public CharController Player { get; private set; }
    public HashSet<CharController> Enemies { get; private set; }


    protected override void Awake() {
        base.Awake();
        Enemies = new HashSet<CharController>();
    }

    private void Start() {
        UIManager.Instance.ReserPlayerPosButton.onClick.AddListener(
            delegate () { Player.Agent.SetDestination(Vector3.back * 2); });
    }
    public void SpawnAll() {
        SpawnPlayer();
        SpawnEnemies();
    }

    public void SpawnPlayer() {
        if (Player == null) {
            Player = Instantiate(playerPrefab);
            Player.Agent.Warp(GetRandomPosOnCircle(spawnRadius));
        } else
            Player.Warp(GetRandomPosOnCircle(spawnRadius));

        Player.UpdateCharState(CharStateDef.Idle);
    }

    public void SpawnEnemies() {
        SpawnEnemies(amountOfEnemyToSpawn);
    }
    public void SpawnEnemies(int amount) {
        for (int i = 0; i < amount; i++) {
            Enemies.Add(Instantiate(enemyPrefab, GetRandomPosOnCircle(spawnRadius), GetRandomRotation()));
        }
    }

    public void RemoveCharacters() {
        //Player.Disable();
        foreach (var enemy in Enemies) {
            enemy.DestroyChar();
        }
        Enemies.Clear();
    }

    private Vector3 GetRandomPosition() {
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(-xZRange.x, xZRange.x);
        pos.z = Random.Range(-xZRange.y, xZRange.y);
        return pos;
    }

    private Vector3 GetRandomPosOnCircle(float radius) {
        Vector2 pos = Random.insideUnitCircle.normalized * radius;
        return new Vector3(pos.x, 0, pos.y);
    }



    private Quaternion GetRandomRotation() {
        return Quaternion.Euler(0, Random.Range(0, 360f), 0);
    }
}
