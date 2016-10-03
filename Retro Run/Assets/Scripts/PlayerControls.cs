using UnityEngine;
using System.Collections;
using Photon;
public class PlayerControls : UnityEngine.MonoBehaviour
{


    public float jumpForce = 4f;
    public float runSpeed = 5f;

    public int missileCount = 1;
    public float missileForce = 1000f;
    public Sprite missileSkin;

    public AudioSource jumpSound;
    public AudioSource missileSound;
    public LayerMask groundLayer;

    private Vector2 movement;
    private Transform groundCheck;
    private Transform wallCheck;
    private Transform ceilingCheck;
    private Transform ceilingCheck2;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprRenderer;

    public GameObject missilePrefab;
    private GameObject missile;

    private bool isGrounded = false;
    private bool isWalled = false;
    private bool isCeiled = false;

    private bool jump = false;
    private bool isStopped = false;
    private bool isSleeping = false;
    private bool isAwaking = false;
    private bool isFinished = false;
    private bool isStunning = false;
    private bool isDancing = false;
    private bool canFlip = true;

    private bool isWon = false;
    private bool isPlayerInit = false;

    private byte animCount = 0;
    private byte animCounter = 0;

    private float tempRun;
    private float tempJump;

    public delegate void CollectAction(int gold);
    public static event CollectAction OnCollectGold;
    public static event CollectAction OnCollectMissile;

    ObjectPoolScript objectPoolScript;

    public delegate void PlayerAction(GameObject go);
    public static event PlayerAction OnMissiled;
    public static event PlayerAction OnEnd;

    [HideInInspector]
    public bool facingRight = true;

    private PhotonView photonview;

    void Awake()
    {
        groundCheck = transform.Find("groundCheck");
        wallCheck = transform.Find("wallCheck");
        ceilingCheck = transform.Find("ceilingCheck");
        ceilingCheck2 = transform.Find("ceilingCheck2");
        sprRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        jumpSound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        objectPoolScript = GetComponent<ObjectPoolScript>();
        photonview = GetComponent<PhotonView>();

    }

    void Start()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y);
        tempRun = runSpeed;
        tempJump = jumpForce;
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


        // Missile
        if (Input.GetKeyDown(KeyCode.LeftControl) && missileCount > 0)

        {
            FireMissile();
            this.photonview.RPC("FireMissile", PhotonTargets.OthersBuffered);
        }

        if (!isGrounded)
        {
            anim.SetInteger("count", 3);
            animCount = 3;
        }
        else if (isGrounded)
        {
            anim.SetInteger("count", 4);
            animCount = 4;
        }

        if (isSleeping && !isGrounded)
        {
            anim.SetInteger("count", 9);
            animCount = 9;
            canFlip = false;
        }
        else if (isSleeping && isGrounded)
        {
            anim.SetInteger("count", 5);
            animCount = 5;
            canFlip = false;
        }
        else if (isAwaking)
        {
            anim.SetInteger("count", 6);
            animCount = 6;
            //isAwaking = false;
            canFlip = true;
        }
        else if (isFinished && isGrounded)
        {
            anim.SetInteger("count", 2);
            animCount = 2;
            //isAwaking = false;
            canFlip = false;
        }
        else if (isFinished && !isGrounded)
        {
            anim.SetInteger("count", 8);
            animCount = 8;
            //isAwaking = false;
            canFlip = false;
        }
        else if (isWon && isGrounded)
        {
            anim.SetInteger("count", 11);
            animCount = 11;
            //isAwaking = false;
            canFlip = false;
        }
        else if (isWon && !isGrounded)
        {
            anim.SetInteger("count", 12);
            animCount = 12;
            //isAwaking = false;
            canFlip = false;
        }

        if (animCounter != animCount)
        {
            animCounter = animCount;
            //controller.SendAnimationState(animCount, facingRight);        
    }
}

    void FixedUpdate()
    {

       
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        

        Vector2 movementVelocity = rb.velocity;

        if (Input.GetAxisRaw("Horizontal") > 0.5f)
        {
            movementVelocity.x = runSpeed;

        }
        else if (Input.GetAxisRaw("Horizontal") < -0.5f)
        {
            movementVelocity.x = -runSpeed;
        }
        else
        {
            movementVelocity.x = 0;
        }

        rb.velocity = movementVelocity;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (jump)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jumpSound.Play();
            jump = false;
        }

        //Flip
        if (movementVelocity.x > 0 && !facingRight)
        {
            // Flip();      
            photonview.RPC("Flip", PhotonTargets.All);
        }
        else if (movementVelocity.x < 0 && facingRight)
        {
            // Flip(); 
            photonview.RPC("Flip", PhotonTargets.All);
        }

    }

    [PunRPC]
    void Flip()
    {
        if (canFlip)
        {
            facingRight = !facingRight;

            sprRenderer.flipX = !sprRenderer.flipX;


            //controller.SendAnimationState(animCount, facingRight);
        }

    }

    [PunRPC]
    public void FireMissile()
    {
        missilePrefab.GetComponent<SpriteRenderer>().sprite = missileSkin;
        float missilePos = 0.5f;
        if (!facingRight)
        {
            missilePos *= -1;
        }
        if (!isGrounded)
        {
            missile = Instantiate(missilePrefab, new Vector2(transform.position.x - missilePos, transform.position.y - 0.1f), Quaternion.identity) as GameObject;
            missile.GetComponent<SpriteRenderer>().sprite = missileSkin;

            //controller.DroppedMissile();
        }
        else
        {
            missile = Instantiate(missilePrefab, new Vector2(transform.position.x + missilePos, transform.position.y), Quaternion.identity) as GameObject;
            missile.GetComponent<SpriteRenderer>().sprite = missileSkin;
            if (facingRight)
            {
                missile.GetComponent<Rigidbody2D>().AddForce(new Vector2(missileForce, 0));
            }
            else
            {
                missile.GetComponent<Rigidbody2D>().AddForce(new Vector2(-missileForce, 0));
            }
            //controller.FiredMissile();
        }
        missileCount--;
		OnMissiled(gameObject);
        OnCollectMissile(missileCount);
        missileSound.Play();
    }


    void OnEnable()
    {

        FinishScript.OnFinished += Finished;
        FinishScript.OnFinishWon += OnRaceWon;
        StunTrapScript.OnStunned += Stunning;
        MissileScript.OnStun += Stunning;
        MissileBoxScript.OnDonate += Donating;
    }

    void OnDisable()
    {

        FinishScript.OnFinished -= Finished;
        FinishScript.OnFinishWon -= OnRaceWon;
        StunTrapScript.OnStunned -= Stunning;
        MissileScript.OnStun -= Stunning;
        MissileBoxScript.OnDonate -= Donating;
    }

    void Finished(GameObject go)
    {

        if (gameObject == go && !isDancing)
        {
            runSpeed = 0;
            jumpForce = 0;

            isStopped = true;
            isAwaking = false;
            isSleeping = false;
            isFinished = true;
        }
    }

    void OnRaceWon(GameObject go)
    {

        if (gameObject == go)
        {
            runSpeed = 0;
            jumpForce = 0;

            isStopped = false;
            isAwaking = false;
            isSleeping = false;
            isFinished = true;
            isDancing = true;
        }
    }

    void Stunning(GameObject go)
    {

        if (gameObject == go)
        {
            if (isGrounded)
            {
                runSpeed = 0;
                jumpForce = 0;

                isAwaking = false;
                isSleeping = true;
            }
        }
    }

    void Recovering()
    {

        isSleeping = false;
        isAwaking = true;

        runSpeed = tempRun;
        jumpForce = tempJump;
    }

    void Donating(GameObject go)
    {

        if (gameObject == go)
        {
            missileCount++;
        }
    }

}
