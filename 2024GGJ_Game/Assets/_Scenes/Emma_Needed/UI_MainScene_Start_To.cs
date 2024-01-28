using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//Emma_Add the UI Scene 

public class UI_MainScene_Start_To : MonoBehaviour
{
    [SerializeField] Animator animator;
    public GameObject tutorialMenu;
    public GameObject producerMenu;
    public GameObject iconShowLater;
    public GameObject iconShowEarly;

    public float fadeInDuration = 5.0f;
    private CanvasGroup canvasGroup;
    private List<GameObject> deactivatedUIObjects = new List<GameObject>();

    public Animator[] buttonAnimators; 
    public float delayBetweenButtons = 0.5f;

    private void Start()
    {
        StartCoroutine(PlayButtonAnimationsSequentially());
        tutorialMenu.SetActive(false);
        producerMenu.SetActive(false); 
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeInDuration;
        }
    }

    IEnumerator PlayButtonAnimationsSequentially()
    {
        foreach (Animator animator in buttonAnimators)
        {
            animator.SetTrigger("PlayAnimation"); 
            yield return new WaitForSeconds(delayBetweenButtons);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Load Main Menu to Level Level Selection Page.
    public void UI_1_MainMenutoLevelSelect()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    IEnumerator LoadSceneAfterDelay()
    {
        // Deactivate all objects in the "UI" layer
        animator.SetBool("isClick", true);
        int uiLayer = LayerMask.NameToLayer("UI");
        foreach (var go in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (go.layer == uiLayer)
            {
                go.SetActive(false);
                deactivatedUIObjects.Add(go);
            }
        }

        iconShowLater.SetActive(true);
        // Wait for 1 second
        yield return new WaitForSeconds(1);
        iconShowLater.SetActive(false);
        iconShowEarly.SetActive(false);
        // Load the scene
        SceneManager.LoadSceneAsync("Level1");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reactivate all objects in the "UI" layer
        foreach (var go in deactivatedUIObjects)
        {
            go.SetActive(true);
        }
    }

    public void ShowTutorial() { 
              tutorialMenu.SetActive(true);
    }

    public void HideTutorial() {  tutorialMenu.SetActive(false); }

    public void ShowProducer() { 
        producerMenu.SetActive(true);
    }

    public void HideProducer() { producerMenu.SetActive(false); }
}