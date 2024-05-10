using UnityEngine;

public class SafeZoneBehavior : MonoBehaviour
{
    [SerializeField] GameplayBehavior gameplayBehaviorInstance;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            gameplayBehaviorInstance.InsideSphere = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            gameplayBehaviorInstance.InsideSphere = false;
    }
}
