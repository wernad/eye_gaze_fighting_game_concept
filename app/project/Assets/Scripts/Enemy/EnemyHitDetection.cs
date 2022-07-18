// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System.Linq;
using UnityEngine;
using System;

public class EnemyHitDetection : HitDetection
{
    private CombatAI combatAI;

    protected override void Start()
    {
        base.Start();

        combatAI = GetComponentInParent<CombatAI>();
    }

    //AI's overriding hit detection to monitor if it's attacks land, get blocked and where it got hit itself.
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
