using UnityEngine;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Container for attack rig. Uses Unity's Animation Rigging package.
/// </summary>
public class AttackRig : MonoBehaviour
{
    private Rig lightPunchRig;
    private Rig heavyPunchRig;
    private Rig lightKickRig;
    private Rig heavyKickRig;

    //Gets references to all rigs responsible for attack animations.
    protected virtual void Start() {
        lightPunchRig = this.transform.Find("LightPunchRig").GetComponent<Rig>();
        heavyPunchRig = this.transform.Find("HeavyPunchRig").GetComponent<Rig>();
        lightKickRig = this.transform.Find("LightKickRig").GetComponent<Rig>();
        heavyKickRig = this.transform.Find("HeavyKickRig").GetComponent<Rig>();
        lightPunchRig.weight = 0;
        heavyPunchRig.weight = 0;
        lightKickRig.weight = 0;
        heavyKickRig.weight = 0;
    }

    /// <summary>
    /// Returns the rig of arm responsible for light punch.
    /// </summary>
    public Rig GetLightPunchRig() {
        return lightPunchRig;
    }

    /// <summary>
    /// Returns the rig of arm responsible for heavy punch.
    /// </summary>
    public Rig GetHeavyPunchRig() {
        return heavyPunchRig;
    }

    /// <summary>
    /// Returns the rig of leg responsible for light kick.
    /// </summary>
    public Rig GetLightKickRig() {
        return lightKickRig;
    }

    /// <summary>
    /// Returns the rig of leg responsible for heavy kick.
    /// </summary>
    public Rig GetHeavyKickRig() {
        return heavyKickRig;
    }    
}
