// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public bool jumping {get; set; } = false;

    private Animator animator;
    private MovementControl movementControl;
    private CharacterController characterController;
    private StateHandler stateHandler;
    private bool falling = false;
    private float velocityThreshold = 0.01f;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        movementControl = GetComponent<MovementControl>();
        characterController = GetComponent<CharacterController>();
        stateHandler = GetComponent<StateHandler>();
    }

    //Checks what animation to play each frame.
    void Update()
    {
        PlayWalkingAnimation();
        PlayJumpAnimation();
        PlayFallingAnimation();
        PlayLandingAnimation();
        PlayCrouchAnimation();
    }

    void PlayWalkingAnimation() {
        if(stateHandler.grounded) {
            if(IsMovingForward() == 1) {
                animator.SetBool("isWalkingF", true);
                animator.SetBool("isWalkingB", false);
            } 
            else if(IsMovingForward() == -1){     
                animator.SetBool("isWalkingF", false);
                animator.SetBool("isWalkingB", true);
            }
            else {
                animator.SetBool("isWalkingF", false);
                animator.SetBool("isWalkingB", false);
            }
        }
    }

    void PlayJumpAnimation() {
          if(jumping) {
              animator.SetTrigger("Jump");
              jumping = false;
          }
    }
    void PlayFallingAnimation() {
        if(characterController.velocity.y < 0) {
            falling = true;
            jumping = false;
        }

        if(falling && !animator.GetCurrentAnimatorStateInfo(0).IsName("Fall")) {
            animator.SetTrigger("Falling");
        }
    }

    void PlayLandingAnimation() {
        if(falling && stateHandler.grounded) {
            falling = false;
            animator.SetTrigger("Land");
        }
    }

    void PlayCrouchAnimation() {
        if(stateHandler.crouching) {
            animator.SetBool("Crouching", true);
        } 
        else {
            animator.SetBool("Crouching", false);
        }
    }

    public int IsMovingForward() {
        if((movementControl.horizontalVelocity > velocityThreshold && stateHandler.GetIfOnLeftSide()) 
            || (movementControl.horizontalVelocity < -velocityThreshold && !stateHandler.GetIfOnLeftSide())) {
            return 1;
        } 
        else if((movementControl.horizontalVelocity > velocityThreshold && !stateHandler.GetIfOnLeftSide()) 
            || (movementControl.horizontalVelocity < -velocityThreshold && stateHandler.GetIfOnLeftSide())) {     
            return -1;
        }

        return 0;
    }
}
