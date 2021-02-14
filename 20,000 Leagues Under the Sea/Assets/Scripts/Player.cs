using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float increaseSpeedBy = 5f;
    public float maxSpeed = 5f;

    public float maxRotation = 0.05f;
    public float rotateBy = 0.25f;
    public BaseHealth health;

    private Rigidbody2D body2D;
    private SpriteRenderer renderer2D;
    private Animator animator;
    private PlayerSubmarineController controller;
    // private Transform transform;

    private bool _isInvincible = false;
    private IEnumerator _inv;

    private int[] _gunCharges = { 0, 0, 0 };
    private Explode explode;

    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        renderer2D = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerSubmarineController>();
        health = GetComponent<BaseHealth>();
        explode = GetComponent<Explode>();
    }

    void Update()
    {

        if (health.currentHealth <= 0)
        {
            // Destroy(gameObject);            
            explode.OnExplode();
        }
        // current vertical velocity
        var velY = body2D.velocity.y;
        var rotation = transform.rotation.z;

        // If the move button is pressed, apply force upwards
        if(controller.moving.y > 0){
            if (velY < maxSpeed){
                body2D.AddForce(new Vector2(0, increaseSpeedBy));
            }

            if (rotation <= maxRotation){
                transform.Rotate(Vector3.forward * rotateBy);
            }
            else{
                transform.Rotate(Vector3.back * rotateBy);
            }
        }
        else{
            if (rotation >= -maxRotation){
                transform.Rotate(Vector3.back * rotateBy);
            }
            else{
                transform.Rotate(Vector3.forward * rotateBy);
            }
        }
    }


    public int GetGunTimer(int gunIndex) {
        if (gunIndex >= _gunCharges.Length) return 0;

        return _gunCharges[gunIndex];
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "EnemyProjectile"){
            health.takeDamage(20);
        }
    }

    public void MakeInvincible()
    {
        if (!_isInvincible) {
            _inv = Invincibility();
            StartCoroutine(_inv);
        } else {
            StopCoroutine(_inv);

            _inv = Invincibility();
            StartCoroutine(_inv);
        }
    }

    IEnumerator Invincibility() 
    {
        _isInvincible = true;

        yield return new WaitForSeconds(3);

        _isInvincible = false;
    }

    public void PickupGun(int gunIndex) {
        if (gunIndex >= _gunCharges.Length) return;

        StartCoroutine(Gun(gunIndex));
    }

    IEnumerator Gun(int i) {
        _gunCharges[i]++;

        yield return new WaitForSeconds(15);

        _gunCharges[i] = (_gunCharges[i] - 1 < 0) ? 0 : _gunCharges[i] -1;
    }
}
