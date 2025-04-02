using System.Collections;
using UnityEngine;
using TMPro;

public class CubeBehavior : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private CubeSpawner gameManager;

    public void Initialize(Vector3 dir, float spd, CubeSpawner manager)
    {
        direction = dir;
        speed = spd;
        gameManager = manager;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("saber"))
        {
            gameManager.IncreaseScore();
            Destroy(gameObject);
        }
    }
}

