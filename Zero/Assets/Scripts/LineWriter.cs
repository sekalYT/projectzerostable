using UnityEngine;

public class LineWriter : MonoBehaviour
{
    public Color trailColor = Color.white;  // Начальный цвет следа (можно менять в инспекторе)

    private Transform characterTransform;
    private LineRenderer lineRenderer;

    private void Start()
    {
        characterTransform = transform.parent; // Получаем ссылку на родительский объект (персонажа)
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = trailColor;
        lineRenderer.endColor = trailColor;
    }

    private void Update()
    {
        UpdateTrailPosition();
    }

    private void UpdateTrailPosition()
    {
        // Плавно перемещаем след к позиции персонажа
        Vector3 characterPosition = characterTransform.position;
        Vector3 trailPosition = transform.position;

        // Расстояние между персонажем и следом (можно настроить, например, 0.1f - для плавности)
        float distance = 0.1f;

        // Плавное перемещение
        transform.position = Vector3.Lerp(trailPosition, characterPosition, distance);
    }

    // Метод для изменения цвета следа
    public void ChangeTrailColor(Color newColor)
    {
        trailColor = newColor;
        lineRenderer.startColor = newColor;
        lineRenderer.endColor = newColor;
    }
}