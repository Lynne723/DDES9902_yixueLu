using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class linetact : MonoBehaviour
{
    public Transform anchorA;
    public Transform targetB;
    public float sagAmount = 2.0f;
    public int resolution = 20;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
    }

    void Update()
    {
        if (anchorA == null || targetB == null) return;

        DrawRope();
    }

    void DrawRope()
    {
        Vector3 start = anchorA.position;
        Vector3 end = targetB.position;


        Vector3 midPoint = (start + end) / 2f;
        Vector3 controlPoint = midPoint + Vector3.down * sagAmount;

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);

            Vector3 point = CalculateBezierPoint(t, start, controlPoint, end);
            lineRenderer.SetPosition(i, point);
        }
    }


    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        //  (1-t)^2 * p0 + 2(1-t)t * p1 + t^2 * p2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}