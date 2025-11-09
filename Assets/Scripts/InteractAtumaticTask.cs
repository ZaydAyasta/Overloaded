using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractAtumaticTask : MonoBehaviour
{
    public KeyCode key = KeyCode.E;
    public float holdTime = 2f;
    public GameObject progressBarPrefab;

    private float timer = 0f;
    private bool isInteracting = false;
    private ProgressBarTask bar;
    private PlayerGrab player;
    private bool _finishTask = false;
    private void Awake()
    {
        StartComponents();
    }
    void Start()
    {
        
        player = FindFirstObjectByType<PlayerGrab>();
    }

    void Update()
    {
        if (_finishTask) return;
        if (!PlayerNear()) return;
        if (player.IsCarryingSomething) return; 

        bool keyHeld = Input.GetKey(key);

        if (keyHeld)
        {
            if (!isInteracting)
            {
                isInteracting = true;
                timer = 0f;
                CreateBar();
            }         
        }


        if(isInteracting)
        {
            
            timer += Time.deltaTime;

            float progress = timer / holdTime;
            bar.SetProgress(progress);

            if (timer >= holdTime)
            {
                OnCompleted();
            }
        }
    }

    void CreateBar()
    {
        if (progressBarPrefab == null) return;

        GameObject b = Instantiate(progressBarPrefab);
        b.transform.SetParent(transform);
        bar = b.GetComponent<ProgressBarTask>();
        bar.SetTarget(transform);
        bar.SetProgress(0f);
    }

    public void StopInteraction()
    {
        isInteracting = false;
        timer = 0f;

        bar.gameObject.SetActive(false);
    }

    public virtual void OnCompleted()
    {
        StopInteraction();
        _finishTask = true;

    }
    public virtual void StartComponents()
    {

    }

    bool PlayerNear()
    {
        if (player == null) return false;

        float dist = Vector3.Distance(player.transform.position, transform.position);
        return dist < 2f;
    }
}
