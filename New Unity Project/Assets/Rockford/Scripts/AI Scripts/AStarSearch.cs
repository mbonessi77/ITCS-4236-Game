using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarSearch : MonoBehaviour {

    //min Heap for Open List
    private PriorityQueue openList = new PriorityQueue(10000);

    //Dictionary for Closed List [Dictionary<Key, Value>] Key = what we search for, Value = what is stored ((using string as key, for the board space[r, c]))
    private Dictionary<string, Node> closedList = new Dictionary<string, Node>(10000);

    //the 2D array to hold the terrain nodes
    private Node[,] terrainSpace;
    private int rows;
    private int cols;

    //stack used to hold and calculate list version of path
    private Stack<Node> pathway = new Stack<Node>();

    //world decomposition script
    [SerializeField]
    private WorldDecomposer worldDecomp;

    //leader component variables
    [SerializeField]
    [Header("Attach character's transform")]
    private Transform trans;

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        //CalulatePath(trans.position.x, trans.position.z);
    }

    public Stack<Node> CalulatePath(float startVecX, float startVecZ, float goalVecX, float goalVecZ)
    {
        rows = worldDecomp.GetRows();
        cols = worldDecomp.GetCols();
        terrainSpace = new Node[rows, cols];

        //reset list to be empty
        openList = new PriorityQueue(10000);
        closedList = new Dictionary<string, Node>(10000);
        pathway = new Stack<Node>();

        //create nodes and each tile
        for (int r = 0; r < rows; r++)
        {
            //each row
            for (int c = 0; c < cols; c++)
            {
                //generate nodes with correct row, column, and pathability (blocked or not)
                terrainSpace[r, c] = new Node(r, c, worldDecomp.GetWorldData(r, c));
            }
        }

        int startRow = Mathf.FloorToInt(Mathf.CeilToInt(startVecZ / worldDecomp.GetNodeSize()) * worldDecomp.GetNodeSize()) / 2;
        int startCol = Mathf.FloorToInt(Mathf.CeilToInt(startVecX / worldDecomp.GetNodeSize()) * worldDecomp.GetNodeSize()) / 2;
        int goalRow = Mathf.FloorToInt(Mathf.CeilToInt(goalVecZ / worldDecomp.GetNodeSize()) * worldDecomp.GetNodeSize()) / 2;
        int goalCol = Mathf.FloorToInt(Mathf.CeilToInt(goalVecX / worldDecomp.GetNodeSize()) * worldDecomp.GetNodeSize()) / 2;

        //assign null to allow enterance to while loop
        Node currentSpace = null;

        //print(worldDecomp.GetRows() + " " + worldDecomp.GetCols());
        //print(rows + " " + cols);
        //print(terrainSpace.Length);
        
        //add starting tile to openList for first movement turn
        openList.Add(terrainSpace[startRow, startCol]);

        //print("rows: " + rows + " cols: " + cols + " gRow: " + goalRow + " gCol: " + goalCol);
        //While not at goal or not completely searched
        while (currentSpace != terrainSpace[goalRow, goalCol] && openList.GetSize() != 0)
        {
            //pop off best move
            currentSpace = openList.Remove();

            //add new node to closedList
            string addKey = "" + currentSpace.GetRow() + "_" + currentSpace.GetCol();
            closedList.Add(addKey, currentSpace);

            //calculating moves [rows]
            for (int r = 1; r < 4; r++)
            {
                //calculating moves [columns]
                for (int c = 1; c < 4; c++)
                {

                    //temp string key for assuring the space to be checked is not already in the closedList
                    string key = "" + (currentSpace.GetRow() - 2 + r) + "_" + (currentSpace.GetCol() - 2 + c);

                    //if space on terrain/is in bounds
                    if ((currentSpace.GetRow() - 2 + r) > -1 && (currentSpace.GetRow() - 2 + r) < rows && (currentSpace.GetCol() - 2 + c) > -1 && (currentSpace.GetCol() - 2 + c) < cols)
                    {

                        //node variable to shorten code
                        Node potentialNode = terrainSpace[currentSpace.GetRow() - 2 + r, currentSpace.GetCol() - 2 + c];

                        //if space is not in the closedList and is traversable type
                        if (potentialNode.GetType() == 0 && !closedList.ContainsKey(key))
                        {

                            //set heuristic value with manhattan method
                            int tempH = (Mathf.Abs(potentialNode.GetRow() - goalRow) + Mathf.Abs(potentialNode.GetCol() - goalCol)) * 10;
                            potentialNode.SetH(tempH);

                            //calculate cost to get to space from start through this space
                            int tempG = currentSpace.GetG();
                            //if a diagonal move
                            if ((r == 1 && c == 1) || (r == 1 && c == 3) || (r == 3 && c == 1) || (r == 3 && c == 3))
                            {
                                tempG += 14;
                            }
                            else
                            {
                                //else adjacent move
                                tempG += 10;
                            }

                            //if moving through this space is better than previous or has not been assigned, reassign/assign cost and parent
                            if (potentialNode.GetG() > tempG || potentialNode.GetG() == 0)
                            {
                                potentialNode.SetG(tempG);
                                potentialNode.SetParent(currentSpace);
                            }

                            //set the F value of the space being checked
                            potentialNode.SetF();

                            //if node is not in openlist already add it to the openlist
                            if (openList.search(potentialNode) == false)
                            {
                                openList.Add(potentialNode);
                            }
                            else
                            {
                                //else already in list, resort the openlist with new values
                                openList.Resort(potentialNode);
                            }
                        }
                    }
                }
            }
        }

        //set goal node as first pathpoint to generate pathway backwards
        Node pathwayNode = terrainSpace[goalRow, goalCol];

        //while not on starting position
        while (pathwayNode.GetParent() != null)
        {
            //add to pathway
            pathway.Push(pathwayNode);

            //assign new pathwayNode
            pathwayNode = pathwayNode.GetParent();
        }

        //get starting tile as part of pathway
        pathway.Push(pathwayNode);

        //print(pathway.Count);

        //return best generated pathway
        return pathway;
    }
}
