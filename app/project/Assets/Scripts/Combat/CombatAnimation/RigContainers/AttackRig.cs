// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;
using UnityEngine.Animations.Rigging;

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

    public Rig GetLightPunchRig() {
        return lightPunchRig;
    }

    public Rig GetHeavyPunchRig() {
        return heavyPunchRig;
    }

    public Rig GetLightKickRig() {
        return lightKickRig;
    }

    public Rig GetHeavyKickRig() {
        return heavyKickRig;
    }    
}
