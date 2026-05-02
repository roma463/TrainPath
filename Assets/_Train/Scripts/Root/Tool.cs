using _Train.Scripts.Character;
using UnityEngine;

namespace _Train.Scripts.Root
{
    public abstract class Tool : MonoBehaviour
    {
        public event System.Action OnUseStarted;
        public event System.Action OnUseCompleted;
        public event System.Action OnUseCanceled;
        
        [Header("Tool Settings")]
        public string toolName;
        public float useDuration = 1f; // Длительность использования
        public AnimationClip useAnimation;
    
        protected bool _isUsing = false;
        protected float _useTimer = 0f;
        protected IInteractable _currentTarget;
    
        public virtual bool CanUse(IInteractable target)
        {
            return target != null;
        }
    
        public virtual void StartUse(IInteractable target)
        {
            if (_isUsing) return;
        
            _currentTarget = target;
            _isUsing = true;
            _useTimer = 0f;
        
            OnUseStarted?.Invoke();
        
            // Запускаем анимацию
            if (useAnimation != null)
            {
                // Проиграть анимацию через Animator
            }
        }
    
        public virtual void UpdateUse(float deltaTime)
        {
            if (!_isUsing) return;
        
            _useTimer += deltaTime;
        
            if (_useTimer >= useDuration)
            {
                CompleteUse();
            }
        }
    
        public virtual void CancelUse()
        {
            if (!_isUsing) return;
        
            _isUsing = false;
            OnUseCanceled?.Invoke();
            _currentTarget = null;
        }
    
        protected virtual void CompleteUse()
        {
            if (_currentTarget != null && _currentTarget.CanInteractWithTool(this))
            {
                _currentTarget.Interact(CharacterContext.Instance);
            }
        
            _isUsing = false;
            OnUseCompleted?.Invoke();
            _currentTarget = null;
        }
    }
}