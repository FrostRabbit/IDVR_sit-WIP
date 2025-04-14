using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // graffiti
    public Text uiText;

    public TextMeshProUGUI HPText;

    public int PlayerMaxHP = 5;

    public int PlayerHP;

    public AudioSource hurtsound;

    private int level = 2;

    private int currentlevel = 1;

    //states
    enum State { NotStarted, Playing, GameOver, WonGame , NextLevel }

    // current state
    State currState;

    // Enemy Manager
    EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        // start as not playing
        currState = State.NotStarted;

        // find the enemy manager
        enemyManager = GameObject.Find("EnemyManager(level1)").GetComponent<EnemyManager>();

        // refresh UI
        RefreshUI();

        // log error if it wasn't found
        if(enemyManager == null)
        {
            Debug.LogError("there needs to be an EnemyManager in the scene");
        }
    }

    void RefreshUI()
    {
        // act according to the state
        switch(currState)
        {
            case State.NotStarted:
                uiText.text = "Shoot here to begin";
                break;

            case State.Playing:
                uiText.text = "Enemies left: " + enemyManager.numEnemies;
                break;

            case State.GameOver:
                uiText.text = "Game Over! Shoot here";
                break;

            case State.WonGame:
                uiText.text = "YOU WON! Shoot here";
                break;

            case State.NextLevel:
                uiText.text = "Next Level! Shoot here";
                break;
        }  
    }

    public void InitGame()
    {
        //don't initiate the game if the game is already running!
        if (currState == State.Playing) return;

        if (currentlevel == 1){
            // set player HP
            PlayerHP = PlayerMaxHP;

            // set HP text
            SetHPText();
        }

        // set the state
        currState = State.Playing;

        string levelText = "EnemyManager(level" + currentlevel + ")";

        Debug.Log(levelText);

        // find the enemy manager
        enemyManager = GameObject.Find(levelText).GetComponent<EnemyManager>();

        // create enemy wave
        enemyManager.CreateEnemyWave();

        // show text on the graffiti
        RefreshUI();
    }


    // game over
    public void GameOver()
    {
        // do nothing if we were already on game over
        if (currState == State.GameOver) return;

        // set the state to game over
        currState = State.GameOver;

        // show text on the graffiti
        RefreshUI();

        // remove all enemies
        enemyManager.KillAll();

        // clear HP text
        HPText.text = "";

        currentlevel = 1;
    }

    // checks whether we've won, and if we did win, refresh UI
    public void HandleEnemyDead()
    {
        if (currState != State.Playing) return;

        RefreshUI();

        // have we won the game?
        if(enemyManager.numEnemies <= 0 && currentlevel == level)
        {
            // set the state of the game
            currState = State.WonGame;

            // show text on the graffiti
            RefreshUI();

            // remove all enemies
            enemyManager.KillAll();

            currentlevel = 1;
        }else if(enemyManager.numEnemies <= 0)
        {
            // set the state of the game
            currState = State.NextLevel;

            // show text on the graffiti
            RefreshUI();

            // remove all enemies
            enemyManager.KillAll();

            // increase level
            currentlevel++;

        }
    }

    public void SetHPText()
    {
        HPText.text = "<color=#a8a3a3>";
        for(int i = 0; i < PlayerMaxHP-PlayerHP; i++)
        {
            HPText.text += "♥";
        }
        HPText.text += "</color>";

        HPText.text += "<color=#f26868>";
        for(int i = 0; i < PlayerHP; i++)
        {
            HPText.text += "♥";
        }
        HPText.text += "</color>";
    }

    public EnemyManager GetCurrentEm()
    {
        return enemyManager;
    }
}
