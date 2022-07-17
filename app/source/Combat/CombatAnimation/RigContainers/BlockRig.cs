using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Container for block rig. Uses Unity's Animation Rigging package.
/// </summary>
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

    /// <summary>
    /// Starts blocking by setting rig weight to 1.
    /// </summary>
    public void StartBlocking() {
        bothHandsRig.weight = 1;
        stateHandler.blocking = true;
    }

    /// <summary>
    /// Stops blocking by setting rig weight to 0.
    /// </summary>
    public void StopBlocking() {
        bothHandsRig.weight = 0;
        stateHandler.blocking = false;
    }

    /// <summary>
    /// Moves target to specific position in front of player. Used during blocking.
    /// </summary>
    /// <param name="targetPos">Current target's position.</param>
    public Vector3 Target2BlockPosition(Vector3 targetPos) {
        Vector3 newMouseTargetPos = CombatUtilities.MoveInfrontOf(blockCenter.position, targetPos, 1f);

        return new Vector3(newMouseTargetPos.x, newMouseTargetPos.y, -1);
    }
}
