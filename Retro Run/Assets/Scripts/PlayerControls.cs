using UnityEngine;
using System.Collections;
using Photon;
public class PlayerControls : UnityEngine.MonoBehaviour {


	public float jumpForce = 4f;
	public float runSpeed  = 0f;

	public AudioSource  jumpSound;
	public LayerMask    groundLayer;

	private Transform   groundCheck;
	private Transform   ceilingCheck;
	private Transform   ceilingCheck2;
	private Rigidbody2D playerRigidbody;
	private Animator    anim;

    private float movement = 0f;

	private bool isGrounded  = false;
	private bool isCeiled    = false;

	private bool jump        = false;

	void Awake()
	{
		groundCheck      = transform.Find("groundCheck");
		ceilingCheck     = transform.Find("ceilingCheck");
		ceilingCheck2    = transform.Find("ceilingCheck2");

		playerRigidbody  = GetComponent<Rigidbody2D>();
		jumpSound        = GetComponent<AudioSource>();
		anim             = GetComponent<Animator>();        
        
	}
	
	void Start () 
	{
		transform.position = new Vector2(transform.position.x, transform.position.y);
	}

	void Update()
	{       
        Move();
	}

    private void Move()
    {

        if ((Input.GetKeyDown("space") || Input.GetButtonDown("Jump")) && (isGrounded))
        {
            jump = true;
        }						
	}

	void FixedUpdate () {				

        movement = Input.GetAxis ("Horizontal");
        playerRigidbody.velocity = new Vector2(movement * runSpeed, playerRigidbody.velocity.y);        

		isGrounded = Physics2D.OverlapCircle (groundCheck.position,   0.1f,  groundLayer);

		if(jump)
		{
			playerRigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
			jumpSound.Play();            
			jump = false;
		}
	}   
}
