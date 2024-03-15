using UnityEngine;

public class WurmBodyEffectsBehavior : MonoBehaviour
{
    public GameObject Head;
    public GameObject LeftEye;
    public GameObject RightEye;

    float _flickerTimer = -1.0f;
    [SerializeField] float _flickerRefreshRate = 1.0f;

    void Update()
    {
        //After player game overs, every second, toggle between wurm being visible and invisible
        if (Head.GetComponent<HeadBehavior>().LostGame) {
            if (_flickerTimer > _flickerRefreshRate) {
                Head.GetComponent<Renderer>().enabled = !Head.GetComponent<Renderer>().enabled;
                LeftEye.GetComponent<Renderer>().enabled = !LeftEye.GetComponent<Renderer>().enabled;
                RightEye.GetComponent<Renderer>().enabled = !RightEye.GetComponent<Renderer>().enabled;

                foreach (GameObject _tail in gameObject.GetComponent<MovementBehavior>().Tails) {
                    _tail.GetComponent<Renderer>().enabled = !_tail.GetComponent<Renderer>().enabled;
                }

                _flickerTimer = 0.0f;
            }

            _flickerTimer += Time.deltaTime;
        }
    }
}