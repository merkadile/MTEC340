using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public TextMeshProUGUI TextBox;

    int _score = 0;

    public int Score {
        get => _score;
        set {
            _score = value;
            TextBox.text = "Score: " + Score.ToString();
        }
    }
}