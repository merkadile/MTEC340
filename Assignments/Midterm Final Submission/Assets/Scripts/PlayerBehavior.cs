using UnityEngine;
using TMPro;

//this script updates the Score Text textbox whenever the player's score is updated.
public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    public int Score {
        get => _score;
        set {
            _score = value;
            scoreText.text = "Score: " + Score.ToString();
        }
    }

    int _score = 0;    
}