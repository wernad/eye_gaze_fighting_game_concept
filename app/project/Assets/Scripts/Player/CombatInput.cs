// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;

public class CombatInput : MonoBehaviour
{
    [SerializeField] private LayerMask aimMask;
    private Transform aimTarget;
    private GameControl gameControl;
    private BlockRig blockRig;
    private StateHandler stateHandler;
    private Combat combat;
    private AimDataReceiver aimData;
    private Camera aimCamera;
    
    void Start()
    {
        gameControl = GameObject.Find("GameManager").GetComponent<GameControl>();
        blockRig = GetComponent<BlockRig>();
        stateHandler = GetComponent<StateHandler>();
        aimData = GetComponent<AimDataReceiver>();
        combat = GetComponent<Combat>();
        aimTarget = transform.Find("PlayerTarget").GetComponent<Transform>();
        aimCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    //Updates aim target position based on mouse position or eye gaze.
    void Update()
    {   
        if(stateHandler.recentlyHit) {
            return;
        }

        if(!gameControl.GetIsGamePaused()) {
            if(gameControl.GetIsMouseAimingOn()) {
                SetTargetPosition(ScreenToWorld(Input.mousePosition));
            } else {
                var (x, y) = aimData.GetCoordinates();
                SetTargetPosition(ScreenToWorld(new Vector3(x, y, -1)));
            }
       
            Attack();
            Block();
        }
        
    }

    //Checks for input for each basic type of attack;
    void Attack() {
        if(!stateHandler.attacking && !stateHandler.blocking) {
            if(Input.GetButtonDown("LightPunch")) {;
                combat.AddMove(CombatUtilities.LightPunch);

            } else if(Input.GetButtonDown("HeavyPunch")) {
                combat.AddMove(CombatUtilities.HeavyPunch);

            } else if(Input.GetButtonDown("LightKick")) {
                combat.AddMove(CombatUtilities.LightKick);

            } else if(Input.GetButtonDown("HeavyKick")) {
                combat.AddMove(CombatUtilities.HeavyKick);
            } 
        }
    }

    //Updates block if button is held or released.
    void Block() {
        if(stateHandler.blockBroken) {
            if(stateHandler.blocking) {
                blockRig.StopBlocking();
                stateHandler.blocking = false;
            }
            
            return;
        }
        
        if(Input.GetButtonDown("Block") && !stateHandler.attacking) {
            blockRig.StartBlocking();
            
        } else if(stateHandler.blocking && !Input.GetButton("Block")) {
            blockRig.StopBlocking();
        }
    }

    //Method calculates proper position of screen position to 3D game world position.
    Vector3 ScreenToWorld(Vector3 screenPosition) {
        if(!gameControl.GetIsMouseAimingOn()) {
            screenPosition.x = screenPosition.x * Screen.width;
            screenPosition.y = screenPosition.y * Screen.height;
        }

        Ray ray = aimCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        Vector3 newPosition = new Vector3(0f, 0f, -1f);
        
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, aimMask) && !stateHandler.attacking) {
            newPosition = new Vector3(hit.point.x, hit.point.y, -1);
        }

        return newPosition;
    }


    //Updates position of target using given position. Restricts target to positions in front of player.
    void SetTargetPosition(Vector3 position) {
        if(!stateHandler.attacking) {
            if(!IsTargetInFront(position)) {
                position.x = transform.position.x + transform.forward.x;
            }

            if(stateHandler.blocking) {
                aimTarget.position = blockRig.Target2BlockPosition(position);
            } else {
                aimTarget.position = position;
            }
        }
    }

    //Checks if input position is in front of player.
    private bool IsTargetInFront(Vector3 targetPos) {
        float playerPositionX = transform.position.x + transform.forward.x;
        if(transform.forward.x < 0) {
            return targetPos.x < playerPositionX;    
        }
        return targetPos.x > playerPositionX;
    }
}