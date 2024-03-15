using TMPro;
using UnityEngine;

public class PressWASD : MonoBehaviour
{
    public GameObject Fruit;

    TextMeshProUGUI _text;

    Vector3 _fruitInstantiatePos;
    
    float _fruitXPos;
    float _fruitYPos;

    void Start()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //Once player presses one of the directional keys, remove the text and spawn the first fruit
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A)) && _text.enabled) {
            _text.enabled = false;

            //Spawn the first fruit
            _fruitXPos = Random.Range(-16, 16);
            _fruitYPos = Random.Range(-18, 14);
            _fruitInstantiatePos = new Vector3 (_fruitXPos * 0.25f + 0.125f, _fruitYPos * 0.25f + 0.125f, 0.0f);
            Instantiate(Fruit, _fruitInstantiatePos, Quaternion.identity);
        }
    }
}