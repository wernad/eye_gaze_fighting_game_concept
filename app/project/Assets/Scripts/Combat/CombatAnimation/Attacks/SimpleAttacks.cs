// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SimpleAttacks : AttacksBase
{
    [SerializeField] private Dictionary<string, float> rigTransitionSpeed = new Dictionary<string, float> {
        {"LightPunch", 7.5f},
        {"HeavyPunch", 5f},
        {"LightKick", 7.5f},
        {"HeavyKick", 5f}
    };

    //Smooth transition between maximum and minimum value of weight for basic attacks.
    private IEnumerator SimpleAttack(Rig rig, string moveName) {
        stateHandler.attacking = true;
        while(rig.weight < 1 && stateHandler.attacking) {
            yield return null;
            rig.weight += rigTransitionSpeed[moveName] * Time.deltaTime;
        }

        while(rig.weight > 0 && stateHandler.attacking) {
            yield return null;
            rig.weight -= rigTransitionSpeed[moveName] * Time.deltaTime;
        }
        rig.weight = 0;
        stateHandler.attacking = false;
    }

    public void LightPunch() {
        StartCoroutine(SimpleAttack(attackRig.GetLightPunchRig(), "LightPunch"));
    }

    public void HeavyPunch() {
        StartCoroutine(SimpleAttack(attackRig.GetHeavyPunchRig(), "HeavyPunch"));
    }

    public void LightKick() {
        StartCoroutine(SimpleAttack(attackRig.GetLightKickRig(), "LightKick"));
    }

    public void HeavyKick() {
        StartCoroutine(SimpleAttack(attackRig.GetHeavyKickRig(), "HeavyKick"));
    }
}
