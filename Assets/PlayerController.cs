using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.02f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // FixedUpdate() gets called certain number of times per sec. Good to use with anything concerning physics
    // Nested 'success' loops allows player to slide against collision objects
    private void FixedUpdate() {
        if (canMove) { // can move when not attacking
            
            if (movementInput != Vector2.zero) { // If movement input isn't 0, move
                bool success = TryMove(movementInput);

                // if isn't successful in moving both directions whilst moving against a collision object, 
                // check to see if it can move in one direction
                if (!success) {
                    success = TryMove(new Vector2(movementInput.x, 0));

                    if (!success) {
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }
                animator.SetBool("isMoving", success);
            }
            else {
                animator.SetBool("isMoving", false);
                }

            // Set the facing direction of the player sprite to movement direction
            if (movementInput.x < 0) { // if moving left
                spriteRenderer.flipX = true;
            }
            else if (movementInput.x > 0) { // if moving right
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction) {
        // only if player is moving then walk, otherwise walking animation will continue when walking against collision object
        if (direction != Vector2.zero) { 
            // Check for potential collisions
            int count = rb.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on, such as layers to collide with
                castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset // The amount to cast equal to the movement, plus an offset
            );

            if (count == 0) { // If no collision, move 
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else return false;
        }
        else return false;
    }

    // receives argument from the parameter, which is the X and Y direction of the player movement upon key presses
    void OnMove(InputValue movementValue) {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack() {
        LockMovement();

        if (spriteRenderer.flipX == true) {
            swordAttack.AttackLeft();
        }
        else swordAttack.AttackRight();
    }

    public void EndSwordAttack() {
        UnlockMovement();
        swordAttack.StopAttack();
    }

    // prevents movement when playing is attacking
    // triggers set in Unity Animation for player_attack
    public void LockMovement() {
        canMove = false;
    }

    public void UnlockMovement() {
        canMove = true;
    }
}
