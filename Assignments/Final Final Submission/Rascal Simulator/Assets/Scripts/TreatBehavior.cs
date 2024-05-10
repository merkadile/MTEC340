using UnityEngine;

public class TreatBehavior : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"){
            PlayerBehavior.Instance.PlayCollectTreat();
            PlayerBehavior.Instance.NumTreats++;
            Destroy(gameObject);
        }
    }
}