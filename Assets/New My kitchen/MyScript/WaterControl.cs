using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaterControl : MonoBehaviour
{
    [Header("Detection Settings (New)")]
    public Transform targetM;           // The designated object M to detect
    public Transform areaCenter;        // The center object (draggable in scene)
    public Vector3 areaSize = new Vector3(5, 2, 5); // Detection dimensions (X, Y, Z)
    public bool showAreaGizmo = true;   // Toggle visibility in Scene view
    public float detectionDelay = 2f;   // Required stay time (2s)

    [Header("Original Spawn Settings")]
    public GameObject objectAPrefab;
    public Transform spawnPointA;
    public Transform targetPointB;
    public Transform objectC;

    [Header("Parameter Configuration")]
    public float spawnInterval = 0.5f;
    public float moveSpeed = 5f;
    public float singleUpHeight = 0.1f;
    public float maxUpHeight = 2f;

    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    private Vector3 objectCOriginalPos;
    private float presenceTimer = 0f; // Counter for the 2s requirement

    void Start()
    {
        if (objectC != null)
        {
            objectCOriginalPos = objectC.position;
        }
        // Removed Button logic as it's now automated
    }

    void Update()
    {
        // Safety check
        if (targetM == null || areaCenter == null) return;

        // Step 1: Detect if Object M is inside the defined box
        if (IsInsideArea(targetM.position))
        {
            presenceTimer += Time.deltaTime;

            // Step 2: Trigger spawning only after staying for 2 seconds
            if (presenceTimer >= detectionDelay && !isSpawning)
            {
                StartSpawning();
            }
        }
        else
        {
            // Reset timer and stop spawning if M leaves the area
            presenceTimer = 0f;
            if (isSpawning)
            {
                StopSpawning();
            }
        }
    }

    /// <summary>
    /// Mathematical check: Is the point inside the local-space box?
    /// No Physics.Raycast or Colliders used.
    /// </summary>
    private bool IsInsideArea(Vector3 worldPos)
    {
        // Convert world position to the local space of the areaCenter
        Vector3 localPos = areaCenter.InverseTransformPoint(worldPos);

        // Check bounds on all 3 axes
        return Mathf.Abs(localPos.x) < areaSize.x / 2f &&
               Mathf.Abs(localPos.y) < areaSize.y / 2f &&
               Mathf.Abs(localPos.z) < areaSize.z / 2f;
    }

    private void StartSpawning()
    {
        isSpawning = true;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnObjectsCoroutine());
        Debug.Log("Object M detected for 2s. Starting operation...");
    }

    private void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        Debug.Log("Object M left the area. Operation stopped.");
    }

    private IEnumerator SpawnObjectsCoroutine()
    {
        while (isSpawning)
        {
            if (objectAPrefab != null && spawnPointA != null)
            {
                GameObject newObject = Instantiate(objectAPrefab, spawnPointA.position, spawnPointA.rotation);
                StartCoroutine(MoveObjectToTarget(newObject));
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator MoveObjectToTarget(GameObject moveObject)
    {
        if (moveObject == null || targetPointB == null) yield break;
        Transform objTrans = moveObject.transform;

        while (moveObject != null && Vector3.Distance(objTrans.position, targetPointB.position) > 0.01f)
        {
            objTrans.position = Vector3.MoveTowards(objTrans.position, targetPointB.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (moveObject != null)
        {
            TriggerObjectCUp();
            Destroy(moveObject);
        }
    }

    private void TriggerObjectCUp()
    {
        if (objectC == null) return;
        float currentUpHeight = objectC.position.y - objectCOriginalPos.y;
        float remainingUpHeight = maxUpHeight - currentUpHeight;

        if (remainingUpHeight > 0)
        {
            float actualUpHeight = Mathf.Min(singleUpHeight, remainingUpHeight);
            objectC.position += Vector3.up * actualUpHeight;
        }
    }

    // Visualization in the Scene View
    void OnDrawGizmos()
    {
        // Drawing existing paths
        if (spawnPointA != null && targetPointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(spawnPointA.position, targetPointB.position);
        }

        // Drawing the 3D Detection Area
        if (showAreaGizmo && areaCenter != null)
        {
            // Apply the transformation matrix to support rotation and movement
            Gizmos.matrix = Matrix4x4.TRS(areaCenter.position, areaCenter.rotation, Vector3.one);

            // Wireframe box
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, areaSize);

            // Semi-transparent solid box
            Gizmos.color = new Color(0, 1, 1, 0.1f);
            Gizmos.DrawCube(Vector3.zero, areaSize);

            // Reset matrix
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}