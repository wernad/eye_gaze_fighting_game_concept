// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;

public class RNGHandler : MonoBehaviour
{
    private int[][] comboAttacks = new int[][] {
        new int[] {CombatUtilities.HeavyPunch, CombatUtilities.LightPunch, CombatUtilities.HeavyPunch},
        new int[] {CombatUtilities.LightPunch, CombatUtilities.HeavyPunch, CombatUtilities.LightPunch, CombatUtilities.HeavyPunch},
    };

    private float useComboThreshold = 0.5f;
    private float useSpecialThreshold = 0.8f;
    private float comboAttackWeight = 0.3f;
    private float specialAttackWeight = 0.4f;
    private float comboChanceModifier = 0f;
    private float specialChanceModifier = 0f;
    private System.Random RNG = new System.Random();

    private float GenerateRandomFloat() {
        return (float)RNG.NextDouble();
    }

    public bool ShouldUseSpecialAttack() {
        if(GenerateRandomFloat() + specialChanceModifier >= useSpecialThreshold) {
            specialChanceModifier = 0;
            return true;
        }

        specialChanceModifier += specialAttackWeight;
        return false;
    }

    public bool ShouldUseComboAttack() {
        if(GenerateRandomFloat() + comboChanceModifier >= useComboThreshold) {
            comboChanceModifier = 0;
            return true;
        }

        comboChanceModifier += comboAttackWeight;
        return false;
    }

    public int GetRandomSimpleAttack(int preferedTarget = 0) {
        if(preferedTarget == 0) {
            return RNG.Next(0,4);
        } else if(preferedTarget == 1) {
            return RNG.Next(0,2);
        }

        return RNG.Next(2,4);
        
    }

    public int[] GetRandomCombo() {
        return comboAttacks[RNG.Next(0,comboAttacks.Length)];
    }

    public int GetRandomInt(int maxRange) {
        return RNG.Next(0, maxRange);
    }

    public bool GetRandomBoolean() {
        return RNG.Next(0, 2) == 0;
    }
}
