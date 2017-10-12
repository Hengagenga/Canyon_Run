//erstellt am 28.08.2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float countdown = 3f;
    bool ticking = false;
    public GameObject explosion;
    public Material material;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (ticking)
        {
            countdown -= Time.deltaTime;
        }

        if (countdown <= 0)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player")
        {
            ticking = true;
            gameObject.GetComponent<Renderer>().material = material;
        }
    }
}
