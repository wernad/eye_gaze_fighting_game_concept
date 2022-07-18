// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using UnityEngine;

public class Combat : MonoBehaviour
{
    private StateHandler stateHandler;
    private CombatTree combatTree;
    private SimpleAttacks simpleAttacks;
    private ComboAttacks comboAttacks;
    private SpecialAttacks specialAttacks;

    //Gets references to all attack types and creates CombatTree.
    void Start() {
        stateHandler = GetComponent<StateHandler>();
        simpleAttacks = GetComponent<SimpleAttacks>();
        comboAttacks = GetComponent<ComboAttacks>();
        specialAttacks = GetComponent<SpecialAttacks>();
        
        InitCombatTree();
    }

    void Update() {
        combatTree.ProcessNextNode();
    }

    public void AddMove(int moveType) {
        combatTree.AddMove(moveType);
    }

    //Creates CombatTree with all attacks, combos and special attacks.
    private void InitCombatTree() {
        combatTree = new CombatTree(stateHandler);
        TreeNode root = combatTree.GetRoot();

        TreeNode temp;

        //Light punch -> Heavy hook -> Light hook -> Heavy hook
        temp = combatTree.AddNode("LightPunch", root, CombatUtilities.LightPunch, simpleAttacks.LightPunch);
        temp = combatTree.AddNode("HeavyHook", temp, CombatUtilities.HeavyPunch, comboAttacks.HeavyHook);
        temp = combatTree.AddNode("LightHook", temp, CombatUtilities.LightPunch, comboAttacks.LightHook);
        temp = combatTree.AddNode("HeavyHook", temp, CombatUtilities.HeavyPunch, comboAttacks.HeavyHook);

        //Heavy punch -> Light hook -> Heavy lower hook
        temp = combatTree.AddNode("HeavyPunch", root, CombatUtilities.HeavyPunch, simpleAttacks.HeavyPunch);
        temp = combatTree.AddNode("LightHook", temp, CombatUtilities.LightPunch, comboAttacks.LightHook);
        temp = combatTree.AddNode("HeavyUppercut", temp, CombatUtilities.HeavyPunch, comboAttacks.HeavyUppercut);

        //Light kick
        temp = combatTree.AddNode("LightKick", root, CombatUtilities.LightKick, simpleAttacks.LightKick);

        //Heavy kick
        temp = combatTree.AddNode("HeavyKick", root, CombatUtilities.HeavyKick, simpleAttacks.HeavyKick);

        //Backward -> Forward -> Special dash attack
        temp = combatTree.AddNode("Backward", root, CombatUtilities.Backward, null);
        temp = combatTree.AddNode("Forward", temp, CombatUtilities.Forward, null);
        temp = combatTree.AddNode("DashPunch", temp, CombatUtilities.LightPunch, specialAttacks.DashPunch);
    }
}
