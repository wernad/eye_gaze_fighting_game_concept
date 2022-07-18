// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;

public class StateHandler : MonoBehaviour
{   
    private float lastTimeHit;
    private float hitDuration = 0.25f;
    public bool dashing { get; set; } = false;
    public bool crouching { get; set; } = false;
    public bool grounded { get; set; } = true;
    public bool blocking { get; set; } = false;
    public bool attacking { get; set; } = false;
    public string moveUsed { get; set; } = "";
    public bool recentlyHit { get; set; } = false;
    public bool blockBroken { get; set; } = false;
    private bool onLeftSide;
    private Transform enemy;
    private string enemyName;

    //Get reference to the oponent's gameobject on this projects creation.
    void Awake() {
        if(gameObject.name == "Player") {
            enemyName = "Enemy";
            enemy = GameObject.Find(enemyName).GetComponent<Transform>();
        } else {
            enemyName = "Player";
            enemy = GameObject.Find(enemyName).GetComponent<Transform>();
        }

        AmIOnLeftSide();
    }

    //Periodically check who is on left side and if this character was recently hit by an attack.
    void Update() {
        AmIOnLeftSide();
        
        if(recentlyHit) {
            if(Time.time > lastTimeHit + hitDuration) {
                recentlyHit = false;
            }
        }
    }

    void AmIOnLeftSide() {
        onLeftSide = gameObject.transform.position.x < enemy.position.x;
    }

    public void setHitTime() {
        recentlyHit = true;
        lastTimeHit = Time.time;
    }

    public Vector3 GetEnemyPosition() {
        return enemy.position;
    }

    public bool GetIfOnLeftSide() {
        return onLeftSide;
    }

    public string GetEnemyName() {
        return enemyName;
    }
}
