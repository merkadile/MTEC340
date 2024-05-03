using UnityEngine;
using System.Collections.Generic;

//this script is responsible for updating the wurm's position during gameplay.
public class MovementBehavior : MonoBehaviour
{
    [SerializeField] GameplayBehavior gameplayBehaviorInstance;
    [SerializeField] PlayerBehavior playerBehaviorInstance;

    public List<GameObject> Tails = new List<GameObject>();
    
    [SerializeField] GameObject head;

    Vector3 _previousBodyPos;
    Vector3 _currentBodyPos;
    
    [SerializeField] float startingMoveSpeed = 10.0f;
    [SerializeField] float speedUpIncrement = 0.2f;

    float _currentMoveSpeed;
    float _movementUpdateTimer = 0.0f;
    float _movementDistance = 0.25f;
    float _xDir = 0.0f;
    float _yDir = 0.0f;

    bool _isMoving = false;
    bool _changedDirNotYetMove = false;

    void Update()
    {
        //only run the functionality in Update() if _GameState is set to Play.
        if (gameplayBehaviorInstance._GameState == GameplayBehavior.GameStates.Play) {
            //update the wurm's movement speed. the speed increases by a small amount each time the player scores,
            //and the speed is also modified to be faster or slower while the player is in a powerup state.
            _currentMoveSpeed =
                (
                    startingMoveSpeed +
                        (
                            speedUpIncrement * playerBehaviorInstance.Score
                        )
                ) *
                gameplayBehaviorInstance._PowerupMovementChange
            ;

            //run the ChangeDirection function when one of the movement keys are pressed, except:
                //the worm cannot immediately start moving in the complete opposite direction,
                //the worm can only change its direction once before its position begins being updated,
                //the worm cannot move right as its first move.
            if (
                Input.GetKeyDown(StaticBehavior.Instance.UpKey) &&
                !(
                    _xDir == 0.0f && _yDir == -1.0f
                ) &&
                !_changedDirNotYetMove
            ) ChangeDirection(0.0f, 1.0f, 270.0f);
            if (
                Input.GetKeyDown(StaticBehavior.Instance.DownKey) &&
                !(
                    _xDir == 0.0f && _yDir == 1.0f
                ) &&
                !_changedDirNotYetMove
            ) ChangeDirection(0.0f, -1.0f, 90.0f);
            if (
                Input.GetKeyDown(StaticBehavior.Instance.LeftKey) &&
                !(
                    _xDir == 1.0f && _yDir == 0.0f
                ) &&
                !_changedDirNotYetMove
            ) ChangeDirection(-1.0f, 0.0f, 0.0f);
            if (
                Input.GetKeyDown(StaticBehavior.Instance.RightKey) &&
                !(
                    _xDir == -1.0f && _yDir == 0.0f
                ) &&
                !_changedDirNotYetMove &&
                _isMoving
            ) ChangeDirection(1.0f, 0.0f, 180.0f);
            
            //if the wurm has started moving already, use a timer to
            //update the position of the wurm at the rate of the wurm's current speed.
            if (_isMoving) {
                if (_movementUpdateTimer > (1.0f / _currentMoveSpeed)) {
                    //move wurm head.
                    _previousBodyPos = head.transform.position;
                    head.transform.position += new Vector3(_xDir, _yDir, 0.0f) * _movementDistance;

                    //move each wurm tail. the new position of each tail should be
                    //the previous position of the previous body part.
                    foreach (GameObject tail_ in Tails) {
                        _currentBodyPos = tail_.transform.position;
                        tail_.transform.position = _previousBodyPos;
                        _previousBodyPos = _currentBodyPos;
                    }
                    
                    //_changedDirNotYetMove gets set to false when the wurm begins moving in the new direction.
                    _changedDirNotYetMove = false;

                    _movementUpdateTimer = 0.0f; //resetting the timer
                }
                _movementUpdateTimer += Time.deltaTime;
            }
        }
    }

    //when called, the ChangeDirection function updates the direction the wurm starts moving in
    //and the rotation of the wurm's head.
    void ChangeDirection(float xValue_, float yValue_, float rotation_)
    {
        _xDir = xValue_;
        _yDir = yValue_;
        head.transform.eulerAngles = new Vector3 (0.0f, 0.0f, rotation_);

        _isMoving = true; //_isMoving gets set to true when the wurm changes direction for the first time
        _changedDirNotYetMove = true; //_changedDirNotYetMove gets set to true whenever the wurm changes direction
    }
}