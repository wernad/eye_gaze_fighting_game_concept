// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System.Collections;
using UnityEngine;

abstract public class MovementControl : MonoBehaviour
{
    protected GameControl gameControl;
    protected CharacterController controller;
    protected AnimationControl animationControl;
    protected StateHandler stateHandler;
    protected Vector3 movement;
    protected float movementSpeed = 5f;
    protected float jumpSpeed = 6f;
    protected float gravityValue = -20f;
    protected float dashDuration = 0.25f;

    public float horizontalVelocity {get; set;}

    protected virtual void Start()
    {
        gameControl = GameObject.Find("GameManager").GetComponent<GameControl>();
        controller = GetComponent<CharacterController>();
        animationControl = GetComponent<AnimationControl>();
        stateHandler = GetComponent<StateHandler>();
    }

    //Abstract methods, overriden by both characters.
    abstract protected void Move();    
    abstract protected void Walk();
    abstract protected void Dash();
    abstract protected void Crouch();
    abstract protected void Jump();

    //Dash is a coroutine that's smoothly moves character quickly.
    public IEnumerator ExecuteDash(int dashDirection, float dashSpeed = 10) {
        if(controller.isGrounded) {
            stateHandler.dashing = true;
            float dashStartTime = Time.time;

            while(Time.time < dashStartTime + dashDuration && !gameControl.GetIsGamePaused()) {
                controller.Move(new Vector3(dashDirection, 0,0) * dashSpeed * Time.deltaTime);
                yield return null;
            }

            stateHandler.dashing = false;
        }
    }

    protected void FaceEnemy(Transform origin, Transform target) {
        Vector3 distance = target.position - origin.position;
        origin.rotation = Quaternion.Slerp(origin.rotation, Quaternion.LookRotation(distance), 5f * Time.deltaTime);
        origin.eulerAngles = new Vector3(0, origin.eulerAngles.y, 0);
    }

    protected void ApplyGravity() {
        movement.y += gravityValue * Time.deltaTime;
    }
}
