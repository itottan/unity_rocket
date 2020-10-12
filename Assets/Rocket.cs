using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Unity scene 管理コード
 */
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
  /**
   * the diff of public and  [SerialzeField]
   * [SerializeField]  can't change from other scripts
   */
  [SerializeField] float rcsThrust = 100f;
  [SerializeField] float mainThrust = 100f;
  [SerializeField] float levelLoadDaly = 2f;

  [SerializeField] AudioClip mainEngine;
  [SerializeField] AudioClip SE_Death;
  [SerializeField] AudioClip SE_Success;

  [SerializeField] ParticleSystem mainEngineParticles;
  [SerializeField] ParticleSystem AE_DeathParticles;
  [SerializeField] ParticleSystem AE_SuccessParticles;

  Rigidbody rigidBody;
  AudioSource audioSource;

  bool isTransitioning = false;
  bool collisionsDisabled = false;

  private int sceneCountInBuildSettings;
  private int currentScene;

  //enum State { Alive, Dying, Transcending }
  /**
   * https://www.sejuku.net/blog/55897 可読性を維持するための定数を列挙して作成出来る
   */
  //State state = State.Alive;
  /**
   * Start is called before the first frame update
   */
  void Start()
  {
    rigidBody = GetComponent<Rigidbody>();  //<> -> whitch Component
    audioSource = GetComponent<AudioSource>();
    sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
    currentScene = SceneManager.GetActiveScene().buildIndex;
  }

  // Update is called once per frame
  void Update()
  {
    if (!isTransitioning)
    {
      RespondOfThrustInput();
      RespondToRotateInput();
    }

    //only if debug on
    if (Debug.isDebugBuild)
    {
      RespondToDebugKeys();
    }
  }

  private void RespondToDebugKeys()
  {
    if (Input.GetKeyDown(KeyCode.L))
    {
      LoadNextScene();
      print("L:NextScene");
    }
    else if (Input.GetKeyDown(KeyCode.C))
    {
      collisionsDisabled = !collisionsDisabled; //toggle
      print("collisions" + collisionsDisabled);
    }
  }

  void OnCollisionEnter(Collision collision)
  /**
   * Collision モジュール　シーン で パーティクル が ゲームオブジェクト とどのように衝突するかを制御  
   * OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
  */
  {
    if (isTransitioning || collisionsDisabled) { return; } //ignore collisions when dead

    switch (collision.gameObject.tag)
    {
      case "Friendly":
        print("オーケー");
        break;
      case "Fuel":
        print("給油");
        break;
      case "Bad":
        print("ゲームオーバー");
        StartDeathSequence();
        break;
      case "Goal":
        print("クリア");
        StartSuccessSequence();
        break;
    }
  }

  private void StartSuccessSequence()
  {
    isTransitioning = true;
    audioSource.Stop();
    audioSource.PlayOneShot(SE_Success);
    AE_SuccessParticles.Play();
    Invoke("LoadNextScene", levelLoadDaly); //parameterise time
  }

  private void StartDeathSequence()
  {
    isTransitioning = true;
    audioSource.Stop();
    audioSource.PlayOneShot(SE_Death);
    AE_DeathParticles.Play();
    Invoke("ReloadScene", levelLoadDaly); //parameterise time
  }

  private void LoadNextScene()
  {
    int nextScene = currentScene + 1;
    if (currentScene == sceneCountInBuildSettings - 1)
    {
      nextScene = 0;
    }
    SceneManager.LoadScene(nextScene);

  }
  private void ReloadScene()
  {
    SceneManager.LoadScene(currentScene);
  }

  private void RespondToRotateInput()
  {
    if (Input.GetKey(KeyCode.A))
    {
      RotateManually(rcsThrust * Time.deltaTime);
    }
    else if (Input.GetKey(KeyCode.D))
    {
      RotateManually(-rcsThrust * Time.deltaTime);
    }
  }

  private void RotateManually(float rotationThisFrame)
  {
    rigidBody.freezeRotation = true; // take manual control of rotation
    transform.Rotate(Vector3.forward * rotationThisFrame);
    // transform  オブジェクトの位置と回転とサイズを保持しているクラス、Rotate() ->回転、Vector3.forward -> Z軸操作、 Time.deltaTime :The completion time in seconds since the last frame (Read Only).
    rigidBody.freezeRotation = false; // resume physics control of rotation
  }

  private void RespondOfThrustInput()
  {
    if (Input.GetKey(KeyCode.Space))
    //can thrust while rotating
    {
      ApplyThrust();
    }
    else
    {
      StopApplyingThrust();
    }
  }

  private void StopApplyingThrust()
  {
    audioSource.Stop();
    mainEngineParticles.Stop();
  }

  private void ApplyThrust()
  {
    rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime *100);
    //ローカル座標に対して Rigidbody に相対的な力を加えます。
    if (!audioSource.isPlaying)
    {
      audioSource.PlayOneShot(mainEngine);
    }
    mainEngineParticles.Play();
  }
}


