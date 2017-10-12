//Erstellt am 14.08.2017

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//moves object along a series of waypoints, useful for moving platforms or hazards

public class MoveToPoints : MonoBehaviour 
{
	public float speed = 4;										//how fast to move
	public float delay;                                     //how long to wait at each waypoint
    public int direction;
    bool paused = false;
    private GameObject target = null;
    private Vector3 offset;



    private void OnEnable()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
        GameManager.OnGameStateChange += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
    }
    //setup
    void Awake()
	{
		
	}
	
	
	void Update()
	{
    
		
	}
	
	//if this is a platform move platforms toward waypoint
	void FixedUpdate()
	{
        if (!paused)
        {
            transform.Translate(Vector3.forward * speed * direction * Time.deltaTime);
            if (target != null)
            {
                target.transform.position = transform.position + offset;
            }
        }
	}
	

    void OnTriggerEnter(Collider check)
    {
            if (check.tag == "Waypoint")
            {
                if (direction == 1)
                {
                    direction = -1;
                } else
                {
                    direction = 1;
                }
        }

            
        
    }


 


    void HandleGameStateChange(GameManager.GameState prevState, GameManager.GameState targetState)
    {
        if (targetState == GameManager.GameState.paused && GameObject.Find("Player").GetComponent<PlayerController>().IsGrounded())
        {
            paused = true;
        }
        else
        {
            paused = false;
        }
    }

}