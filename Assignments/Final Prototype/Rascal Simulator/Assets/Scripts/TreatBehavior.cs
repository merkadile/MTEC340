using UnityEngine;

public class TreatBehavior : MonoBehaviour
{
    void Update()
    {
        //TODO: animations + distance from ground
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            PlayerBehavior.Instance.NumTreats++;
            Destroy(gameObject);
            //play sfx
        }
    }
}