using System;
using System.Collections;
using System.Collections.Generic;
using _Train.Scripts.Character.Data;
using _Train.Scripts.Character.MovementStateMachine;
using _Train.Scripts.Train;
using _Train.Scripts.UI;
using UnityEngine;

namespace _Train.Scripts.Character
{
    public class Character : MonoBehaviour, IDamageable
    {
        public float VerticalVelocity { get; set; }
        public bool IsLockRestoreStamina { get; set; }
        public bool IsPassenger { get; private set; }
        
        [field: SerializeField] public CharacterCollision Collision { get; private set; }
        [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
        [field: SerializeField] public bool IsLockPlayer { get; private set; }

        [SerializeField] private CharacterBaseData characterBaseData;
        [SerializeField] private CapsuleCollider characterCollider;
        [SerializeField] private Rigidbody characterRigidbody;
        [SerializeField] private CharacterStateMachine _stateMachine;
        [SerializeField] private GameObject _localClient;
        [SerializeField] private GameObject _remoteClient;
        [SerializeField] private GameUi gameUi;
        [SerializeField] private CharacterContext characterContext;
        [SerializeField] private EnergyUI energyUI;
        [SerializeField] private float runEnergyPerSecond = 12f;
        [SerializeField] private float jumpEnergyCost = 18f;
        [SerializeField] private float energyRegenPerSecond = 10f;
        [SerializeField] private float minEnergyToAct = 1f;

        private Vector3 _startPosition;
        private Coroutine _lockStaminaRoutine;
        
        public event Action<float> EnergyNormalizedChanged;
        
        public bool IsWalking => _stateMachine.CurrentStateType == CharacterStateType.Walk;
        public bool IsRunning => _stateMachine.CurrentStateType == CharacterStateType.Run;
        public bool IsSitMoving => _stateMachine.CurrentStateType == CharacterStateType.SitMove;
        
        public Rigidbody CharacterRigidbody => characterRigidbody;

        public CapsuleCollider CharacterCollider => characterCollider;
        
#if UNITY_EDITOR
        public bool IsInvisible { get; private set; }
#endif

        private void Awake()
        {
            _startPosition = transform.position;
        }
        
        public void LockPlayer()
        {
            IsLockPlayer = true;
        }

        public void UnlockPlayer()
        {
            IsLockPlayer = false;
        }
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            characterRigidbody.useGravity = true;
            
            Collision.OnDamagedByGround += DamageByGround;
                
            Initialize();
        }

        private void LockRestoreStamina(bool isFull)
        {
            if (!isFull && _lockStaminaRoutine == null)
            {
                _lockStaminaRoutine = StartCoroutine(DelayStartRestoreStamina());
            }
        }

        private IEnumerator DelayStartRestoreStamina()
        {
            IsLockRestoreStamina = true;
            
            yield return new WaitForSeconds(characterBaseData.TimeForStartRestoreStamina);
            
            IsLockRestoreStamina = false;
            _lockStaminaRoutine = null;
        }

        public void SetPassengers(bool isPass, IConnectableObject parent = null)
        {
            IsPassenger = isPass;
        }

        private void OnDestroy()
        {
            Collision.OnDamagedByGround -= DamageByGround;
            
            if (_lockStaminaRoutine != null)
                StopCoroutine(_lockStaminaRoutine);
        }

        private void DamageByGround(int groundDamage)
        {
            TakeDamage(groundDamage);
        }

        public void Initialize()
        {
            _stateMachine.Initialize();
            gameUi.Initialize();
        }

        public void TakeDamage(int damageAmount)
        {
        }
        
        private Vector3 CalculateCorrectParabolicVelocity(Vector3 direction, float totalForce, float height)
        {
            var horizontalDir = new Vector3(direction.x, 0, direction.z).normalized;
            var verticalSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * height);
            var horizontalSpeed = Mathf.Sqrt(totalForce * totalForce - verticalSpeed * verticalSpeed);

            if (float.IsNaN(horizontalSpeed))
            {
                horizontalSpeed = totalForce * 0.7f;
                verticalSpeed = totalForce * 0.3f;
            }

            var horizontalVelocity = horizontalDir * horizontalSpeed;
            var verticalVelocity = Vector3.up * verticalSpeed;

            return horizontalVelocity + verticalVelocity;
        }
        
        public void Move(Vector3 velocity, bool applyGravity = false)
        {
            if (Collision.AngularVelocity != Vector3.zero)
            {
                var directionToCenter =  (Collision.RootGroundTransform.position - transform.position);
                velocity += -Vector3.Cross(Collision.AngularVelocity, directionToCenter);
            }
            
            velocity += Collision.LinearVelocity;
            // Debug.Log("Character FixedUpdate");
            
            if (applyGravity)
            {
                ApplyGravity(velocity);
                characterRigidbody.linearVelocity = new Vector3(velocity.x, VerticalVelocity, velocity.z);
            }
            else
            {
                characterRigidbody.linearVelocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -characterBaseData.MaxVerticalVelocity, characterBaseData.MaxVerticalVelocity), velocity.z);
            }
            
            VerticalVelocity = characterRigidbody.linearVelocity.y;
        }
        
        private void ApplyGravity(Vector3 targetVeloсity)
        {
            var gravityEffect = _stateMachine.Context.Character.Gravity * Time.fixedDeltaTime;
            VerticalVelocity = targetVeloсity.y + gravityEffect;
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -characterBaseData.MaxVerticalVelocity, characterBaseData.MaxVerticalVelocity);
        } 

        private IEnumerator StartRespawn()
        {
            yield return new WaitForSeconds(10f);
            
            Respawn();
        }
        
        public void Respawn()
        {
            RpcRespawnResetData();
        }
        

        private void RpcRespawnResetData()
        {
            _stateMachine.Context.Character.UnlockPlayer();
            _stateMachine.Context.CameraController.UnlockCamera();
            _stateMachine.ChangeStateByType(CharacterStateType.Idle);
        }
        
#if UNITY_EDITOR
        public void ChangeInvisibilityState(bool isInvisible)
        {
            IsInvisible = isInvisible;
        }
#endif

        #region Event Handlers

        private void OnDead()
        {
            StartCoroutine(StartRespawn());
        }

        #endregion
    }
}
