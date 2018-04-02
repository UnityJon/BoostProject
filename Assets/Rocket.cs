using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    AudioSource m_MyAudioSource;
  
    [SerializeField] float m_rcsThrust=150;
    [SerializeField] float m_mainThrust = 50;
    [SerializeField] float levelLoadDelay = 2;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip dyingExplosion;
    [SerializeField] AudioClip wonLevel;

    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem dyingParticles;
    [SerializeField] ParticleSystem wonParticles;

    [SerializeField] bool collisionsOn = true;

    bool playerTransitioning= false;

    Rigidbody m_rigidBody;

    // Use this for initialization
    void Start () {
        m_rigidBody = GetComponent<Rigidbody>();
        m_MyAudioSource = GetComponent<AudioSource>();
	}

    

    // Update is called once per frame 
    void Update()
    {
        if (!playerTransitioning)
        {
            RespondToThrust();
            RespondToRotate();
        }
        if (Debug.isDebugBuild)
        {
            CheckForNextLevel();
            CheckForCollisionToggle();
        }
    }

    private void CheckForNextLevel ()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
    }

    private void CheckForCollisionToggle()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsOn = !collisionsOn;
        }
    }

    private void RespondToThrust()
    {
        if (Input.GetKey(KeyCode.Space))
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
        m_MyAudioSource.Stop();
        engineParticles.Stop();
    }

    private void ApplyThrust()
    {
        m_rigidBody.AddRelativeForce(Vector3.up * m_mainThrust*Time.deltaTime);
        if (!m_MyAudioSource.isPlaying)
        {
            m_MyAudioSource.PlayOneShot(mainEngine);
        }
        engineParticles.Play();
    }

    private void RespondToRotate()
    {
        m_rigidBody.angularVelocity = Vector3.zero;  //Remove any existing rotation
        float rotateThisFrame = m_rcsThrust * Time.deltaTime;
        bool APressed = Input.GetKey(KeyCode.A);
        bool DPressed = Input.GetKey(KeyCode.D);
        if (APressed & !DPressed)
        {
            transform.Rotate(Vector3.forward*rotateThisFrame);
        }
        else if (DPressed & !APressed)
        {
            transform.Rotate(-Vector3.forward*rotateThisFrame);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (playerTransitioning) return;
                 switch (collision.gameObject.tag)
                {
                    case "Friendly":
                        //Do nothing
                        break;

            case "Finish":
                StartWonLevel();
                break;

            default:
                if (collisionsOn)
                {
                    StartDeathSequence();
                }
                break;
        }
    }

    private void StartDeathSequence()
    {
        playerTransitioning=true;
        Invoke("LoadFirstScene", levelLoadDelay);
        m_MyAudioSource.Stop();
        m_MyAudioSource.PlayOneShot(dyingExplosion);
        engineParticles.Stop();
        dyingParticles.Play();
    }

    private void StartWonLevel()
    {
        playerTransitioning = true;
        Invoke("LoadNextScene", levelLoadDelay);
        m_MyAudioSource.Stop();
        m_MyAudioSource.PlayOneShot(wonLevel);
        engineParticles.Stop();
        wonParticles.Play();
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
        playerTransitioning=false;
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex>= SceneManager.sceneCountInBuildSettings)
            {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
        playerTransitioning = false;
    }
}
