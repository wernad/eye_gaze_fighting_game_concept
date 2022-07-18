// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;

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
