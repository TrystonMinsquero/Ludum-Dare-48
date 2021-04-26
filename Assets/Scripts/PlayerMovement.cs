using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float drag = 1f;
    public float scale = 1.5f;
    public float gravity = 2f;


    public float upRotation = 80;
    public float downHorizontalRotation = 25;
    public float horizontalRotaion = 35;

    Player player;
    [System.NonSerialized]
    public Controls controls;
    Rigidbody2D rb;
    Animator anim;

    bool facingRight;
    bool grounded = true;
    bool falling;
    bool onRightWall;
    bool onLeftWall;
    bool splat;

    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.Enable();
        player = this.GetComponent<Player>();
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        

        rb.drag = drag;
        rb.gravityScale = gravity;
        transform.localScale = Vector3.one * scale;
    }

    private void FixedUpdate()
    {

        Vector2 playerMovement = controls.Gameplay.Movement.ReadValue<Vector2>();

        if (falling)
        {
            rb.gravityScale = 0;

            float minPlayerHeight = LevelManager.cam.transform.position.y - 5 + this.GetComponent<BoxCollider2D>().size.y / 2;
            float maxPlayerHeight = LevelManager.cam.transform.position.y + 5 - this.GetComponent<BoxCollider2D>().size.y / 2;
            //Directional movement
            if (playerMovement != Vector2.zero)
            {
                //Check movement
                Vector2 vel = rb.velocity;

                //Check x velocity
                if (playerMovement.x != 0 && (playerMovement.x > 0 && !onRightWall || playerMovement.x < 0 && !onLeftWall))
                    vel.x = playerMovement.x * moveSpeed;

                //Check y velocity
                if (playerMovement.y != 0 && (transform.position.y < maxPlayerHeight && playerMovement.y <= 0 || transform.position.y > minPlayerHeight && playerMovement.y >= 0))
                    vel.y = playerMovement.y * moveSpeed;

                vel.y = playerMovement.y * moveSpeed;
                rb.velocity = vel;

                //Check facing
                if (playerMovement.x != 0)
                {
                    facingRight = playerMovement.x > 0 ? true : false;
                    Vector3 scaleTemp = Vector3.one * scale;
                    scaleTemp.x = facingRight ? scale : -scale;
                    transform.localScale = scaleTemp;
                }
            }

            if(!LevelManager.timeSlowed && controls.Gameplay.SlowTime.ReadValue<float>() > 0 && DataControl.timeSlows > 0)
            {
                DataControl.timeSlows--;
                StartCoroutine(SoundManager.SlowTime(Shop.timeChargeDuration, Shop.speedReduction));
            }
                

            if (transform.position.y > maxPlayerHeight && !splat)
                transform.position = new Vector2(transform.position.x, maxPlayerHeight);

            if (transform.position.y < minPlayerHeight)
                transform.position = new Vector2(transform.position.x, minPlayerHeight);

            if (splat && transform.position.y > maxPlayerHeight + 3)
            {
                Debug.Log("Die");
                StartCoroutine(player.Die());
            }

        }
        else
        {
            rb.gravityScale = gravity;
            if (playerMovement.x != 0)
            {
                //Check movement
                Vector2 vel = Vector2.zero;
                if (playerMovement.x > 0 && !onRightWall || playerMovement.x < 0 && !onLeftWall)
                    vel.x = playerMovement.x * moveSpeed;
                else
                    vel.x = 0;
                vel.y = rb.velocity.y;
                rb.velocity = vel;

                //Check facing
                facingRight = playerMovement.x > 0 ? true : false;

            }

            //Jump
            if (playerMovement.y > 0 && grounded)
            {
                //Debug.Log("Jump");
                SoundManager.playSound(SoundManager.jump);
                grounded = false;
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * 2);
            }

        }

        CheckAnimation(playerMovement);



        onLeftWall = false;
        onRightWall = false;
    }

    public void CheckAnimation(Vector2 playerMovement)
    {
        int suitNum = -1;
        switch (DataControl.suitLevel)
        {
            case 0: suitNum = 2;break;
            case 1: suitNum = 0;break;
            case 2: suitNum = 1;break;
        }
        if (suitNum < 0)
            Debug.LogError("Suit level not 0, 1, or 2");

        if (splat)
        {
            Vector3 scaleTemp = Vector3.one * scale;
            scaleTemp.x = facingRight ? -scale : scale;
            transform.localScale = scaleTemp;
            anim.Play("Travis Splat_" + suitNum);
            return;
        }

        string state = "Travis ";
        if (falling)
        {
            //Check scalling
            Vector3 scaleTemp = Vector3.one * scale;
            scaleTemp.x = facingRight ? -scale : scale;
            transform.localScale = scaleTemp;

            float rotateValue = -1;

            state += "Fall ";
            //Horizontal Down
            if(playerMovement.y < 0 && playerMovement.x != 0)
            {
                state += "Down";
                rotateValue = downHorizontalRotation;
            }
            //Horizontal Only
            if(playerMovement.y == 0 && playerMovement.x != 0)
            {
                state += "Direction";
                rotateValue = downHorizontalRotation;
            }
            //Up
            if (playerMovement.y > 0)
            {
                state += "Up";
                rotateValue = upRotation;
            }
            //Down only
            if (playerMovement.y < 0 && playerMovement.x == 0)
            {
                state += "Down";
                rotateValue = 0;
            }
            if(playerMovement == Vector2.zero)
            {
                state += "Idle";
                rotateValue = 0;
            }

            if (rotateValue < 0)
                Debug.Log("You messed up");

            rotateValue *= facingRight ? 1 : -1;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotateValue));
            
        }
        else
        {
            //Check scalling
            Vector3 scaleTemp = Vector3.one * scale;
            scaleTemp.x = facingRight ? scale : -scale;
            transform.localScale = scaleTemp;

            if (!grounded)
            {
                state += "Jump";
            }
            else if(playerMovement.x != 0)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Travis Walk_" + suitNum))
                    return;
                state += "Walk";
            }
            else
            {
                state += "Idle";
            }

        }

        state += "_" + suitNum;

        anim.Play(state);
    }

    

    public void Splat()
    {

        controls.Disable();
        splat = true;
        SoundManager.playSound(SoundManager.splat);
        StartCoroutine(player.DieAfterTime(1));
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            //Debug.Log(contact.collider);
            if (contact.collider.CompareTag("Ground"))
            {
                //Debug.Log("Ground!");
                grounded = true;
            }

            if (contact.collider.CompareTag("Right Wall"))
            {
                //Debug.Log("Right Wall!");
                onRightWall = true;
            }

            if (contact.collider.CompareTag("Left Wall"))
            {
                //Debug.Log("Left Wall!");
                onLeftWall = true;
            }

            if(contact.collider.CompareTag("Obstacle"))
            {
                if (player.hasBubble)
                    player.PopBubble();
                else
                    Splat();
            }

        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            
            if (contact.collider.CompareTag("Right Wall"))
            {
                //Debug.Log("Right Wall!");
                onRightWall = true;
            }

            if (contact.collider.CompareTag("Left Wall"))
            {
                //Debug.Log("Left Wall!");
                onLeftWall = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Transition"))
        {
            falling = true;
            grounded = false;

            LevelManager.NextLevelSection();
            LevelManager.walls.SetActive(true);
            LevelManager.falling = true;
            StartCoroutine(SoundManager.TransitionToSong(SoundManager.themes[(int)LevelManager.levelSection]));
        }


    }

}
