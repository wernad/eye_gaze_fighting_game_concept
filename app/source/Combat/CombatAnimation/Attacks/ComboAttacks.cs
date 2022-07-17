using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Contains methods used to animate combo attacks.
/// </summary>
/// <remarks>
/// Attack animations are accomplished via Unity's Animation Rigging package so animations take aiming into account. 
/// </remarks>
public class ComboAttacks : AttacksBase
{
    private Transform aimTarget;
    private float rotationSpeed = 720;
    private float transitionSpeed = 0.1f;
    private float lightPunchMaxRange = 1.9f;
    private float moveDuration = 0.25f;
    
    //Find reference to target object.
    protected override void Start() {
        base.Start();
        aimTarget = transform.Find(gameObject.name + "Target").GetComponent<Transform>();
    }

    //Move object in circular motion vertically or horizontally based on parameters.
    //Set rigs weight to maximum during the motion to animate "hook" motion.
    private IEnumerator Hook(bool isUppercut, bool isLightPunch, Rig rig) {    
        GameObject centerObject;
        Quaternion centerRotation;

        Vector3 centerPosition, directionTowardsEnd;        
        Vector3 endTargetPosition = aimTarget.position; //Store the position that player or AI is aiming at.

        if(isLightPunch) {
            aimTarget.position = rig.GetComponentInChildren<TwoBoneIKConstraint>().data.tip.position;
            rig.weight = 1;
        } else {
            aimTarget.position = rig.GetComponentInChildren<ChainIKConstraint>().data.tip.position;
            rig.weight = 1;
        }

        if(Vector3.Distance(aimTarget.position, endTargetPosition) > lightPunchMaxRange) {
            endTargetPosition = CombatUtilities.MoveInfrontOf(aimTarget.position, endTargetPosition, lightPunchMaxRange);
        }

        centerPosition = (aimTarget.position + endTargetPosition) / 2;
        directionTowardsEnd = (endTargetPosition - centerPosition).normalized;
        centerRotation = Quaternion.LookRotation(directionTowardsEnd);

        centerObject = new GameObject();
        centerObject.transform.position = centerPosition;
        centerObject.transform.rotation = centerRotation;

        stateHandler.attacking = true;
        rig.weight = 1;

        Vector3 axisToOrbit = DetermineAxis(isUppercut, isLightPunch, centerObject);
        float timeStart = Time.time;
        while(timeStart + moveDuration > Time.time && stateHandler.attacking) {
            yield return null;
            
            aimTarget.transform.RotateAround(centerPosition, axisToOrbit, rotationSpeed * Time.deltaTime);
        }

        while(rig.weight > 0 && stateHandler.attacking) {
            yield return new WaitForEndOfFrame();
            rig.weight -= transitionSpeed;
        }

        Destroy(centerObject);

        rig.weight = 0;
        stateHandler.attacking = false;
    }

    //Determines which axis should object rotate based on parameters.
    private Vector3 DetermineAxis(bool isUppercut, bool isLightPunch, GameObject centerObject) {
        if(isUppercut) {
            return -centerObject.transform.right;

        } else if(isLightPunch) {
            return -centerObject.transform.up;
        }

        return centerObject.transform.up;
    }
    
    /// <summary>
    /// Starts coroutine that performs horizontal hook with light punch rig.
    /// </summary>
    public void LightHook() {
        StartCoroutine(Hook(false, true, attackRig.GetLightPunchRig()));
    }

    /// <summary>
    /// Starts coroutine that performs horizontal hook with heavy punch rig.
    /// </summary>
    public void HeavyHook() {
        StartCoroutine(Hook(false, false, attackRig.GetHeavyPunchRig()));
    }
    
    /// <summary>
    /// Starts coroutine that performs vertical hook with light punch rig.
    /// </summary>
    public void LightUppercut() {
        StartCoroutine(Hook(true, true, attackRig.GetLightPunchRig()));
    }
    
    /// <summary>
    /// Starts coroutine that performs vertical hook with heavy punch rig.
    /// </summary>
    public void HeavyUppercut() {
        StartCoroutine(Hook(true, false, attackRig.GetHeavyPunchRig()));
    }
}
