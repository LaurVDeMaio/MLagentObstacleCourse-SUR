using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stats : MonoBehaviour
{

    List<int> Goals;
    int numEpisodes = 0;

    int highest = 0;
    int highepisode = 0;
    System.TimeSpan hightime;

    System.Diagnostics.Stopwatch stopwatch;

    string csv = "";
    string csv_filename = "";

    void Awake()
    {
        Goals = new List<int>();

        for(int i = 0; i < 100; i++)
        {
            Goals.Add(0);
        }

        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        string scene_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string date_time = System.DateTime.Now.ToString("yyyyddMHHmmss");
        string folder = "csv/";

#if UNITY_EDITOR_WIN
folder = "csv\\";
#endif

        csv_filename = folder + "stats_" + scene_name + "_" + date_time + ".csv";
        csv = "highest,episode,time\n";
    }

    public int StartEpisode()
    {

        numEpisodes++;

        if (numEpisodes % 10 == 0)
        {
            Debug.Log("Beginning Episode: " + numEpisodes);
        }

        return numEpisodes;
    }

    public void AddGoal(int agentEpisode, int x)
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

        if (total > highest)
        {
            highest = total;
            hightime = stopwatch.Elapsed;
            highepisode = agentEpisode;
            csv += highest + "," + highepisode + "," + ConvertTime(hightime) + "\n";
            System.IO.File.WriteAllText(csv_filename, csv);
        }

        Debug.Log("Goals: " + total + "/100 -- " + highest + " -- " + ConvertTime(hightime) + " -- " + highepisode);
    }

    string ConvertTime(System.TimeSpan ts)
    {
        return System.String.Format("{0:00}:{1:00}:{2:00}",
            ts.Hours, ts.Minutes, ts.Seconds);
    }
    
}
