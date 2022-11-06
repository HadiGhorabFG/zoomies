using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraTargetFollow : MonoBehaviour
{
    private Camera _cam;
    [SerializeField] private Transform _target;
    [SerializeField] private float _lookSpeed = 10;
    [SerializeField] private float _followSpeed = 10;

    [SerializeField] private AnimationCurve fadeCurve;
    [SerializeField] private float accelerationFadePower = 1f;
    [SerializeField] private float accelerationFadePowerFOV = 1f;
    [SerializeField] private Vector3 _offset;
    private Vector3 _startOffset;

    private PlayerMovement _playerMovement;
    
    private RaycastHit _camHit;
    private float _startFieldOfView;

    private void Start()
    {
        _cam = Camera.main;
        _startFieldOfView = _cam.fieldOfView;
        _playerMovement = _target.parent.GetComponent<PlayerMovement>();
        _startOffset = _offset;
    }

    private void Update()
    {
        WallClipping();
    }

    private void FixedUpdate()
    {
        LookAtTarget();
        FollowTarget();

        AccelerationFade();
    }

    private void AccelerationFade()
    {
        _offset.z = _startOffset.z - (accelerationFadePower * fadeCurve.Evaluate(_playerMovement.Acceleration) * Time.deltaTime);
        _cam.fieldOfView = _startFieldOfView + (accelerationFadePowerFOV * fadeCurve.Evaluate(_playerMovement.Acceleration) * Time.deltaTime);
    }

    private void LookAtTarget()
    {
        Vector3 lookDirection = _target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * _lookSpeed);
    }

    private void FollowTarget()
    {
        Vector3 pos = _target.position + _target.right * _offset.x + _target.forward * _offset.z + _target.up * _offset.y;

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * _followSpeed);
    }
    
    private void WallClipping()
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(_cam.transform.parent);
        obj.transform.localPosition = new Vector3(_cam.transform.localPosition.x, _cam.transform.localPosition.y,
            _cam.transform.localPosition.z - 0.1f);
        if (Physics.Linecast(_target.transform.position, obj.transform.position, out _camHit))
        {
            _cam.transform.position = _camHit.point;
            _cam.transform.localPosition = new Vector3(_cam.transform.localPosition.x, _cam.transform.localPosition.y,
                _cam.transform.localPosition.z + 0.1f);
        }
        Destroy(obj);
    }
}
