using UnityEngine;

/// <summary>
/// Handles movement of the AI. Decisions are made by CombatAI script.
/// </summary>
public class MovementHandler : MovementControl
{
    private bool touchingWall = false;
    private bool shouldCrouch = false;
    private bool shouldJump = false;
    private int dashDirection;
    private float xPositionDuringWallCollision = -100;
    
    protected override void Start()
    {   
        base.Start();
        horizontalVelocity = 0f;
    }

    //Contains all check and movement methods calls, then updates character's positions.
    void Update()
    {
        if(stateHandler.recentlyHit) {
            return;
        }

        if(!gameControl.GetIsGamePaused()) {
            if(controller.enabled == false) {
                controller.enabled = true; 
            }
            
            Move();
        } else {
            controller.enabled = false; 
        }
    }

    //Checks if any type of move should be performed based on flag variables set by AI script.
    protected override void Move()
    {
        if(stateHandler.dashing) {
            return;
        }

        if(controller.enabled == false) {
            controller.enabled = true;
        }

        Crouch();
        Walk();
        Jump();

        ApplyGravity();
            
        controller.Move(movement * Time.deltaTime);
    }

    protected override void Walk() {
        movement.x = horizontalVelocity * movementSpeed;
    }

    protected override void Dash()
    {
        StartCoroutine(ExecuteDash(dashDirection));
    }

    protected override void Crouch()
    {
        if(shouldCrouch && controller.isGrounded) {
            stateHandler.crouching = true;
        } else {
            stateHandler.crouching = false;
        }
    }

    protected override void Jump()
    {
        if(shouldJump && controller.isGrounded && !stateHandler.crouching) {
            animationControl.jumping = true;
            movement.y = Mathf.Sqrt(jumpSpeed * -gravityValue);
            shouldJump = false;
        } 
    }

    //Updates position of last collision with wall.
    void OnControllerColliderHit(ControllerColliderHit hit)
    {   
        string otherColliderName = hit.collider.gameObject.name;

        if(otherColliderName == "LeftWall" || otherColliderName == "RightWall") {
            xPositionDuringWallCollision = transform.position.x;
        } 

        if(transform.position.x == xPositionDuringWallCollision) {
            touchingWall = true;
        } else {
            touchingWall = false;
        }
    }

    /// <summary>
    /// Starts crouching.
    /// </summary>
    public void StartCrouching() {
        shouldCrouch = true;
    }

    /// <summary>
    /// Stops crouching.
    /// </summary>
    public void StopCrouching() {
        shouldCrouch = false;
    }

    /// <summary>
    /// Jumps once.
    /// </summary>
    public void UseJump() {
        shouldJump = true;
    }

    /// <summary>
    /// Sets walking speed to given value.  
    /// </summary>
    /// <param name="speed">Walking speed.</param>
    public void SetWalkSpeed(float speed) {
        horizontalVelocity = speed;
    }

    /// <summary>
    /// Sets direction of dash (approach, retreat).  
    /// </summary>
    /// <param name="direction">Direction of dash.</param>
    public void SetDashDirection(int direction) {
        dashDirection = direction;
    }

    /// <summary>
    /// Encapsulates dash function.
    /// </summary>
    public void UseDash() {
        Dash();
    }

    /// <summary>
    /// Checks if AI is touching wall.
    /// </summary>
    public bool GetWallTouching() {
        return touchingWall;
    }
}
