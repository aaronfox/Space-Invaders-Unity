using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Config params
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = .05f;
    [SerializeField] GameObject laser;
    [SerializeField] float laserSpeed = 11f;
    [SerializeField] float projectileFiringPeriod = 0.1f;


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
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }
}
