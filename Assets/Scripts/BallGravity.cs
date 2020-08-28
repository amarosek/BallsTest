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

    public static List<BallGravity> Attractors;

    void Start()
    {
        volume = 4 * Math.PI * Math.Pow(gameObject.transform.localScale.x, 3) / 3;
    }

    void FixedUpdate()
    {
        if (GameObject.Find("Balls").GetComponent<BallSpawner>().spawnedBalls < 100)
        {
            foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius))
            {
                Vector3 forceDirection = transform.position - collider.transform.position;
                GetComponent<Rigidbody>().AddForce(-forceDirection.normalized * pullForce * Time.fixedDeltaTime);
            }
        }
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
            BallGravity script = collision.collider.GetComponent<BallGravity>();

            if (script != null && script.GetInstanceID() > GetInstanceID())
            {
                ContactPoint contact = collision.contacts[0];

                var newBall = Instantiate(ball, contact.point, Quaternion.identity);

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

                if (newBall.GetComponent<Rigidbody>().mass > 50 * smallSphereMass)
                {
                    Destroy(newBall);
                    for (int i = 0; i < 50; i++)
                    {
                        StartCoroutine(DisableCollisions());
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
