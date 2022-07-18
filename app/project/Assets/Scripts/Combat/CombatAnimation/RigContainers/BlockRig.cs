// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BlockRig : MonoBehaviour
{
    [SerializeField] private Transform blockCenter;
    private Rig bothHandsRig;   
    protected StateHandler stateHandler;

    //Gets reference to rig responsible for blocking animation.
    void Start()
    {
        stateHandler = GetComponent<StateHandler>();
        bothHandsRig = this.transform.Find("BothHandsRig").GetComponent<Rig>();
        bothHandsRig.weight = 0;
    }

    public void StartBlocking() {
        bothHandsRig.weight = 1;
        stateHandler.blocking = true;
    }

    public void StopBlocking() {
        bothHandsRig.weight = 0;
        stateHandler.blocking = false;
    }

    //Moves target to specific position in front of player. Used during blocking.
    public Vector3 Target2BlockPosition(Vector3 targetPos) {
        Vector3 newMouseTargetPos = CombatUtilities.MoveInfrontOf(blockCenter.position, targetPos, 1f);

        return new Vector3(newMouseTargetPos.x, newMouseTargetPos.y, -1);
    }
}
