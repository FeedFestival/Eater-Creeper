using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public Vector3 MousePosition;
    public Animator Animator;
    public Animator AnimatorExtension;
    public Transform Hand;
    //

    private Transform _parent;
    private int _tweenId;
    private float _rotateDuration;
    private IEnumerator _afterPartOfPathMoving;

    private float _handFireTime;
    private IEnumerator _afterFire;

    private int _ltHandScaleId;
    private readonly Vector3 _toHandScale = new Vector3(0.8f, 0.8f, 0.8f);

    private bool _lookAtMouseClick;
    private GameObject _go;

    private float rotSpeed = 15f;

    void Start()
    {
        _rotateDuration = 2f;
        _handFireTime = 2f;

        Hand.localScale = Vector3.zero;

        Init();
    }

    public void Init()
    {
        //MoveAround();
    }

    void MoveAround()
    {
        _afterPartOfPathMoving = AfterPartOfPathMoving();
        StartCoroutine(_afterPartOfPathMoving);
        LTDescr _movePartOfPath = LeanTween.rotateAround(gameObject, Vector3.forward, 180f, _rotateDuration).setEase(LeanTweenType.linear);
        _tweenId = _movePartOfPath.id;
    }

    void RotateToMousePosition()
    {
        _afterPartOfPathMoving = AfterPartOfPathMoving();
        StartCoroutine(_afterPartOfPathMoving);
        LTDescr _movePartOfPath = LeanTween.rotateAround(gameObject, Vector3.forward, 180f, _rotateDuration).setEase(LeanTweenType.linear);
        _tweenId = _movePartOfPath.id;
    }

    IEnumerator AfterPartOfPathMoving()
    {
        yield return new WaitForSeconds(_rotateDuration);

        StopRotating();
        MoveAround();
    }

    private void StopRotating()
    {
        LeanTween.cancel(_tweenId);
        StopCoroutine(_afterPartOfPathMoving);
    }

    void Update()
    {
        if (_lookAtMouseClick == false)
        {
            if (Input.GetMouseButtonUp(0))
            {
                FireHand();
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                CloseHand();

            }
        }

        if (_lookAtMouseClick)
        {
            FaceEnemy();
        }
    }

    private void FireHand()
    {
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (_go == null)
        {
            _go = Instantiate(Resources.Load("Prefabs/LookAt", typeof(GameObject)),
                    Vector3.zero, Quaternion.identity
                    ) as GameObject;
            _go.transform.SetParent(transform.parent);
        }
        _lookAtMouseClick = true;
        Animator.SetBool("SlingShot", true);
        AnimatorExtension.SetBool("SlingShot", true);

        _afterFire = AfterFire(_handFireTime);
        StartCoroutine(_afterFire);

        _ltHandScaleId  = LeanTween.scale(Hand.gameObject, _toHandScale, _handFireTime).setEase(LeanTweenType.easeOutBack).id;
    }

    private IEnumerator AfterFire(float time)
    {
        yield return new WaitForSeconds(time);

        _afterFire = null;
    }

    private void CloseHand()
    {
        LeanTween.cancel(_ltHandScaleId);
        if (_afterFire != null)
            StopCoroutine(_afterFire);
        _afterFire = null;

        Hand.localScale = Vector3.zero;
        _lookAtMouseClick = false;
        Animator.SetBool("SlingShot", false);
        AnimatorExtension.SetBool("SlingShot", false);
    }

    private void FaceEnemy()
    {
        // Move enemy inside localSpace to mousePosition
        var lookAt = new Vector3(MousePosition.x, MousePosition.y, 0f);
        transform.LookAt(lookAt);
        _go.transform.position = new Vector3(MousePosition.x, MousePosition.y, 0f);
    }
}
