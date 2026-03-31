using System;
using _Train.Scripts.Train;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Train.Scripts.Character
{
    public class CharacterCollision : MonoBehaviour
    {
        private readonly float _tresholdAngle = 45f;

        public event Action OnGroundedPerformed;
        public event Action OnGroundedCanceled;
        public event Action<int> OnDamagedByGround;

        [Header("Ground")]
        [SerializeField] private float _radius;
        [SerializeField] private Transform _groundPoint;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _distance;
        [FormerlySerializedAs("distanceToDamage")]
        [Header("Ground damage")]
        [SerializeField] private float minDamageVelocity;
        [SerializeField] private int groundDamage;
        [SerializeField] private Rigidbody rigidbody;

        private bool _isGrounded;
        private float _angle;
        private Collider[] _matchedColliders;
        private IMovableObject _currentMovableObject;
        private Vector3 _safeLinearVelocity;
        
        public Vector3 GroundNormal { get; private set; }
        public Vector3 GroundPoint { get; private set; }
        public Collider LastCollider { get; private set; }
        public Transform RootGroundTransform { get; private set; }
        public bool WillDamage { get; private set; }
        
        public bool UseMovableObject => _currentMovableObject != null && IsGrounded;
        public Vector3 LinearVelocity => UseMovableObject ? _currentMovableObject.LinearVelocity : _safeLinearVelocity;
        public Vector3 AngularVelocity => UseMovableObject ? _currentMovableObject.AngularVelocity : Vector3.zero;
        
        
        public bool IsGrounded
        {
            get => _isGrounded;
            private set
            {
                if (_isGrounded != value)
                {
                    _isGrounded = value;
                    (value ? OnGroundedPerformed : OnGroundedCanceled)?.Invoke();
                }
            }
        }

        private void FixedUpdate()
        {
            GroundCheck();
        }

        private void GroundCheck()
        {
            RaycastHit hit = new RaycastHit();

            if (Physics.SphereCast(_groundPoint.position + Vector3.up * _radius * 1.5f, _radius, Vector3.down, out hit, _distance, _groundLayer, QueryTriggerInteraction.Ignore))
            {
                GroundNormal = hit.normal;
                GroundPoint = hit.point;
                Debug.DrawLine(_groundPoint.position, hit.normal + _groundPoint.position);
            }

            if (!IsGrounded && hit.collider != null)
            {
                if (rigidbody.linearVelocity.y < minDamageVelocity)
                {
                    OnDamagedByGround?.Invoke(groundDamage);
                }
                
                IsGrounded = true;
                RootGroundTransform = hit.transform.root;
                
                if (hit.transform.root.TryGetComponent(out IMovableObject movableObject))
                {
                    _currentMovableObject = movableObject;
                }
                else
                {
                    _currentMovableObject = null;
                }
            }
            else if (IsGrounded && hit.collider == null)
            {
                IsGrounded = false;
                
                if (UseMovableObject)
                    _safeLinearVelocity = _currentMovableObject.LinearVelocity;
            }
        }

        public float GetDistanceToGround()
        {
            if (Physics.Raycast(_groundPoint.position, Vector3.down, out var hit, 50f, _groundLayer))
            {
                return hit.distance;
            }

            return 0f;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_groundPoint.position + Vector3.up * _radius * 1.5f, _radius);

            Gizmos.matrix = Matrix4x4.TRS(
                transform.position + _groundPoint.position,
                transform.rotation,
                Vector3.one);

            Gizmos.matrix = Matrix4x4.identity;
            
            Gizmos.DrawLine(_groundPoint.position + Vector3.up * _radius * 1.5f, _groundPoint.position + Vector3.up * (_radius * 1.5f - _distance));
            Gizmos.DrawLine(_groundPoint.position + Vector3.up * _radius * 1.5f, _groundPoint.position + Vector3.up * (_radius * 1.5f - _distance));
        }
    }
}
