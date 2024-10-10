using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D rb2D;
    public int maxHealth;
    public int currentHealth;
    public float deathAnimation;
    public float animattionTimer;
    public float deathCount = 1;

    public float moveSpeed;
    public float direction;

    public float jumpForce;
    public bool isGrounded;
    public Transform checkGround;
    public float distance;
    public LayerMask isGround;
    public int numOfJump;
    public int maxNumOfJump;

    public Transform checkAttack;
    public LayerMask enemyLayer;
    public float attackDelay;
    public float attackSpeed;
    public float attackTimer = 0.0f;
    public int damage;
    public float attackRange;

    public SpriteRenderer spriteRenderer;
    /*
    public void Start()
    {
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();       
        Physics2D.queriesStartInColliders = false;
        currentHealth = maxHealth;
    }

    public void Update()
    {
        attackTimer += Time.deltaTime;
        Die();     
    }
    */

    public void TakeDamage(int damage)
    {
        Debug.Log("veli");
        //anim.SetTrigger("TakeHit");
        currentHealth -= damage;
        attackTimer = attackSpeed / 2;
    }

    public void Die()
    {
        if (currentHealth <= 0)
        {
            if (deathCount > 0)
            {
                deathCount--;
            }

            if (deathCount <= 0)
            {
                //anim.SetTrigger("Die");
                GetComponent<Rigidbody2D>().isKinematic = true;
                GetComponent<Collider2D>().enabled = false;
                rb2D.linearVelocity = new Vector2(0f, 0f);
            }

            animattionTimer += Time.deltaTime;

            if (deathAnimation <= animattionTimer)
            {
                GetComponent<Animator>().enabled = false;
                this.enabled = false;
            }
        }
    }
    /*
    public void DamageEnemy()
    {
        Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(checkAttack.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemys)
        {
            if (enemy.GetComponent<IDamageable>() != null)
            {
                Debug.Log("ali");
                enemy.GetComponent<IDamageable>().TakeDamage(damage);
            }
            
        }
    }*/

    public void Attack(bool click)
    {
        if (deathCount > 0)
        {
            if (click && attackTimer >= attackSpeed)
            {
                //anim.SetTrigger("Attack");
                Invoke("DamageEnemy", attackDelay);
                attackTimer = 0.0f;
            }
        }
    }

    public void Move(float horizontalInput)
    {
        if (direction == -1)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        if (direction == 1)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        rb2D.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb2D.linearVelocity.y);

        //anim.SetBool("isRunning", true);

    }

    public virtual void Jump(bool space)
    {
        isGrounded = Physics2D.OverlapCircle(checkGround.position, distance, isGround);

        if (space && numOfJump > 0)
        {
            rb2D.linearVelocity = Vector2.up * jumpForce;
            //anim.SetTrigger("Jump");
            numOfJump--;

        }
        if (isGrounded)
        {
            numOfJump = maxNumOfJump - 1;
        }

    }

}
