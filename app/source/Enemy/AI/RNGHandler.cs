using System;
using UnityEngine;

/// <summary>
/// Contains method for determining certain degree of randomness of AI's actions.
/// </summary>
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

    /// <summary>
    /// Decides if AI should use special attack or not.
    /// </summary>
    /// <remarks>
    /// Special attack chance is increased on false.
    public bool ShouldUseSpecialAttack() {
        if(GenerateRandomFloat() + specialChanceModifier >= useSpecialThreshold) {
            specialChanceModifier = 0;
            return true;
        }

        specialChanceModifier += specialAttackWeight;
        return false;
    }

    /// <summary>
    /// Decides if AI should use combo attack or not.
    /// </summary>
    /// <remarks>
    /// If not, simple attack is used instead. Combo attack chance is increased on false.
    /// </remarks>
    public bool ShouldUseComboAttack() {
        if(GenerateRandomFloat() + comboChanceModifier >= useComboThreshold) {
            comboChanceModifier = 0;
            return true;
        }

        comboChanceModifier += comboAttackWeight;
        return false;
    }

    /// <summary>
    /// Decides which simple attack to use.
    /// </summary>
    /// <param name="preferedTarget">Determines what attack types to pick from. 0 is all, 1 is punches, 2 is kicks.</param>
    public int GetRandomSimpleAttack(int preferedTarget = 0) {
        if(preferedTarget == 0) {
            return RNG.Next(0,4);
        } else if(preferedTarget == 1) {
            return RNG.Next(0,2);
        }

        return RNG.Next(2,4);
    }

    /// <summary>
    /// Decides which combo to use.
    /// </summary>
    public int[] GetRandomCombo() {
        return comboAttacks[RNG.Next(0,comboAttacks.Length)];
    }

    /// <summary>
    /// Returns a random number between 0 and max.
    /// </summary>
    /// <param name="max">Maximum number to be returned.</param>
    public int GetRandomInt(int maxRange) {
        return RNG.Next(0, maxRange);
    }

    /// <summary>
    /// Returns a random boolean.
    /// </summary>
    public bool GetRandomBoolean() {
        return RNG.Next(0, 2) == 0;
    }
}
