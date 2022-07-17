using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains all UI containers for both characters and the game UI elements (round count, timer, etc.), including all the methods for updating the UI.
/// </summary>
/// <remarks>
/// Serves as a base class. GameControl does all the work.
/// </remarks>
public class UIControl : MonoBehaviour
{
    //Inner class representing UI of each character.
    [Serializable] protected class CharacterUI {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider blockSlider;
        [SerializeField] private Slider energySlider;
        [SerializeField] private Text roundsWonText;
        
        private int maxHealth = 200, maxBlock = 100, minEnergy = 0;

        public void InitCharacter() {
            healthSlider.value = maxHealth;
            blockSlider.value = maxBlock;
            energySlider.value = minEnergy;
            roundsWonText.text = "";
        }

        public void ResetCharacter() {
            healthSlider.value = maxHealth;

            blockSlider.value = maxBlock;

            energySlider.value = minEnergy;
        }

        public void ResetRounds() {
            roundsWonText.text = "";
        }

        public void UpdateHealth(int newValue) {
            healthSlider.value = newValue;
        }

        public void UpdateBlock(float newValue) {
            blockSlider.value = newValue;
        }

        public void UpdateEnergy(int newValue) {
            energySlider.value = newValue;
        }

        public void UpdateRoundsWon() {
            roundsWonText.text += "V";
        }

        public int GetCurrentHealth() {
            return (int)healthSlider.value;
        }

        public int GetRoundsWon() {
            return roundsWonText.text.Length;
        }
    }    

    [SerializeField] protected CharacterUI player = new CharacterUI();
    [SerializeField] protected CharacterUI enemy = new CharacterUI();

    [SerializeField] protected Text winnerText;
    [SerializeField] protected Text currentRoundText;
    [SerializeField] protected Text timerText;
    [SerializeField] protected Text coordinatesText;
    [SerializeField] protected Image notificationBackground;

    [SerializeField] protected Button startGameButton;
    [SerializeField] protected Button restartGameButton;
    [SerializeField] protected Button trainingModeButton;
    [SerializeField] protected Button mouseAimButton;
    [SerializeField] protected Button quitGameButton;

    protected RectTransform startGameButtonRect;
    protected Text startGameButtonText;
    protected Text trainingModeButtonText;
    protected Text mouseAimingButtonText;
    
    //Resets UI of characters and gets referenc
    protected virtual void Start()
    {
        player.InitCharacter();
        enemy.InitCharacter();
        startGameButtonRect = startGameButton.GetComponent<RectTransform>();
        startGameButtonText = startGameButton.GetComponentInChildren<Text>();
        trainingModeButtonText = trainingModeButton.GetComponentInChildren<Text>();
        mouseAimingButtonText = mouseAimButton.GetComponentInChildren<Text>();
    }

    /// <summary>
    /// Updates the health bars of the characters.
    /// </summary>
    public void UpdateHealthBar(int health, string character) {
        if(character == "Enemy") {
            enemy.UpdateHealth(health);
        } else {
            player.UpdateHealth(health);
        }
    }

    /// <summary>
    /// Updates the block bars of the characters.
    /// </summary>
    public void UpdateBlockBar(float blockValue, string character) {
        if(character == "Enemy") {
            enemy.UpdateBlock(blockValue);
        } else {
            player.UpdateBlock(blockValue);
        }
    }

    /// <summary>
    /// Updates the energy bars of the characters.
    /// </summary>
    public void UpdateEnergyBar(int energyValue, string character) {
        if(character == "Enemy") {
            enemy.UpdateEnergy(energyValue);
        } else {
            player.UpdateEnergy(energyValue);
        }
    }
    
    /// <summary>
    /// Updates the on screen coordinates of the player's gaze. 
    /// </summary>
    /// <remarks>
    /// Used for debugging.
    /// </remarks>
    public void UpdateCoordinatesText(float x, float y) {
        coordinatesText.text = String.Format("x: {0}\ny: {1}", x.ToString("F"), y.ToString("F"));
    }
}
