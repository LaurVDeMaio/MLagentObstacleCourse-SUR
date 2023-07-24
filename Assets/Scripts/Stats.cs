using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stats : MonoBehaviour
{

    List<int> Goals;
    int numEpisodes = 0;

    int highest = 0;
    
    void Awake()
    {
        Goals = new List<int>();

        for(int i = 0; i < 100; i++)
        {
            Goals.Add(0);
        }
    }

    public void StartEpisode()
    {
        numEpisodes++;

        if (numEpisodes % 10 == 0)
        {
            Debug.Log("Beginning Episode: " + numEpisodes);
        }
    }

    public void AddGoal(int x)
    {
        Goals.RemoveAt(0);

        if(x == 1) {
            Goals.Add(1);
        }
        else if(x == 0) {
            Goals.Add(0);
        }

        int total = 0;
        foreach (var g in Goals)
        {
            total += g;
        }

        if (total > highest) highest = total;

        Debug.Log("Goals: " + total + "/100 -- " + highest);

    }
    
    void Update()
    {
        
    }
}
