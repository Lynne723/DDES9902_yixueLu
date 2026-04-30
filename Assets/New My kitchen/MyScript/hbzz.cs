using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaSlotManager : MonoBehaviour
{
    [System.Serializable]
    public class SlotArea
    {
        public string areaName;
        public string requiredTag;
        public Transform slotCenter;
        public Vector3 areaSize = new Vector3(2f, 2f, 2f);
        [HideInInspector] public bool isOccupied = false;
        [HideInInspector] public GameObject lockedObject;
        [HideInInspector] public Vector3 targetLocalScale;
    }

    [Header("area setting")]
    public SlotArea[] leftAreas = new SlotArea[3];
    public SlotArea[] rightAreas = new SlotArea[3];

    [Header("gaz setting")]
    public bool showGizmos = true;
    [Range(0, 1)] public float entryThreshold = 0.3f;

    private List<SlotArea> occupiedAreas = new List<SlotArea>();

    void Update()
    {
        ProcessSide(leftAreas);
        ProcessSide(rightAreas);
    }

    void LateUpdate()
    {
        foreach (var area in occupiedAreas)
        {
            if (area.lockedObject != null && area.slotCenter != null)
            {
                Transform t = area.lockedObject.transform;
                if (t.parent != area.slotCenter) t.SetParent(area.slotCenter, false);

                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = area.targetLocalScale;
            }
        }
    }

    void ProcessSide(SlotArea[] sideAreas)
    {
        for (int i = 0; i < sideAreas.Length; i++)
        {
            SlotArea currentArea = sideAreas[i];
            if (currentArea.isOccupied || currentArea.slotCenter == null) continue;

            if (i > 0 && !sideAreas[i - 1].isOccupied) break;

            GameObject[] targets = GameObject.FindGameObjectsWithTag(currentArea.requiredTag);
            foreach (GameObject target in targets)
            {
                if (IsQualified(target.transform, currentArea))
                {
                    LockObjectToArea(target, currentArea);
                    break;
                }
            }
        }
    }

    bool IsQualified(Transform obj, SlotArea area)
    {
        Vector3 localPos = area.slotCenter.InverseTransformPoint(obj.position);
        bool xInside = Mathf.Abs(localPos.x) <= area.areaSize.x * 0.5f;
        bool yInside = Mathf.Abs(localPos.y) <= area.areaSize.y * 0.5f;
        bool zInside = Mathf.Abs(localPos.z) <= area.areaSize.z * 0.5f;

        if (xInside && yInside && zInside)
        {
            float offsetXPercent = Mathf.Abs(localPos.x) / (area.areaSize.x * 0.5f);
            float offsetYPercent = Mathf.Abs(localPos.y) / (area.areaSize.y * 0.5f);
            float offsetZPercent = Mathf.Abs(localPos.z) / (area.areaSize.z * 0.5f);

            return (offsetXPercent < (1f - entryThreshold) &&
                    offsetYPercent < (1f - entryThreshold) &&
                    offsetZPercent < (1f - entryThreshold));
        }
        return false;
    }

    void LockObjectToArea(GameObject obj, SlotArea area)
    {
        area.isOccupied = true;
        area.lockedObject = obj;

       
        Vector3 worldScale = obj.transform.lossyScale;
        Vector3 parentScale = area.slotCenter.lossyScale;
        area.targetLocalScale = new Vector3(
            worldScale.x / parentScale.x,
            worldScale.y / parentScale.y,
            worldScale.z / parentScale.z
        );

       
        Component[] allComponents = obj.GetComponentsInChildren<Component>();

     
        for (int i = allComponents.Length - 1; i >= 0; i--)
        {
            Component comp = allComponents[i];

            
            if (comp is Transform || comp is MeshFilter || comp is MeshRenderer)
                continue;

          
            if (comp is AreaSlotManager)
                continue;

         
            Destroy(comp);
        }

        
        occupiedAreas.Add(area);

      
        obj.transform.SetParent(area.slotCenter, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = area.targetLocalScale;

        Debug.Log($"<color=green>[lock]</color> {obj.name} clear¡£");
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        DrawSideGizmos(leftAreas);
        DrawSideGizmos(rightAreas);
    }

    void DrawSideGizmos(SlotArea[] sideAreas)
    {
        if (sideAreas == null) return;
        for (int i = 0; i < sideAreas.Length; i++)
        {
            var area = sideAreas[i];
            if (area.slotCenter == null) continue;
            Gizmos.matrix = area.slotCenter.localToWorldMatrix;
            if (area.isOccupied) Gizmos.color = Color.red;
            else if (i == 0 || sideAreas[i - 1].isOccupied) Gizmos.color = Color.green;
            else Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            Gizmos.DrawWireCube(Vector3.zero, area.areaSize);
        }
    }
}