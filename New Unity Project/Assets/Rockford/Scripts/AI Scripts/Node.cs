using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    private int row, col, f, g, h, type;
    private Node parent;

    public Node(int r, int c, int t)
    {
        row = r;
        col = c;
        type = t;
        parent = null;
        //type 0 is traversable, 1 is not
    }

    //mutator methods to set values
    public void SetF()
    {
        f = g + h;
    }
    public void SetG(int value)
    {
        g = value;
    }
    public void SetH(int value)
    {
        h = value;
    }
    public void SetParent(Node n)
    {
        parent = n;
    }

    //accessor methods to get values
    public int GetF()
    {
        return f;
    }
    public int GetG()
    {
        return g;
    }
    public int GetH()
    {
        return h;
    }
    public Node GetParent()
    {
        return parent;
    }
    public int GetRow()
    {
        return row;
    }
    public int GetCol()
    {
        return col;
    }
    public int GetType()
    {
        return type;
    }

    public float GetPosX()
    {
        return (col * 2) + 1;
    }
    public float GetPosZ()
    {
        return (row * 2) + 1;
    }


    public string ToString()
    {
        string toStr = "Node: ".PadRight(6) + (row + "_" + col).PadRight(5) + " F: ".PadRight(4) + ("" + f).PadRight(4)
            + " G: ".PadRight(4) + ("" + g).PadRight(4) + " H: ".PadRight(4) + ("" + h).PadRight(4) + "T: ".PadRight(4) + ("" + type).PadRight(4) + " Parent: ".PadRight(9);
        if (parent == null)
        {
            toStr += "Null".PadRight(4);
        }
        else
        {
            toStr += ("" + parent.GetRow() + "_" + parent.GetCol()).PadRight(5);
        }

        return toStr;
    }
}
