using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private AnimationCurve lightIntensityCurve;
    [SerializeField] private AnimationCurve lightTemperatureCurve;
    
    [Header("Time Settings")]
    [SerializeField, Range(0, 24)] private float currentTime = 12f;
    [SerializeField] private float dayDuration = 120f;
    
    [Header("Lighting Settings")]
    [SerializeField] private string lightingSettings; // Ссылка на Lighting Settings
    [SerializeField] private AnimationCurve ambientIntensityCurve; // Кривая для ambient intensity
    
    [Header("Sun Rotation")]
    [SerializeField] private Transform sunTransform;
    [SerializeField] private float sunRotationOffset = 0f; // Смещение вращения
    
    private float currentRotationAngle;
    private float currentIntensity;
    private float currentTemperature;
    private float currentAmbientIntensity;
    
    // Свойства
    public float CurrentTime => currentTime;
    public bool IsDay => currentTime >= 6 && currentTime <= 18;
    
    private void Start()
    {
        if (directionalLight == null)
            directionalLight = GetComponent<Light>();
            
        if (sunTransform == null && directionalLight != null)
            sunTransform = directionalLight.transform;
            
        if (lightingSettings == null)
        {
            // Получаем текущие настройки освещения
            lightingSettings = RenderSettings.sun != null ? 
                RenderSettings.sun.lightmapBakeType == LightmapBakeType.Realtime ? 
                null : RenderSettings.sun.lightmapBakeType.ToString() : null;
        }
        
        InitializeCurves();
        UpdateLight();
    }
    
    private void Update()
    {
        currentTime += Time.deltaTime * (24f / dayDuration);
        
        if (currentTime >= 24f)
            currentTime -= 24f;
            
        UpdateLight();
    }
    
    private void UpdateLight()
    {
        if (directionalLight == null)
            return;
            
        // Нормализуем время от 0 до 1
        float normalizedTime = currentTime / 24f;
        
        // Получаем значения из кривых
        currentIntensity = lightIntensityCurve.Evaluate(normalizedTime);
        currentTemperature = lightTemperatureCurve.Evaluate(normalizedTime);
        
        // Применяем к свету
        directionalLight.intensity = currentIntensity;
        directionalLight.colorTemperature = currentTemperature;
        
        // Обновляем ambient lighting
        UpdateAmbientLighting(normalizedTime);
        
        // Вращаем солнце
        UpdateSunRotation(normalizedTime);
    }
    
    private void UpdateSunRotation(float normalizedTime)
    {
        if (sunTransform == null)
            return;
        
        // Угол вращения: 0 часов = -90° (под горизонтом), 6 часов = 0° (горизонт), 
        // 12 часов = 90° (зенит), 18 часов = 180° (горизонт), 24 часа = 270° (под горизонтом)
        // Используем синусоидальную кривую для более естественного движения
        float angle = Mathf.Lerp(-90f, 270f, normalizedTime);
        currentRotationAngle = angle;
        
        // Применяем вращение
        sunTransform.rotation = Quaternion.Euler(currentRotationAngle + sunRotationOffset, 170f, 0f);
        
        // Солнце всегда включено, но интенсивность управляет его видимостью
        // Это решает проблему с пропаданием на скайбоксе
        directionalLight.enabled = true;
    }
    
    private void UpdateAmbientLighting(float normalizedTime)
    {
        // Получаем интенсивность ambient из кривой (днем 1, ночью 0)
        float ambientMultiplier = ambientIntensityCurve.Evaluate(normalizedTime);
        
        // Меняем интенсивность ambient освещения
        RenderSettings.ambientIntensity = ambientMultiplier;
        
        // Опционально: меняем цвет ambient в зависимости от времени суток
        if (currentTime < 6) // Ночь - рассвет
        {
            float t = currentTime / 6f;
            RenderSettings.ambientLight = Color.Lerp(new Color(0.1f, 0.1f, 0.2f), new Color(0.3f, 0.2f, 0.4f), t);
        }
        else if (currentTime < 12) // Утро - день
        {
            float t = (currentTime - 6f) / 6f;
            RenderSettings.ambientLight = Color.Lerp(new Color(0.3f, 0.2f, 0.4f), new Color(0.5f, 0.6f, 0.7f), t);
        }
        else if (currentTime < 18) // День - вечер
        {
            float t = (currentTime - 12f) / 6f;
            RenderSettings.ambientLight = Color.Lerp(new Color(0.5f, 0.6f, 0.7f), new Color(0.4f, 0.3f, 0.5f), t);
        }
        else // Вечер - ночь
        {
            float t = (currentTime - 18f) / 6f;
            RenderSettings.ambientLight = Color.Lerp(new Color(0.4f, 0.3f, 0.5f), new Color(0.1f, 0.1f, 0.2f), t);
        }
    }
    
    private void InitializeCurves()
    {
        // Исправленные кривые: ночью интенсивность 0, днем 1
        if (lightIntensityCurve == null || lightIntensityCurve.keys.Length == 0)
        {
            lightIntensityCurve = new AnimationCurve();
            lightIntensityCurve.AddKey(0f, 0f);      // 0 часов - ночь
            lightIntensityCurve.AddKey(0.2f, 0f);    // ~5 часов - все еще ночь
            lightIntensityCurve.AddKey(0.25f, 0.3f); // 6 часов - рассвет
            lightIntensityCurve.AddKey(0.5f, 1f);    // 12 часов - полдень
            lightIntensityCurve.AddKey(0.75f, 0.3f); // 18 часов - закат
            lightIntensityCurve.AddKey(0.8f, 0f);    // ~19 часов - закат закончился
            lightIntensityCurve.AddKey(1f, 0f);      // 24 часа - ночь
        }
        
        // Исправленные кривые температуры: днем теплее (5500K), ночью холоднее (2000K)
        if (lightTemperatureCurve == null || lightTemperatureCurve.keys.Length == 0)
        {
            lightTemperatureCurve = new AnimationCurve();
            lightTemperatureCurve.AddKey(0f, 2000f);   // Ночь - холодный/красный
            lightTemperatureCurve.AddKey(0.2f, 2000f); // Ночь
            lightTemperatureCurve.AddKey(0.25f, 3500f); // Рассвет - теплый
            lightTemperatureCurve.AddKey(0.5f, 5500f);  // День - нейтральный/белый
            lightTemperatureCurve.AddKey(0.75f, 3500f); // Закат - теплый
            lightTemperatureCurve.AddKey(0.8f, 2000f);  // Ночь
            lightTemperatureCurve.AddKey(1f, 2000f);    // Ночь
        }
        
        // Ambient интенсивность: днем 1, ночью 0
        if (ambientIntensityCurve == null || ambientIntensityCurve.keys.Length == 0)
        {
            ambientIntensityCurve = new AnimationCurve();
            ambientIntensityCurve.AddKey(0f, 0f);      // Ночь
            ambientIntensityCurve.AddKey(0.2f, 0f);    // Ночь
            ambientIntensityCurve.AddKey(0.25f, 0.3f); // Рассвет
            ambientIntensityCurve.AddKey(0.5f, 1f);    // День
            ambientIntensityCurve.AddKey(0.75f, 0.3f); // Закат
            ambientIntensityCurve.AddKey(0.8f, 0f);    // Ночь
            ambientIntensityCurve.AddKey(1f, 0f);      // Ночь
        }
    }
    
    // Публичные методы
    public void SetTime(float hours)
    {
        currentTime = Mathf.Clamp(hours, 0f, 24f);
        UpdateLight();
    }
    
    public void AddTime(float hours)
    {
        currentTime += hours;
        if (currentTime >= 24f)
            currentTime -= 24f;
        if (currentTime < 0f)
            currentTime += 24f;
        UpdateLight();
    }
    
    public void SetDayDuration(float duration)
    {
        dayDuration = Mathf.Max(1f, duration);
    }
    
    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60);
        return $"{hours:00}:{minutes:00}";
    }
    
    // Визуализация в редакторе
    private void OnValidate()
    {
        if (Application.isPlaying && directionalLight != null)
        {
            UpdateLight();
        }
    }
}
