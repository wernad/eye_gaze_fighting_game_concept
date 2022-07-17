using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Keeps check of body parts hit, damage taken and if damage was taken or blocked. Attached to AI.
/// </summary>
public class EnemyStatus : CharacterStatus
{
    private float lastHealth;
    private float lastBlock;    
    private string lastBodyPartHit;    
    private List<Tuple<int, float>> recentDamageTaken = new List<Tuple<int, float>>();
    private float timeWindow = 2f;

    protected override void Start() {
        base.Start();
        lastHealth = maxHealth;
        lastBlock = maxBlock;
    }

    /// <summary>
    /// AI uses this method to monitor how much damage it has taken in last 2 seconds.
    /// </summary>
    public override void TakeDamage(string partHit, string moveName) {
        lastBodyPartHit = partHit;

        base.TakeDamage(partHit, moveName);

        int damageTaken = (int)(combatMovesDamageValues[moveName] * bodyPartsDamageModifiers[partHit]);
        recentDamageTaken.Add(new Tuple<int, float>(damageTaken, Time.fixedTime));
    }

    /// <summary>
    /// Updates list of recent hits.
    /// </summary>
    public void RemoveOldSources() {
        if(recentDamageTaken.Count > 0 && (Time.fixedTime - recentDamageTaken.ElementAt(0).Item2) <= timeWindow) {
            recentDamageTaken = recentDamageTaken.Where(source => (Time.fixedTime - source.Item2) <= timeWindow).ToList();
        }
    }

    /// <summary>
    /// Returns total damage taken in last 2 seconds.
    /// </summary>
    public int GetDamageTaken() {
        RemoveOldSources();
        List<int> values = new List<int>();
        foreach(Tuple<int, float> source in recentDamageTaken) {
            values.Add(source.Item1);
        }

        return values.Sum();
    }

    /// <summary>
    /// Remove recent hits older than 2 seconds.
    /// </summary>
    public void ClearDamageSources() {
        recentDamageTaken.Clear();
    }

    /// <summary>
    /// Get health points since last update.
    /// </summary>
    public float GetLastHealth() {
        return lastHealth;
    }

    /// <summary>
    /// Get block points since last update.
    public float GetLastBlock() {
        return lastBlock;
    }

    /// <summary>
    /// Get last body part hit.
    /// </summary>
    public string GetBodyPartHit() {
        return lastBodyPartHit;
    }

    /// <summary>
    /// Update last health to current health.
    /// </summary>
    public void UpdateLastHealth() {
        lastHealth = GetCurrentHealth();
    }
}
