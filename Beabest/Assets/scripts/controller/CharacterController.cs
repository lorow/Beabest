using UnityEngine;
using SA;

namespace Controller
{
    public class CharacterController : MonoBehaviour
    {
        [Header("Init")]
        [SerializeField] 
        private GameObject _activeModel;
        [SerializeField]
        private CameraController.CameraFollower _cameraFollower;

        [Header("Inputs")]
        [SerializeField]
        private float _vertical;
        [SerializeField]
        private float _horizontal;
        [SerializeField]
        private float _moveAmout;
        [SerializeField]
        private bool _runInput;

        [Header("Stats")]
        [SerializeField]
        private float _moveSpeed = 2;
        [SerializeField]
        private float _runSpeed = 4;
        [SerializeField]
        private float _rotateSpeed = 5;
        [SerializeField]
        private float _toGround = 0.5f;
        
        private float _targetSpeed;

        [Header("States")]
        [SerializeField]
        private bool _run;
        [SerializeField]
        private bool _onGround;
        [SerializeField]
        private bool _lockOn;
        
        [SerializeField] 
        private Animator _anim;
        private Rigidbody _rigid;
        private LayerMask _ignoreLayers;

        private Vector3 _moveDirection;
        private Vector3 _targetDir;

        private Vector3 _v;
        private Vector3 _h;

        private void Start()
        {
            Debug.Log("pierdol się");
        }

        private void Awake()
        {
            Debug.Log("pierdol się2");
            _cameraFollower.Target = this.transform;
            SetupAnimator();
            SetupRigidbody();
            SetupIgnoreLayers();
        
            _anim.SetBool("onGround", true);
        }

        private void Update()
        {
            _onGround = OnGround();
            _anim.SetBool("onGround", _onGround);
        }

        private void FixedUpdate()
        {
            GetInput();
            UpdateStates();
            HandleMovement(); 
           _cameraFollower.UpdateCamera(Time.fixedDeltaTime);
        }

        private void HandleMovement()
        {
            _rigid.drag = (_moveAmout > 0 || _onGround == false) ? 0 : 4;
            if (_run)
            {
                _targetSpeed = _runSpeed;
                _lockOn = false;
            }
            else
                _targetSpeed = _moveSpeed;

            if (_onGround)
                _rigid.velocity = _moveDirection * (_targetSpeed * _moveAmout);
                
            if (!_lockOn)
            {
                _targetDir = _moveDirection;
                _targetDir.y = 0;

                if (_targetDir == Vector3.zero)
                    _targetDir = transform.forward;
                
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(_targetDir),
                    Time.fixedDeltaTime * _moveAmout * _rotateSpeed);
            }

            HandleMovementAnimations();
        }
        
        private void HandleMovementAnimations()
        {
            _anim.SetBool("run", _run);
            _anim.SetFloat("vertical", _moveAmout, 0.4f, Time.deltaTime);
        }

        private void SetupAnimator()
        {
            if (_activeModel == null)
            {
                _anim = GetComponentInChildren<Animator>(); //this can be dangerous
                if (_anim == null)
                    Debug.LogError("ERROR: no model found");
                else
                    _activeModel = _anim.gameObject;
            }

            if (_anim == null)
                _anim = _activeModel.GetComponent<Animator>();

            _anim.applyRootMotion = false;
        }

        private void SetupRigidbody()
        {
            _rigid = GetComponent<Rigidbody>();
            _rigid.angularDrag = 999;
            _rigid.drag = 4;
            _rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void SetupIgnoreLayers()
        {
            gameObject.layer = 8; //controller layer
            _ignoreLayers = ~(1 << 9);
        }

        private void GetInput()
        {
            _vertical = Input.GetAxis("Vertical");
            _horizontal = Input.GetAxis("Horizontal");
            _runInput = Input.GetButton("RunInput");
        }
        
        private bool OnGround()
        {
            RaycastHit hit;
            if (Physics.Raycast(
                transform.position + (Vector3.up * _toGround),
                -Vector3.up,
                out hit,
                _toGround + 0.3f,
                _ignoreLayers
            ))
            {
                transform.position = hit.point;
                return true;
            }
            return false;
        }

        private void UpdateStates()
        {
            _v = _vertical * _cameraFollower.transform.forward;
            _h = _horizontal * _cameraFollower.transform.right;

            _moveDirection = (_v + _h).normalized;
            _moveAmout =  Mathf.Clamp01(Mathf.Abs(_horizontal) + Mathf.Abs(_vertical));

            if (_runInput && _moveAmout > 0)
                _run = true;
            else
                _run = false;

        }
    }
}
