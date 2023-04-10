using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform currentButtonLocation;

    public GameObject hoverMenuPrefab;

    private GameObject hoverMenuPrefabInstance;

    private string buttonType;

    void Start()
    {
        currentButtonLocation = gameObject.GetComponent<RectTransform>(); //get hovered button location
        buttonType = gameObject.name; //get buttonType by button's name
    }


    void InstantiateHoverMenu()
    {
        hoverMenuPrefabInstance = Instantiate(hoverMenuPrefab); //Instantiate Hover Menu Prefab
        hoverMenuPrefabInstance.GetComponent<RectTransform>().parent = gameObject.GetComponentInParent<RectTransform>(); //Set instance as parent of children of canvas
        hoverMenuPrefabInstance.GetComponent<RectTransform>().position = currentButtonLocation.position; //Move Menu to button's location
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case "Purchase Button": //if hovered on purchase button
                InstantiateHoverMenu();
                if (GameManager.instance.blankingVocab1.ContainsKey(gameObject.GetComponentInChildren<TextMeshProUGUI>().text)) //if the button is for blanking vocab
                {
                    hoverMenuPrefabInstance.GetComponentInChildren<TextMeshProUGUI>().text = "0 Mental Capacity"; //show free cost
                }
                else
                {
                    hoverMenuPrefabInstance.GetComponentInChildren<TextMeshProUGUI>().text = //if not blanking vocab
                        GameManager.instance.vocabPrice + " Mental Capacity"; //show the exact price
                }
                break;
            
            case "Speak Button": //if hovered on speak button
                if (GameManager.instance.blankingVocab1.ContainsKey(gameObject.GetComponentInChildren<TextMeshProUGUI>().text)) //if the button is for blanking vocab
                {
                    InstantiateHoverMenu();
                    hoverMenuPrefabInstance.GetComponentInChildren<TextMeshProUGUI>().text = "Recover " +
                        GameManager.instance.blankingVocabRecoveryAmount + " Mental Capacity"; //show recover cost amount
                }
                break;
            
            case "BrainStorm": //if hovered on brainstorm button
                InstantiateHoverMenu();
                hoverMenuPrefabInstance.GetComponentInChildren<TextMeshProUGUI>().text = "1 Mental Capacity"; //show cost
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(hoverMenuPrefabInstance); //destroy menu instance when ending hover
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Destroy(hoverMenuPrefabInstance); //destroy menu instance when clicked on button
    }
}
