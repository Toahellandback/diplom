using UnityEngine;
using UnityEngine.InputSystem;

public class HeroKnight : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField] private float m_jumpForce = 7.5f;
    [SerializeField] private float m_rollForce = 6.0f;

    [Header("Effects")]
    [SerializeField] private bool m_noBlood = false;
    [SerializeField] private GameObject m_slideDust;

    private Animator m_animator;
    private Rigidbody2D m_body2d;

    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;

    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    [Header("Wall Slide")]
    [SerializeField] private float m_wallSlideSpeed = 2f;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 m_wallJumpForce = new Vector2(8f, 12f);

    private bool m_isWallJumping = false;
    private float m_wallJumpTimer = 0f;
    private float m_wallJumpDuration = 0.2f;

    private int m_facingDirection = 1;
    private int m_currentAttack = 0;

    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;

    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    // Input
    private float inputX;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();

        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();

        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    private void Update()
    {
        HandleTimers();
        HandleGroundCheck();
        HandleMovement();
        HandleAnimations();
    }

    private void HandleTimers()
    {
        m_timeSinceAttack += Time.deltaTime;

        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rolling = false;
        }
    }

    private void HandleGroundCheck()
    {
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", true);
        }

        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", false);
        }
    }

    private void HandleMovement()
    {
        inputX = 0;

        if (Keyboard.current.aKey.isPressed)
            inputX = -1;

        if (Keyboard.current.dKey.isPressed)
            inputX = 1;

        // Flip sprite
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Movement
        if (!m_rolling && !m_isWallJumping)
        {
            m_body2d.linearVelocity =
                new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);
        }

        // Animator
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);
    }

    private void HandleAnimations()
    {
        // Wall Slide
        m_isWallSliding =
            (m_wallSensorR1.State() && m_wallSensorR2.State()) ||
            (m_wallSensorL1.State() && m_wallSensorL2.State());

        m_animator.SetBool("WallSlide", m_isWallSliding);
        if (m_isWallSliding && !m_grounded)
        {
            if (m_body2d.linearVelocity.y < -m_wallSlideSpeed)
            {
                m_body2d.linearVelocity =
                    new Vector2(
                        m_body2d.linearVelocity.x,
                        -m_wallSlideSpeed
                    );
            }
        }

        // Death
        if (Keyboard.current.eKey.wasPressedThisFrame && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        // Hurt
        else if (Keyboard.current.qKey.wasPressedThisFrame && !m_rolling)
        {
            m_animator.SetTrigger("Hurt");
        }

        // Attack
        else if (Mouse.current.leftButton.wasPressedThisFrame &&
                 m_timeSinceAttack > 0.25f &&
                 !m_rolling)
        {
            m_currentAttack++;

            if (m_currentAttack > 3)
                m_currentAttack = 1;

            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);

            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Mouse.current.rightButton.wasPressedThisFrame && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            m_animator.SetBool("IdleBlock", false);
        }

        // Roll
        else if (Keyboard.current.leftShiftKey.wasPressedThisFrame &&
                 !m_rolling &&
                 !m_isWallSliding)
        {
            m_rolling = true;
            m_rollCurrentTime = 0;

            m_animator.SetTrigger("Roll");

            m_body2d.linearVelocity =
                new Vector2(m_facingDirection * m_rollForce,
                m_body2d.linearVelocity.y);
        }

        // Jump
        else if (Keyboard.current.spaceKey.wasPressedThisFrame &&
         !m_rolling)
        {
            // NORMAL JUMP
            if (m_grounded)
            {
                m_animator.SetTrigger("Jump");

                m_grounded = false;
                m_animator.SetBool("Grounded", false);

                m_body2d.linearVelocity =
                    new Vector2(
                        m_body2d.linearVelocity.x,
                        m_jumpForce
                    );

                m_groundSensor.Disable(0.2f);
            }

            // WALL JUMP
            else if (m_isWallSliding)
            {
                m_isWallJumping = true;
                m_wallJumpTimer = m_wallJumpDuration;

                m_animator.SetTrigger("Jump");

                float jumpDirection =
                    m_facingDirection == 1 ? -1 : 1;

                m_body2d.linearVelocity = Vector2.zero;

                m_body2d.linearVelocity =
                    new Vector2(
                        jumpDirection * m_wallJumpForce.x,
                        m_wallJumpForce.y
                    );

                // Flip character
                if (jumpDirection == 1)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                    m_facingDirection = 1;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                    m_facingDirection = -1;
                }
            }
        }

        // Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        // Idle
        else
        {
            m_delayToIdle -= Time.deltaTime;

            if (m_delayToIdle < 0)
            {
                m_animator.SetInteger("AnimState", 0);
            }
        }

        if (m_isWallJumping)
        {
            m_wallJumpTimer -= Time.deltaTime;

            if (m_wallJumpTimer <= 0)
            {
                m_isWallJumping = false;
            }
        }
    }

    // Animation Event
    private void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            GameObject dust = Instantiate(
                m_slideDust,
                spawnPosition,
                transform.localRotation);

            dust.transform.localScale =
                new Vector3(m_facingDirection, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                m_body2d.linearVelocity =
                    new Vector2(m_body2d.linearVelocity.x, m_jumpForce * 0.5f);

                m_animator.SetTrigger("Jump");

                m_grounded = false;
                m_animator.SetBool("Grounded", false);
                m_groundSensor.Disable(0.2f);

                break;
            }
        }
    }


}