// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    //AI overrides this method to monitor how much damage it has taken in last 2 seconds.
    public override void TakeDamage(string partHit, string moveName) {
        lastBodyPartHit = partHit;

        base.TakeDamage(partHit, moveName);

        int damageTaken = (int)(combatMovesDamageValues[moveName] * bodyPartsDamageModifiers[partHit]);
        recentDamageTaken.Add(new Tuple<int, float>(damageTaken, Time.fixedTime));
    }

    //Updates list of recent hits.
    public void RemoveOldSources() {
        if(recentDamageTaken.Count > 0 && (Time.fixedTime - recentDamageTaken.ElementAt(0).Item2) <= timeWindow) {
            recentDamageTaken = recentDamageTaken.Where(source => (Time.fixedTime - source.Item2) <= timeWindow).ToList();
        }
    }

    public int GetDamageTaken() {
        RemoveOldSources();
        List<int> values = new List<int>();
        foreach(Tuple<int, float> source in recentDamageTaken) {
            values.Add(source.Item1);
        }

        return values.Sum();
    }

    public void ClearDamageSources() {
        recentDamageTaken.Clear();
    }

    public float GetLastHealth() {
        return lastHealth;
    }

    public float GetLastBlock() {
        return lastBlock;
    }

    public string GetBodyPartHit() {
        return lastBodyPartHit;
    }

    public void UpdateLastHealth() {
        lastHealth = GetCurrentHealth();
    }
}
