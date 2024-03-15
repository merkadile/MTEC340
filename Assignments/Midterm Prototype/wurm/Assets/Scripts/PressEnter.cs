using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartGame : MonoBehaviour
{
    float _flickerTimer = 0.0f;
    [SerializeField] float _flickerRefreshRate = 1.0f;
    TextMeshProUGUI _text;

    void Start()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //Every second, toggle between text being visible and invisible
        if (_flickerTimer > _flickerRefreshRate) {
            _text.enabled = !_text.enabled;

            _flickerTimer = 0.0f;
        }
        _flickerTimer += Time.deltaTime;

        //Upon pressing the return key, load the gameplay scene
        if (Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene("Gameplay");
        }
    }
}