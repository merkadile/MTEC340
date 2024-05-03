using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

//this script gives certain behavior to text gameobjects it gets attached to.
public class FlickerTextPressEnter : MonoBehaviour
{
    Coroutine _flicker;

    void Update()
    {
        //if the FlickerText coroutine is not currently active, start it.
        if (_flicker == null)
            _flicker = StartCoroutine(FlickerText());

        //upon pressing the enter key, load the Gameplay scene.
        if (Input.GetKeyUp(StaticBehavior.Instance.EnterKey)) {
            SceneManager.LoadScene("Gameplay"); 
        }
    }
    
    //when started, the FlickerText coroutine waits until the FlickerInterval value specified in StaticBehavior has passed,
    //then it toggles the visibility of the text, before finally ending the coroutine.
    IEnumerator FlickerText()
    {
        yield return new WaitForSeconds(StaticBehavior.Instance.FlickerInterval);
        gameObject.GetComponent<TextMeshProUGUI>().enabled = !gameObject.GetComponent<TextMeshProUGUI>().enabled;

        _flicker = null; //at the very end of the coroutine, tell the program that the coroutine is no longer active
    }
}