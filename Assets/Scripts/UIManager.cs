using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text livesText;
    public BasketballShooter basketballShooter;

    void Update()
    {
        scoreText.text = "Score: " + basketballShooter.score;
        livesText.text = "Lives: " + basketballShooter.currentLives;
    }
}
