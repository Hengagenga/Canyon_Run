//Erstellt am 08.08.2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float DistanceToTarget;
    bool input = true;
	public Vector3 currentSpeed;
	public Transform mainCam,  floorChecks;
    private enum Grundtype{
        ice, mud, normal
    };

    private Grundtype typ;

	public int endurance = 100;                 //defines the maximum endurance value. It is spent while sprinting and replenishes over time
	public float accel = 8f;					//acceleration/deceleration in air or on the ground
	public float airAccel = 10f;			
	public float decel = 8f;
	public float airDecel = 1.1f;
    [Range(0f, 5f)]
    public float rotateSpeed = 0.7f;	//how fast to rotate on the ground, how fast to rotate in the air
	public float maxSpeed = 10;								//maximum speed of movement in X/Z axis
	public float slopeLimit = 40, slideAmount = 35;			//maximum angle of slopes you can walk on, how fast to slide down slopes you can't
	public float movingPlatformFriction = 7.7f;				//you'll need to tweak this to get the player to stay on moving platforms properly
    Vector3 savedVelocity;
    Vector3 savedAngularVelocity;
 

    //jumping
    public Vector3 jumpForce =  new Vector3(0, 2, 0);		//normal jump force
	public Vector3 secondJumpForce = new Vector3(0, 3, 0); //the force of a 2nd consecutive jump
	public Vector3 thirdJumpForce = new Vector3(0, 5, 0);	//the force of a 3rd consecutive jump
	public float jumpDelay = 0.5f;                          //how fast you need to jump after hitting the ground, to do the next type of jump
	public float jumpLeniancy = 0.17f;						//how early before hitting the ground you can press jump, and still have it work
    private Transform[] floorCheckers;

    public bool doubleJump = true;
	public int multiJump;
	public int jumpcount = 0;

	private bool recover = false;                           //once all endurance has been spent, the player has to recover first before using it again
	private int onJump;
	private bool grounded;

	private Quaternion screenMovementSpace;
	private float  groundedCount, curDecel, curRotateSpeed, slope;


    public bool rotate = true;
	private Rigidbody rigid;
    // Use this for initialization

    public enum GameType
    {
        threeD,
        Sidescroll,
        RunAway
    };

    public delegate void GameTypeChangeEvent(GameType prevState, GameType targetState);
    public static event GameTypeChangeEvent OnGameTypeChange;

    private GameType m_currentType;
    public GameType currentType
    {
        get
        {
            return m_currentType;
        }
        private set
        {
            if (OnGameTypeChange != null)
            {
                OnGameTypeChange(m_currentType, value);
            }
            m_currentType = value;

        }
    }


    private void OnEnable()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
        GameManager.OnGameStateChange += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
    }

    void Awake()
	{
        typ = Grundtype.normal;
        mainCam = GameObject.FindGameObjectWithTag ("MainCamera").transform;


		rigid = GetComponent<Rigidbody>();
		rigid.interpolation = RigidbodyInterpolation.Interpolate;
		if(GetComponent<Collider>().material.name == "Default (Instance)")
		{
			PhysicMaterial pMat = new PhysicMaterial();
			pMat.name = "Frictionless";
			pMat.frictionCombine = PhysicMaterialCombine.Multiply;
			pMat.bounceCombine = PhysicMaterialCombine.Multiply;
			pMat.dynamicFriction = 0f;
			pMat.staticFriction = 0f;
			GetComponent<Collider>().material = pMat;
		}


        floorCheckers = new Transform[floorChecks.childCount];
        for (int i = 0; i < floorCheckers.Length; i++)
            floorCheckers[i] = floorChecks.GetChild(i);


    }


	void Start () {
		
	}
	
	void Update () {

        Debug.Log(IsGrounded());
            

        if (Input.GetKeyDown("y"))
        {
            if (currentType == GameType.threeD)
            {
                currentType = GameType.Sidescroll;
            } else
            {
                currentType = GameType.threeD;
            }
        }

        if (!grounded)
        {
            //rigid.AddForce(new Vector3(0, -10, 0), ForceMode.Acceleration);
        }


		JumpCalculations ();

		curDecel = (grounded) ? decel : airDecel;
        curRotateSpeed = rotateSpeed;

		//screenMovementSpace = Quaternion.Euler (0, mainCam.eulerAngles.y, 0);
		//screenMovementForward = screenMovementSpace * Vector3.forward;
		//screenMovementRight = screenMovementSpace * Vector3.right;

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical" );

			if (Input.GetButton ("Sprint") && endurance >= 1) {
				Debug.Log ("sprint");
				maxSpeed = 20;
				accel = 50f;
				endurance -= 2;
				if (endurance <= 1)
					recover = true;
			} else {
				accel = 8f;
				maxSpeed = 8;
				if (endurance < 70f)
				{
					endurance += 1;
					if (recover == true && endurance >= 70)
						recover = false;
				}
			}

		

	}

	void FixedUpdate () {
		grounded = IsGrounded ();

        rigid.AddForce(0,-12,0);

        //move
        if (typ == Grundtype.mud  && input ) {
            accel = 2f;
            curDecel = decel;
            Move();
        } else if (typ == Grundtype.ice && input)
        {
            curDecel = 2f;
            accel = 3f;
            if (Input.GetAxis("Horizontal")!= 0 || Input.GetAxis("Vertical") != 0) {
                rigid.AddForce(transform.forward * (accel-1) * 6);

            }
        } else
        {
            if (input)
            {
                Move();
            }

        }
        //rotate
        if (rotate)
        {
            float turnSpeed = curRotateSpeed * 5;

            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");


            Vector3 movement;
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                
                movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
                if (currentType == GameType.Sidescroll)
                {
            
                    moveVertical = moveVertical * -1;
                    movement = new Vector3(moveVertical, 0.0f, moveHorizontal);

                }
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
                
            }
        }



        //managespeed

            currentSpeed = rigid.velocity;
		if (currentSpeed.magnitude > 0 && grounded)
		{
			rigid.AddForce ((currentSpeed * -1) * curDecel * Time.deltaTime, ForceMode.VelocityChange);

		}
		if (rigid.velocity.magnitude > maxSpeed)
			rigid.AddForce ((currentSpeed * -1) * curDecel * Time.deltaTime, ForceMode.VelocityChange);

	}

	public bool IsGrounded()
	{
		float dist = GetComponent<Collider>().bounds.extents.y;
        bool done = false;

        foreach (Transform floorCheck in floorCheckers)
        {
            RaycastHit hit;
            if (Physics.Raycast(floorCheck.position, Vector3.down, out hit, dist + 0.05f))
            {

                IOnPlayerBounce PB = hit.transform.GetComponent<IOnPlayerBounce>();
                if (PB != null && done == false)
                {
                    PB.OnPlayerBounce();
                    done = true;
                }


                if (hit.transform.tag == "Ice")
                {
                    typ = Grundtype.ice;
                }
                else if (hit.transform.tag == "Mud")
                {
                    typ = Grundtype.mud;
                }
                else
                {
                    typ = Grundtype.normal;
                }

                if (!hit.transform.GetComponent<Collider>().isTrigger)
                {
                    slope = Vector3.Angle(hit.normal, Vector3.up);


                    if (slope > slopeLimit)
                    {
                        Vector3 slide = new Vector3(0f, -slideAmount, 0f);
                        rigid.AddForce(slide, ForceMode.Force);
                    }



                    return true;
                }
            }
        }
		return false;
	}



	private void JumpCalculations()
	{
		//keep how long we have been on the ground
		groundedCount = (grounded) ? groundedCount += Time.deltaTime : 0f;

		if(groundedCount < 0.25 && groundedCount != 0  && GetComponent<Rigidbody>().velocity.y < 1)
		{
			jumpcount = 0;

		}
		//if were on ground within slope limit
		if (grounded && slope < slopeLimit)
		{
			//and we press jump, or we pressed jump justt before hitting the ground
			if (Input.GetButtonDown ("Jump" ) )
			{	

				//increment our jump type if we haven't been on the ground for long
				onJump = (groundedCount < jumpDelay) ? Mathf.Min(2, onJump + 1) : 0;
				//execute the correct jump (like in mario64, jumping 3 times quickly will do higher jumps)
				if (onJump == 0)
					Jump (jumpForce);
				else if (onJump == 1)
					Jump (secondJumpForce);
				else if (onJump == 2){
					Jump (thirdJumpForce);
					onJump --;
				}
			} 
		}

		if (doubleJump)
		{
			if (!grounded && jumpcount < multiJump && Input.GetButtonDown("Jump"))
			{
				jumpcount++;
				doubleJump = true;
				Jump(jumpForce);

			}   
		}
	}

	public void Jump(Vector3 jumpVelocity)
	{

        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
		rigid.AddRelativeForce (jumpVelocity, ForceMode.Impulse);
	}

	 void Move()
	{
        Vector3 movement;

        // Distance ( speed = distance / time --> distance = speed * time)
        float distance = accel * Time.deltaTime;

        // Input on x ("Horizontal")
        float hAxis = Input.GetAxis("Horizontal");

        // Input on z ("Vertical")
        float vAxis = Input.GetAxis("Vertical");

        // Movement vector
        if (currentType == GameType.Sidescroll) {
            vAxis = Input.GetAxis("Vertical") * -1;
            movement = new Vector3(vAxis * distance, 0f, hAxis * distance);
        } else

        {
            movement = new Vector3(hAxis * distance, 0f, vAxis * distance);
        }
        // Current position
        Vector3 currPosition = transform.position;

        // New position
        Vector3 newPosition = currPosition + movement;

        // Move 
        transform.position = Vector3.MoveTowards(transform.position, newPosition, 0.3f);

    }

    void HandleGameStateChange(GameManager.GameState prevState, GameManager.GameState targetState)
    {
        if (targetState == GameManager.GameState.paused && IsGrounded())
        {
            savedVelocity = rigid.velocity;
            savedAngularVelocity = rigid.angularVelocity;
            rigid.isKinematic = true;
            rotate = false;
            input = false;
        }
        else
        {
         
            rigid.isKinematic = false;
            rigid.AddForce(savedVelocity, ForceMode.VelocityChange);
            rigid.AddTorque(savedAngularVelocity, ForceMode.VelocityChange);
            rotate = true;
            input = true;
        }
    }


}
