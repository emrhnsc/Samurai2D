using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask enemyLayer;

    [Header("Attack")]
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] int attackDamege = 40;
    [SerializeField] float attackRate = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Movement")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    bool isAlive = true;
    bool isGrounded;

    [Header("Audio")]
    [SerializeField] AudioClip swordClip;
    [SerializeField][Range(0f, 1f)] float swordVolume = 1f;
    [SerializeField] AudioClip footstepClip;
    [SerializeField][Range(0f, 1f)] float footstepVolume = 1f;
    private AudioSource audioSource;


    BoxCollider2D feetCollider;
    Vector2 moveInput;
    Rigidbody2D rb2d;
    Animator animator;


    //Scripts
    CameraShake cameraShake;
    EnemyController enemyController;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        feetCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        //Scripts
        cameraShake = GameObject.FindGameObjectWithTag("ShakeCam").GetComponent<CameraShake>();
        enemyController = GetComponent<EnemyController>();
    }


    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        GroundCheck();
        JumpAnimationState();
    }

    void JumpAnimationState()
    {
        if (isGrounded)
        {
            animator.SetBool("Jump", false);
        }
        else
        {
            animator.SetBool("Jump", true);
        }
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if (value.isPressed)
        {
            rb2d.velocity += new Vector2(0f, jumpForce);
            JumpAnimationState();

        }
    }


    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }

        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }


    void Attack()
    {
        animator.SetTrigger("Attack");
        AudioSource.PlayClipAtPoint(swordClip, Camera.main.transform.position, swordVolume);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (CapsuleCollider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyController>().TakeDamage(attackDamege);
            if (enemy.transform.position.x > transform.position.x)
            {
                cameraShake.CamShakeRight();
            }
            else
            {
                cameraShake.CamShakeLeft();
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb2d.velocity.y);
        rb2d.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        animator.SetBool("Run", playerHasHorizontalSpeed);
    }

    void RunSFX()
    {
        audioSource.pitch = Random.Range(0f, 1f);
        AudioSource.PlayClipAtPoint(footstepClip, Camera.main.transform.position, footstepVolume);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector3(Mathf.Sign(rb2d.velocity.x), 1f);
        }
    }

    void GroundCheck()
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}

