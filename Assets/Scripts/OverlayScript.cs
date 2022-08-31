using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class OverlayScript : MonoBehaviour
{
    public GameObject playerOverlayPrefab;
    public Transform overlayParent;
    public Camera cam;

    public UIManager uiManager;

    public float overlayScale;

    List<GameObject> playersAndEnemies = new List<GameObject>();
    List<GameObject> overlayObjects = new List<GameObject>();

    private void Awake()
    {
        SetUpOverlays();
    }

    private void Update()
    {
        DisplayPlayerOverlay();
    }

    public void SetUpOverlays()
    {
        playersAndEnemies = uiManager.GetAllPeople();

        foreach(Transform child in overlayParent)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < playersAndEnemies.Count; i++)
        {
            GameObject overlayObj = Instantiate(playerOverlayPrefab);
            overlayObj.transform.SetParent(overlayParent);

            Data data = playersAndEnemies[i].GetComponent<Data>();

            // health
            overlayObj.transform.GetChild(0).GetComponent<Slider>().maxValue = data.startingHealth;
            overlayObj.transform.GetChild(0).GetComponent<Slider>().value = data.startingHealth;
             
            // armour
            overlayObj.transform.GetChild(1).GetComponent<Slider>().maxValue = data.startingArmour;
            overlayObj.transform.GetChild(1).GetComponent<Slider>().value = data.startingArmour;

            // token text
            overlayObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + ((data.roundTokens == 0) ? "" : data.roundTokens);

            overlayObjects.Add(overlayObj);
        }
    }

    void DisplayPlayerOverlay()
    {
        for (int i = 0; i < overlayObjects.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(playersAndEnemies[i].transform.position);
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);

            RectTransform rectTransform = overlayObjects[i].GetComponent<RectTransform>();
            rectTransform.localPosition = canvasPos;
            rectTransform.localScale = new Vector3(cam.orthographicSize * overlayScale, cam.orthographicSize * overlayScale, cam.orthographicSize * overlayScale);
        }
    }
}
