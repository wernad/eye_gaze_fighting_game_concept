using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tree structure used to determine what move to make depending on the current node and input. Root node has no Action set.
/// </summary>
public class CombatTree
{
    public CombatTree(StateHandler stateHandler) {
        this.stateHandler = stateHandler;
        this.currentNode = this.root;
    }
    
    private StateHandler stateHandler;
    private TreeNode root = new TreeNode("root", null);
    private TreeNode currentNode;
    private Queue<int> moveQueue = new Queue<int>();
    private float lastExecutionTime = 0;
    private float chainingTimeout = 1f;

    //Performs current node if it contains action to perform. Stores name of the move performed.
    void ProcessNode(TreeNode node) {         
        currentNode = node;
        stateHandler.moveUsed = currentNode.GetName();
    
        if(currentNode.IsCombatMoveSet()) {
            currentNode.Execute();
        }
    
        lastExecutionTime = Time.time;
    }

    /// <summary>
    /// Moves to the next node in the tree based on input. 
    /// If input leads nowhere, current node is set to root node and method tries to search next node again. 
    /// If not found, nothing happens.
    /// </summary>
    public void ProcessNextNode() {
        if(!stateHandler.grounded) {
            currentNode = root;
        }

        if(Time.time > lastExecutionTime + chainingTimeout) {
            currentNode = root;
        }

        if(moveQueue.Count > 0) {
            int currentMove;
            TreeNode foundNode;

            currentMove = moveQueue.Peek();
            
            if(currentNode.GetChildren().TryGetValue(currentMove, out foundNode)) {
                ProcessNode(foundNode);
            } else if(currentNode.GetName() != "root") {
                currentNode = root;
                
                if(currentNode.GetChildren().TryGetValue(currentMove, out foundNode)) {
                    ProcessNode(foundNode);
                }
            } 
            
            moveQueue.Dequeue();
        }
    }   

    /// <summary>
    /// Adds child to the parent.
    /// </summary>
    /// <param name="name">Name of the child node.</param>
    /// <param name="parent">Parent node.</param>
    /// <param name="moveType">Move type child node represents.</param>
    /// <param name="combatMove">Action which is called when child node is processed.</param>
    public TreeNode AddNode(string name, TreeNode parent, int moveType, Action combatMove) {
        TreeNode newNode = new TreeNode(name, combatMove);
        parent.AddChild(moveType, newNode);
        return newNode;
    }

    /// <summary>
    /// Adds input to tree queue.
    /// </summary>
    public void AddMove(int moveType) {
        this.moveQueue.Enqueue(moveType);
    }

    /// <summary>
    /// Returns root node.
    /// </summary>
    public TreeNode GetRoot() {
        return this.root;
    }

    /// <summary>
    /// Prints tree structure. 
    /// </summary>
    /// <remarks>
    /// Used for debugging.
    /// </remarks>
    public void PrintAll(TreeNode node) {
        if(node.GetChildren().Count == 0) {
            return;
        }
        Debug.Log(node.GetName() + ": " + String.Join(" ", node.GetChildren().Select(element => string.Format("{0},", element.Value.GetName()))));
        foreach(KeyValuePair<int, TreeNode> kv in node.GetChildren()) {
            PrintAll(kv.Value);
        }
    }
}
