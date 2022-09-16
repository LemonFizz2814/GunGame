using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AccuracyGraph : MonoBehaviour
{
    [Header("Graph data")]
    public float graphX;
    public float graphY;
    public float lineThickness;
    public float margin;
    public Transform lineParent;

    public void CreateGraph(WeaponComponent.WeaponStats _weaponStats)
    {
        foreach (Transform child in lineParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _weaponStats.accuracy.Length - 1; i++)
        {
            Vector2 positionA = new Vector3((_weaponStats.accuracy[i].distance * graphX) + margin, (_weaponStats.accuracy[i].accuracy * graphY) + margin);
            Vector2 positionB = new Vector3((_weaponStats.accuracy[i + 1].distance * graphX) + margin, (_weaponStats.accuracy[i + 1].accuracy * graphY) + margin);

            GameObject lineObj = new GameObject("LineObj" + i, typeof(Image));
            lineObj.transform.SetParent(lineParent);

            Vector2 dir = (positionA - positionB).normalized;
            //float angle = Vector2.Angle(positionA, positionB);
            float angle = Mathf.Atan2(positionB.y - positionA.y, positionB.x - positionA.x) * Mathf.Rad2Deg;
            float distance = Vector3.Distance(positionA, positionB);

            RectTransform rectTransform = lineObj.transform.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, lineThickness);
            rectTransform.anchoredPosition = positionA + dir * distance * -0.5f;
            rectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    public void CreateLine(int _x)
    {
        GameObject lineObj = new GameObject("AccuracyLine" + _x, typeof(Image));
        lineObj.transform.SetParent(lineParent);

        RectTransform rectTransform = lineObj.transform.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(80, 2);
        rectTransform.anchoredPosition = new Vector2(_x * graphX, 45);
        rectTransform.localEulerAngles = new Vector3(0, 0, 90);

        lineObj.GetComponent<Image>().color = new Color32(255, 0, 70, 255);
    }
}
