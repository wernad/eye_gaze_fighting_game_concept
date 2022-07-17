using System;
using System.Collections.Generic;

/// <summary>
/// Represents node of CombatTree. Contains name of the node, move type it represents and Action to call when node is processed. Also contains children nodes, if any.
/// </summary>
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

        /// <summary>
        /// Get name of the node.
        /// </summary>
        public string GetName() {
            return this.name;
        }

        /// <summary>
        /// Get children of the node.
        /// </summary>
        public Dictionary<int, TreeNode> GetChildren() {
            return this.children;
        }

        /// <summary>
        /// Add child to the node.
        /// </summary>
        /// <param name="moveType">Type of move a new child node represents.</param>
        /// <param name="node">The child to add.</param>
        public void AddChild(int moveType, TreeNode node) {
            this.children.Add(moveType, node);
        }

        /// <summary>
        /// Process the Action of the node.
        /// </summary>
        public void Execute() {
            this.combatMove();
        }

        /// <summary>
        /// Check if Action is set.
        /// </summary>
        public bool IsCombatMoveSet() {
            if(this.combatMove == null) {
                return false;
            }

            return true;
        }
    }
