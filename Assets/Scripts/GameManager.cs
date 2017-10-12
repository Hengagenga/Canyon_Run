//Erstellt am 08.08.2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public enum GameState
    {
        playing,
        paused,
        gameOver
    };


    private GameState m_currentState;
    public GameState currentState
    {
        get
        {
            return m_currentState;
        }
        private set
        {
            if (OnGameStateChange != null)
            {
                OnGameStateChange(m_currentState, value);
            }
            m_currentState = value;

        }
    }

    public delegate void GameStateChangeEvent(GameState prevState, GameState targetState);
    public static event GameStateChangeEvent OnGameStateChange;

    private int sceneIndex = 0;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }else {
           Destroy(gameObject);
        }
    }

    void Start () {
        currentState = GameState.playing;
    	}
	
	void Update () {
        //if (Input.GetKeyDown(KeyCode.N)) 
        //{
        //    LoadNextScene();
        //}

        if (Input.GetButtonDown("Cancel"))
        {
            if (currentState == GameState.playing)
            {
                currentState = GameState.paused;
            } else
            {
                currentState = GameState.playing;
            }
            print(currentState);
        }

        if (currentState == GameState.gameOver)
        {
            //endscreen
            SceneManager.LoadScene(2);
            currentState = GameState.playing;
        }
	}

    void LoadNextScene()
    {
        sceneIndex++;
        SceneManager.LoadScene(sceneIndex);
    }

    public bool IsPlaying()
    {
        return currentState == GameState.playing;
    }

    public void ResumeGame()
    {
         currentState = GameState.playing;
    }

    public void ded()
    {
        Debug.Log("GameOver");
        currentState = GameState.gameOver;
    }

}
