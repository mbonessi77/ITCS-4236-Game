using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue {

    private Node[] heap;
    private int size;

    public PriorityQueue()
    {
        heap = new Node[10000];
        size = 0;
    }

    public PriorityQueue(int s)
    {
        heap = new Node[s];
        size = 0;
    }

    public int GetSize()
    {
        return size;
    }

    public Node Peek()
    {
        return heap[1];
    }

    public bool IsEmpty()
    {
        return size == 0;
    }

    public void Add(Node c)
    {

        //make sure the heap isnt full
        if (size + 2 > heap.Length)
        {
            return;
        }

        //increase the size
        size++;

        //add new node to the next open position in the heap
        heap[size] = c;

        //create an index variable to keep track of where our node is in the heap
        int index = size;

        //continue to compare the node to its parents until it is the a child of the root node
        while (index > 1)
        {

            //get parent nodes index
            int parentIndex = index / 2;

            //compare new nodes F value to its parents to see if it needs to be swapped
            if (heap[index].GetF() < heap[parentIndex].GetF())
            {
                //swap nodes
                Node temp = heap[index];
                heap[index] = heap[parentIndex];
                heap[parentIndex] = temp;

                //update index to parents index after swap
                index = parentIndex;
            }
            else
            {
                //parent nodes f value is lower, no swap needed
                break;
            }
        }
    }

    public Node Remove()
    {

        //make sure the heap isnt empty
        if (IsEmpty())
        {
            return null;
        }

        //store temporary reference to root node, so we can we return it at the end
        Node temp = heap[1];

        //move node in the last position to the root
        heap[1] = heap[size];
        heap[size] = null;
        size--;

        //store the index of the node we moved to the root
        int index = 1;

        //continue to compare index nodes f value to its childrens as long as there are children
        while (index <= size / 2)
        {

            //store index of child nodes
            int leftChildIndex = index * 2;
            int rightChildIndex = leftChildIndex + 1;

            //store f values of child nodes
            int leftChildFValue = heap[leftChildIndex].GetF();
            //backup: if there is no right child node, it will always be a higher f value than the left
            int rightChildFValue = leftChildFValue + 1;

            //if there is a right child, get its actual f value
            if (rightChildIndex <= size)
            {
                rightChildFValue = heap[rightChildIndex].GetF();
            }

            //determine the lower f value of the two children
            int lowerFValue;
            int lowerIndex;

            if (rightChildFValue < leftChildFValue)
            {
                lowerFValue = rightChildFValue;
                lowerIndex = rightChildIndex;
            }
            else
            {
                lowerFValue = leftChildFValue;
                lowerIndex = leftChildIndex;
            }

            //determine if a swap should be made with the parent node and the lower priority child
            if (heap[index].GetF() > lowerFValue)
            {

                //swap
                Node swap = heap[index];
                heap[index] = heap[lowerIndex];
                heap[lowerIndex] = swap;

                //update the index since it was moved to a child position
                index = lowerIndex;
            }
            else
            {//parent f value is lower, no need to swap
                break;
            }
        }

        //return the original root node
        return temp;
    }

    public void Resort(Node n)
    {

        int index = SearchForIndex(n);

        //continue to compare the node to its parents until it is the a child of the root node
        while (index > 1)
        {

            //get parent nodes index
            int parentIndex = index / 2;

            //compare new nodes F value to its parents to see if it needs to be swapped
            if (heap[index].GetF() < heap[parentIndex].GetF())
            {

                //swap nodes
                Node temp = heap[index];
                heap[index] = heap[parentIndex];
                heap[parentIndex] = temp;

                //update index to parents index after swap
                index = parentIndex;
            }
            else
            {
                //parent nodes f value is lower, no swap needed
                break;
            }
        }
    }

    //method to search for particular node index
    private int SearchForIndex(Node nd)
    {

        int indexFound = 0;

        for (int i = 1; i <= size; i++)
        {
            if (heap[i] == nd)
            {
                indexFound = i;
                break;
            }
            else
            {
                indexFound = 0;
            }
        }

        return indexFound;
    }

    //method to search if a particular node is in priority queue
    public bool search(Node nd)
    {
        bool wasFound = false;

        for (int i = 1; i <= size; i++)
        {
            if (heap[i] == nd)
            {
                wasFound = true;
            }
        }

        return wasFound;
    }
}
