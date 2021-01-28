using System;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    [SerializeField] GameObject[] systems = null;
    [SerializeField, ReadOnly] GameStateDef gameState;
    [SerializeField] LayerMask playerLayer = 1 >> 8;
    public LayerMask PlayerLayer => playerLayer;
    public PlayerCamera playerCamera;
    public Events.OnGameStateChangedEvent onGameStateChanged;
    public GameStateDef GameState => gameState;

    protected override void Awake() {
        base.Awake();
        foreach (var system in systems) {
            Instantiate(system, transform);
        }
    }

    public void TogglePause() {
        switch (gameState) {
            case GameStateDef.Running:
                UpdateGameState(GameStateDef.Paused);
                break;
            case GameStateDef.Paused:
                UpdateGameState(GameStateDef.Running);
                break;
        }
        playerCamera.CheckCam();
    }

    internal void RestartGame() {
        SpawnManager.Instance.RemoveCharacters();
        SpawnManager.Instance.SpawnAll();
        UpdateGameState(GameStateDef.Running);
    }

    private void Start() {
        UpdateGameState(GameStateDef.Running);
        SpawnManager.Instance.SpawnAll();
    }

    internal void QuitGame() {
        Application.Quit();
    }

    public void UpdateGameState(GameStateDef target) {
        switch (gameState) {
            case GameStateDef.Idle:
                break;
            case GameStateDef.Running:
                break;
            case GameStateDef.Paused:
                Time.timeScale = 1;
                break;
            case GameStateDef.Over:
                break;
            case GameStateDef.Win:
                break;
        }
        onGameStateChanged?.Invoke(gameState, target);
        gameState = target;
        switch (target) {
            case GameStateDef.Idle:
                break;
            case GameStateDef.Running:
                break;
            case GameStateDef.Paused:
                Time.timeScale = 0;
                break;
            case GameStateDef.Over:
                break;
            case GameStateDef.Win:
                break;
        }
    }

}
public enum GameStateDef { Idle, Running, Paused, Over, Win }

public interface IOnGameStateChanged {
    void OnGameStateChanged(GameStateDef previous, GameStateDef target);
}
