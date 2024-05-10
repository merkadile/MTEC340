using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayBehavior : MonoBehaviour
{
    public bool InsideSphere = true;
    
    Coroutine _timerSystem = null;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject losebox;
    [SerializeField] GameObject winBox;

    [SerializeField] AudioClip lose;
    [SerializeField] AudioClip collectTreat;
    [SerializeField] AudioClip startTimer;

    [SerializeField] GameObject safeZone;

    [SerializeField] Material safeZoneRed;
    [SerializeField] Material safeZoneNormal;

    float _timeLeft = 10.0f;

    bool _lost = false;
    bool _won = false;
    bool _cantBeOutside = true;
    bool _timerActive = false;

    void Update()
    {
        if (_timerSystem == null)
            _timerSystem = StartCoroutine(TimerSystem());

        if (_cantBeOutside && !InsideSphere && !_lost) {
            _lost = true;
            losebox.SetActive(true);
            GetComponent<AudioSource>().PlayOneShot(lose);
        }

        if (_timerActive && !_won && !_lost) {
            _timeLeft = _timeLeft - Time.deltaTime;
            timerText.text = "GO! Time Left: " + _timeLeft + "s";
        }

        if (PlayerBehavior.Instance.NumTreats == 5 && InsideSphere && !_lost && !_won) {
            winBox.SetActive(true);
            GetComponent<AudioSource>().PlayOneShot(startTimer);
            _won = true;
        }
    }

    IEnumerator TimerSystem()
    {
        if (_won) {
            yield break;
        }

        _timerActive = false;
        timerText.text = "Location Monitored... Hold";
        safeZone.GetComponent<Renderer>().material = safeZoneRed;
        _cantBeOutside = true;

        yield return new WaitForSeconds(5.0f);

        if (_lost) {
            yield break;
        }

        _timerActive = true;
        safeZone.GetComponent<Renderer>().material = safeZoneNormal;
        _cantBeOutside = false;
        GetComponent<AudioSource>().PlayOneShot(startTimer);

        yield return new WaitForSeconds(10.0f);

        if (PlayerBehavior.Instance.NumTreats < 5 && !_lost) {
            _lost = true;
            losebox.SetActive(true);
            GetComponent<AudioSource>().PlayOneShot(lose);
        }

        _timerSystem = null;
    }
}