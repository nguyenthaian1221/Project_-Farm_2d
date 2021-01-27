﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public bool dialogueActive;
    GameObject toolbar;
    GameObject inventory;
    GameObject shop;
    GameObject chest;
    public string[] sentences;
    private int index;
    public Text textDisplay;
    public float typingSpeed;

    

    public GameObject pressToContinue;

    private void Awake()
    {
        toolbar = GameObject.FindWithTag("toolbar");
        shop = GameObject.FindWithTag("shop");
        //Debug.Log(shop);
        chest = GameObject.FindWithTag("chest");
        inventory = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.CompareTag("inventory"));
    }

    // Start is called before the first frame update
    void Start()
    {
        toolbar.SetActive(false);
        shop.SetActive(false);
        chest.SetActive(false);
        inventory.SetActive(false);
        
        //Debug.Log(inventory);

        StartCoroutine(Type());
    }


    // Update is called once per frame
    void Update()
    {
        if (textDisplay.text == sentences[index] && dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            GoToNextSentence();
            pressToContinue.SetActive(true);

        }

        //inventory.SetActive(false);
        //toolbar.SetActive(false);
        
    }

    IEnumerator Type()
    {
        foreach(char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void GoToNextSentence()
    {
        pressToContinue.SetActive(false);

        if (index < sentences.Length - 1)
        {
            textDisplay.text = "";
            index++;
            StartCoroutine(Type());
            
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dialogueBox.SetActive(false);
                toolbar.SetActive(true);
                chest.SetActive(true);

                shop.SetActive(true);
            }
            
        }
    }
}
