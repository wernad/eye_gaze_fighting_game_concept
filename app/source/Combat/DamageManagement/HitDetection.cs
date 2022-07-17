using System.Linq;
using UnityEngine;
using System;

/// <summary>
/// Detects collisions (hits) between characters. Determines if character should take damage to health or block.
/// </summary>
public class HitDetection : MonoBehaviour
{
    protected string myColliderName;
    protected CharacterStatus myStatus;
    protected CharacterStatus enemyStatus;
    protected StateHandler myStateHandler;
    protected StateHandler enemyStateHandler;
    protected string enemyColliderName;
    protected string[] damageCollidersNames = {"LHand", "RHand", "LFoot", "RFoot"};
    protected string[] armsCollidersNames = {"LHand", "RHand", "LForearm", "RForearm", "LUpperarm", "RUpperarm"};
    protected string[] lowerBodyCollidersNames = {"LFoot", "RFoot", "LShin", "RShin", "LThigh", "RThigh"};
    
    //Gets references to state and status of both characters.
    protected virtual void Start()
    {
        myColliderName = gameObject.name;
        myStateHandler = GetComponentInParent<StateHandler>();
        myStatus = GetComponentInParent<CharacterStatus>();

        enemyStateHandler = GameObject.Find(myStateHandler.GetEnemyName()).GetComponent<StateHandler>();
        enemyStatus =  GameObject.Find(myStateHandler.GetEnemyName()).GetComponent<CharacterStatus>();
    }

    //On hit detection, determines what should take damage (health, block).
    //Adds energy to both characters. 
    //Updates recentlyHit variable for character hit.
    protected virtual void OnTriggerEnter(Collider collider)
    {   
        
        enemyColliderName = collider.gameObject.name;
        
        if(enemyStateHandler) {
            if(damageCollidersNames.Contains(enemyColliderName)) {
                if(enemyStateHandler.attacking) {
                    enemyStateHandler.attacking = false;
                    if(CanTakeDamage(myColliderName, myStateHandler)) {
                        myStatus.TakeDamage(myColliderName, enemyStateHandler.moveUsed);
                        myStateHandler.setHitTime();
                        
                        if(myStateHandler.attacking) {
                            myStateHandler.attacking = false;
                        }
                    } else {
                        myStateHandler.blockBroken = myStatus.DamageBlock(enemyStateHandler.moveUsed);
                    }
                    myStatus.AddEnergy(enemyStateHandler.moveUsed);
                    enemyStatus.AddEnergy(enemyStateHandler.moveUsed);
                }
            }
        }
    }

    //Checks if character can take damage to health.
    protected bool CanTakeDamage(string colliderName, StateHandler stateHandler) {
        if(!stateHandler.blocking) {
            return true;
        } else {
            if(armsCollidersNames.Contains(colliderName)) {
                return false;
            } else if (lowerBodyCollidersNames.Contains(colliderName) && stateHandler.crouching) {
                return false;
            }
        }
        return true;
    }
}
