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
    public static GameManager instance;
    
    public Dictionary<string, int> vocabForPurchase1;

    public Dictionary<string, int> blankingVocab1;

    public int vocabPrice = 2;

    public int blankingVocabRecoveryAmount = 2;

    [HideInInspector] public int mentalCapacity;

    public int initialMentalCapacity = 3;

    private int currentSpeakingSentence = 0;

    private GameObject[] purchaseButton;

    private GameObject[]  speakButton;

    private TextMeshProUGUI mentalCapacityText;

    private TextMeshProUGUI speechBox;

    private TextMeshProUGUI targetSentenceText;

    private RectTransform progressBar;

    private int vocabsOwnedQuantity = 0;

    public float timeForEachVocab = 15;

    private float timeLeft;

    private List<string> vocabSpoken;
    
    private void Awake()
    {
        instance = this; //set Game Manager to Singleton
    }

    void Start()
    {
        mentalCapacity = initialMentalCapacity; //set MC to initial MC
        
        //set list/dict
        vocabForPurchase1 = new Dictionary<string, int>();
        blankingVocab1 = new Dictionary<string, int>();
        vocabSpoken = new List<string>();
        
        progressBar = GameObject.Find("Fill").GetComponent<RectTransform>(); //get progress bar
        
        timeLeft = timeForEachVocab; //set timer
        
        DisassembleSpeech();
        AddBlankingVocabs();
        
        //set a 5 button purchaseButton array
        purchaseButton = new GameObject[5];
        purchaseButton = GameObject.FindGameObjectsWithTag("Purchase Button");
        foreach (GameObject button in purchaseButton)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        } 
        
        //set a 8 button speakButton array
        speakButton = new GameObject[8];
        speakButton = GameObject.FindGameObjectsWithTag("Speak Button");
        foreach (GameObject button in speakButton)
        {
            button.SetActive(false);
        } 
        foreach (GameObject button in speakButton)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        
        //set TMProUGUI
        mentalCapacityText = GameObject.FindWithTag("Mental Capacity Text").GetComponent<TextMeshProUGUI>();
        speechBox = GameObject.Find("Speech Box").GetComponent<TextMeshProUGUI>();
        targetSentenceText = GameObject.Find("Target Sentence").GetComponent<TextMeshProUGUI>();
        
        //set vocab pool
        Refresh();
        mentalCapacity += 1;
        
        
    }
    
    void Update()
    {
        //restart shortcut
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        //losing condition
        if (timeLeft <= 0)
        {
            Debug.Log("Game Over!");
            SceneManager.LoadScene(1);
            return;
        }
        
        CountDownTime();
        DisplayResources();
        DisplayTargetSentence();
    }

    void DisassembleSpeech()  //disassemble the speech text file into vocabForPurchase1 dict
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

    private void AddBlankingVocabs() //set blanking vocabs dict and merge it with purchase dict
    {
        blankingVocab1.Add("like", 0);
        // blankingVocab1.Add("for example", 0);
        blankingVocab1.Add("uhh", 0);
        blankingVocab1.Add("I mean", 0);
        blankingVocab1.Add("well", 0);
        blankingVocab1.Add("you know", 0);
        blankingVocab1.Add("hmm", 0);
        // blankingVocab1.Add("let me see", 0);
        blankingVocab1.Add("so", 0);
        blankingVocab1.Add("okay", 0);
        blankingVocab1.Add("sorry", 0);
        vocabForPurchase1 = vocabForPurchase1.Union(blankingVocab1).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    void DisplayResources()
    {
        mentalCapacityText.text = "Mental Capacity: " + mentalCapacity.ToString();
    }

    void DisplaySentence(string vocab) //add space and , for different vocabs
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
        TextAsset speechText = Resources.Load<TextAsset>("speech1");
        string[] speechLines = speechText.text.Split("\n");
        List<string> vocabsInLine = new List<string>();
        List<string> vocabSpokenWithoutBlanking = new List<string>();
        // foreach (string vocab in vocabSpoken)
        // {
        //     if (blankingVocab1.ContainsKey(vocab))
        //     {
        //         vocabSpoken.Remove(vocab);
        //     }
        // }
        //
        // vocabSpokenWithoutBlanking = vocabSpoken;
        targetSentenceText.text = speechLines[currentSpeakingSentence]; //set target sentence text
        
        string[] vocabs = speechLines[currentSpeakingSentence].Split(" ");
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
            vocabsInLine.Add(vocab.ToLower()); //set target sentence vocab list
        }
        
        if (vocabsInLine.SequenceEqual(vocabSpoken, StringComparer.Ordinal)) //compare spoken list and target list, if same, go to next sentence
        {
            currentSpeakingSentence += 1;
            vocabSpoken.Clear();
            speechBox.text = "";
        }

        if (currentSpeakingSentence >= speechLines.Length) //if finished all sentences, wins
        {
            Debug.Log("A successful speech");
            SceneManager.LoadScene(2);
        }
        // print(currentSpeakingSentence);
        // foreach (string vocab in vocabsInLine)
        // {
        //     print(vocab + vocabsInLine.IndexOf(vocab));
        // }
        //
        // foreach (string vocabu in vocabSpoken)
        // {
        //     print("spoken: " + vocabu + vocabsInLine.IndexOf(vocabu));
        // }
    }

    void CountDownTime() //timer and timer bar
    {
        timeLeft -= Time.deltaTime;
        progressBar.localScale = new Vector3(timeLeft / timeForEachVocab, 1, 1);
    }

    public void Refresh() //click brainstorm button to refresh pool
    {
        if (mentalCapacity >= 1)
        {
            mentalCapacity -= 1;
            mentalCapacityText.color = Color.black;
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
            mentalCapacityText.color = Color.red;
        }
    }
    
    public void Buy() //click on purchase button to buy vocabs
    {
        string clickedButtonText =
            EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        int price = vocabForPurchase1[clickedButtonText];
        if (price <= mentalCapacity && vocabsOwnedQuantity < 8) 
        {
            mentalCapacity -= price;
            mentalCapacityText.color = Color.black;
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
            mentalCapacityText.color = Color.red;
        }
    }

    public void PlayHand() //click on speak button to say the vocab
    {
        string clickedButtonText =
            EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text;
        if (!blankingVocab1.ContainsKey(clickedButtonText))
        {
            vocabSpoken.Add(clickedButtonText);
        }
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
