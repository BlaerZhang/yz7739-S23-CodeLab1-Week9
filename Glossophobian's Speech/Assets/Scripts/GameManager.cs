using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Dictionary<string, int> vocabForPurchase1;

    public Dictionary<string, int> blankingVocab1;

    public int vocabPrice = 2;

    public int blankingVocabRecoveryAmount = 2;

    public static int mentalCapacity;

    public int initialMentalCapacity = 3;

    private GameObject[] purchaseButton;

    private GameObject[]  speakButton;

    private TextMeshProUGUI mentalCapacityText;

    private TextMeshProUGUI speechBox;

    private RectTransform progressBar;

    private int vocabsOwnedQuantity = 0;

    public float timeForEachVocab = 15;

    private float timeLeft;

    private List<string> vocabSpoken;

    // Start is called before the first frame update
    void Start()
    {
        mentalCapacity = initialMentalCapacity;
        vocabForPurchase1 = new Dictionary<string, int>();
        blankingVocab1 = new Dictionary<string, int>();
        vocabSpoken = new List<string>();
        progressBar = GameObject.Find("Fill").GetComponent<RectTransform>();
        timeLeft = timeForEachVocab;
        
        DisassembleSpeech();
        AddBlankingVocabs();
        
        purchaseButton = new GameObject[5];
        purchaseButton = GameObject.FindGameObjectsWithTag("Purchase Button");
        foreach (GameObject button in purchaseButton)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        
        speakButton = new GameObject[8];
        speakButton = GameObject.FindGameObjectsWithTag("Speak Button");
        foreach (GameObject button in speakButton)
        {
            button.SetActive(false);
        }
        // Debug.Log(speakButton[7]);
        
        foreach (GameObject button in speakButton)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        
        mentalCapacityText = GameObject.FindWithTag("Mental Capacity Text").GetComponent<TextMeshProUGUI>();
        speechBox = GameObject.Find("Speech Box").GetComponent<TextMeshProUGUI>();
        
        Refresh();
        mentalCapacity += 1;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (timeLeft <= 0)
        {
            Debug.Log("Game Over!");
            return;
        }
        CountDownTime();
        DisplayResources();
        DisplayTargetSentence();
    }

    void DisassembleSpeech()
    {
        TextAsset speechText = Resources.Load<TextAsset>("speech1");
        string[] speechLines = speechText.text.Split("\n");
        foreach (string line in speechLines)
        {
            string[] vocabs = line.Split(" ");
            for (int i = 0; i < vocabs.Length; i++) 
            {
                string vocab = vocabs[i];
                if (vocab.Contains(","))
                {
                    vocab = vocab.Substring(0, vocab.Length - 1);
                    // Debug.Log(vocab);
                }
                
                if (vocab.Contains("."))
                {
                    vocab = vocab.Substring(0, vocab.Length - 2);
                    // Debug.Log(vocab);
                }

                if (!vocabForPurchase1.ContainsKey(vocab.ToLower()))
                {
                    vocabForPurchase1.Add(vocab.ToLower(), vocabPrice);
                }
            }
        }
        vocabForPurchase1.Remove("");
        // Debug.Log(VocabForPurchase1.Keys);
    }

    private void AddBlankingVocabs()
    {
        blankingVocab1.Add("like", 0);
        blankingVocab1.Add("for example", 0);
        blankingVocab1.Add("uhh", 0);
        blankingVocab1.Add("I mean", 0);
        blankingVocab1.Add("well", 0);
        blankingVocab1.Add("you know", 0);
        blankingVocab1.Add("hmm", 0);
        blankingVocab1.Add("let me see", 0);
        blankingVocab1.Add("so", 0);
        blankingVocab1.Add("okay", 0);
        blankingVocab1.Add("sorry", 0);
        vocabForPurchase1 = vocabForPurchase1.Union(blankingVocab1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    void DisplayResources()
    {
        mentalCapacityText.text = "Mental Capacity: " + mentalCapacity.ToString();
    }

    void DisplaySentence(string vocab)
    {
        if (blankingVocab1.ContainsKey(vocab)) 
        {
            speechBox.text += vocab + ", ";
        }
        else
        {
            speechBox.text += vocab + " ";
        }
    }

    void DisplayTargetSentence()
    {
        
    }

    void CountDownTime()
    {
        timeLeft -= Time.deltaTime;
        progressBar.localScale = new Vector3(timeLeft / timeForEachVocab, 1, 1);
    }

    public void Refresh()
    {
        if (mentalCapacity >= 1)
        {
            mentalCapacity -= 1;
            foreach (GameObject button in purchaseButton)
            {
                button.SetActive(true);
                int ramdomMaxIndex = vocabForPurchase1.Count;
                button.GetComponentInChildren<TextMeshProUGUI>().text =
                    vocabForPurchase1.Keys.ElementAt(Random.Range(0, ramdomMaxIndex));
            }
        }
        else
        {
            Debug.Log("Not Enough Capacity");
        }
    }
    
    public void Buy()
    {
        string clickedButtonText =
            EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        int price = vocabForPurchase1[clickedButtonText];
        if (price <= mentalCapacity && vocabsOwnedQuantity < 8) 
        {
            mentalCapacity -= price;
            EventSystem.current.currentSelectedGameObject.SetActive(false);
            foreach (GameObject button in speakButton)
            {
                if (!button.activeSelf)
                {
                    button.SetActive(true);
                    button.GetComponentInChildren<TextMeshProUGUI>().text = clickedButtonText;
                    vocabsOwnedQuantity += 1;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Not Enough Capacity or Slots");
        }
    }

    public void PlayHand()
    {
        string clickedButtonText =
            EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        vocabSpoken.Add(clickedButtonText);
        EventSystem.current.currentSelectedGameObject.SetActive(false);
        vocabsOwnedQuantity -= 1;
        if (blankingVocab1.ContainsKey(clickedButtonText))
        {
            mentalCapacity += blankingVocabRecoveryAmount;
        }
        timeLeft = timeForEachVocab;
        DisplaySentence(clickedButtonText);
    }
}
