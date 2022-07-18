// Author: Ján Kučera
// Email: xkucer0b@stud.fit.vutbr.cz
using System.Collections;
using UnityEngine;

public class GameControl : UIControl
{
    private int roundsToWin = 2;
    private int currentRound = 1;
    private bool gameover = false;
    private int roundDuration = 99;
    private int timeLeft;
    private float nextTimerUpdate;

    private GameObject playerObject;
    private GameObject enemyObject;
    private CharacterStatus playerStatus;
    private EnemyStatus enemyStatus;
    private Vector3 playerSpawnPosition;
    private Vector3 enemySpawnPosition;
    private Vector3 faceLeftRotation = new Vector3(0, -90, 0);

    private bool afterFirstStart = false;
    private bool isTrainingModeOn = false;
    private bool isMouseAimingOn = true;

    private bool isPaused = false;
    private bool countdownInProcess = false;

    //Gets references to gameObjects of characters and their positions.
    void Awake() {
        playerObject = GameObject.Find("Player");
        enemyObject  = GameObject.Find("Enemy");

        playerStatus = playerObject.GetComponent<CharacterStatus>();
        enemyStatus = enemyObject.GetComponent<EnemyStatus>();

        playerSpawnPosition = playerObject.GetComponent<Transform>().position; 
        enemySpawnPosition = enemyObject.GetComponent<Transform>().position;
    }

    //Set up UI, pause game and show menu on start.
    protected override void Start() {
        base.Start();
        
        ResetTimer();

        currentRoundText.text = "Round " + currentRound;
        notificationBackground.enabled = false;

        Pause();
        SetButtonVisibility(true);
    }

    //Periodically check all logic of game. Pause, unpause, gameover, update timer, check health of character.
    void Update() {
        if(countdownInProcess || !afterFirstStart) {
            return;
        }

        if(gameover) {
            if(Input.GetButtonDown("Jump")) {
                gameover = false;
                ResetGame();
            }

            if(Input.GetKeyDown(KeyCode.Escape)) {
                Application.Quit();
            }
        } else {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                if(isPaused) {
                    Unpause();
                    SetButtonVisibility(false);
                } else {
                    Pause();
                    SetButtonVisibility(true);
                }
            }

            if(!isPaused) {
                if(isTrainingModeOn) {
                    return;
                }

                UpdateTimer();

                if(player.GetCurrentHealth() == 0) {
                    if (enemy.GetCurrentHealth() == 0) {
                        DecideRoundWinner();
                    } else {
                        DecideRoundWinner("Enemy");
                    }
                } else if (enemy.GetCurrentHealth() == 0) {
                    DecideRoundWinner("Player");           
                }
            }
        }
    }

    private void ResetGame() {
        currentRound = 1;
        currentRoundText.text = currentRound.ToString();
        player.ResetRounds();
        enemy.ResetRounds();
        ResetRound();
    }

    private void ResetRound() {
        Pause();
        ResetUI();
        ResetTimer();
        ResetCharacters();
        StartCoroutine(RoundStartCountdown());
    }

    //Shows countdown to start of the round.
    private IEnumerator RoundStartCountdown() {
        countdownInProcess = true;
        notificationBackground.enabled = true;
        for(int i = 3; i > 0; i--) {
            winnerText.text = "Round start in <color=#00ff00ff>" + i +"</color>";
            yield return new WaitForSecondsRealtime(1);
        }

        winnerText.text = "";
        notificationBackground.enabled = false;
        countdownInProcess = false;
        Unpause();
    }

    private void ResetUI() {
        player.ResetCharacter();
        enemy.ResetCharacter();
        currentRoundText.text = "Round " + currentRound;
    }

    private void ResetTimer() {
        timeLeft = roundDuration;
        timerText.text = timeLeft.ToString();
        nextTimerUpdate = Time.time + 1f;
    }

    //Resets positons of characters.
    private void ResetCharacters() {    
        playerObject.GetComponent<CharacterStatus>().ResetCharacter();
        playerObject.transform.position = playerSpawnPosition;
        playerObject.transform.eulerAngles  = faceLeftRotation;

        enemyObject.GetComponent<CharacterStatus>().ResetCharacter();
        enemyObject.transform.position = enemySpawnPosition;
        enemyObject.transform.eulerAngles = faceLeftRotation * -1;
    }

    private void UpdateTimer() {
        if(Time.time > nextTimerUpdate) {
            timeLeft--;
            timerText.text =  timeLeft.ToString();
            nextTimerUpdate = Time.time + 1f;
            if(timeLeft == 0) {
               isPaused = true; 
               DecideTimeoutWinner();
            }
        }
    }

    private void DecideTimeoutWinner() {
        if(player.GetCurrentHealth() < enemy.GetCurrentHealth()) {
            enemy.UpdateRoundsWon();
        } else if(player.GetCurrentHealth() > enemy.GetCurrentHealth()) {
            player.UpdateRoundsWon();
        } else {
            player.UpdateRoundsWon();
            enemy.UpdateRoundsWon();
        }  

        DecideGameWinner();
    }

    private void DecideRoundWinner(string winner = null) {
        if(winner == null) {
            player.UpdateRoundsWon();
            enemy.UpdateRoundsWon();
        } else if(winner == "Player") {
            player.UpdateRoundsWon();
        } else {
            enemy.UpdateRoundsWon();
        }

        DecideGameWinner();
    }

    private void DecideGameWinner() {
        if(enemy.GetRoundsWon() == roundsToWin) {
            if(player.GetRoundsWon() == roundsToWin) {
                GameOver(2);
            } else {
                GameOver(1);
            }
        } else if(player.GetRoundsWon() == roundsToWin) {
            GameOver(0);
        } else {
            currentRound++;
            ResetRound();
        }
    }
    
    //Shows game over screen with winner highlighted.
    private void GameOver(int result) {
        gameover = true;
        Pause();
        notificationBackground.enabled = true;
        
        string winner;
        string resetText = "\nPress Space to restart.\nPress Esc to quit.";

        if(result == 0) {
            winner = "Winner is <color=#00ff00ff>Player</color>";
        } else if (result == 1) {
            winner = "Winner is <color=#00ff00ff>Computer</color>";
        } else {
            winner = "It's a <color=#00ff00ff>draw</color>";    
        }

        winnerText.text = winner + resetText;   
    }

    private void Pause() {
        Time.timeScale = 0;
        isPaused = true;
    }

    private void Unpause() {
        Time.timeScale = 1;
        isPaused = false;
        SetButtonVisibility(false);
    }

    //Hides/shows menu.
    private void SetButtonVisibility(bool visible) {
        startGameButton.gameObject.SetActive(visible);
        restartGameButton.gameObject.SetActive(visible);
        trainingModeButton.gameObject.SetActive(visible);
        mouseAimButton.gameObject.SetActive(visible);
        quitGameButton.gameObject.SetActive(visible);
    }

    //Updates menu after start button is pressed.
    public void StartGame()
    {
        if(!afterFirstStart) {
            startGameButtonText.text = "Resume Game";
            startGameButtonRect.anchoredPosition = new Vector2(startGameButtonRect.anchoredPosition.x, 80);
            restartGameButton.interactable = true;
            afterFirstStart = true;

            StartCoroutine(RoundStartCountdown());
        } else {
            Unpause();
            SetButtonVisibility(false);
        }
    }

    public void RestartMatch()
    {
        SetButtonVisibility(false);
        ResetGame();
    }

    //Toggles training mode, starting or stoping periodic call of methods on CharacterStatus scripts of both characters.
    //Method resets their health, block and energy.
    public void ToggleTrainingMode()
    {
        string buttonText = "Training Mode: ";
        if(isTrainingModeOn) {
            isTrainingModeOn = false;
            trainingModeButtonText.text = buttonText + "<color='red'>Off</color>";

            playerStatus.CancelInvoke("SetValuesToMax");
            enemyStatus.CancelInvoke("SetValuesToMax");

            playerStatus.ResetCharacter();
            enemyStatus.ResetCharacter();

            player.ResetCharacter();
            enemy.ResetCharacter();
        } else {
            isTrainingModeOn = true;
            trainingModeButtonText.text = buttonText + "<color='green'>On</color>";

            playerStatus.InvokeRepeating("SetValuesToMax", 0f, 2f);
            enemyStatus.InvokeRepeating("SetValuesToMax", 0f, 2f);
        }
    }

    public void ToggleMouseAiming()
    {   
        string buttonText = "Aim Mode: ";
        if(isMouseAimingOn) {
            isMouseAimingOn = false;
            mouseAimingButtonText.text = buttonText + "<color='green'>Eye Gaze</color>";
        } else {
            isMouseAimingOn = true;
            mouseAimingButtonText.text = buttonText + "<color='green'>Mouse</color>";
        }
    }

    public void QuitGame() {
        Application.Quit();
    }

    public bool GetIsGamePaused() {
        return isPaused;
    }

    public bool GetIsTrainingModeOn() {
        return isTrainingModeOn;
    }

    public bool GetIsMouseAimingOn() {
        return isMouseAimingOn;
    }
}
