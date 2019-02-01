using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Config params
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = .05f;
    [SerializeField] int health = 200;

    [Header("Laser Projectile")]
    [SerializeField] GameObject laser;
    [SerializeField] float laserSpeed = 11f;
    [SerializeField] float projectileFiringPeriod = 0.1f;

    [Header("Player FX")]
    [SerializeField] AudioClip enemyExplosionSFX;
    [SerializeField] [Range(0, 1)] float explosionSoundVolume = 0.75f;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.75f;
    [SerializeField] AudioClip shootSound;

    Coroutine firingCoroutine;
    float xMin;
    float xMax;
    float yMin;
    float yMax;
    

    // Start is called before the first frame update
    void Start() {
        SetUpMoveBoundaries();
    }

    

    // Update is called once per frame
    void Update() {
        Move();
        Fire();
    }

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1")) {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if(Input.GetButtonUp("Fire1")) {
            StopCoroutine(firingCoroutine);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var newXPos = transform.position.x + deltaX;
        // transform.position = new Vector2(newXPos, transform.position.y);

        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var newYPos = transform.position.y + deltaY;
        transform.position = new Vector2(Mathf.Clamp(newXPos, xMin, xMax), Mathf.Clamp(newYPos, yMin, yMax));
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    IEnumerator FireContinuously() {

        while(true) {
            GameObject firedLaser = Instantiate(
                laser,
                transform.position,
                Quaternion.identity) as GameObject;
            firedLaser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();

        // Prevent Null Reference Exception if game object has no DamageDealer Component
        if (!damageDealer)
        {
            return;
        }
        ProcessHit(other, damageDealer);
    }

    private void ProcessHit(Collider2D other, DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die(other);
        }
    }

    private void Die(Collider2D other)
    {
        Destroy(this.gameObject);
        Destroy(other.gameObject);
        AudioSource.PlayClipAtPoint(enemyExplosionSFX, Camera.main.transform.position, explosionSoundVolume);
    }
}
