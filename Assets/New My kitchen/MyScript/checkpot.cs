using UnityEngine;
using TMPro;

public class checkpot : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform targetA;
    public string targetTag = "Target";
    public Vector3 areaSize = new Vector3(5f, 2f, 5f);
    public Transform areaCenter;

    [Header("Visuals & UI")]
    public Light warningLight;
    public TextMeshProUGUI timeText;
    public bool showGizmos = true;

    [Header("Smoke Effect Settings")]
    public ParticleSystem smokeEffect;
    public Transform smokeSpawnPoint;

    [Header("Audio Settings")] 
    public AudioSource alarmAudio;

    [Header("Alarm Parameters")]
    public float flashSpeed = 5f;
    public float entryRequiredTime = 2f;
    public float totalRequiredTime = 10f;

    private float targetAInAreaTimer = 0f;
    private float activationTimer = 0f;
    private bool isFlashing = false;

    void Update()
    {
        if (areaCenter == null || targetA == null) return;

        bool isAInside = IsInside(targetA.position);
        bool isTagItemInside = CheckTagItemInside();

        if (isAInside)
        {
            if (targetAInAreaTimer < entryRequiredTime)
            {
                targetAInAreaTimer += Time.deltaTime;
            }
        }
        else
        {
            targetAInAreaTimer = 0f;
            ResetSystem();
            return;
        }

        if (targetAInAreaTimer >= entryRequiredTime && isTagItemInside)
        {
            if (activationTimer < totalRequiredTime)
            {
                activationTimer += Time.deltaTime;
                UpdateUI(totalRequiredTime - activationTimer, "ACTIVATING...");
            }
            else
            {
                isFlashing = true;
                UpdateUI(0, "ALARM ACTIVE");
            }
        }
        else
        {
            isFlashing = false;
            activationTimer = 0f;

            if (targetAInAreaTimer < entryRequiredTime)
            {
                UpdateUI(entryRequiredTime - targetAInAreaTimer, "AWAITING TARGET A...");
            }
            else if (!isTagItemInside)
            {
                UpdateUI(0, "WAITING FOR TAGGED OBJECT...");
            }
        }

        if (isFlashing)
        {
            HandleFlashing();

           
            if (smokeEffect != null)
            {
                if (smokeSpawnPoint != null)
                {
                    smokeEffect.transform.position = smokeSpawnPoint.position;
                }

                if (!smokeEffect.isPlaying)
                {
                    smokeEffect.Play();
                }
            }

           
            if (alarmAudio != null && !alarmAudio.isPlaying)
            {
                alarmAudio.Play();
            }
        }
        else
        {
            if (warningLight != null) warningLight.enabled = false;

            
            if (smokeEffect != null && smokeEffect.isPlaying)
            {
                smokeEffect.Stop();
            }

            
            if (alarmAudio != null && alarmAudio.isPlaying)
            {
                alarmAudio.Stop();
            }
        }
    }

    void ResetSystem()
    {
        activationTimer = 0f;
        isFlashing = false;
        if (warningLight != null) warningLight.enabled = false;
        if (smokeEffect != null) smokeEffect.Stop();

    
        if (alarmAudio != null) alarmAudio.Stop();

        UpdateUI(0, "IDLE");
    }

   
    bool IsInside(Vector3 worldPos)
    {
        Vector3 localPos = areaCenter.InverseTransformPoint(worldPos);
        return Mathf.Abs(localPos.x) < areaSize.x / 2f &&
               Mathf.Abs(localPos.y) < areaSize.y / 2f &&
               Mathf.Abs(localPos.z) < areaSize.z / 2f;
    }

    bool CheckTagItemInside()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (var obj in taggedObjects)
        {
            if (IsInside(obj.transform.position)) return true;
        }
        return false;
    }

    void HandleFlashing()
    {
        if (warningLight == null) return;
        warningLight.enabled = Mathf.Sin(Time.time * flashSpeed) > 0;
    }

    void UpdateUI(float timeLeft, string statusMessage)
    {
        if (timeText == null) return;
        if (isFlashing)
        {
            timeText.text = "WARNING: AREA BREACHED";
            timeText.color = Color.red;
        }
        else
        {
            timeText.text = $"{statusMessage}\nTime: {timeLeft:F1}s";
            timeText.color = Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || areaCenter == null) return;
        Gizmos.color = Color.cyan;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(areaCenter.position, areaCenter.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, areaSize);

        if (smokeSpawnPoint != null)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(smokeSpawnPoint.position, 0.2f);
        }
    }
}