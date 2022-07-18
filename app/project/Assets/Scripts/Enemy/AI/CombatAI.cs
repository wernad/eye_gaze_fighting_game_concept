// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System.Linq;
using UnityEngine;

public class CombatAI : MonoBehaviour
{   
    //References to other scripts
    private GameControl gameControl;
    private MovementHandler movementHandler;
    private StateHandler stateHandler;
    private StateHandler playerStateHandler;
    private Transform aimTarget;
    private RNGHandler rng;
    private Combat combat;
    private EnemyStatus status;
    private EnemyHitDetection hitDetection;
    private BlockRig blockRig;

    private GameObject[] playerUpperBody;
    private GameObject[] playerLowerBody;

    private int[] dashPunchSequence = {CombatUtilities.Backward, CombatUtilities.Forward, CombatUtilities.LightPunch};
    
    //Defense related variables
    private bool retreating = false;
    private float startBlockingTime = 0;
    private float blockDuration = 0.75f;
    private float retreatDamageThreshold = 80f;
    private float dashDamageThreshold = 80f;
    private float minOffenseDistance = 2.1f;
    private float startRetreatingTime;
    private float retreatDuration = 2f;  

    //Offense related variables
    private bool approaching = false;
    private bool moveInProgress = false;
    private int[] currentMove;
    private int currentMovePart = 0;
    private float offenseCooldown = 0.5f;
    private float lastOffenseTime = 0;
    private bool lastAttackBlocked = false; 
    private bool lastAttackHit = true;

    //Acquire all needed references.
    void Start()
    {
        gameControl = GameObject.Find("GameManager").GetComponent<GameControl>();
        aimTarget = transform.Find("EnemyTarget").GetComponent<Transform>();
        rng = GetComponent<RNGHandler>();
        combat = GetComponent<Combat>();
        blockRig = GetComponent<BlockRig>();
        stateHandler = GetComponent<StateHandler>();
        movementHandler = GetComponent<MovementHandler>();
        status = GetComponent<EnemyStatus>();
        hitDetection = GetComponent<EnemyHitDetection>();
        playerStateHandler = GameObject.Find("Player").GetComponent<StateHandler>();
        playerUpperBody = GameObject.FindGameObjectsWithTag("PlayerUpperBody");
        playerLowerBody = GameObject.FindGameObjectsWithTag("PlayerLowerBody");
    }

    //Each frame determine what to do.
    void Update() {
        if(stateHandler.recentlyHit) {
            return;
        }

        if(gameControl.GetIsTrainingModeOn()) {
            moveInProgress = false;
            Stay();
        } else if(!gameControl.GetIsGamePaused()) {
            WhatToDo();
        }
    }

    //Decides between offense and defense, but let's certain actions to finish, like blocking or retreating.
    void WhatToDo() {
        if(retreating && movementHandler.GetWallTouching()) {
            Stay();
        }

        if(stateHandler.blockBroken) {
            if(stateHandler.blocking) {
                blockRig.StopBlocking();
            }

            if(movementHandler.GetWallTouching()){
                Offense();
                status.UpdateLastHealth();
            }
        }

        if(status.GetLastHealth() == status.GetCurrentHealth()) {
            if(stateHandler.blocking && startBlockingTime + blockDuration > Time.time) {
                return;
            }

            if(retreating && startRetreatingTime + retreatDuration > Time.time) {
                return;
            }

            if(lastOffenseTime + offenseCooldown > Time.time) {
                return;
            }

            Offense();
        } else {
            if(moveInProgress) {
                status.UpdateLastHealth();
                return;
            }

            Defense();
        }
    }

    //Moves closer to the player if the player is too far away.
    //Otherwise decides what type of attack to use. 
    //Decision made is based partly on random numbers, partly on enemy state (crouching, bocking).
    void Offense() {
        lastOffenseTime = Time.time;

        if(stateHandler.blocking) {
            blockRig.StopBlocking();
        }

        if(stateHandler.crouching) {
            movementHandler.StopCrouching();
        }

        float distance = GetDistanceFromPlayer();
        if(distance > minOffenseDistance) {
            if(distance > minOffenseDistance * 2 && !approaching) {
                movementHandler.SetDashDirection(stateHandler.GetIfOnLeftSide() ? 1 : -1);
                movementHandler.UseDash();
            }
            
            if(!approaching) {
                Approach();
            }

        } else {
            if(stateHandler.attacking) {
                return;
            }
            
            if(approaching) {
                Stay();
            }

            if(moveInProgress) {
                ProcessMove();
                return;
            }

            if(rng.ShouldUseSpecialAttack()) {
                if(status.GetCurrentEnergy() >= 60) {
                    SetRandomTarget(playerUpperBody);
                    foreach(int movePart in dashPunchSequence) {
                        combat.AddMove(movePart);
                    }
                    
                }
            } else if(rng.ShouldUseComboAttack()) {
                lastAttackHit = true;
                moveInProgress = true;
                currentMove = rng.GetRandomCombo();
                currentMovePart = 0;

                if(currentMove[currentMovePart] < 2) {
                    SetRandomTarget(playerUpperBody);
                } else {
                    SetRandomTarget(playerLowerBody);
                }

                ProcessMove();
            } else {
                if(playerStateHandler.crouching) {
                    if(rng.GetRandomBoolean()) {
                        stateHandler.crouching = true;
                        combat.AddMove(rng.GetRandomSimpleAttack(1));
                    } else {
                        SetRandomTarget(playerLowerBody);
                        combat.AddMove(rng.GetRandomSimpleAttack(2));
                    }
                } else {
                    int attack = rng.GetRandomSimpleAttack();
                    
                    if(attack < 2) {
                        SetRandomTarget(playerUpperBody);
                    } else {
                        SetRandomTarget();
                    }
                    
                    combat.AddMove(attack);
                }
            }
        }
    }

    //Processes combo move until it's finished.
    //Updates target if attack is blocked or it missed.
    void ProcessMove() {
        if(lastAttackBlocked) {
            if(currentMove[currentMovePart] < 2) {
                SetRandomTarget(playerUpperBody);
            } else {
                SetRandomTarget(playerLowerBody);
            }
            
        } else if(lastAttackHit == false) {
            if(GetDistanceFromPlayer() > minOffenseDistance){
                moveInProgress = false;
                return;
            } else {
                if(currentMove[currentMovePart] < 2) {
                    SetRandomTarget(playerUpperBody);
                } else {
                    SetRandomTarget(playerLowerBody);
                }
            }
        }
        
        lastAttackHit = false;
        combat.AddMove(currentMove[currentMovePart]);
        currentMovePart++;

        if(currentMovePart >= currentMove.Length) {
            moveInProgress = false;
        }
        
    }

    //Decides what to do (block, retreat) based on damage taken in recent couple of seconds.
    void Defense() {
        float damageTaken = status.GetDamageTaken();

        if(approaching) {
            Stay();
        }

        if(!stateHandler.blockBroken && !stateHandler.blocking && damageTaken <= retreatDamageThreshold) {
            Block();
        }

        if(!movementHandler.GetWallTouching()) {
            if(damageTaken >= dashDamageThreshold) {
                if(stateHandler.crouching) {
                    movementHandler.StopCrouching();
                }

                if(retreating) {
                    Stay();
                }

                if(stateHandler.blocking) {
                    blockRig.StopBlocking();
                }

                movementHandler.SetDashDirection(stateHandler.GetIfOnLeftSide() ? -1 : 1);
                movementHandler.UseDash();

            } else if(damageTaken >= retreatDamageThreshold) {
                if(stateHandler.crouching) {
                    movementHandler.StopCrouching();
                }

                Retreat();
                startRetreatingTime = Time.time;
            } 
        }

        status.UpdateLastHealth();
    }

    //Sets target to player's body part and start blocking.
    //Starts crouching as well if it was hit in lower body half.
    void Block() {
        GameObject newTarget = FindBodyPart(status.GetBodyPartHit(), playerUpperBody);
        if(newTarget != null) {
            ChangeTarget(newTarget);
        } else {
            newTarget = FindBodyPart(status.GetBodyPartHit(), playerLowerBody);
            ChangeTarget(newTarget);
            movementHandler.StartCrouching();
        }

        startBlockingTime = Time.time;
        aimTarget.position = blockRig.Target2BlockPosition(aimTarget.position);
        blockRig.StartBlocking();
    }

    //Stops moving.
    void Stay() {
        approaching = false;
        retreating = false;
        movementHandler.SetWalkSpeed(0);
    }

    //Starts approaching player.
    void Approach() {
        approaching = true;
        retreating = false;
        movementHandler.SetWalkSpeed(stateHandler.GetIfOnLeftSide() ? 1 : -1);
    }

    //Start distancing itself from player.
    void Retreat() {
        approaching = false;
        retreating = true;
        movementHandler.SetWalkSpeed(stateHandler.GetIfOnLeftSide() ? -1 : 1);
    }

    float GetDistanceFromPlayer() {
        float distance = Vector3.Distance(transform.position, stateHandler.GetEnemyPosition());
        return distance;
    }

    void SetRandomTarget() {
        GameObject[] wholeBody = playerUpperBody.Union(playerLowerBody).ToArray();
        aimTarget.position = wholeBody[rng.GetRandomInt(wholeBody.Length)].transform.position;
    }

    void SetRandomTarget(GameObject[] bodyHalf) {
        aimTarget.position = bodyHalf[rng.GetRandomInt(bodyHalf.Length)].transform.position;
    }

    void ChangeTarget(GameObject bodyPart) {      
        aimTarget.position = bodyPart.transform.position;
    }

    void ChangeTarget(string bodyPart) {      
        aimTarget.position = FindBodyPart(bodyPart, playerUpperBody.Union(playerLowerBody).ToArray()).transform.position;
    }

    GameObject FindBodyPart(string bodyPart, GameObject[] bodyHalf) {
        foreach(GameObject obj in bodyHalf) {
            if(obj.name == bodyPart) {
                return obj;
            }
        }
        return null;
    }

    public void SetLastAttackHit(bool value) {
        lastAttackHit = value;
    }
    public void SetLastAttackBlocked(bool value) {
        lastAttackBlocked = value;
    }
}
