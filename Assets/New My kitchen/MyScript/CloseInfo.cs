using UnityEngine;

public class TimedMultiDestroyer : MonoBehaviour
{
    [Header("target setting")]
    public Transform targetA;         
    public GameObject targetCanvas; 
    public GameObject targetObjectB; 

    [Header("area setting")]
    public Transform areaCenter;     
    public Vector3 areaSize = new Vector3(5f, 5f, 5f); 

    [Header("time setting")]
    public float requiredStayTime = 1.5f;
    private float currentStayTimer = 0f; 

    [Header("giz setting")]
    public bool showOutlineInScene = true;
    public Color outlineColor = Color.cyan;

    private void Update()
    {
       
        if (targetA == null || areaCenter == null) return;

        
        if (IsPointInVolume(targetA.position))
        {
            currentStayTimer += Time.deltaTime;

            if (currentStayTimer >= requiredStayTime)
            {
                ExecuteDestruction();
            }
        }
        else
        {
            
            currentStayTimer = 0f;
        }
    }

    private void ExecuteDestruction()
    {
       
        if (targetCanvas != null)
        {
            Destroy(targetCanvas);
        }

       
        if (targetObjectB != null)
        {
            Destroy(targetObjectB);
        }

        Debug.Log($"target A stay  {requiredStayTime}s£¬Canvas and object B done destory¡£");

        
        this.enabled = false;
    }

    private bool IsPointInVolume(Vector3 worldPoint)
    {
        
        Vector3 localPoint = areaCenter.InverseTransformPoint(worldPoint);

        return Mathf.Abs(localPoint.x) <= (areaSize.x / 2f) &&
               Mathf.Abs(localPoint.y) <= (areaSize.y / 2f) &&
               Mathf.Abs(localPoint.z) <= (areaSize.z / 2f);
    }

    private void OnDrawGizmos()
    {
        if (!showOutlineInScene || areaCenter == null) return;

        Gizmos.color = outlineColor;
        Gizmos.matrix = areaCenter.localToWorldMatrix;

        
        Gizmos.DrawWireCube(Vector3.zero, areaSize);

        
        if (Application.isPlaying && targetA != null && IsPointInVolume(targetA.position))
        {
            Gizmos.color = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.3f);
            Gizmos.DrawCube(Vector3.zero, areaSize);
        }
    }
}