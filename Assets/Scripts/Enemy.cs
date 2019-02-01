//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject laser;
    [SerializeField] float projectileSpeed = 10f;
    

    [Header("Enemy FX")]
    [SerializeField] GameObject particleVFX;
    [SerializeField] float deathVFXTime = 1f;
    [SerializeField] AudioClip enemyExplosionSFX;
    [SerializeField] [Range(0, 1)] float explosionSoundVolume = 0.75f;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.75f;
    [SerializeField] AudioClip shootSound;

    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0)
        {
            Fire();
        }
    }

    private void Fire()
    {
        GameObject firedLaser = Instantiate(this.laser, 
            transform.position,
            Quaternion.identity) as GameObject;
        firedLaser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1 * projectileSpeed);
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
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
        GameObject explosionVFX = Instantiate(particleVFX, transform.position, transform.rotation);
        Destroy(explosionVFX, deathVFXTime);
        AudioSource.PlayClipAtPoint(enemyExplosionSFX, Camera.main.transform.position, explosionSoundVolume);
    }
}
