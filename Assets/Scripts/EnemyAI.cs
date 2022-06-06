using UnityEngine;

public enum EnemyState
{
    Wander,
    Follow,
    Die,
};

public class EnemyAI : MonoBehaviour
{
    GameObject player;
    public EnemyState currState = EnemyState.Wander;

    public Transform target;

    Rigidbody2D rb2d;

    public float range = 2f;
    public float moveSpeed = 2f;

    bool doFollow = false;

    CircleCollider2D circleCollider;
    Animator animator;

    EnemyController enemyController;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyController = GetComponent<EnemyController>();
        rb2d = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!enemyController.isAlive) { return; }
        switch (currState)
        {
            case EnemyState.Wander:
                break;
            case EnemyState.Follow:
                Follow();
                break;
            case EnemyState.Die:
                break;
        }

        animator.SetBool("Run", doFollow);

        if (IsPlayerInRange(range) && currState != EnemyState.Die)
        {
            currState = EnemyState.Follow;
        }
        //else if (!IsPlayerInRange(range) && currState != EnemyState.Die)
        //{
        //    currState = EnemyState.Wander;
        //}
    }

    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }


    //void Wander()
    //{
    //    {
    //        if (IsFacingRight())
    //        {
    //            rb2d.velocity = new Vector2(moveSpeed, 0f);
    //        }
    //        else
    //        {
    //            rb2d.velocity = new Vector2(-moveSpeed, 0f);
    //        }
    //    }
    //}

    //void OnTriggerExit2D(Collider2D collision)
    //{
    //    transform.localScale = new Vector2(-(Mathf.Sign(rb2d.velocity.x)), 1f);
    //}


    void Follow()
    {
        if (doFollow && enemyController.isAlive)
        {
            if (transform.position.x > target.position.x)
            {
                //target is left
                rb2d.velocity = new Vector2(-moveSpeed, 0f);
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            }
            else if (transform.position.x < target.position.x)
            {
                //target is right
                rb2d.velocity = new Vector2(moveSpeed, 0f);
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (circleCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            doFollow = true;
        }
        else
        {
            doFollow = false;
        }

    }
}
