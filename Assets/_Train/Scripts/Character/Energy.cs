using System;
using _Train.Scripts.Character.Data;
using _Train.Scripts.Character.MovementStateMachine;
using Mirror;
using UnityEngine;

namespace _Train.Scripts.Character
{
    public class Energy : MonoBehaviour
    {
        public event Action<float> OnEnergyNormalizedChanged;
        public event Action OnEnergyEnded;
        
        [SerializeField] private CharacterBaseData characterBaseData;
        [SerializeField] private CharacterStateMachine stateMachine;
        [SerializeField] private Character character;
    
        private float _energy;

        private bool _inRegenZone;

        private void Start()
        {
            _energy = characterBaseData.MaxEnergy;
        }
        
        private void OnEnergyChanged(float oldValue, float newValue)
        {
            float norm = characterBaseData.MaxEnergy <= 0f ? 0f : Mathf.Clamp01(newValue / characterBaseData.MaxEnergy);
            OnEnergyNormalizedChanged?.Invoke(norm);
        }

        public void SetRegenZoneState(bool regenZoneState)
        {
            _inRegenZone = regenZoneState;
        }
    
        private void FixedUpdate()
        {
            if (EnergyInSpend() && _energy > 0f)
            {
                var spendValue = GetSpendValue() * Time.fixedDeltaTime;
                _energy = Mathf.Clamp(_energy - spendValue, 0, characterBaseData.MaxEnergy);

                // if (_syncEnergy == 0)
                // {
                //     OnEnergyEnded?.Invoke();
                // }
            }

            if (_inRegenZone && _energy < characterBaseData.MaxEnergy)
            {
                var regenValue = characterBaseData.IdleEnergyPerSecond * Time.fixedDeltaTime;
                _energy = Mathf.Clamp(_energy + regenValue, 0, characterBaseData.MaxEnergy);
            }
        }
        
        private bool EnergyInSpend()
        {
            return character.IsRunning || character.IsWalking;
        }

        private float GetSpendValue()
        {
            if (character.IsRunning)
                return characterBaseData.RunEnergyPerSecond;
            
            if (character.IsWalking)
                return characterBaseData.WalkEnergyPerSecond;
            

            throw new Exception("Character is not spending stamina");
        }

        public void SpendForJump()
        {
            _energy = Mathf.Clamp(_energy - characterBaseData.JumpEnergyPerSecond, 0, characterBaseData.MaxEnergy);
        }
    }
}
