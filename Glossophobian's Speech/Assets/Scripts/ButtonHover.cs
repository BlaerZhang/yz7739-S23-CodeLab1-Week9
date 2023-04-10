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
        currentButtonLocation = gameObject.GetComponent<RectTransform>();
        buttonType = gameObject.name;
    }


    void InstantiateHoverMenu()
    {
        hoverMenuPrefabInstance = Instantiate(hoverMenuPrefab);
        hoverMenuPrefabInstance.GetComponent<RectTransform>().parent = gameObject.GetComponentInParent<RectTransform>();
        hoverMenuPrefabInstance.GetComponent<RectTransform>().position = currentButtonLocation.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case "Purchase Button":
                InstantiateHoverMenu();
                if (GameManager.instance.blankingVocab1.ContainsKey(gameObject.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    hoverMenuPrefabInstance.GetComponentInChildren<TextMeshProUGUI>().text = "0 Mental Capacity";
                }
                else
                {
                    hoverMenuPrefabInstance.GetComponentInChildren<TextMeshProUGUI>().text =
                        GameManager.instance.vocabPrice + " Mental Capacity";
                }
                break;
            
            case "Speak Button":
                if (GameManager.instance.blankingVocab1.ContainsKey(gameObject.GetComponentInChildren<TextMeshProUGUI>().text))
                {
                    InstantiateHoverMenu();
                    hoverMenuPrefabInstance.GetComponentInChildren<TextMeshProUGUI>().text = "Recover " +
                        GameManager.instance.blankingVocabRecoveryAmount + " Mental Capacity";
                }
                break;
            
            case "BrainStorm":
                InstantiateHoverMenu();
                hoverMenuPrefabInstance.GetComponentInChildren<TextMeshProUGUI>().text = "1 Mental Capacity";
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(hoverMenuPrefabInstance);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Destroy(hoverMenuPrefabInstance);
    }
}
