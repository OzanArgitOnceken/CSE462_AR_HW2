using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
public class fileManager : MonoBehaviour
{
    public float scspeed = 0.01f; 
    public Vector3[] coordinates1;
    public Vector3[] coordinates2;
    public string filepath1 = "Assets/coordinates1.txt";
    public string filepath2 = "Assets/coordinates2.txt";
    // Start is called before the first frame update
    void Start()
    {
        string[] lines1 = File.ReadAllLines(filepath1);
        int x = int.Parse(lines1[0]);
        coordinates1 = new Vector3[x];
        GameObject[] sphereObject = new GameObject[x];
        GameObject[] sphereObject2 = new GameObject[x];
        string[] lines2 = File.ReadAllLines(filepath2);
        int y = int.Parse(lines2[0]);
        coordinates2 = new Vector3[y];

        for (int i = 0; i < x; ++i)
        {
            string[] s1 = lines1[i + 1].Split(' ');
            float x1 = float.Parse(s1[0]);
            float y1 = float.Parse(s1[1]);
            float z1 = float.Parse(s1[2]);
            coordinates1[i] = new Vector3(x1, y1, z1);
            
            sphereObject[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereObject[i].transform.localPosition = new Vector3(x1, y1, z1);
            sphereObject[i].transform.localScale = new Vector3(1, 1, 1);
            sphereObject[i].GetComponent<MeshRenderer>().material.color = Color.green;
            sphereObject[i].tag = "sphereObject";
            string[] s2 = lines2[i + 1].Split(' ');
            float x2 = float.Parse(s2[0]);
            float y2 = float.Parse(s2[1]);
            float z2 = float.Parse(s2[2]);
            coordinates2[i] = new Vector3(x2, y2, z2);

            sphereObject2[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereObject2[i].transform.localPosition = new Vector3(x2, y2, z2);
            sphereObject2[i].transform.localScale = new Vector3(1, 1, 1);
            sphereObject2[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
            sphereObject2[i].tag = "sphereObject";

        }
        
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void trans()
    {
        Debug.Log("trans Started");
        Ransac();
        scale();
        Debug.Log("trans finished");
    }
    public void Ransac()
    {
        Debug.Log("Ransac Started");
        // get transform child count
        int amount = 12;

        // collect child positions into array
        var points = coordinates1;
        int index = 0;
        foreach (Transform t in transform)
        {
            points[index++] = t.position;
        }


        // maximum distance to the line, to be considered as an inlier point
        float threshold = 0.75f;
        float bestScore = Mathf.Infinity;

        // results array (all the points within threshold distance to line)
        Vector3[] bestInliers = coordinates2;
        Vector3 bestPointA = Vector3.zero;
        Vector3 bestPointB = Vector3.zero;

        // how many search iterations we should do
        int iterations = 30;
        for (int i = 0; i < iterations; i++)
        {
            // take 2 points randomly selected from dataset
            int indexA = Random.Range(0, amount);
            int indexB = Random.Range(0, amount);
            var pointA = points[indexA];
            var pointB = points[indexB];

            // reset score and list for this round of iteration
            float currentScore = 0;
            // temporary list for points found in one search
            List<Vector3> currentInliers = new List<Vector3>();

            // loop all points in the dataset
            for (int n = 0; n < amount; n++)
            {
                // take one point form all points
                var p = points[n];
                // get distance to line, NOTE using editor only helper method
                var currentError = HandleUtility.DistancePointToLine(p, pointA, pointB);

                // distance is within threshold, add to current inliers point list
                if (currentError < threshold)
                {
                    currentScore += currentError;
                    currentInliers.Add(p);
                }
                else // outliers
                {
                    currentScore += threshold;
                }
            } // for-all points

            // check score for the best line found
            if (currentScore < bestScore)
            {
                bestScore = currentScore;
                bestInliers = currentInliers.ToArray();
                bestPointA = pointA;
                bestPointB = pointB;
            }
        } // for-iterations


        // show results

        // draw the searched line
        Debug.DrawRay(bestPointA, (bestPointA - bestPointB).normalized * 999, Color.yellow, 99);
        Debug.DrawRay(bestPointA, (bestPointB - bestPointA).normalized * 999, Color.yellow, 99);

        for (int i = 0, length = bestInliers.Length; i < length; i++)
        {
            // draw cross for all points within line
            DrawDebug(bestInliers[i], Color.green, 0.5f);
        }

        Debug.Log("Ransac Finished");
    }
    void DrawDebug(Vector2 pos, Color color, float scale = 0.05f)
    {
        Debug.DrawRay(pos, Vector2.up * scale, color, 99);
        Debug.DrawRay(pos, -Vector2.up * scale, color, 99);
        Debug.DrawRay(pos, Vector2.right * scale, color, 99);
        Debug.DrawRay(pos, -Vector2.right * scale, color, 99);
    }
    public void scale()
    {
        scaleup_button();
    }
    public void scaleup_button()
    {
        GameObject.FindWithTag("sphereObject").transform.localScale += new Vector3(scspeed, scspeed, scspeed);
    }
}