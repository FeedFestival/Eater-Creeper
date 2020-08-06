using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurveRenderer : MonoBehaviour
{
    public BezierCurve Path;
    
    //

    private LineRenderer LineRenderer;
    private int VertexCount = 200;

    void Awake()
    {
        LineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void Start()
    {
        DrawCubicCurve();
    }

    void DrawCubicCurve(/*.anchorPoints*/)
    {
        var pathPoints = Path.GetAnchorPoints().ToList();

        var allPositions = new List<Vector3>();

        for (var curPointIndex = 0; curPointIndex < pathPoints.Count; curPointIndex++)
        {
            if ((curPointIndex + 1) >= pathPoints.Count)
                break;

            var curPoint = pathPoints[curPointIndex];
            var nextPoint = pathPoints[curPointIndex + 1];

            var point1 = curPoint.position;
            var point2 = curPoint.globalHandle2;
            var point3 = nextPoint.globalHandle1;
            var point4 = nextPoint.position;

            var positions = new Vector3[VertexCount];
            for (int i = 1; i < VertexCount + 1; i++)
            {
                float t = i / (float)VertexCount;
                positions[i - 1] = CaculateCubicBezierPoint(t, point1, point2, point3, point4);
            }

            allPositions.AddRange(positions);

            LineRenderer.positionCount = allPositions.Count;
            LineRenderer.SetPositions(allPositions.ToArray());
        }
    }

    private Vector3 CaculateCubicBezierPoint(float t, Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * point1;
        point += 3 * uu * t * point2;
        point += 3 * u * tt * point3;
        point += ttt * point4;

        return point;
    }
}
