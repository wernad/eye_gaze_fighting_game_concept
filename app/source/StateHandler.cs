using UnityEngine;

/// <summary>
/// Contains all the necessary information about character's state (crouching, blocking, etc.), including oponents Transform and name.
/// </summary>
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

    /// <summary>
    /// Sets the character's state to recently hit and updates the time of last hit.
    /// </summary>
    public void setHitTime() {
        recentlyHit = true;
        lastTimeHit = Time.time;
    }

    /// <summary>
    /// Returns the character's position.
    /// </summary>
    public Vector3 GetEnemyPosition() {
        return enemy.position;
    }

    /// <summary>
    /// Returns bool value if character is on left side of the screen.
    /// </summary>
    public bool GetIfOnLeftSide() {
        return onLeftSide;
    }

    /// <summary>
    /// Returns enemy's object name.
    /// </summary>
    public string GetEnemyName() {
        return enemyName;
    }
}
