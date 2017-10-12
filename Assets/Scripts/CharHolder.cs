//erstellt am 17.08.2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharHolder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider check)
    {
            if (check.tag == "Player")
            {

                check.transform.parent = transform;
            }
        }

    void OnTriggerExit(Collider check)
    {
        if (check.tag == "Player")
        {
            check.transform.parent = null;
        }
    }

}
