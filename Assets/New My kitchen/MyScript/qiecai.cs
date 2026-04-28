using UnityEngine;
using System.Collections;

public class SphereAreaManager : MonoBehaviour
{
    [Header("标签设置")]
    public string targetTag = "cai";

    [Header("区域设置")]
    public Transform areaCenterA;
    // --- 修改部分：将 Size 改为半径 ---
    public float areaRadiusA = 2f;
    // ----------------------------
    public Transform areaCenterB;
    public Vector3 areaSizeB = new Vector3(3f, 3f, 3f);
    public bool showGizmos = true;

    [Header("物体D 往复运动设置")]
    public GameObject objectD;
    public Transform posNodeA;
    public Transform posNodeB;
    public float moveSpeed = 2f;
    public float moveDuration = 5f;

    [Header("物体A 刷新设置")]
    public GameObject prefabA;
    public float spawnInterval = 1f;
    public float spawnDuration = 5f;

    private float timerA = 0f;
    private bool isExecuting = false;

    void Update()
    {
        CheckAreaA();
    }

    void CheckAreaA()
    {
        if (isExecuting || areaCenterA == null) return;

        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        bool anyInside = false;

        foreach (var obj in targets)
        {
            // --- 修改部分：改用距离判断 ---
            // 如果目标点与球心的距离小于等于半径，则视为在区域内
            float distance = Vector3.Distance(obj.transform.position, areaCenterA.position);
            if (distance <= areaRadiusA)
            {
                anyInside = true;
                break;
            }
            // ----------------------------
        }

        if (anyInside)
        {
            timerA += Time.deltaTime;
            if (timerA >= 0.01f)
            {
                StartCoroutine(ExecuteSequence());
                timerA = 0f;
            }
        }
        else
        {
            timerA = 0f;
        }
    }

    IEnumerator ExecuteSequence()
    {
        isExecuting = true;
        Coroutine moveRoutine = StartCoroutine(MoveObjectD());
        Coroutine spawnRoutine = StartCoroutine(SpawnObjectsInB());

        yield return new WaitForSeconds(Mathf.Max(moveDuration, spawnDuration));

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);

        isExecuting = false;
    }

    IEnumerator MoveObjectD()
    {
        if (objectD == null || posNodeA == null || posNodeB == null) yield break;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = Mathf.PingPong(Time.time * moveSpeed, 1f);
            objectD.transform.position = Vector3.Lerp(posNodeA.position, posNodeB.position, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SpawnObjectsInB()
    {
        if (prefabA == null || areaCenterB == null) yield break;
        float elapsed = 0f;
        while (elapsed < spawnDuration)
        {
            float rx = Random.Range(-areaSizeB.x / 2, areaSizeB.x / 2);
            float ry = Random.Range(-areaSizeB.y / 2, areaSizeB.y / 2);
            float rz = Random.Range(-areaSizeB.z / 2, areaSizeB.z / 2);

            Vector3 randomPos = areaCenterB.position + new Vector3(rx, ry, rz);
            Instantiate(prefabA, randomPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // --- 修改部分：绘制球体 ---
        if (areaCenterA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(areaCenterA.position, areaRadiusA);
        }
        // ----------------------------

        if (areaCenterB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(areaCenterB.position, areaSizeB);
        }

        if (posNodeA != null && posNodeB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(posNodeA.position, posNodeB.position);
            Gizmos.DrawSphere(posNodeA.position, 0.1f);
            Gizmos.DrawSphere(posNodeB.position, 0.1f);
        }
    }
}