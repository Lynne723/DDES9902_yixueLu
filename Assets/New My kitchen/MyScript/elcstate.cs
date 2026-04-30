using UnityEngine;

public class elcstate : MonoBehaviour
{
    [Header("object setting")]
    public Transform objectA;
    public Transform targetPos;
    public GameObject objectB;
    public GameObject objectC;

    [Header("value setting")]
    public float distanceThreshold = 0.5f;
    public float stayTimeRequired = 1.0f;

    private float timer = 0f;
    private bool isStateSwapped = false;

    void Update()
    {
        if (objectA == null || targetPos == null || objectB == null || objectC == null) return;


        float distance = Vector3.Distance(objectA.position, targetPos.position);

        if (distance <= distanceThreshold)
        {

            if (!isStateSwapped)
            {
                timer += Time.deltaTime;

                if (timer >= stayTimeRequired)
                {
                    SetTriggerState(true);
                }
            }
        }
        else
        {

            timer = 0f;
            if (isStateSwapped)
            {
                SetTriggerState(false);
            }
        }
    }


    void SetTriggerState(bool swapped)
    {
        isStateSwapped = swapped;
        objectB.SetActive(swapped);
        objectC.SetActive(!swapped);

        Debug.Log(swapped ? "approach pos£ºB active/C inact" : "already leave pos£ºB inact/C active");
    }
}