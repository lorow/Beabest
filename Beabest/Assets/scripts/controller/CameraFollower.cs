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
        private float _collisionFixingSpeed = 19;
        [SerializeField] 
        private float defZ;

        private float currentZ;

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

        private Vector3 _defaultCameraPosition;
        
        [SerializeField]
        private LayerMask _ignoreLayers;
        
        private void Start()
        {
            _camTransform = Camera.main.transform;
            _defaultCameraPosition = _camTransform.position;
            _pivot = _camTransform.parent;
            _ignoreLayers = ~(1 << 8);
            currentZ = defZ;
        }

        public void UpdateCamera(float delta)
        {
            GatherInputs();
            SelectSpeed();
            FollowTarget(delta);
            HandleRotation();
            HandlePivotPosition(delta);
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
            //detects if the player is using mouse or controller
            if (_cH != 0 || _cH != 0)
            {
                _h = _cH;
                _v = _cV;
                _targetSpeed = _controllerSpeed;
            }
            else
                _targetSpeed = _mouseSpeed;
        }

        private void FollowTarget(float delta)
        {     
            _targetPosition = Vector3.Lerp(transform.position, Target.position, delta * _followSpeed);   
            transform.position = _targetPosition;
        }

        private void HandlePivotPosition(float delta)
        {
            float targetZ = defZ;
            CameraCollion(defZ, ref targetZ);

            currentZ = Mathf.Lerp(currentZ, targetZ, delta * _collisionFixingSpeed);
            Vector3 tp = new Vector3(0, 1.89f, 0);

            tp.z = currentZ;
            _camTransform.localPosition = tp;
        }

        private void CameraCollion(float targetZ, ref float actualZ)
        {
            float step = Mathf.Abs(targetZ);
            int stepCount = 2;

            float stepIncrement = step / stepCount;

            RaycastHit hit;

            Vector3 origin = _pivot.position;

            Vector3 direction = -_pivot.forward;

            if (Physics.Raycast(origin, direction, out hit, step, _ignoreLayers))
            {
                float distance = Vector3.Distance(hit.point, origin);
                actualZ = -(distance / 2);
            }
            else
            {
                for (int i = 0; i < stepCount +1 ; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector3 dir = Vector3.zero;
                        Vector3 secondOrigin = origin + (direction * i * stepIncrement);
                        switch (i)
                        {                          
                            case 0:
                                dir = _camTransform.right;
                                break;

                            case 1:
                                dir = -_camTransform.right;
                                break;
                                 
                            case 2:
                                dir = _camTransform.up;
                                break;
                                 
                            case 3:
                                dir = -_camTransform.up;
                                break;                            
                        }

                        if (Physics.Raycast(secondOrigin, dir, out hit, 0.2f, _ignoreLayers))
                        {
                            float distance = Vector3.Distance(secondOrigin, origin);
                            actualZ = -(distance / 2);
                            if (actualZ < 0.2f)
                                actualZ = 0;
                            
                            return;
                        }
                    }
                }
            }
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

