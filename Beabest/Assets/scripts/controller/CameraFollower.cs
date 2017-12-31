using UnityEngine;

namespace CameraController
{
    public class CameraFollower : MonoBehaviour
    {
        public Transform Target;

        private Transform _pivot;
        private Transform _camTransform;

        [Header("Controller inputs")]
        private float _h;
        private float _v;
        private float _cH;
        private float _cV;
        
        [Header("Camera params")]
        [SerializeField]
        private float _mouseSpeed = 4;
        [SerializeField]
        private float _controllerSpeed = 7;
        [SerializeField]
        private float _targetSpeed;
        [SerializeField]
        private float _followSpeed = 9;

        [SerializeField]
        private float _turnSmoothing = .1f;
        [SerializeField]
        private float _minAngle = -35;
        [SerializeField]
        private float _maxAngle = 35;

        private float _smoothX;
        private float _smoothY;
        private float _smoothXVelocity;
        private float _smoothYVelocity;

        private float _lookAngle;
        private float _tiltAngle;
        
        private Vector3 _targetPosition;
        
        private void Start()
        {
            _camTransform = Camera.main.transform;
            _pivot = _camTransform.parent;
        }

        public void UpdateCamera(float detla)
        {
            GatherInputs();
            SelectSpeed();
            FollowTarget(detla);
            HandleRotation();
        }

        private void GatherInputs()
        {
            _h  = Input.GetAxis("Mouse X");
            _v  = Input.GetAxis("Mouse Y");
            _cH = Input.GetAxis("RightAxis X");
            _cV = Input.GetAxis("RightAxis Y");
        }

        private void SelectSpeed()
        {
            if (_cH != 0 || _cH != 0)
            {
                _h = _cH;
                _v = _cV;
                _targetSpeed = _controllerSpeed;
            }
            else
                _targetSpeed = _mouseSpeed;
        }

        private void FollowTarget(float Delta)
        {
            _targetPosition = Vector3.Lerp(transform.position, Target.position, Delta * _followSpeed);
            transform.position = _targetPosition;
        }

        private void HandleRotation()
        {
            if (_turnSmoothing > 0)
            {
                _smoothX = Mathf.SmoothDamp(_smoothX, _h, ref _smoothXVelocity, _turnSmoothing);
                _smoothY = Mathf.SmoothDamp(_smoothY, _v, ref _smoothYVelocity, _turnSmoothing);
            }
            else
            {
                _smoothX = _h;
                _smoothY = _v;
            }

            _lookAngle += _smoothX * _targetSpeed;
            transform.rotation = Quaternion.Euler(0, _lookAngle, 0);

            _tiltAngle -= _smoothY * _targetSpeed;
            _tiltAngle = Mathf.Clamp(_tiltAngle, _minAngle, _maxAngle);
            _pivot.localRotation = Quaternion.Euler(_tiltAngle, 0,0);
        }
    }
}

