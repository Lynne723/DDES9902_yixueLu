using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionRegionManager : MonoBehaviour
{
    [Header("全局配置")]
    public bool showGizmos = true;
    public string targetTag = "Pian";
    public KeyCode closeKey = KeyCode.F;
    public KeyCode manualKey = KeyCode.I;

    [Header("关键引用")]
    public Transform playerP;
    public Transform tomato;
    public GameObject d1;
    public GameObject d2;

    [System.Serializable]
    public class MissionRegion
    {
        public string name;
        public Transform center;
        public Vector3 size = new Vector3(2, 2, 2);
        public GameObject panel;
        [HideInInspector] public int activationCount = 0;
        [HideInInspector] public bool isCompleted = false;
    }

    // 请确保在Inspector中列表长度为5：0(区域1), 1(区域2), 2(区域3A), 3(区域3B), 4(区域4)
    public List<MissionRegion> regions = new List<MissionRegion>();

    private Coroutine activeUIHandler;

    void Update()
    {
        // 健壮性检查：确保列表已填充
        if (regions.Count < 5) return;

        HandleTriggerLogic();
        HandleManualActivation();
    }

    private void HandleTriggerLogic()
    {
        // 区域1: P进入
        CheckRegionTrigger(0, IsInRegion(playerP, regions[0]));

        // 区域2: P进入
        CheckRegionTrigger(1, IsInRegion(playerP, regions[1]));

        // 区域3: P在3A 且 Tomato在3B -> 销毁D1, 激活Panel3
        // index 2 是 3A, index 3 是 3B
        if (!regions[2].isCompleted && IsInRegion(playerP, regions[2]) && IsInRegion(tomato, regions[3]))
        {
            if (d1 != null) Destroy(d1);
            TriggerPanel(2); // 激活Panel3
        }

        // 区域4: P与标签Pian都在区域4，停留2s -> 销毁D2, 激活Panel4
        CheckDwellTrigger(4);
    }

    private bool IsInRegion(Transform obj, MissionRegion region)
    {
        if (obj == null || region.center == null) return false;

        Vector3 relativePos = obj.position - region.center.position;
        // 使用绝对值判断是否在长方形边界内
        return Mathf.Abs(relativePos.x) <= region.size.x * 0.5f &&
               Mathf.Abs(relativePos.y) <= region.size.y * 0.5f &&
               Mathf.Abs(relativePos.z) <= region.size.z * 0.5f;
    }

    private void CheckRegionTrigger(int index, bool condition)
    {
        if (condition && !regions[index].isCompleted)
        {
            TriggerPanel(index);
        }
    }

    private float dwellTimer = 0f;
    private void CheckDwellTrigger(int index)
    {
        if (regions[index].isCompleted) return;

        bool pIn = IsInRegion(playerP, regions[index]);
        GameObject pianObj = GameObject.FindWithTag(targetTag);
        bool pianIn = pianObj != null && IsInRegion(pianObj.transform, regions[index]);

        if (pIn && pianIn)
        {
            dwellTimer += Time.deltaTime;
            if (dwellTimer >= 2f) // 满足2秒
            {
                if (d2 != null) Destroy(d2);
                TriggerPanel(index); // 激活Panel4
                dwellTimer = 0;
            }
        }
        else
        {
            dwellTimer = 0;
        }
    }

    private void HandleManualActivation()
    {
        if (Input.GetKeyDown(manualKey))
        {
            // Panel2激活次数为0，按i激活Panel1
            if (regions[1].activationCount == 0) TriggerPanel(0, true);
            // Panel3激活次数为0，按i激活Panel2
            else if (regions[2].activationCount == 0) TriggerPanel(1, true);
            // Panel4激活次数为0，按i激活Panel3
            else if (regions[4].activationCount == 0) TriggerPanel(2, true);
        }
    }

    public void TriggerPanel(int index, bool isManual = false)
    {
        if (regions[index].panel == null) return;

        // 如果是非手动触发（即进入区域触发），标记为已完成，下次进入不再触发
        if (!isManual) regions[index].isCompleted = true;

        regions[index].activationCount++;

        if (activeUIHandler != null) StopCoroutine(activeUIHandler);
        activeUIHandler = StartCoroutine(PanelSequence(regions[index].panel));
    }

    IEnumerator PanelSequence(GameObject targetUI)
    {
        targetUI.SetActive(true);
        float elapsed = 0f;
        while (elapsed < 1.5f) // 修正 1.5s 为 1.5f
        {
            elapsed += Time.deltaTime;
            if (Input.GetKeyDown(closeKey)) break; // 监听F键
            yield return null;
        }
        targetUI.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || regions == null) return;

        for (int i = 0; i < regions.Count; i++)
        {
            if (regions[i].center != null)
            {
                // 给不同区域一点颜色区分（可选）
                Gizmos.color = (i == 4) ? Color.red : Color.cyan;
                Gizmos.DrawWireCube(regions[i].center.position, regions[i].size);
            }
        }
    }
}