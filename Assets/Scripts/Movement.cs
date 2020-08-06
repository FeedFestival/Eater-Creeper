using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public BezierCurve Path;
    public List<Vector3> BezzierPath;

    //

    private List<BezierPoint> _pathPoints;

    private int curPointIndex;

    private int _tweenId;
    private float partOfPathTime = 1.5f;
    private IEnumerator _afterPartOfPathMoving;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        SetupPath();
    }

    public void SetupPath()
    {
        _pathPoints = Path.GetAnchorPoints().ToList();
        curPointIndex = 0;
        MoveToNextPoint();
    }

    private void MoveToNextPoint()
    {
        FixNextPoint();

        var curPoint = _pathPoints[curPointIndex];
        transform.position = curPoint.position;
        var nextPoint = _pathPoints[curPointIndex + 1];

        if (BezzierPath != null)
            BezzierPath = null;

        BezzierPath = new List<Vector3>()
                {
                    curPoint.position,                  // startPoint
                    nextPoint.globalHandle1,            // endControl
                    curPoint.globalHandle2,             // startControl
                    nextPoint.position                  // endPoint
                };

        LTBezierPath path = new LTBezierPath(
            BezzierPath.ToArray()
            );

        _afterPartOfPathMoving = AfterPartOfPathMoving();
        StartCoroutine(_afterPartOfPathMoving);
        LTDescr _movePartOfPath = LeanTween.move(gameObject, path, partOfPathTime).setEase(LeanTweenType.linear);
        _tweenId = _movePartOfPath.id;
    }

    IEnumerator AfterPartOfPathMoving()
    {
        yield return new WaitForSeconds(partOfPathTime);

        curPointIndex++;
        StopPathMoving();
        MoveToNextPoint();
    }

    private void StopPathMoving()
    {
        LeanTween.cancel(_tweenId);
        StopCoroutine(_afterPartOfPathMoving);
    }

    private void FixNextPoint()
    {
        if (curPointIndex == _pathPoints.Count - 1)
            curPointIndex = 0;
    }
}
