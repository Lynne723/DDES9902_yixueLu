using UnityEngine;

public class TimedMultiDestroyer : MonoBehaviour
{
    [Header("目标设置")]
    public Transform targetA;         // 指定检测进入区域的物体 A
    public GameObject targetCanvas;  // 指定要销毁的 Canvas
    public GameObject targetObjectB; // 指定要销毁的另一个物体 B

    [Header("三维区域设置")]
    public Transform areaCenter;     // 区域中心（可拖拽场景物体）
    public Vector3 areaSize = new Vector3(5f, 5f, 5f); // 区域尺寸

    [Header("时间设置")]
    public float requiredStayTime = 1.5f; // 停留时间阈值
    private float currentStayTimer = 0f;    // 内部计时器

    [Header("显示设置")]
    public bool showOutlineInScene = true;
    public Color outlineColor = Color.cyan;

    private void Update()
    {
        // 基础非空检查
        if (targetA == null || areaCenter == null) return;

        // 判定物体 A 是否在三维空间内
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
            // 离开区域则重置计时
            currentStayTimer = 0f;
        }
    }

    private void ExecuteDestruction()
    {
        // 销毁 Canvas
        if (targetCanvas != null)
        {
            Destroy(targetCanvas);
        }

        // 同时销毁物体 B
        if (targetObjectB != null)
        {
            Destroy(targetObjectB);
        }

        Debug.Log($"目标 A 停留满 {requiredStayTime}s，Canvas 和 物体 B 已同步销毁。");

        // 完成任务后禁用脚本，防止重复执行 Update
        this.enabled = false;
    }

    private bool IsPointInVolume(Vector3 worldPoint)
    {
        // 考虑到你之前提到的坐标修正，这里使用本地空间判定最为稳妥
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

        // 绘制线框长方体
        Gizmos.DrawWireCube(Vector3.zero, areaSize);

        // 运行时可视化：进入区域后变为半透明实心，表示正在计时
        if (Application.isPlaying && targetA != null && IsPointInVolume(targetA.position))
        {
            Gizmos.color = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0.3f);
            Gizmos.DrawCube(Vector3.zero, areaSize);
        }
    }
}