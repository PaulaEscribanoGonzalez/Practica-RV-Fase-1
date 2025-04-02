using System.Collections;
using UnityEngine;
using TMPro;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform player;
    public float spawnDistance = 5f;
    public float spawnInterval = 2.5f;
    public float speed = 3f;
    public float destinationOffsetRange = 2f;
    public TextMeshPro scoreText;
    private int score = 0;

    private Transform rightHand;
    private Transform leftHand;

    void Start()
    {
        FindControllers();
        StartCoroutine(SpawnCubes());
    }

    void FindControllers()
    {
        // Buscar XR Interaction Setup
        GameObject xrSetup = GameObject.Find("XR Interaction Setup");
        if (xrSetup == null)
        {
            Debug.LogError("Error: No se encontró 'XR Interaction Setup' en la jerarquía.");
            return;
        }

        // Buscar XR Origin (XR Rig) dentro de XR Interaction Setup
        Transform xrOrigin = xrSetup.transform.Find("XR Origin (XR Rig)");
        if (xrOrigin == null)
        {
            Debug.LogError("Error: No se encontró 'XR Origin (XR Rig)' dentro de 'XR Interaction Setup'.");
            return;
        }

        // Buscar Camera Offset dentro de XR Origin (XR Rig)
        Transform cameraOffset = xrOrigin.Find("Camera Offset");
        if (cameraOffset == null)
        {
            Debug.LogError("Error: No se encontró 'Camera Offset' dentro de 'XR Origin (XR Rig)'.");
            return;
        }

        // Buscar Right Controller dentro de Camera Offset
        rightHand = cameraOffset.Find("Right Controller");
        if (rightHand == null)
        {
            Debug.LogError("Error: No se encontró 'Right Controller' dentro de 'Camera Offset'.");
        }

        // Buscar Left Controller dentro de Camera Offset
        leftHand = cameraOffset.Find("Left Controller");
        if (leftHand == null)
        {
            Debug.LogError("Error: No se encontró 'Left Controller' dentro de 'Camera Offset'.");
        }

        if (rightHand != null && leftHand != null)
        {
            Debug.Log("Ambos controladores encontrados correctamente.");
        }
    }

    IEnumerator SpawnCubes()
    {
        while (score < 20)
        {
            SpawnCube();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnCube()
    {
        if (cubePrefab == null) Debug.LogError("Error: cubePrefab no está asignado.");
        if (player == null) Debug.LogError("Error: player no está asignado.");
        if (scoreText == null) Debug.LogError("Error: scoreText no está asignado.");

        if (cubePrefab == null || player == null || scoreText == null) return;

        if (rightHand == null && leftHand == null)
        {
            Debug.LogError("Error: No se encontraron los controladores.");
            return;
        }

        // Elegir aleatoriamente entre Right Controller y Left Controller
        Transform chosenHand = (Random.value > 0.5f && rightHand != null) ? rightHand : leftHand;

        // Generar el cubo en la dirección del controlador pero con Y = 1.5 (más cerca de la altura del sable)
        Vector3 spawnPos = chosenHand.position + chosenHand.forward * spawnDistance;
        spawnPos.y = 1.5f; // Ahora los cubos siempre aparecen a Y=1.5

        // Desviación aleatoria para que no siempre vayan al mismo punto
        float offset = Random.Range(-destinationOffsetRange, destinationOffsetRange);
        Vector3 targetPos = new Vector3(player.position.x + offset, 1.5f, player.position.z);
        Vector3 direction = (targetPos - spawnPos).normalized;

        Debug.Log("Instanciando cubo en posición: " + spawnPos + " desde " + chosenHand.name);

        GameObject cube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);

        if (cube == null)
        {
            Debug.LogError("Error: No se pudo instanciar el cubo.");
            return;
        }

        Debug.Log("Cubo instanciado correctamente.");

        // Verifica si el prefab YA tiene Rigidbody y Collider antes de agregar otro.
        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = cube.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }

        if (cube.GetComponent<BoxCollider>() == null)
        {
            cube.AddComponent<BoxCollider>();
        }

        CubeBehavior behavior = cube.GetComponent<CubeBehavior>();
        if (behavior == null)
        {
            behavior = cube.AddComponent<CubeBehavior>();
        }

        if (behavior == null)
        {
            Debug.LogError("Error: No se pudo agregar o encontrar el script CubeBehavior.");
            return;
        }

        behavior.Initialize(direction, speed, this);
    }


    public void IncreaseScore()
    {
        score++;
        scoreText.text = "Score: " + score;

        if (score >= 20)
        {
            scoreText.text = "Game Over!";  // Cambiar el texto a "Game Over!" cuando el jugador llegue a 20 puntos
            Debug.Log("Game Over!");  // También lo mostramos en la consola
            StopAllCoroutines();  // Detener la instanciación de cubos
        }
    }

}
