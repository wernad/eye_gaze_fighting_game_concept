using UnityEngine;

/// <summary>
/// Base class for all attack animting scripts. Contains attack rig and state handler of a character.
/// </summary>
public class AttacksBase : MonoBehaviour
{
    protected AttackRig attackRig;
    protected StateHandler stateHandler;

    //References to attack rigs and state of character.
    protected virtual void Start()
    {
        attackRig = GetComponent<AttackRig>();
        stateHandler = GetComponent<StateHandler>();
    }
}
