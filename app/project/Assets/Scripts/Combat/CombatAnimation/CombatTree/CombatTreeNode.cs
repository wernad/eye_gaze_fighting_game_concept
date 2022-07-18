// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System;
using System.Collections.Generic;

public class TreeNode {
#nullable enable
        public TreeNode(string name, Action? combatMove) {
            this.name = name;
            this.combatMove = combatMove;
        }

        private Action? combatMove;
#nullable disable

        private string name;
        private Dictionary<int, TreeNode> children = new Dictionary<int, TreeNode>();

        public string GetName() {
            return this.name;
        }

        public Dictionary<int, TreeNode> GetChildren() {
            return this.children;
        }

        public void AddChild(int moveType, TreeNode node) {
            this.children.Add(moveType, node);
        }

        public void Execute() {
            this.combatMove();
        }

        public bool IsCombatMoveSet() {
            if(this.combatMove == null) {
                return false;
            }

            return true;
        }
    }
