//Erstellt am 10.08.2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    GameObject player;
    PlayerController PC;

    int type = 0; //0=3d, 1= Side, 2 = topdown

    private void OnEnable()
    {
        PlayerController.OnGameTypeChange -= HandleGameTypeChange;
        PlayerController.OnGameTypeChange += HandleGameTypeChange;
    }

    private void OnDisable()
    {
        PlayerController.OnGameTypeChange -= HandleGameTypeChange;
    }


    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        PC = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {
        if (type == 0)
        {
            transform.position = new Vector3(0, 6, player.transform.position.z - 10);
            transform.rotation = Quaternion.Euler(20, 0, 0);
        } else if (type == 1) {
            transform.position = new Vector3(10, 2, player.transform.position.z );
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }

    }

    //hätte auch ohne eventsystem geklappt

    void HandleGameTypeChange(PlayerController.GameType prevType, PlayerController.GameType targetType)
    {
        if (targetType == PlayerController.GameType.threeD)
        {
            type = 0;
        } else
            if (targetType == PlayerController.GameType.Sidescroll)
            {
                type = 1;
            }
            else 
                if (targetType == PlayerController.GameType.Sidescroll)
                {
                    type = 1;
                }
            
        }
    }

