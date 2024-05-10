using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Linq;
using UnityEngine.Audio;

//the script is responsible for handling the rest of the game's functionality during gameplay besides wurm movement.
public class GameplayBehavior : MonoBehaviour
{
    [SerializeField] MovementBehavior movementBehaviorInstance;
    [SerializeField] PlayerBehavior playerBehaviorInstance;

    public enum GameStates {
        Play,
        Pause,
        GameOver
    }

    public enum Powerups {
        None,
        Blue,
        Red
    }

    [HideInInspector] public GameStates _GameState = GameStates.Play;

    [HideInInspector] public Powerups _Powerup = Powerups.None;

    Coroutine _gameOverWurmFlicker;
    Coroutine _powerUpCollect;

    [SerializeField] GameObject fruitPrefab;
    [SerializeField] GameObject tailPrefab;
    [SerializeField] GameObject sfxSource;
    [SerializeField] GameObject mxSource;
    [SerializeField] GameObject head;
    [SerializeField] GameObject leftEye;
    [SerializeField] GameObject rightEye;
    [SerializeField] GameObject pauseSymbol;

    [SerializeField] TextMeshProUGUI movementText;

    [SerializeField] AudioMixer musicMixer;

    [SerializeField] AudioClip eatFruitSFX;
    [SerializeField] AudioClip startPauseOrResetGameSFX;
    [SerializeField] AudioClip gameOverSFX;
    [SerializeField] AudioClip powerUpSFX;

    GameObject _newTail;
    GameObject _newFruit;
    GameObject _lastTailInList;

    Vector3 _tailInstantiatePos;
    Vector3 _fruitInstantiatePos;
    Vector3 _headFacingDir;
    Vector3 _overlapCheckSize = new Vector3(0.2f, 0.2f, 0.0f);

    [SerializeField] int powerupAwayFromWallAmount = 3;

    [SerializeField] Color normalWurmColor;
    [SerializeField] Color normalFruitColor;
    [SerializeField] Color blueColor;
    [SerializeField] Color redColor;

    Color _tempColor;

    [SerializeField] int scoreThresholdBeforePowerups = 10;

    int _fruitXPos;
    int _fruitYPos;

    [HideInInspector] public float _PowerupMovementChange = 1.0f;

    [SerializeField] float powerupFrequency = 1.0f;
    [SerializeField] float bluePowerUpLength = 7.5f;
    [SerializeField] float redPowerUpLength = 5.0f;
    [SerializeField] float bluePowerUpSpeedChange = 0.667f;
    [SerializeField] float redPowerUpSpeedChange = 1.5f;

    float _randomizePowerupSpawn;
    float _headFacingX;
    float _headFacingY;
    float _currentLowPassCutoffFreq;
    float _lowPassEnabledValue = 750.0f;

    int _numPowerUpFlickers = 10;

    bool _wurmInFlickerNormalColor = false;

    void Start()
    {
        //upon starting or resetting the game, play the startPauseOrResetGameSFX.
        sfxSource.GetComponent<AudioSource>().PlayOneShot(startPauseOrResetGameSFX, 1.0f);
    }

    void Update()
    {
        //once the player first presses one of the directional keys (other than right),
        //start the music loop, remove the text, and call the SpawnFruit function for the first time,
        //passing in the usual arguments for spawning a powerup-less fruit.
        if (
            (
                Input.GetKeyDown(StaticBehavior.Instance.UpKey) ||
                Input.GetKeyDown(StaticBehavior.Instance.DownKey) ||
                Input.GetKeyDown(StaticBehavior.Instance.LeftKey)
            ) &&
            movementText.enabled
        ) {
            mxSource.GetComponent<AudioSource>().Play();
            movementText.enabled = false;
            SpawnFruit(0, normalFruitColor);
        }

        //upon pressing the quit key, the game quits.
        if (Input.GetKeyDown(StaticBehavior.Instance.QuitKey))
            Application.Quit();

        //after player game overs, if the GameOverWurmFlicker coroutine is not currently active, start it,
        //passing in the FlickerInterval value specified in StaticBehavior as the argument for the interval_ parameter.
        if (_gameOverWurmFlicker == null && _GameState == GameStates.GameOver)
            _gameOverWurmFlicker = StartCoroutine(GameOverWurmFlicker(StaticBehavior.Instance.FlickerInterval));
    }

    void LateUpdate()
    {
        //upon pressing the pause key during gameplay,
        //play the startPauseOrResetGameSFX sound effect, then call the ActivatePause function,
        //passing in true if _GameState is set to Play and false if _GameState is set to Pause.
        if (
            Input.GetKeyDown(StaticBehavior.Instance.PauseKey) &&
            _GameState != GameStates.GameOver &&
            !movementText.enabled
        ) {
            sfxSource.GetComponent<AudioSource>().PlayOneShot(startPauseOrResetGameSFX, 1.0f);
            ActivatePause(_GameState == GameStates.Play);
        }
    }

    public void ActivatePause(bool condition_)
    {
        //apply a low pass filter to the music when the game is paused.
        _currentLowPassCutoffFreq = (condition_) ? _lowPassEnabledValue : 22000.0f;
        musicMixer.SetFloat("Cutoff Frequency", _currentLowPassCutoffFreq);

        //if condition_ is true, set _GameState to Pause. if false, set it to Play.
        _GameState = condition_ ? GameStates.Pause : GameStates.Play;

        pauseSymbol.SetActive(condition_); //the pause symbol is activated if condition_ is true and deactivated if false

        //if condition_ is true, hide the fruit and the wurm. if false, show them.
        _newFruit.GetComponent<Renderer>().enabled = !condition_;
        head.GetComponent<Renderer>().enabled = !condition_;
        leftEye.GetComponent<Renderer>().enabled = !condition_;
        rightEye.GetComponent<Renderer>().enabled = !condition_;
        foreach (GameObject tail_ in movementBehaviorInstance.Tails)
            tail_.GetComponent<Renderer>().enabled = !condition_;
        
        //if condition_ is true, pause all game functionality. if false, resume game functionality.
        Time.timeScale = condition_ ? 0 : 1;
    }

    //gets called when collision is detected with an object with the "Fruit" tag.
    public void CollideWithFruit()
    {
        //if _Powerup is set to Blue and the PowerUpCollect coroutine is not currently active, start it.
        //otherwise, if _Powerup is set to Red and the PowerUpCollect coroutine is not currently active, start it.
        if (_Powerup == Powerups.Blue && _powerUpCollect == null)
            _powerUpCollect = StartCoroutine(PowerUpCollect(bluePowerUpSpeedChange, bluePowerUpLength, blueColor));
        else if (_Powerup == Powerups.Red && _powerUpCollect == null)
            _powerUpCollect = StartCoroutine(PowerUpCollect(redPowerUpSpeedChange, redPowerUpLength, redColor));

        Destroy(_newFruit); //destroy the fruit

        _randomizePowerupSpawn = Random.value; //set _randomizePowerupSpawn to a random value between 0 and 1

        //check if _randomizePowerupSpawn is a value that would trigger the spawn of a powerup,
        //and that there is no powerup currently in play nor is the wurm in a powerup state,
        //and that the score is at least what is required to spawn a powerup.
        //if the conditions are met, set _Powerup to either blue or red, and call the SpawnFruit function,
        //passing in arguments of the distance the fruit should spawn away from the wall
        //and the corresponding color of the powerup.
        if (
            _randomizePowerupSpawn < (powerupFrequency * 0.5) &&
            playerBehaviorInstance.Score >= (scoreThresholdBeforePowerups - 1) &&
            _Powerup == Powerups.None
        ) {
            _Powerup = Powerups.Blue;
            SpawnFruit(powerupAwayFromWallAmount, blueColor);
        }
        else if (
            _randomizePowerupSpawn < powerupFrequency &&
            playerBehaviorInstance.Score >= (scoreThresholdBeforePowerups - 1) &&
            _Powerup == Powerups.None
        ) {
            _Powerup = Powerups.Red;
            SpawnFruit(powerupAwayFromWallAmount, redColor);
        }

        //otherwise, call the SpawnFruit function and pass in the usual arguments for spawning a powerup-less fruit.
        else
            SpawnFruit(0, normalFruitColor);

        //if the wurm is currently in a powerup state, call the SpawnTail function, passing in the corresponding powerup color.
        if (_Powerup == Powerups.Blue && _powerUpCollect != null)
            SpawnTail(blueColor);
        else if (_Powerup == Powerups.Red && _powerUpCollect != null)
            SpawnTail(redColor);

        //otherwise, call the SpawnTail function, passing in the normal wurm color.
        else
            SpawnTail(normalWurmColor);

        sfxSource.GetComponent<AudioSource>().PlayOneShot(eatFruitSFX, 1.0f); //play the eatFruitSFX sound effect

        playerBehaviorInstance.Score++; //increase the player's score by 1
    }

    //gets called when collision is detected with an object that doesn't have the "Fruit" tag.
    public void GameOver()
    {
        //only run after the first collision with a non-fruit.
        if (_GameState != GameStates.GameOver) {
            //call the activate pause function, passing in false
            //(making sure the wurm is visible and the pause symbol is invisible before doing anything else.)
            ActivatePause(false);

            //if the wurm is in a powerup state, call the ChangeWurmColor function, passing in the color of the powerup.
            if (_Powerup == Powerups.Blue && _powerUpCollect != null)
                ChangeWurmColor(blueColor);
            else if (_Powerup == Powerups.Red && _powerUpCollect != null)
                ChangeWurmColor(redColor);

            //start the GameOverWurmFlicker coroutine for the first time, passing in an interval that is twice what is normal
            //(to get the wurm's flicker out of sync from the text, so that they alternate visibility.)
            _gameOverWurmFlicker = StartCoroutine(GameOverWurmFlicker(2 * StaticBehavior.Instance.FlickerInterval));

            mxSource.GetComponent<AudioSource>().Stop(); //stop the music loop

            sfxSource.GetComponent<AudioSource>().PlayOneShot(gameOverSFX, 1.75f); //play the gameOverSFX sound effect (loud)

            _GameState = GameStates.GameOver; //set _GameState to GameOver

            SceneManager.LoadScene("GameOver", LoadSceneMode.Additive); //load the GameOver scene

            Destroy(_newFruit); //destroy the fruit
        }
    }

    void SpawnFruit(int distanceFromWall_, Color color_)
    {
        //get the x and y components of the vector representing the direction that the head is facing.
        _headFacingX =
            (head.transform.eulerAngles.z % 180.0f == 0.0f) ?
            (head.transform.eulerAngles.z / 90 - 1) : 0;
        _headFacingY =
            !(head.transform.eulerAngles.z % 180.0f == 0.0f) ?
            (head.transform.eulerAngles.z / 90 - 2) : 0;
        
        _headFacingDir = new Vector3(_headFacingX, _headFacingY, 0.0f); //determine the direction the head is facing

        //choose a random position in x,y space to place the fruit,
        //taking in account the distance the fruit must spawn away from the wall if it is a powerup.
        //if that position overlaps with something, or it is directly in front of the snake's head, pick again.
        do {
            _fruitXPos = Random.Range(-16 + distanceFromWall_, 16 - distanceFromWall_);
            _fruitYPos = Random.Range(-18 + distanceFromWall_, 14 - distanceFromWall_);
            _fruitInstantiatePos = new Vector3 (_fruitXPos * 0.25f + 0.125f, _fruitYPos * 0.25f + 0.125f, 0.0f);
        } while (
            Physics2D.OverlapBox((Vector2)_fruitInstantiatePos, (Vector2)_overlapCheckSize, 0.0f) || 
            (_fruitInstantiatePos - head.transform.position).normalized == _headFacingDir
        );

        //instantiate the new fruit in the chosen position.
        _newFruit = Instantiate(fruitPrefab, _fruitInstantiatePos, Quaternion.identity);

        //set the color of the fruit, making the blue and red powerups visually distinct from the normal fruit.
        _newFruit.GetComponent<Renderer>().material.color = color_;

        //if the game is currently paused, make sure the fruit is invisible.
        if (_GameState == GameStates.Pause)
            _newFruit.GetComponent<Renderer>().enabled = false;
    }

    void SpawnTail(Color color_)
    {
        //instantiate a new tail at the position of the last tail on the wurm,
        //then add the new tail to the list of tails, making it the last tail on the wurm.
        _lastTailInList = movementBehaviorInstance.Tails.Last();
        _tailInstantiatePos = _lastTailInList.transform.position;
        _newTail = Instantiate(tailPrefab, _tailInstantiatePos, Quaternion.identity);
        movementBehaviorInstance.Tails.Add(_newTail);

        //set the color of the tail based on the color_ paramter,
        //unless the wurm is in a powerup state but not currently colored the powerup color.
        if (!_wurmInFlickerNormalColor)
            _newTail.GetComponent<Renderer>().material.color = color_;

        //if the game is currently paused, make sure the new tail is invisible.
        if (_GameState == GameStates.Pause)
            _newTail.GetComponent<Renderer>().enabled = false;
    }

    void ChangeWurmColor(Color color_) {
        //set the wurm's color to the paramter color_.
        head.GetComponent<Renderer>().material.color = color_;
        foreach (GameObject tail_ in movementBehaviorInstance.Tails)
            tail_.GetComponent<Renderer>().material.color = color_;
    }

    //when started, the GameOverWurmFlicker coroutine waits until the specified interval_ value has passed,
    //then it toggles the visibility of the wurm, before finally ending the coroutine.
    IEnumerator GameOverWurmFlicker(float interval_)
    {
        yield return new WaitForSeconds(interval_);
        head.GetComponent<Renderer>().enabled = !head.GetComponent<Renderer>().enabled;
        leftEye.GetComponent<Renderer>().enabled = !leftEye.GetComponent<Renderer>().enabled;
        rightEye.GetComponent<Renderer>().enabled = !rightEye.GetComponent<Renderer>().enabled;
        foreach (GameObject tail_ in movementBehaviorInstance.Tails) {
            tail_.GetComponent<Renderer>().enabled = !tail_.GetComponent<Renderer>().enabled;
        }

        _gameOverWurmFlicker = null; //at the very end of the coroutine, tell the program that the coroutine is no longer active
    }

    //when started, the PowerUpCollect coroutine puts the wurm in a powerup state, described below,
    //then ends after returning to normal.
    IEnumerator PowerUpCollect(float movementChange_, float powerupLength_, Color color_)
    {
        sfxSource.GetComponent<AudioSource>().PlayOneShot(powerUpSFX, 1.0f); //play the powerUpSFX sound effect

        //changes the speed of the wurm depending on the value of the movementChange_ paramter.
        _PowerupMovementChange = movementChange_;

        //calls the ChangeWurmColor function, passing in color_.
        ChangeWurmColor(color_);

        //nothing happens until the last 25% of the length of time the powerup is supposed to last.
        yield return new WaitForSeconds(powerupLength_ * 0.75f);

        //for the last 25% of the powerup length,
        //flicker the wurm between its normal color and the powerup color for a total of _numPowerUpFlickers times.
        for (int numFlickers_ = 0; numFlickers_ < _numPowerUpFlickers; numFlickers_++) {
            yield return new WaitForSeconds(powerupLength_ * 0.25f / _numPowerUpFlickers); //dividing the last 25% of the time into smaller units.

            //if the player game overs, exit the coroutine immediately.
            if (_GameState == GameStates.GameOver)
                yield break;
            
            //_tempColor is equal to normalWurmColor on even number iterations and equal to color_ on odd.
            _tempColor = (numFlickers_ % 2 == 0) ? normalWurmColor : color_;

            //call the ChangeWurmColor function, passing in _tempColor.
            ChangeWurmColor(_tempColor);

            //let the SpawnTail function know what color the wurm currently is.
            _wurmInFlickerNormalColor =
                normalWurmColor == _tempColor
            ;
        }
        //after the powerup ends, call the ChangeWurmColor function, passing in normalWurmColor.
        ChangeWurmColor(normalWurmColor);

        _PowerupMovementChange = 1.0f; //reset the movement speed to normal

        _Powerup = Powerups.None; //reset _Powerup to None

        _powerUpCollect = null; //at the very end of the coroutine, tell the program that the coroutine is no longer active
    }
}