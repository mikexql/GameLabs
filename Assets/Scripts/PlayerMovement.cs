using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float maxSpeed = 20;
    public float upSpeed = 10;
    private bool onGroundState = true;

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public JumpOverGoomba jumpOverGoomba;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioSource marioDeath;
    public float deathImpulse = 15;
    // stomp detection thresholds
    [Tooltip("Minimum downward velocity (negative) required to count as a stomp")]
    public float stompVelocityThreshold = -2.0f;
    [Tooltip("Maximum Y difference (playerY - enemyY) allowed to count as a stomp")]
    public float stompYTolerance = 0.5f;
    private Vector3 initialPosition;

    //Testing refactoring
    GameManager gameManager;
    // state
    [System.NonSerialized]
    public bool alive = true;

    public Transform gameCamera;

    // other methods
    
    void Awake(){
        // other instructions
        // subscribe to Game Restart event
        GameManager.instance.gameRestart.AddListener(ResetGame);
        //GameManager.instance.gameOver.AddListener(ResetGame);
    }

    public void ResetGame()
    {
        // reset position
        marioBody.transform.position = initialPosition;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset Goomba

        marioAnimator.SetTrigger("gameRestart");
        alive = true;
        gameCamera.position = new Vector3(0.0f, gameCamera.position.y, gameCamera.position.z);

    }

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
        gameManager = GameManager.instance;
        initialPosition = marioBody.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);
    void OnCollisionEnter2D(Collision2D col)
    {

        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) & !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Determine whether the player is stomping the enemy (from above)
            // We consider it a stomp when the player's vertical velocity is downward enough
            // AND the player's Y is sufficiently above the enemy's Y (within tolerance).
            float playerVy = marioBody.linearVelocity.y;
            float playerY = transform.position.y;
            float enemyY = other.transform.position.y;

            bool isStomp = (playerVy <= stompVelocityThreshold) && ((playerY - enemyY) > 0f) && ((playerY - enemyY) <= stompYTolerance);

            if (isStomp)
            {
                // Stomp: trigger enemy-specific behavior if available, and give a small bounce
                // Call Mario's stomp handler directly to process the enemy stomp.
                var stompHandler = GetComponent<StompGoomba>();
                if (stompHandler != null)
                {
                    stompHandler.HandleStomp(other.gameObject);
                }
            }
            else
            {
                if (alive)
                {
                    Debug.Log("Collided with goomba from side or below - Mario dies");
                    marioAnimator.Play("mario-die");
                    marioDeath.PlayOneShot(marioDeath.clip);
                    alive = false;
                }
            }

        }
        if (other.gameObject.CompareTag("Ground") && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    private bool moving = false;
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void GameOverScene()
    {
        //This is used in the animation event
        gameManager.GameOver();
    }

    private bool jumpedState = false;
    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }

}