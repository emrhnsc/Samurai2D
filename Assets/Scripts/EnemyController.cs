using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] float dieDuration = 5f;

    [Header("Audio")]
    [SerializeField] AudioClip swordHitClip;
    [SerializeField][Range(0f, 1f)] float swordHitVolume = 1f;
    [SerializeField] AudioClip deathClip;
    [SerializeField][Range(0f, 1f)] float deathVolume = 1f;

    int currentHealth;
    [HideInInspector] public bool isAlive;

    Animator animator;
    SpriteRenderer spriteRenderer;
    public Transform playerPosition;
    CircleCollider2D circleCollider;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerPosition = FindObjectOfType<PlayerMovement>().GetComponent<Transform>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    void Update()
    {
        if (!isAlive) { return; }
        FlipEnemyToPlayer();
    }

    void FlipEnemyToPlayer()
    {
        if (playerPosition.transform.position.x < transform.position.x) //player on the left
        {
            transform.localScale = new Vector3(-1, 1);
        }
        else if (playerPosition.transform.position.x > transform.position.x) // player on the right
        {
            transform.localScale = new Vector3(1, 1);
        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("TakeHitW");

        AudioSource.PlayClipAtPoint(swordHitClip, Camera.main.transform.position, swordHitVolume);

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isAlive = false;
        animator.SetTrigger("Die");

        AudioSource.PlayClipAtPoint(deathClip, Camera.main.transform.position, deathVolume);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        yield return new WaitForSeconds(dieDuration);

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
