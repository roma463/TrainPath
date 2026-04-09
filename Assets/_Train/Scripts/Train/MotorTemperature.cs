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
        [SerializeField] private float minTemperature = 0f;
        [SerializeField] private float ambientTemperature = 20f; // Температура окружающей среды

        [SerializeField] private float thermalInertia = 10f; // Тепловая инерция (чем выше, тем медленнее меняется температура)

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
            _currentTemperature = Mathf.Clamp(_currentTemperature, ambientTemperature - 10f, maxTemperature);

            // Вызываем событие
            OnTemperatureChanged?.Invoke(_currentTemperature, NormalizedTemperature);

            // Применяем эффекты от перегрева
            CheckOverheating();
        }

        private float CalculateHeatingPower()
        {
            // Базовая мощность нагрева от нагрузки двигателя
            float load = trainMotor.NormalPower;
            float baseHeating = heatingCurve.Evaluate(load);

            // Дополнительный нагрев от радиаторов/охлаждения (если есть)
            // float coolingEfficiency = 1f;
            // foreach (var spendTemp in _spendTemperatures)
            // {
            //     coolingEfficiency -= spendTemp.SpendPercent() / 100f;
            // }

            // Чем выше температура, тем меньше эффективность охлаждения
            float temperaturePenalty = 1f - Mathf.Clamp01((_currentTemperature - 60f) / 40f) * 0.5f;

            return baseHeating * temperaturePenalty;
        }

        private float CalculateCoolingPower()
        {
            // Разница с температурой окружающей среды
            float deltaTemp = _currentTemperature - ambientTemperature;

            if (deltaTemp <= 0)
                return 0; // Не охлаждаемся, если холоднее окружающей среды

            // Базовая мощность охлаждения зависит от температуры
            float baseCooling = coolingCurve.Evaluate(NormalizedTemperature);

            // Эффективность системы охлаждения
            float coolingEfficiency = 1f;
            foreach (var spendTemp in _spendTemperatures)
            {
                coolingEfficiency -= spendTemp.SpendPercent() / 100f;
            }

            // Закон Ньютона-Рихмана: охлаждение пропорционально разнице температур
            return baseCooling * deltaTemp * coolingEfficiency;
        }

        private void InitializeCurves()
        {
            // Кривая нагрева: при нагрузке 0 - минимальный нагрев, при 1 - максимальный
            if (heatingCurve == null || heatingCurve.keys.Length == 0)
            {
                heatingCurve = new AnimationCurve();
                heatingCurve.AddKey(0f, 5f); // Холостой ход - небольшой нагрев
                heatingCurve.AddKey(0.3f, 15f); // Экономичный режим
                heatingCurve.AddKey(0.6f, 35f); // Средняя нагрузка
                heatingCurve.AddKey(0.8f, 50f); // Высокая нагрузка
                heatingCurve.AddKey(1f, 80f); // Максимальная нагрузка
            }

            // Кривая охлаждения: коэффициент охлаждения в зависимости от температуры
            if (coolingCurve == null || coolingCurve.keys.Length == 0)
            {
                coolingCurve = new AnimationCurve();
                coolingCurve.AddKey(0f, 2f); // Холодный - слабое охлаждение
                coolingCurve.AddKey(0.3f, 3f); // Начало работы радиатора
                coolingCurve.AddKey(0.6f, 5f); // Активное охлаждение
                coolingCurve.AddKey(0.85f, 8f); // Максимальное охлаждение
                coolingCurve.AddKey(1f, 12f); // Аварийное охлаждение
            }
        }

        private void CheckOverheating()
        {
            if (_currentTemperature >= maxTemperature * 0.9f)
            {
                // Перегрев - штраф к мощности
                float overheatFactor =
                    1f - Mathf.Clamp01((_currentTemperature - maxTemperature * 0.9f) / (maxTemperature * 0.1f));
                // trainMotor.SetPowerMultiplier(overheatFactor);

                if (_currentTemperature >= maxTemperature)
                {
                    // Критический перегрев
                    Debug.LogWarning("Двигатель перегрет! Требуется охлаждение!");
                }
            }
            else
            {
                // trainMotor.SetPowerMultiplier(1f);
            }
        }

        // Ручное охлаждение (например, включение вентиляторов)
        public void ActivateEmergencyCooling(float power)
        {
            _currentTemperature -= power * Time.fixedDeltaTime;
            _currentTemperature = Mathf.Clamp(_currentTemperature, ambientTemperature, maxTemperature);
        }

        // Получение равновесной температуры для заданной нагрузки
        public float GetEquilibriumTemperature(float load)
        {
            float equilibriumTemp = ambientTemperature;
            float step = 1f;

            // Простой численный метод поиска равновесной температуры
            for (int i = 0; i < 100; i++)
            {
                float heating = heatingCurve.Evaluate(load);
                float cooling =
                    coolingCurve.Evaluate(
                        (equilibriumTemp - ambientTemperature) / (maxTemperature - ambientTemperature));
                float deltaTemp = equilibriumTemp - ambientTemperature;

                float netFlow = heating - cooling * deltaTemp;

                if (Mathf.Abs(netFlow) < 0.1f)
                    break;

                equilibriumTemp += netFlow * step;
                equilibriumTemp = Mathf.Clamp(equilibriumTemp, ambientTemperature, maxTemperature);
            }

            return equilibriumTemp;
        }
    }
}