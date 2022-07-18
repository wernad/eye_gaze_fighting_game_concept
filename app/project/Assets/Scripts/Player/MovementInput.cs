// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;

public class MovementInput : MovementControl
{
    private Combat combat;
    private int dashDirection = 0;
    private float lastWalkPressedTime = 0f;
    private float doubleTapTimeout = 1f;
    private bool walkButtonDown = false;
    private bool walkPressed = false;

    protected override void Start()
    {
        base.Start();
        combat = GetComponent<Combat>();
    }

    //Takes input only if game is not paused or character wasn't hit.
    //Character Controller is disabled during pause or gameover.
    void Update()
    {
        if(stateHandler.recentlyHit) {
            return;
        }

        if(!gameControl.GetIsGamePaused()) {
            Move();
        } else {
            controller.enabled = false; 
        }
    }

    //Contains all check and movement methods calls, then updates character's positions.
    protected override void Move() {
        if(stateHandler.dashing) {
            return;
        }

        if(controller.enabled == false) {
            controller.enabled = true; 
        }

        stateHandler.grounded = controller.isGrounded;

        if(stateHandler.attacking && stateHandler.grounded) {
            return;
        }

        CheckIfWalkPressed();

        Crouch();
        Walk();
        Dash();
        Jump();

        ApplyGravity();

        controller.Move(movement * Time.deltaTime);
    }

    //Check for horizontal input and updates character's velocity.
    protected override void Walk() {
        if(stateHandler.crouching) {
            horizontalVelocity = 0;
        } else {
            horizontalVelocity = Input.GetAxisRaw("Horizontal");
        }

        if(walkPressed) {
            if(animationControl.IsMovingForward() == 1) {
                combat.AddMove(CombatUtilities.Forward);
            } else if(animationControl.IsMovingForward() == -1) {
                combat.AddMove(CombatUtilities.Backward);
            }
        }

        movement.x = horizontalVelocity * movementSpeed;
    }

    protected override void Jump() {
        if(Input.GetButton("Jump") && controller.isGrounded && !stateHandler.crouching) {
            animationControl.jumping = true;
            movement.y = Mathf.Sqrt(jumpSpeed * -gravityValue);
        }
    }

    protected override void Crouch() {
        if(Input.GetButton("Crouch") && controller.isGrounded) {
            stateHandler.crouching = true;
        } 
        else {
            stateHandler.crouching = false;
        }
    }

    //Check for double tap to perform dash.
    protected override void Dash() {
        if(stateHandler.dashing) {
            return;
        }

        if(walkPressed) {
            if(Time.time < lastWalkPressedTime + doubleTapTimeout) {
                if(dashDirection == horizontalVelocity) {
                    StartCoroutine(ExecuteDash(dashDirection));
                    lastWalkPressedTime = 0f;
                    dashDirection = 0;
                    return;
                }
            }
            dashDirection = (int)horizontalVelocity;
            lastWalkPressedTime = Time.time;
        }
    }

    //Method used to detect double tap of horizontal buttons.
    private void CheckIfWalkPressed() {
        if(horizontalVelocity != 0) {
            if(!walkButtonDown) {
                walkButtonDown = true;
                walkPressed = true;
                return;
            }
            walkPressed = false;
        } else {
            walkButtonDown = false;
            walkPressed = false;
        }        
    }
}

