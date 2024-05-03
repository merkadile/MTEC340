using UnityEngine;

//this script recognizes when the head has collided with another gameobject,
//and triggers the necessary functionality required when a collision occurs.
public class HeadBehavior : MonoBehaviour
{
    [SerializeField] GameplayBehavior gameplayBehaviorInstance;

    void OnCollisionEnter2D(Collision2D other_)
    {
        if (other_.gameObject.tag == "Fruit")
            gameplayBehaviorInstance.CollideWithFruit();
        else
            gameplayBehaviorInstance.GameOver();
    }
}