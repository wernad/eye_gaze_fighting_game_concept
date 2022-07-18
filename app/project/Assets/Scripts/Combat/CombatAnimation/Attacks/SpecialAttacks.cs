// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpecialAttacks : AttacksBase
{
    private Transform aimTarget;
    private MovementControl movementControl;
    private CharacterStatus characterStatus;
    private float transitionSpeed = 0.1f;
    private int dashPunchCost = 60;

    //Get references to aim target and other scripts.
    protected override void Start() {
        base.Start();
        movementControl = GetComponent<MovementControl>();
        characterStatus = GetComponent<CharacterStatus>();
        aimTarget = transform.Find(gameObject.name + "Target").GetComponent<Transform>();
    }

    //Performs dash with rig's weight set to maximum, creating "Dash Punch" special attack.
    private IEnumerator DashPunchCoroutine() {   
        Rig rig = attackRig.GetLightPunchRig();
        stateHandler.attacking = true;

        rig.weight = 1;
        StartCoroutine(movementControl.ExecuteDash(stateHandler.GetIfOnLeftSide() ? 1 : -1, 20f));
        while(stateHandler.dashing && stateHandler.attacking) {
            yield return null;
        }

        while(rig.weight > 0 && stateHandler.attacking) {
            yield return new WaitForEndOfFrame();
            rig.weight -= transitionSpeed;
        }
        
        rig.weight = 0;
        stateHandler.attacking = false;

    }

    private bool CheckEnergyBar(int requiredEnergy) {
        return characterStatus.GetCurrentEnergy() >= requiredEnergy;
    }

    //Check energy before perforiming special attack;
    public void DashPunch() {
        if(CheckEnergyBar(dashPunchCost)) {
            characterStatus.UseEnergy(dashPunchCost);
            StartCoroutine(DashPunchCoroutine());
        } else {
            Debug.Log("Not enough energy");
        }
    }
}
