using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class HeadBehavior : MonoBehaviour
{
    public GameObject Tail;
    public GameObject Fruit;
    public GameObject Wurm;
    GameObject _newTail;
    GameObject _lastTailInList;

    [SerializeField] Player _player;

    Vector3 _tailInstantiatePos;
    Vector3 _fruitInstantiatePos;

    float _fruitXPos;
    float _fruitYPos;

    public bool LostGame = false;

    void Start()
    {
        _lastTailInList = Wurm.GetComponent<MovementBehavior>().Tails.Last();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //If the collision is with a fruit, spawn a new one. If not, game over.
        if (other.gameObject.tag == "Fruit") {
            SpawnFruit(other.gameObject);
        }
        else {
            LostGame = true;
            Destroy(GameObject.FindWithTag("Fruit"));
            SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
        }
    }

    public void SpawnFruit(GameObject _fruit) {
        //Eat the fruit, add 1 to the player's score
        Destroy(_fruit);
        _player.Score++;

        //Spawn a new fruit
        _fruitXPos = Random.Range(-16, 16);
        _fruitYPos = Random.Range(-18, 14);
        _fruitInstantiatePos = new Vector3 (_fruitXPos * 0.25f + 0.125f, _fruitYPos * 0.25f + 0.125f, 0.0f);

        if (!LostGame)
            Instantiate(Fruit, _fruitInstantiatePos, Quaternion.identity);

        //Add another tail
        _tailInstantiatePos = _lastTailInList.transform.position;
        _newTail = Instantiate(Tail, _tailInstantiatePos, Quaternion.identity);
        Wurm.GetComponent<MovementBehavior>().Tails.Add(_newTail);
        _lastTailInList = Wurm.GetComponent<MovementBehavior>().Tails.Last();
    }
}