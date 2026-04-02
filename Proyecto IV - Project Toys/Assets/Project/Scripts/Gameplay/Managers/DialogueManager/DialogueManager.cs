using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
//using EasyTextEffects;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;

    [Header("Settings")]
    [SerializeField] private int letterWaitFrames = 1;

    //Runtime Dialogue State
    private Queue<Sentence> sentences = new Queue<Sentence>();
    private bool isTyping = false;
    private bool cancelTyping = false;
    private string currentVoiceBlip = "";

    //References
    private SoundManager sm;
    private Player player;

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        sm = FindAnyObjectByType<SoundManager>();
        player = FindAnyObjectByType<Player>();
        
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (player.input.Player.Interact.WasPressedThisFrame())
        {
            if (isTyping) //Skip Typing Animation
            {
                cancelTyping = true;
            }
            else //Next Sentence
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartComment(DialogueComment comment)
    {
        sentences.Clear();
        player.canMove = false;
        
        dialoguePanel.SetActive(true);

        foreach (Sentence sentence in comment.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndComment();
            return;
        }

        Sentence currentSentence = sentences.Dequeue();

        // PlayAtStart Event
        if (currentSentence.sentenceEvent.WhenToPlay == DialogueEvent.PlayWhen.PlayAtStart)
        {
            StartCoroutine(DelayEvent(currentSentence.sentenceEvent.timeOffset, currentSentence.sentenceEvent.uEvent));
        }

        // 2. ACTUALIZAR LA INTERFAZ CON EL PERFIL DEL PERSONAJE
        if (currentSentence.character != null)
        {
            nameText.text = currentSentence.character.characterName;
            dialogueText.color = currentSentence.character.textColor;
            currentVoiceBlip = currentSentence.character.voiceBlipName;

            Sprite newPortrait = currentSentence.character.GetPortrait(currentSentence.emotion);
            if (newPortrait != null)
            {
                portraitImage.sprite = newPortrait;
                portraitImage.enabled = true;
            }
            else
            {
                portraitImage.enabled = false;
            }
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    private IEnumerator TypeSentence(Sentence sentence)
    {
        isTyping = true;
        cancelTyping = false;
        dialogueText.text = "";
        
        /*if (dialogueText.TryGetComponent(out TextEffect effect))
        {
            effect.enabled = false;
        }*/

        bool isTag = false;
        
        foreach (char letter in sentence.sentenceText)
        {
            if (cancelTyping)
            {
                dialogueText.text = sentence.sentenceText;
                break;
            }

            dialogueText.text += letter;

            if (letter == '<') isTag = true;
            else if (letter == '>') isTag = false;

            if (!isTag)
            {
                if (!string.IsNullOrEmpty(currentVoiceBlip) && sm != null)
                {
                    sm.Play(currentVoiceBlip);
                }
                
                for (int i = 0; i < letterWaitFrames; i++)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        // PlayAtEnd Event
        if (sentence.sentenceEvent.WhenToPlay == DialogueEvent.PlayWhen.PlayAtEnd)
        {
            StartCoroutine(DelayEvent(sentence.sentenceEvent.timeOffset, sentence.sentenceEvent.uEvent));
        }
        
        /*if (dialogueText.TryGetComponent(out TextEffect finalEffect))
        {
            finalEffect.enabled = true;
        }*/

        isTyping = false;
    }

    private void EndComment()
    {
        dialoguePanel.SetActive(false);
        player.canMove = true;
    }

    private IEnumerator DelayEvent(float delay, UnityEvent uEvent)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        uEvent?.Invoke();
    }
}

