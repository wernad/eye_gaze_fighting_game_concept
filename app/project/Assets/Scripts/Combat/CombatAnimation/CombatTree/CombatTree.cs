// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

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

    //Tries to find and set next node on tree, 
    //if no node is found on current node that's not root, 
    //searches root node, if no node is found, sets current node to root.
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

    public TreeNode AddNode(string name, TreeNode parent, int moveType, Action combatMove) {
        TreeNode newNode = new TreeNode(name, combatMove);
        parent.AddChild(moveType, newNode);
        return newNode;
    }

    public void AddMove(int moveType) {
        this.moveQueue.Enqueue(moveType);
    }

    public TreeNode GetRoot() {
        return this.root;
    }

    //Used for debug to check proper tree structure.
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
