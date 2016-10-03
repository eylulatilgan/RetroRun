using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class NpcAI : MonoBehaviour {

	public Transform target;

	public float updateRate = 2f;

	private Seeker seeker;
	private Rigidbody2D rb;

	public Path path;
	public float speed = 300f;
	public ForceMode2D forceMode;

	[HideInInspector]
	public bool pathIsEnded = false;

	public float nextWaypointDistance = 3;
	private int currentWaypoint = 0;

	private bool isGrounded = false;
	private Transform groundCheck;
	public LayerMask groundLayer;
	private Transform wallCheck;
	public LayerMask wallLayer;

	private Transform platformCheck;
	private Transform jumpCheck;
	public LayerMask platformLayer;

	private Animator anim;
	private bool isSleeping   = false;
	private bool isAwaking    = false;
	private bool isStunning   = false;
	private bool isFinished   = false;
	private bool isWon		  = false;

	public int missileCount = 1;
	public float missileForce = 1000f;
	public Sprite missileSkin;
	public GameObject missilePrefab;
	private GameObject missile;

	private float tempSpeed;
	public bool canSleep = true;
	private bool canJump = true;

	public float stuckLimit = 0.02f;


	void Awake()
	{        
		groundCheck = transform.Find("groundCheck");
		wallCheck = transform.Find("wallCheck");
		platformCheck = transform.Find("platformCheck");
		jumpCheck = transform.Find("jumpCheck");

		seeker = GetComponent<Seeker>();
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>(); 
	}

	void Start()
	{
		tempSpeed = speed;

		if(target == null)
		{
			Debug.LogError("No Target Found");
			return;
		}

		seeker.StartPath (transform.position, target.position, OnPathComplete);

		StartCoroutine (UpdatePath());
	}

	IEnumerator UpdatePath()
	{
		if(target == null)
		{
			yield return false;
		}

		seeker.StartPath (transform.position, target.position, OnPathComplete);

		yield return new WaitForSeconds (1f/updateRate);
		StartCoroutine (UpdatePath());
	}

	public void OnPathComplete(Path p)
	{
		Debug.Log("Got A Path.");
		if(!p.error)
		{
			path = p;
			currentWaypoint = 0;
		}
	}

	void FixedUpdate()
	{
		if(target == null)
		{
			return;
		}

		if(path == null)
		{
			return;
		}

		if(currentWaypoint >= path.vectorPath.Count)
		{
			if(pathIsEnded)
			{
				return;
			}

			Debug.Log("End of Path.");
			pathIsEnded = true;
			return;
		}

		AnimationStates ();



		isGrounded = Physics2D.OverlapCircle (groundCheck.position, 0.1f, groundLayer);

		//Jump
		if((Physics2D.OverlapCircle (wallCheck.position, 0.1f, wallLayer) || (rb.velocity.x < stuckLimit)) && isGrounded && canJump)
		{
			rb.AddForce(new Vector2(1f, 7f), ForceMode2D.Impulse);
		}

		pathIsEnded = false;

		Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;

		rb.AddForce (dir, forceMode);

		float dist = Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]);

		if(dist < nextWaypointDistance)
		{
			currentWaypoint++;
			return;
		}



		// Missile
		if (missileCount > 0)
		{
			//Ray2D missileRay = new Ray2D(transform.position, Vector2.right);
			RaycastHit2D hit = Physics2D.Raycast(new Vector2 (transform.position.x + 0.5f, transform.position.y), Vector2.right);

			if(hit != null && hit.collider.tag == "Player")
			{
				FireMissile ();
			}
		}       
	}

	public void FireMissile ()
	{
		missilePrefab.GetComponent<SpriteRenderer> ().sprite = missileSkin;
		float missilePos = 0.5f;

		missile = Instantiate (missilePrefab, new Vector2 (transform.position.x + missilePos, transform.position.y), Quaternion.identity) as GameObject;
		missile.GetComponent<SpriteRenderer> ().sprite = missileSkin;
		missile.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (missileForce, 0));

		missileCount--;
	}

	void AnimationStates ()
	{
		if (!isGrounded) 
		{
			anim.SetInteger ("count", 3);
		}
		else if (isGrounded) 
		{
			anim.SetInteger ("count", 4);
		}
		if (isSleeping && !isGrounded) 
		{
			anim.SetInteger ("count", 9);
		}
		else if (isSleeping && isGrounded) 
		{
			anim.SetInteger ("count", 5);
		}
		else if (isAwaking) 
		{
			anim.SetInteger ("count", 6);
			isAwaking = false;
		}
		else if (isFinished && isGrounded) 
		{
			anim.SetInteger ("count", 2);
		}
		else if (isFinished && !isGrounded) 
		{
			anim.SetInteger ("count", 8);
		}
		else if (isWon && isGrounded) 
		{
			anim.SetInteger ("count", 11);
		}
		else if (isWon && !isGrounded) 
		{
			anim.SetInteger ("count", 12);
		}
	}


	void OnEnable() 
	{
		StunTrapScript.OnStunned     += Stunning;
		MissileScript.OnStun         += Stunning;
		MissileBoxScript.OnDonate    += Donating;
		FinishScript.OnFinished		 += Finished;
		FinishScript.OnFinishWon	 += WonRace;

	}

	void OnDisable() 
	{
		StunTrapScript.OnStunned     -= Stunning;
		MissileScript.OnStun         -= Stunning;
		MissileBoxScript.OnDonate    -= Donating;
		FinishScript.OnFinished		 -= Finished;
		FinishScript.OnFinishWon	 -= WonRace;

	}



	void Stunning(GameObject go) {

		if(gameObject == go && canSleep) {
			speed   = 0;

			isAwaking  = false;
			isSleeping = true;
			canJump = false;
		}
	}

	void Recovering() {

		isSleeping = false;
		canJump = true;
		isAwaking  = true;

		speed   = tempSpeed;
	}

	void Donating(GameObject go) {

		if(gameObject == go) {
			missileCount++;
		}
	}

	void Finished(GameObject go)
	{
		if(gameObject == go)
		{
			speed = 0;

			isAwaking = false;
			isSleeping = false;
			canJump = false;
			isFinished = true;
		}
	}

	void WonRace(GameObject go)
	{
		if(gameObject == go)
		{
			speed = 0;

			isAwaking = false;
			isSleeping = false;
			canJump = false;
			isWon = true;
		}
	}

}
