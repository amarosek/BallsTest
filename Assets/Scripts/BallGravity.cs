using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGravity : MonoBehaviour
{
    [SerializeField] float pullRadius = 100;
    [SerializeField] float pullForce = 2;
    [SerializeField] GameObject ball;

    public int mergesCount = 0;
    public bool collided = false;

    public double volume;
    public double radius;
    const float smallSphereMass = 1;
    const float smallSphereScale = 0.3f;

    void Start()
    {
        // calculate ball volume for future ball connection logic
        volume = 4 * Math.PI * Math.Pow(gameObject.transform.localScale.x, 3) / 3;
    }

    void FixedUpdate()
    {
        // if statement for balls gravity logic when under 100 balls in scene
        if (GameObject.Find("Balls").GetComponent<BallSpawner>().spawnedBalls < 100)
        {

            // adding artificial "gravity" to every object appearing in scene
            foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius))
            {
                Vector3 forceDirection = transform.position - collider.transform.position;
                GetComponent<Rigidbody>().AddForce(-forceDirection.normalized * pullForce * Time.fixedDeltaTime);
            }
        }

        // reverse gravity pull if balls count higher than 100
        else
        {
            foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius))
            {
                Vector3 forceDirection = transform.position - collider.transform.position;
                GetComponent<Rigidbody>().AddForce(forceDirection.normalized * pullForce * Time.fixedDeltaTime);
                GetComponent<BallGravity>().collided = true;
            }
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (!collided)
        {

            // check ball ids to create only one ball from collision
            BallGravity script = collision.collider.GetComponent<BallGravity>();

            if (script != null && script.GetInstanceID() > GetInstanceID())
            {
                // set contact between balls point
                ContactPoint contact = collision.contacts[0];

                // create new ball from ball prefab, contact point and without rotation
                var newBall = Instantiate(ball, contact.point, Quaternion.identity);

                // set newBall properties as new volume, new radius, new mass, velocity vector and pullForce
                volume += script.volume;
                radius = Math.Pow((3 * volume) / (4 * Mathf.PI), 1D / 3);
                newBall.transform.localScale = new Vector3((float)radius, (float)radius, (float)radius);

                newBall.GetComponent<Rigidbody>().velocity = new Vector3(gameObject.transform.localScale.x + script.transform.localScale.x,
                                                                         gameObject.transform.localScale.y + script.transform.localScale.y,
                                                                         gameObject.transform.localScale.z + script.transform.localScale.z);
                newBall.GetComponent<Rigidbody>().mass = gameObject.GetComponent<Rigidbody>().mass + script.GetComponent<Rigidbody>().mass;
                newBall.GetComponent<BallGravity>().mergesCount++;
                newBall.GetComponent<BallGravity>().pullForce = newBall.GetComponent<Rigidbody>().mass;

                collided = true;

                // if statement for destroying balls with mass higher than 50 small balls masses
                if (newBall.GetComponent<Rigidbody>().mass > 50 * smallSphereMass)
                {
                    // destroy potential ball with mass bigger than 50
                    Destroy(newBall);

                    // create 50 balls from last collision
                    for (int i = 0; i < 50; i++)
                    {

                        // disable collisions for 0.5s 
                        StartCoroutine(DisableCollisions());

                        // set newBall properties as new radius, new mass, random velocity vector and pullForce
                        newBall = Instantiate(ball, contact.point, Quaternion.identity);
                        newBall.GetComponent<Rigidbody>().mass = 1;
                        newBall.GetComponent<BallGravity>().pullForce = 1;
                        newBall.transform.localScale = new Vector3(smallSphereScale, smallSphereScale, smallSphereScale);
                        newBall.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(-30, 30));
                    }
                    StopCoroutine(DisableCollisions());
                }
            }
            Destroy(gameObject);
        }
    }

    IEnumerator DisableCollisions()
    {
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;

        yield return new WaitForSeconds(0.5f);

        gameObject.GetComponent<Rigidbody>().detectCollisions = true;
    }
}
