using System.Collections.Generic;
using UnityEngine;

public class MovementBehavior : MonoBehaviour
{
    public GameObject Head;
    public List<GameObject> Tails = new List<GameObject>();

    [SerializeField] KeyCode _upKey = KeyCode.W;
    [SerializeField] KeyCode _downKey = KeyCode.S;
    [SerializeField] KeyCode _leftKey = KeyCode.A;
    [SerializeField] KeyCode _rightKey = KeyCode.D;
    
    [SerializeField] float _movementUpdateRefreshRate = 0.1f;
    float _movementUpdateTimer = 0.0f;
    float _movementDistance = 0.25f;
    float _xDir = 0.0f;
    float _yDir = 0.0f;

    bool _isMoving = false;
    bool _changedDirNotYetMove = false;

    Vector3 _previousBodyPos;
    Vector3 _currentBodyPos;

    void Update()
    {
        if (!Head.GetComponent<HeadBehavior>().LostGame) {
            //Change the direction the worm is travelling in by pressing WASD (except the worm cannot immediately start moving in the complete opposite direction)
            if (Input.GetKeyDown(_upKey) && !(_xDir == 0.0f && _yDir == -1.0f) && !_changedDirNotYetMove) {
                _xDir = 0.0f;
                _yDir = 1.0f;
                Head.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 270.0f);
                _isMoving = true;
                _changedDirNotYetMove = true;
            }
            if (Input.GetKeyDown(_downKey) && !(_xDir == 0.0f && _yDir == 1.0f) && !_changedDirNotYetMove) {
                _xDir = 0.0f;
                _yDir = -1.0f;
                Head.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 90.0f);
                _isMoving = true;
                _changedDirNotYetMove = true;
            }
            if (Input.GetKeyDown(_leftKey) && !(_xDir == 1.0f && _yDir == 0.0f) && !_changedDirNotYetMove) {
                _xDir = -1.0f;
                _yDir = 0.0f;
                Head.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
                _isMoving = true;
                _changedDirNotYetMove = true;
            }
            if (Input.GetKeyDown(_rightKey) && !(_xDir == -1.0f && _yDir == 0.0f) && !_changedDirNotYetMove && _isMoving) {
                _xDir = 1.0f;
                _yDir = 0.0f;
                Head.transform.eulerAngles = new Vector3 (0.0f, 0.0f, 180.0f);
                _isMoving = true;
                _changedDirNotYetMove = true;
            }
            
            //Every 0.1 seconds, update the position of the worm
            if (_movementUpdateTimer > _movementUpdateRefreshRate) {
                //Move worm head
                _previousBodyPos = Head.transform.position;
                Head.transform.position += new Vector3(_xDir, _yDir, 0.0f) * _movementDistance;

                //Move each worm tail
                foreach (GameObject tail in Tails) {
                    if (_isMoving) {
                        _currentBodyPos = tail.transform.position;
                        tail.transform.position = _previousBodyPos;
                        _previousBodyPos = _currentBodyPos;
                    }
                }
                
                _changedDirNotYetMove = false;
                _movementUpdateTimer = 0.0f;
            }
            _movementUpdateTimer += Time.deltaTime;
        }
    }
}