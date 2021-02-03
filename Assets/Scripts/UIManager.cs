using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {
    [SerializeField] Transform healthParent = null;
    [SerializeField] PauseMenu pauseMenu = null;
    [SerializeField] CharUI charUIPrefab = null;
    [SerializeField] Button resetPlayerPosButton = null;
    public Button ReserPlayerPosButton => resetPlayerPosButton;
    private readonly HashSet<CharUI> charUIs = new HashSet<CharUI>();
    public RectTransform RectTransform { get; private set; }

    protected override void Awake() {
        base.Awake();
        RectTransform = GetComponent<RectTransform>();
        GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
    }

    public Vector3 GetCanvasPos(Vector3 worldSpacePos, float offsetHeightPercent = 0.01f) {
        Vector2 screenPoint = GameManager.Instance.playerCamera.Camera.WorldToScreenPoint(worldSpacePos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, screenPoint, null, out Vector2 canvasPos);
        canvasPos.y = offsetHeightPercent * RectTransform.rect.height;
        return canvasPos;
    }

    void HandleGameStateChanged(GameStateDef previousState, GameStateDef targetState) {
        pauseMenu.gameObject.SetActive(targetState == GameStateDef.Paused);
    }

    public CharUI GetCharUI() {
        CharUI charUI = Instantiate(charUIPrefab, healthParent);
        charUIs.Add(charUI);
        return charUI;
    }

    public void DestroyCharUI(CharUI charUI) {
        charUIs.Remove(charUI);
        charUI.Disable();
        Destroy(charUI.gameObject,1);
    }
    private void Update() {
        foreach (var charUI in charUIs) {
            charUI.FollowTarget();
        }
    }
}



