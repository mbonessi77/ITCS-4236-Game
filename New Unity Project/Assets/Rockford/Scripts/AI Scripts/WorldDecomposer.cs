using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDecomposer : MonoBehaviour {

    //array for world data
	private int [,] worldData;
	
    //size of nodes to decompose
    [SerializeField]
    private float nodeSize;
    private float nodeCenterOffset;

    //the width and length of the terrain area
    [SerializeField]
    private int terrainWidth;
	[SerializeField]
    private int terrainLength;

    //size of terrain in nodes
	private int rows;
	private int cols;

	void Start()
    {
        //set the number of node rows and columns
		rows = (int)(terrainLength / nodeSize);
		cols = (int)(terrainWidth / nodeSize);

        //create the array for world data
		worldData = new int [rows, cols];

        //where the ray should be created related to each node location
        nodeCenterOffset = nodeSize / 2f;

        //initially decompose the world
        Invoke("DecomposeWorld", 1f);
	}

    public void DecomposeWorld()
    {
        //where to start the decomposition
		float startX = 0;
		float startZ = 0;

        //decompose each node
		for(int row = 0; row < rows; row++)
        {
			for(int col = 0; col < cols; col++)
            {
                //center the raycast
				float x = startX + nodeCenterOffset + (nodeSize * col);
				float z = startZ + nodeCenterOffset + (nodeSize * row);

                //create raycast start point above the desired location
				Vector3 startPos = new Vector3 (x, 20f, z);

				//does our raycast hit anything at this point in the map
				RaycastHit hit;

                //bit shift the index of the layer (8) and (9) to get two bit masks
                int layerMask0 = 1 << 12;
				int layerMask1 = 1 << 8;
                int layerMask2 = 1 << 9;
                int layerMask3 = 1 << 10;

                //this would cast rays only against colliders in layer 8 or against layer 9.
                //but instead we want to collide against everything except layer 8 and layer 9. The ~ operator does this, it inverts a bitmasks.
                //int layerMask4 = ~(layerMask1 | layerMask2 | layerMask3);
                int layerMask4 = ~(layerMask1 | layerMask2 | layerMask3 | layerMask0);
                int layerMask5 = (layerMask1 | layerMask2 | layerMask3);

                //does the ray intersect any objects not on the Ground, Player, or AI layer
                if (Physics.Raycast(startPos, Vector3.down, out hit, Mathf.Infinity, layerMask4))
                {
					//print("Hit something at row: " + row + " col: " + col);
					//Debug.DrawRay (startPos, Vector3.down * 20, Color.red, 50000);
					worldData [row, col] = 1;
				}
                else if (Physics.Raycast(startPos, Vector3.down, out hit, Mathf.Infinity, layerMask5))
                {
                    //else if it intersects with Ground, Player or AI layer
					Debug.DrawRay (startPos, Vector3.down * 20, Color.green, 50000);
					worldData [row, col] = 0;
				}
                else if (Physics.Raycast(startPos, Vector3.down, out hit, Mathf.Infinity, layerMask0))
                {
                    //print("Hit something at row: " + row + " col: " + col);
                    //Debug.DrawRay (startPos, Vector3.down * 20, Color.red, 50000);
                    worldData[row, col] = 1;
                }
                else
                {
                    //else it hits nothing
                    //print("Hit nothing at row: " + row + " col: " + col);
                    //Debug.DrawRay (startPos, Vector3.down * 20, Color.red, 50000);
                    worldData[row, col] = 1;
                }
			}
		}
	}

    //give access to world data through function call
    public int GetWorldData(int r, int c)
    {
        //return desired node data
        return worldData[r, c];
    }

    //accessor methods to get values
    public int GetRows()
    {
        return rows;
    }

    public int GetCols()
    {
        return cols;
    }

    public float GetNodeSize()
    {
        return nodeSize;
    }
}
