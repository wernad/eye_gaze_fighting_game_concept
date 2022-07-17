using System.Linq;
using System;
using UnityEngine;

/// <summary>
/// Checks for collisions (hits) with player. Used by CombatAI to determine if AI should attack or not.
/// </summary>
public class EnemyHitDetection : HitDetection
{
    private CombatAI combatAI;

    protected override void Start()
    {
        base.Start();

        combatAI = GetComponentInParent<CombatAI>();
    }

    //Overriding hit detection to monitor if AI's attacks land or get blocked, if and where it got hit itself.
    protected override void OnTriggerEnter(Collider collider)
    {   
        base.OnTriggerEnter(collider);
        
        if(enemyStateHandler) {
            if(damageCollidersNames.Contains(myColliderName)) {
                if(myStateHandler.attacking) {
                    if(CanTakeDamage(enemyColliderName, enemyStateHandler)) {
                        combatAI.SetLastAttackBlocked(false);
                        combatAI.SetLastAttackHit(true);
                    } else {
                        combatAI.SetLastAttackBlocked(true);
                    }
                }
            }
        }
    }
}
