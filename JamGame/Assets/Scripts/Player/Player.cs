using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.Cinemachine;

public class Player : Creature
{
    float dashTimer;
    public float dashCoolDown;

    private float jumpTimeCounter;
    public float jumpTime;
    public bool isJumping;
    public float cayoteTimeCounter;
    public float cayoteTime;
    private float jumpBlocker = 0.2f;
    public float jumpBufferTime;
    private float jumpBufferCounter;

    [SerializeField] private LayerMask _cornerCorrectLayer;
    [SerializeField] private float _topRaycastLength;
    [SerializeField] private Vector3 _edgeRaycastOffset;
    [SerializeField] private Vector3 _innerRaycastOffset;
    private bool _canCornerCorrect;


    [SerializeField] private float slidingSpeed;
    public LayerMask wall;
    public Transform checkWall;
    public float wallDistance;
    public float jumpedPositionX = 6666;

    public Transform[] checkwalls;

    float horizontalInput;
    public CinemachineCamera virtualCamera;

    [SerializeField] private AudioListener audioListener;

    private void Awake()
    {
        maxHealth = 1;
        moveSpeed = 6;
        direction = 1;
        jumpForce = 10;
        checkGround = FindChildObjectByName("CheckGround");
        distance = 0.1f;
        isGround = LayerMask.GetMask("Ground");
        maxNumOfJump = 2;
        checkAttack = FindChildObjectByName("CheckAttack");
        enemyLayer = LayerMask.GetMask("Creature");
        jumpTime = 0.25f;
        cayoteTime = 0.2f;
        jumpBufferTime = 0.2f;
        _cornerCorrectLayer = LayerMask.GetMask("Wall","Ground");
        _topRaycastLength = 0.85f;
        _edgeRaycastOffset.x = 0.4f;
        _innerRaycastOffset.x = 0.18f;
        slidingSpeed = 1;
        wall = LayerMask.GetMask("Wall");
        checkWall = FindChildObjectByName("CheckWall");
        wallDistance = 0.1f;
        jumpedPositionX = 6666;
        Transform wallChecks = FindChildObjectByName("WallChecks");
        for (int i = 0; i<10; i++)
        {
            checkwalls[i] = wallChecks.GetChild(i); 
        }
        //virtualCamera = FindChildObjectByName("Virtual Camera").GetComponent<CinemachineVirtualCamera>();       
    }
    public void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        // anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        Physics2D.queriesStartInColliders = false;
        currentHealth = maxHealth;
        dashTimer = dashCoolDown;     
    }

    public void Update()
    {
        dashTimer += Time.deltaTime;
        Dash();
        attackTimer += Time.deltaTime;
        Die();
        
        Attack(Input.GetKeyDown((KeyCode.F)));
                
        Jump(Input.GetKeyDown((KeyCode.Space)));     
    }

    private void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput > 0)
        {
            direction = 1;
            Move(horizontalInput);
        }
        else if (horizontalInput < 0)
        {
            direction = -1;
            Move(horizontalInput);
        }
        else
        {
            rb2D.linearVelocity = new Vector2(0f, rb2D.linearVelocity.y);
        }
        if (rb2D.linearVelocity.x == 0)
        {
            //anim.SetBool("isRunning", false);
        }
        CheckCollisions();
        if (_canCornerCorrect) CornerCorrect(rb2D.linearVelocity.y);

        WallSlide();
    }

    public void Dash()
    {
        bool isThereAWall = true;

        foreach (Transform checkwall2 in checkwalls)
        {
            if(Physics2D.OverlapCircle(checkwall2.position, 0.7f, wall))
            {
                isThereAWall = false; break;
            }
            
        }

        if (Input.GetKeyDown((KeyCode.Q)) && dashCoolDown <= dashTimer)
        {
            moveSpeed = 20;
            
            StartCoroutine(Waiter(0.2f, "Dash"));
            Move(direction);
            if(isThereAWall)
            {
                rb2D.isKinematic = true;
            }
            
        }
    }

    IEnumerator Waiter(float second, string functionName)
    {
        yield return new WaitForSeconds(second);

        if (functionName == "Dash")
        {
            dashTimer = 0;
            moveSpeed = 6;
            rb2D.isKinematic = false;
        }
    }

    public override void Jump(bool space)
    {
        isGrounded = Physics2D.OverlapCircle(checkGround.position, distance, isGround);
      
        if (isGrounded && jumpBlocker<=0)
        {
            cayoteTimeCounter = cayoteTime;
        }
        else
        {
            cayoteTimeCounter -= Time.deltaTime;
        }

        if (cayoteTimeCounter>0)
        {
            numOfJump = maxNumOfJump;
        }
        else if(cayoteTimeCounter <= 0 && numOfJump==maxNumOfJump)
        {
            numOfJump = 0;
        }

        if (space)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }


        if (jumpBufferCounter > 0 && numOfJump > 0)
        {
            jumpedPositionX = this.gameObject.transform.position.x;
            rb2D.linearVelocity = Vector2.up * jumpForce;
            //anim.SetTrigger("Jump");
            numOfJump--;
            jumpTimeCounter = jumpTime;
            isJumping = true;
            cayoteTimeCounter = 0;
            jumpBlocker = 0.2f;
            jumpBufferCounter = 0;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb2D.linearVelocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
        jumpBlocker -= Time.deltaTime;
    }

    void CornerCorrect(float Yvelocity)
    {
        //Push player to the right
        RaycastHit2D _hit = Physics2D.Raycast(transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength, Vector3.left, _topRaycastLength, _cornerCorrectLayer);
        if (_hit.collider != null)
        {
            float _newPos = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * _topRaycastLength,
                transform.position - _edgeRaycastOffset + Vector3.up * _topRaycastLength);
            transform.position = new Vector3(transform.position.x + _newPos, transform.position.y, transform.position.z);
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Yvelocity);
            return;
        }

        //Push player to the left
        _hit = Physics2D.Raycast(transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength, Vector3.right, _topRaycastLength, _cornerCorrectLayer);
        if (_hit.collider != null)
        {
            float _newPos = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * _topRaycastLength,
                transform.position + _edgeRaycastOffset + Vector3.up * _topRaycastLength);
            transform.position = new Vector3(transform.position.x - _newPos, transform.position.y, transform.position.z);
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Yvelocity);
        }
    }

    private void CheckCollisions()
    {
        //Corner Collisions
        _canCornerCorrect = Physics2D.Raycast(transform.position + _edgeRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) &&
                            !Physics2D.Raycast(transform.position + _innerRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) ||
                            Physics2D.Raycast(transform.position - _edgeRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer) &&
                            !Physics2D.Raycast(transform.position - _innerRaycastOffset, Vector2.up, _topRaycastLength, _cornerCorrectLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //Corner Check
        Gizmos.DrawLine(transform.position + _edgeRaycastOffset, transform.position + _edgeRaycastOffset + Vector3.up * _topRaycastLength);
        Gizmos.DrawLine(transform.position - _edgeRaycastOffset, transform.position - _edgeRaycastOffset + Vector3.up * _topRaycastLength);
        Gizmos.DrawLine(transform.position + _innerRaycastOffset, transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength);
        Gizmos.DrawLine(transform.position - _innerRaycastOffset, transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength);

        //Corner Distance Check
        Gizmos.DrawLine(transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength,
                        transform.position - _innerRaycastOffset + Vector3.up * _topRaycastLength + Vector3.left * _topRaycastLength);
        Gizmos.DrawLine(transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength,
                        transform.position + _innerRaycastOffset + Vector3.up * _topRaycastLength + Vector3.right * _topRaycastLength);

    }

    public void WallSlide()
    {
        bool isInWall = Physics2D.OverlapCircle(checkWall.position, wallDistance, wall);
        if(isInWall && !isGrounded && horizontalInput != 0)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, Mathf.Clamp(rb2D.linearVelocity.y,-slidingSpeed,float.MaxValue));
            if(jumpedPositionX==6666 || Mathf.Abs(this.gameObject.transform.position.x - jumpedPositionX) >= 2)
            {
                numOfJump = 1;
            }        
        }
    }

    private Transform FindChildObjectByName(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    

}

