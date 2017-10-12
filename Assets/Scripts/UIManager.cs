using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    private CanvasGroup canvasgroup;
    public GameObject player;

    // Use this for initialization
    private void OnEnable()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
        GameManager.OnGameStateChange += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
    }

    void Awake () {


        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        canvasgroup = GetComponent<CanvasGroup>();
    }

    void ShowMenue()
    {
        //canvasgroup.alpha = 0.5f;
        //canvasgroup.interactable = true;
        StopAllCoroutines();
        StartCoroutine("FadeIn", .5f);
    }

    void HideMenue()
    {
        //canvasgroup.alpha = 0;
        //canvasgroup.interactable = false;
        StopAllCoroutines();
        StartCoroutine("FadeOut", .5f);
    }

    private IEnumerator FadeIn(float duration)
    {
        float initalAlpha = canvasgroup.alpha;

       

        for (float t = 0; t <duration; t += Time.deltaTime)
        {
            canvasgroup.alpha = Mathf.Lerp(initalAlpha, 1f, t/duration);
            yield return new WaitForEndOfFrame();
        }
        canvasgroup.interactable = true;
    }

    private IEnumerator FadeOut(float duration)
    {
        float initalAlpha = canvasgroup.alpha;

        canvasgroup.interactable = false;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasgroup.alpha = Mathf.Lerp(initalAlpha, 0, t / duration);
            yield return new WaitForEndOfFrame();
        }
    }


    void HandleGameStateChange(GameManager.GameState prevState, GameManager.GameState targetState)
    {
        if (targetState == GameManager.GameState.paused && player.GetComponent<PlayerController>().IsGrounded())
        {
            ShowMenue();
        }
        else
        {
            HideMenue();
        }
    }


}
