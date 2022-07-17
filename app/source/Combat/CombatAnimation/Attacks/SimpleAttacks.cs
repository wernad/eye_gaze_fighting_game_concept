using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Handles animating of simple attacks. Animating is done via Coroutines. 
/// </summary>
/// <remarks>
/// Attack animations are accomplished via Unity's Animation Rigging package so animations take aiming into account. 
/// </remarks>
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

    /// <summary>
    /// Starts coroutine that performs simple attack with light punch rig.
    /// </summary>
    public void LightPunch() {
        StartCoroutine(SimpleAttack(attackRig.GetLightPunchRig(), "LightPunch"));
    }

    /// <summary>
    /// Starts coroutine that performs simple attack with heavy punch rig.
    /// </summary>
    public void HeavyPunch() {
        StartCoroutine(SimpleAttack(attackRig.GetHeavyPunchRig(), "HeavyPunch"));
    }

    /// <summary>
    /// Starts coroutine that performs simple attack with light kick rig.
    /// </summary>
    public void LightKick() {   
        StartCoroutine(SimpleAttack(attackRig.GetLightKickRig(), "LightKick"));
    }

    /// <summary>
    /// Starts coroutine that performs simple attack with heavy kick rig.
    /// </summary>
    public void HeavyKick() {
        StartCoroutine(SimpleAttack(attackRig.GetHeavyKickRig(), "HeavyKick"));
    }
}
