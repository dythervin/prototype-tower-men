using UnityEngine;
using UnityEngine.AI;

public class CharMotor : MonoBehaviour {
    CharController charController;
    AttackController attackController;
    CharData charData;
    CharStateDef CharState => charController.CharState;
    Animator Animator => charController.Animator;
    NavMeshAgent Agent => charController.Agent;
    private bool onTower = false;

    private void Awake() {
        charController = GetComponent<CharController>();
        attackController = GetComponent<AttackController>();
        charData = GetComponent<CharData>();
    }
    private void Update() {
        if (CharState == CharStateDef.CanAttack || GameManager.Instance.GameState != GameStateDef.Running)
            return;


        if (Agent.velocity == Vector3.zero) {
            if (onTower && attackController != null && attackController.EnemyInVision()) {
                charController.UpdateCharState(CharStateDef.CanAttack);
            } else
                charController.UpdateCharState(CharStateDef.Idle);
        } else {
            charController.UpdateCharState(CharStateDef.Moving);
            float moveSpeed = Agent.velocity.magnitude / Agent.speed;
            Animator.SetFloat(CharData.moveSpeedHash, moveSpeed);
            if (moveSpeed < charData.AnimWalkThreshHold) {
                Animator.SetFloat(CharData.moveAnimSpeedHash, moveSpeed * charData.AnimMaxMoveSpeed / charData.AnimWalkThreshHold);
            } else {
                Animator.SetFloat(CharData.moveAnimSpeedHash, charData.AnimMaxMoveSpeed);
            }
        }


        if (Input.GetMouseButtonDown(0)) {
            Ray ray = GameManager.Instance.playerCamera.Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(Tags.walkable)) {
                Agent.SetDestination(hit.point);
                charController.UpdateCharState(CharStateDef.Moving);
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Tags.tower)) {
            onTower = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(Tags.tower)) {
            onTower = false;
            if (charController.CharState == CharStateDef.CanAttack)
                charController.UpdateCharState(CharStateDef.Idle);
        }
    }
}
