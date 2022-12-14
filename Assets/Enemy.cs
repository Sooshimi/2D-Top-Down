using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;

    public float health = 2;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public float Health {
        set {
            health = value;
            if (health <= 0) {
                Defeated();
            }
        }
        get {
            return health;
        }
    }

    public void Defeated() {
        animator.SetTrigger("defeated");
    }

    public void RemoveEnemy() {
        Destroy(gameObject);
    }
}
