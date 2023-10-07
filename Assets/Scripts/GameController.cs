using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public enum Mode { Normal, Distance, Vision };
public class GameController : MonoBehaviour
{
    public TextMeshProUGUI PlayerPositionText;
    public TextMeshProUGUI PlayerVelocityText;
    public TextMeshProUGUI DistanceText;
    private Vector3 oldPosition;
    private float playerVelocity;
    private float distance;
    private LineRenderer lineRenderer;
    private Mode modes;
    /*private GameObject[] pickUps;*/

    // Start is called before the first frame update
    void Start()
    {
        oldPosition = transform.position;
        playerVelocity = 0f;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.GetComponent<LineRenderer>().enabled = false;
        modes = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(Enum.GetValues(typeof(States)).Length);
            modes = (Mode)(((int)modes + 1) % 3);
            Debug.Log(modes);
        }

        GameObject[] pickUps = GameObject.FindGameObjectsWithTag("PickUp");

        switch (modes)
        {
            case Mode.Normal:
                foreach(GameObject gameObject in pickUps)
                {
                    gameObject.GetComponent<Renderer>().material.color = Color.white;
                }
                DistanceText.gameObject.SetActive(false);
                lineRenderer.GetComponent<LineRenderer>().enabled = false;
                /*SetDebugText(false);*/
                break;
            
            case Mode.Distance:
                {
                    lineRenderer.GetComponent<LineRenderer>().enabled = true;
                    PlayerPositionText.gameObject.SetActive(true);
                    PlayerVelocityText.gameObject.SetActive(true);
                    DistanceText.gameObject.SetActive(true);
                    playerVelocity = (transform.position - oldPosition).magnitude / Time.deltaTime;
                    oldPosition = transform.position;
                    DistanceCalculate(Color.blue, false, pickUps);
                    SetDebugText(true);
                }break;

            case Mode.Vision:
                PlayerPositionText.gameObject.SetActive(false);
                PlayerVelocityText.gameObject.SetActive(false);
                DistanceText.gameObject.SetActive(true);
                //lineRenderer.GetComponent<LineRenderer>().enabled = true;
                DistanceCalculate(Color.green, true, pickUps);
                SetDebugText(true);
                break;
        }
    }

    //Setting debug text
    private void SetDebugText(bool active)
    {
        if(active)
        {
            PlayerPositionText.text = "PlayerPosition: " + oldPosition.ToString();
            PlayerVelocityText.text = "PlayerVelocity: " + playerVelocity.ToString("0.00") + "unit/s";
            DistanceText.text = "Distance: " + distance.ToString("0.00");
        }
        else
        {
            PlayerPositionText.text = "";
            PlayerVelocityText.text = "";
            DistanceText.text = "" ;
        }
        
    }

    //Calculating distance between cloest pickup and player
    private void DistanceCalculate(Color targetColor, bool vision, GameObject[] pickUps)
    {
        distance = float.MaxValue;
        /*GameObject[] pickUps = GameObject.FindGameObjectsWithTag("PickUp");*/
        Vector3 endPosition = new Vector3();
        int target = 0;
        for (int i = 0; i < pickUps.Length; i++)
        {
            if (distance > (pickUps[i].transform.position - transform.position).magnitude)
            {
                target = i;
                endPosition = pickUps[i].transform.position;
                distance = (endPosition - transform.position).magnitude;
            }
            pickUps[i].GetComponent<Renderer>().material.color = Color.white;
        }
        pickUps[target].GetComponent<Renderer>().material.color = targetColor;
        if(vision)
        {
            pickUps[target].transform.LookAt(transform.position);
            RenderDistance(transform.position, transform.position + (transform.position  - oldPosition));
        }
        else
        {
            RenderDistance(transform.position, endPosition);
        }
    }

    //Rendering line
    private void RenderDistance(Vector3 startPosition, Vector3 endPosition)
    {
        // 0 for the start point , position vector ¡¯ startPosition ¡¯
        lineRenderer.SetPosition(0, startPosition);
        // 1 for the end point , position vector ¡¯endPosition ¡¯
        lineRenderer.SetPosition(1, endPosition);
        // Width of 0.1 f both at origin and end of the line
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

}
