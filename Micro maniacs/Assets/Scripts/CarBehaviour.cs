using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerCount { player1, player2}

public class CarBehaviour : MonoBehaviour {

    public playerCount player;
    public Transform camera;
    public Transform cameraPosition;
    private bool cameraTracking = true;

    [Header("Car settings")]
    public bool controller;

    public float speed;
    public float rotSpeed;

    [Header("FX")]
    public ParticleSystem DeathFX;

    private bool alive = true;
    private Rigidbody rb;
    private Vector3 checkpoint;
    private RaycastHit hitInfo;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(SetCheckPoint());
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 4 && alive)
        {
            StartCoroutine(Death());
            alive = false;
        }

        if (cameraTracking)
        {
            camera.position = cameraPosition.position;
            cameraPosition.LookAt(transform);
            camera.rotation = Quaternion.Lerp(camera.rotation, cameraPosition.rotation, 0.5f);
        }

        if (!controller)
        {
            if (player == playerCount.player1)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    rb.AddForce(transform.forward * Time.deltaTime * speed);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    rb.AddForce(transform.forward * Time.deltaTime * -speed);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
                }
            }
            if (player == playerCount.player2)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    rb.AddForce(transform.forward * Time.deltaTime * speed);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    rb.AddForce(transform.forward * Time.deltaTime * -speed);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
                }
            }
        }
        if (controller)
        {
            if (player == playerCount.player1)
            {
                rb.AddForce(transform.forward * Time.deltaTime * speed * Input.GetAxis("Vertical"));
                transform.Rotate(transform.up * Time.deltaTime * rotSpeed * Input.GetAxis("Horizontal"));
            }
        }
    }

    private IEnumerator Death()
    {
        cameraTracking = false;
        DeathFX.transform.position = transform.position;
        DeathFX.Play();
        yield return new WaitForSeconds(1);
        transform.position = checkpoint;
        cameraTracking = true;
        alive = true;
    }

    private IEnumerator SetCheckPoint()
    {
        while (true)
        {
            if (Physics.Raycast(transform.position, transform.up * -10, out hitInfo))
            {
                if (hitInfo.transform.CompareTag("Furniture"))
                {
                    checkpoint = hitInfo.point;
                   
                }
            }
            yield return new WaitForSeconds(3);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Player"))
        {
            Bump(col.transform);
        }
    }

    private void Bump(Transform collisionTransform)
    {
        float force = rb.velocity.magnitude * 200;
        if (collisionTransform.GetComponent<Rigidbody>().velocity.magnitude * 200 < force)
        {
            collisionTransform.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, 200);
            Debug.Log(force + "col" + collisionTransform.GetComponent<Rigidbody>().velocity.magnitude * 200);
        }
    }
}
