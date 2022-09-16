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
            GenerateOverlay(i);
        }
    }

    void GenerateOverlay(int _i)
    {
        GameObject overlayObj = Instantiate(playerOverlayPrefab);
        overlayObj.transform.SetParent(overlayParent);

        Data data = playersAndEnemies[_i].GetComponent<Data>();

        UpdateValues(overlayObj, data);

        overlayObjects.Add(overlayObj);
    }

    public void UpdateOverlayDisplays()
    {
        for (int i = 0; i < overlayObjects.Count; i++)
        {
            Data data = playersAndEnemies[i].GetComponent<Data>();

            UpdateValues(overlayObjects[i], data);
        }
    }

    void UpdateValues(GameObject _overlayObj, Data _data)
    {
        // health
        _overlayObj.transform.GetChild(0).GetComponent<Slider>().maxValue = _data.startingHealth;
        _overlayObj.transform.GetChild(0).GetComponent<Slider>().value = _data.startingHealth;

        // armour
        _overlayObj.transform.GetChild(1).GetComponent<Slider>().maxValue = _data.startingArmour;
        _overlayObj.transform.GetChild(1).GetComponent<Slider>().value = _data.startingArmour;

        // token text
        _overlayObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + ((_data.roundTokens == -1) ? "" : _data.roundTokens);
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
            float scale = overlayScale / cam.orthographicSize;
            rectTransform.sizeDelta = new Vector2(scale, scale);
        }
    }

    public void AddedEnemy(GameObject _gameObject)
    {
        playersAndEnemies.Add(_gameObject);
        GenerateOverlay(playersAndEnemies.Count - 1);
    }
    public void RemoveEnemy(GameObject _gameObject)
    {
        int index = overlayObjects.FindIndex(x => x.Equals(_gameObject));
        Destroy(overlayObjects[index].gameObject);
        overlayObjects.Remove(_gameObject);
        playersAndEnemies.Remove(_gameObject);
    }
}
