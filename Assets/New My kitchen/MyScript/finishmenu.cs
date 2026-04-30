using UnityEngine;
using System.Collections;

public class finishmenu : MonoBehaviour
{
    [Header("area setting")]
    public Transform zoneCenter;      
    public Vector3 zoneSize = new Vector3(5, 5, 5); 
    public bool showGizmos = true;    
    public Color gizmoColor = Color.cyan;

    [Header("value setting")]
    public string targetTag = "Player";
    public GameObject targetCanvas;
    public float stayThreshold = 3f;  
    public float destructionDelay = 2f; 

    private float timer = 0f;
    private bool isCanvasTriggered = false;
    private Transform targetTransform;

    void Update()
    {
        
        if (targetTransform == null)
        {
            GameObject targetObj = GameObject.FindWithTag(targetTag);
            if (targetObj != null) targetTransform = targetObj.transform;
            return;
        }

        
        if (isCanvasTriggered) return;

       
        if (IsPointInBox(targetTransform.position))
        {
            timer += Time.deltaTime;
            if (timer >= stayThreshold)
            {
                TriggerCanvasSequence();
            }
        }
        else
        {
            timer = 0f; 
        }
    }

    
    bool IsPointInBox(Vector3 point)
    {
        if (zoneCenter == null) return false;

        
        Bounds bounds = new Bounds(zoneCenter.position, zoneSize);
        return bounds.Contains(point);
    }

    void TriggerCanvasSequence()
    {
        isCanvasTriggered = true;
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(true);
            StartCoroutine(DestroyAfterDelay());
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destructionDelay);
        if (targetCanvas != null)
        {
            Destroy(targetCanvas);
        }
        
        
    }

    
    private void OnDrawGizmos()
    {
        if (showGizmos && zoneCenter != null)
        {
            Gizmos.color = gizmoColor;
           
            Gizmos.DrawWireCube(zoneCenter.position, zoneSize);

            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.1f);
            Gizmos.DrawCube(zoneCenter.position, zoneSize);
        }
    }
}