using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stats : MonoBehaviour
{

    List<int> Goals;
    public int numEpisodes = 0;
    
    void Awake()
    {
        Goals = new List<int>();

        for(int i = 0; i < 100; i++)
        {
            Goals.Add(0);
        }
    }

    public void AddGoal(int x)
    {
        Goals.RemoveAt(0);

        if(x == 1)
        {
            Goals.Add(1);
        }
        else if(x == 0) {
            Goals.Add(0);
        }

    }
    
    void Update()
    {
        if(numEpisodes % 100 == 0)
        {
            Debug.Log("Beginning Episode: " + numEpisodes);
            Debug.Log("Number of Successes: ");
        }
    }
}
