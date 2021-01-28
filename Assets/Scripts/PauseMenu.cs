using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    [SerializeField] Button resumeButton = null;
    [SerializeField] Button quitButton = null;
    [SerializeField] Button restartButton = null;

    public void HandleResumeClicked() {
        GameManager.Instance.TogglePause();
    }

    public void HandleQuitClicked() {
        GameManager.Instance.QuitGame();
    }
    public void HandleRestartClicked() {
        GameManager.Instance.RestartGame();
    }
}
