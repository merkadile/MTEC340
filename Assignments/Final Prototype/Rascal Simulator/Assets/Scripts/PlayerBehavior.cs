using UnityEngine;
using TMPro;

public class PlayerBehavior : MonoBehaviour
{
    public static PlayerBehavior Instance;

    [SerializeField] TextMeshProUGUI numTreatsText;

    public int NumTreats {
        get => _numTreats;
        set {
            _numTreats = value;
            numTreatsText.text = "Treats Collected: " + NumTreats.ToString();
        }
    }

    int _numTreats = 0;

    void Start()
    {
        if (Instance != null && Instance != this )
            Destroy(this);
        else
            Instance = this;
    }
}