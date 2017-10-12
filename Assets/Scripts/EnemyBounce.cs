// erstellt am 17.08.2017
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBounce : MonoBehaviour, IOnPlayerBounce {
    public GameObject player;
    public int hp = 1;
    private float delay = 1f;
    private float realdelay = 0f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		realdelay -= Time.deltaTime;
	}


  public void OnPlayerBounce ()
    {
        if (realdelay<= 0) {
            hp -= 1;
            print(hp); 
            player.gameObject.GetComponent<Rigidbody>().AddForce(0, 15, 0, ForceMode.Impulse);
            if (hp == 0)
            {
                Destroy(gameObject);
            }
            realdelay += delay;
        } 
    }
}
