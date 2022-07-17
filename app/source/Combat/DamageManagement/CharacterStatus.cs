using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains health, energy and block values for a character. Calculates how much damage should character take based on attack type and body part hit.
/// </summary>
public class CharacterStatus : MonoBehaviour
{
    private GameControl gameControl;
    private StateHandler stateHandler;
    private int health, energy;
    private int blockRechargeRate;
    private int defaultRechargeRate = 5;
    private int fastRechargeRate = 30;
    private float block;
    private bool blockRecharging = false;

    protected int maxHealth = 200, maxBlock = 100, maxEnergy = 100;

    protected Dictionary<string, int> combatMovesDamageValues = new Dictionary<string, int> {
        //Special moves
        {"DashPunch", 40},
        //{"RisingPunch", 35},
        
        //Combo moves
        {"HeavyHook", 25},
        {"LightHook", 20},
        {"HeavyUppercut", 20},
        {"LowerUppercut", 15},

        //Simple moves
        {"HeavyKick", 15},
        {"LightKick", 10},
        {"HeavyPunch", 10},
        {"LightPunch", 5},
    };

    protected Dictionary<string, float> bodyPartsDamageModifiers = new Dictionary<string, float> {
        {"Head", 1.5f},
        {"Neck", 1.4f},
        {"Torso", 1.2f},
        {"RUpperarm", 1f},
        {"LUpperarm", 1f},
        {"RForearm", 1.1f},
        {"LForearm", 1.1f},
        {"RHand", 1f},
        {"LHand", 1f},
        {"RThigh", 1f},
        {"LThigh", 1f},
        {"RShin", 1.1f},
        {"LShin", 1.1f},
        {"RFoot", 1f},
        {"LFoot", 1f},
    };

    protected virtual void Start()
    {
        gameControl = GameObject.Find("GameManager").GetComponent<GameControl>();
        stateHandler = GetComponent<StateHandler>();
        ResetCharacter();
    }

    //Periodically check if block needs to be recharged.
    void Update() {
        RechargeBlock();
    }

    //Used for reseting character during training mode.
    private void SetValuesToMax() {
        health = maxHealth;
        block = maxBlock;
        energy = maxEnergy;

        gameControl.UpdateHealthBar(health, gameObject.name);
        gameControl.UpdateBlockBar(block, gameObject.name);
        gameControl.UpdateEnergyBar(energy, gameObject.name);
    }

    //Checks if block needs recharging. 
    //Recharge speed depends on block left during check. 
    //No block left sets maximum recharge speed.
    private void RechargeBlock() {
        if(stateHandler.recentlyHit) {
            if(blockRecharging) {
                blockRecharging = false;
            }
            
            return;
        }

        if(!blockRecharging) {
            if(block < maxBlock) {
                if(block == 0) {
                    blockRechargeRate = fastRechargeRate;
                } else {
                    blockRechargeRate = defaultRechargeRate;
                }

                blockRecharging = true;
            }
        } else {
            block += blockRechargeRate * Time.deltaTime;
            gameControl.UpdateBlockBar(block, gameObject.name);

            if(block >= maxBlock) {
                block = maxBlock;
                blockRecharging = false;
            }
        }
    }

    /// <summary>
    /// Take health damage and update health bar accordingly. Damage taken is based on move used and body part hit.
    /// </summary>
    /// <param name="partHit">Name of the body part hit.</param>
    /// <param name="moveName">Name of the move that hit the character.</param>
    public virtual void TakeDamage(string partHit, string moveName) {
        health -= (int)(combatMovesDamageValues[moveName] * bodyPartsDamageModifiers[partHit]);
        gameControl.UpdateHealthBar(health, gameObject.name);
        
        if(health <= 0)
        {
            health = maxHealth;
        }
    }

    /// <summary>
    /// Take block damage and update block bar accordingly. Damage is based on move used.
    /// </summary>
    /// <param name="moveName">Name of the move that hit the character.</param>
    public bool DamageBlock(string moveName) {
        block -= combatMovesDamageValues[moveName];
        gameControl.UpdateBlockBar(block, gameObject.name);

        if(block <= 0) {
            return true;
        }  

        return false;
    }

    /// <summary>
    /// Add energy and update energy bar.
    /// </summary>
    /// <param name="moveName">Name of the move that generated energy.</param>
    public void AddEnergy(string moveName) {
        energy += (int) (combatMovesDamageValues[moveName] / 2);
        if(energy > maxEnergy) {
            energy = maxEnergy;
        }
        
        gameControl.UpdateEnergyBar(energy, gameObject.name);
    }

    /// <summary>
    /// Use energy on special attack. Update energy bar.
    /// </summary>
    /// <param name="cost">Amount of energy used up.</param>
    public void UseEnergy(int cost) {
        energy -= cost;
    
        if(energy < 0) {
            energy = 0;
        }

        gameControl.UpdateEnergyBar(energy, gameObject.name);
    }
    
    /// <summary>
    /// Get current health.
    /// </summary>
    public float GetCurrentHealth() {
        return health;
    }

    /// <summary>
    /// Get current Energy.
    /// </summary>
    public int GetCurrentEnergy() {
        return energy;
    }

    /// <summary>
    /// Reset characters health, block and energy values.
    /// </summary>
    public void ResetCharacter() {
        health = maxHealth;
        block = maxBlock;
        energy = 0;
    }
}
