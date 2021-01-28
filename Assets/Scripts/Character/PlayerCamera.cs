using UnityEngine;

public class PlayerCamera : Singleton<PlayerCamera> {
    [Header("Must be child of character")]
    [SerializeField] Transform character = null;
    [SerializeField] Vector3 offset;
    [SerializeField] Camera cam = null;
    public Camera Camera => cam;

    private void Start() {
        GameManager.Instance.playerCamera = this; 
        offset = transform.localPosition;
        transform.parent = null;
    }

    private void LateUpdate() {
        if (character != null)
            transform.position = character.position + offset;
        else
            character = SpawnManager.Instance.Player.transform;
    }

    public void CheckCam() {
        if (cam == null)
            cam = Camera.main;
    }
}
