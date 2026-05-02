using System;
using _Train.Scripts.Train.Motors;
using _Train.Scripts.Train.Root;
using UnityEngine;

namespace _Train.Scripts.Train
{
    public class MotorTemperature : MonoBehaviour
    {
        [SerializeField] private TrainMotor trainMotor;
        [SerializeField] private AnimationCurve heatingCurve; // Кривая нагрева в зависимости от нагрузки
        [SerializeField] private AnimationCurve coolingCurve; // Кривая охлаждения в зависимости от температуры
        [SerializeField] private float maxTemperature = 100f;
        [SerializeField] private float ambientTemperature = 20f; // Температура окружающей среды
        [SerializeField] private float thermalInertia = 3f; // Тепловая инерция (чем выше, тем медленнее)

        [SerializeField] private GameObject[] spendTemperatureObjects;

        private float _currentTemperature;
        private ISpendTemperature[] _spendTemperatures;

        public float NormalizedTemperature => _currentTemperature / maxTemperature;
        public float CurrentTemperature => _currentTemperature;

        // События
        public System.Action<float, float> OnTemperatureChanged;

        private void Start()
        {
            _currentTemperature = ambientTemperature;

            _spendTemperatures = new ISpendTemperature[spendTemperatureObjects.Length];
            for (int i = 0; i < spendTemperatureObjects.Length; i++)
            {
                _spendTemperatures[i] = spendTemperatureObjects[i].GetComponent<ISpendTemperature>();
            }

            InitializeCurves();
        }

        private void FixedUpdate()
        {
            // Расчет теплового баланса
            float heatingPower = CalculateHeatingPower();
            float coolingPower = CalculateCoolingPower();

            // Чистый тепловой поток
            float netHeatFlow = (heatingPower - coolingPower) / thermalInertia;

            // Изменение температуры
            _currentTemperature += netHeatFlow * Time.fixedDeltaTime;

            // Ограничиваем температуру
            _currentTemperature = Mathf.Clamp(_currentTemperature, ambientTemperature, maxTemperature);

            // Вызываем событие
            OnTemperatureChanged?.Invoke(_currentTemperature, NormalizedTemperature);

            // Применяем эффекты от перегрева
            CheckOverheating();
        }

        private float CalculateHeatingPower()
        {
            // Простой нагрев от нагрузки двигателя
            float load = trainMotor.NormalPower;
            float heating = heatingCurve.Evaluate(load);
            
            return heating;
        }

        private float CalculateCoolingPower()
        {
            // Разница с температурой окружающей среды
            float deltaTemp = _currentTemperature - ambientTemperature;

            if (deltaTemp <= 0)
                return 0; // Не охлаждаемся, если холоднее окружающей среды

            // Простое охлаждение зависит только от температуры
            float cooling = coolingCurve.Evaluate(NormalizedTemperature);
            
            // Эффективность системы охлаждения (радиаторы, вентиляторы)
            float coolingEfficiency = 0f;
            foreach (var spendTemp in _spendTemperatures)
            {
                coolingEfficiency += spendTemp.SpendPercent() / 100f;
            }

            if (coolingEfficiency > 0)
                return cooling * coolingEfficiency;
            else
                return cooling;
        }

        private void InitializeCurves()
        {
            // Кривая нагрева: при нагрузке 0 - минимальный нагрев, при 1 - максимальный
            if (heatingCurve == null || heatingCurve.keys.Length == 0)
            {
                heatingCurve = new AnimationCurve();
                heatingCurve.AddKey(0f, 0f);      // Холостой ход - не греется
                heatingCurve.AddKey(0.3f, 2f);    // Малая нагрузка
                heatingCurve.AddKey(0.6f, 6f);    // Средняя нагрузка
                heatingCurve.AddKey(0.8f, 10f);   // Высокая нагрузка
                heatingCurve.AddKey(1f, 15f);     // Полная мощность
            }

            // Кривая охлаждения: чем горячее, тем быстрее остывает
            if (coolingCurve == null || coolingCurve.keys.Length == 0)
            {
                coolingCurve = new AnimationCurve();
                coolingCurve.AddKey(0f, 0f);      // Холодный - не остывает
                coolingCurve.AddKey(0.3f, 1f);    // Немного теплый
                coolingCurve.AddKey(0.6f, 2.5f);  // Горячий
                coolingCurve.AddKey(0.8f, 4f);    // Очень горячий
                coolingCurve.AddKey(1f, 5f);      // Критический - быстро остывает
            }
        }

        private void CheckOverheating()
        {
            if (_currentTemperature >= maxTemperature * 0.9f)
            {
                if (_currentTemperature >= maxTemperature)
                {
                    // Критический перегрев
                    Debug.LogWarning("Двигатель перегрет! Требуется охлаждение!");
                }
            }
        }

        // Ручное охлаждение (включение вентиляторов)
        public void ActivateEmergencyCooling(float power)
        {
            _currentTemperature -= power * Time.fixedDeltaTime;
            _currentTemperature = Mathf.Clamp(_currentTemperature, ambientTemperature, maxTemperature);
        }
    }
}