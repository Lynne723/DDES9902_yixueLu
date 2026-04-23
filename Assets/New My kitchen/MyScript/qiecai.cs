using UnityEngine;
using System.Collections;

public class RectangleAreaManager : MonoBehaviour
{
    [Header("Tag Settings")]
    public string targetTag = "cai";    // The tag of objects to detect in Area A

    [Header("Area Settings")]
    public Transform areaCenterA;      // Center reference for Area A
    public Vector3 areaSizeA = new Vector3(2f, 2f, 2f);
    public Transform areaCenterB;      // Center reference for Area B
    public Vector3 areaSizeB = new Vector3(3f, 3f, 3f);
    public bool showGizmos = true;    // Toggle visibility of area outlines in Scene view

    [Header("Object D Reciprocating Motion")]
    public GameObject objectD;
    public Transform posNodeA;         // Start position handle (draggable in Scene)
    public Transform posNodeB;         // End position handle (draggable in Scene)
    public float moveSpeed = 2f;
    public float moveDuration = 5f;    // Total time the movement cycle lasts

    [Header("Object A Spawning")]
    public GameObject prefabA;
    public float spawnInterval = 1f;   // Time between each spawn
    public float spawnDuration = 5f;   // Total time the spawning process lasts

    private float timerA = 0f;
    private bool isExecuting = false;

    void Update()
    {
        CheckAreaA();
    }

    // 1. Spatial Detection (Rectangle/Box Bounds)
    void CheckAreaA()
    {
        if (isExecuting || areaCenterA == null) return;

        // Create a mathematical bounding box for Area A
        Bounds boundsA = new Bounds(areaCenterA.position, areaSizeA);

        // Find all objects with the specified tag
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        bool anyInside = false;

        foreach (var obj in targets)
        {
            // Check if the object's pivot point is inside the box
            if (boundsA.Contains(obj.transform.position))
            {
                anyInside = true;
                break;
            }
        }

        if (anyInside)
        {
            timerA += Time.deltaTime;
            // Trigger sequence if an object stays for more than 2 seconds
            if (timerA >= 2f)
            {
                StartCoroutine(ExecuteSequence());
                timerA = 0f; // Reset timer to prevent multiple triggers
            }
        }
        else
        {
            timerA = 0f; // Reset timer if the area is empty or sequence is interrupted
        }
    }

    // 2. Main Execution Sequence
    IEnumerator ExecuteSequence()
    {
        isExecuting = true;

        // Start both motion and spawning coroutines simultaneously
        Coroutine moveRoutine = StartCoroutine(MoveObjectD());
        Coroutine spawnRoutine = StartCoroutine(SpawnObjectsInB());

        // Wait for the defined duration (max of move or spawn durations)
        yield return new WaitForSeconds(Mathf.Max(moveDuration, spawnDuration));

        // Stop behaviors after time expires
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);

        isExecuting = false;
    }

    // 3. Object D Movement Logic (Ping-Pong)
    IEnumerator MoveObjectD()
    {
        if (objectD == null || posNodeA == null || posNodeB == null) yield break;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            // Calculate interpolation factor using PingPong for back-and-forth motion
            float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
            objectD.transform.position = Vector3.Lerp(posNodeA.position, posNodeB.position, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // 4. Random Spawning in Area B
    IEnumerator SpawnObjectsInB()
    {
        if (prefabA == null || areaCenterB == null) yield break;

        float elapsed = 0f;
        while (elapsed < spawnDuration)
        {
            // Generate a random position within the box dimensions
            float rx = Random.Range(-areaSizeB.x / 2, areaSizeB.x / 2);
            float ry = Random.Range(-areaSizeB.y / 2, areaSizeB.y / 2);
            float rz = Random.Range(-areaSizeB.z / 2, areaSizeB.z / 2);

            Vector3 randomPos = areaCenterB.position + new Vector3(rx, ry, rz);
            Instantiate(prefabA, randomPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }
    }

    // 5. Visualize Areas and Paths in Scene View
    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Draw Detection Area A (Green)
        if (areaCenterA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(areaCenterA.position, areaSizeA);
        }

        // Draw Spawn Area B (Cyan)
        if (areaCenterB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(areaCenterB.position, areaSizeB);
        }

        // Draw Movement Path for Object D (Yellow)
        if (posNodeA != null && posNodeB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(posNodeA.position, posNodeB.position);
            Gizmos.DrawSphere(posNodeA.position, 0.1f);
            Gizmos.DrawSphere(posNodeB.position, 0.1f);
        }
    }
}