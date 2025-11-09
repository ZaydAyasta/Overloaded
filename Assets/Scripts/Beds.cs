using UnityEngine;

public class Beds : MonoBehaviour
{
    //Tiempo de cambio a la siguiente animacion
    [Header ("Animation")]
    Animator anim;
    [SerializeField]float _timeChangeAnimation = 0.01f;
    string _animationName = "FastTask";

    [Header ("Tender")]
    bool _isMade = false;
    [SerializeField] float _makeDuration;

    [Header("Área de interacción")]
    [Tooltip("Trigger 2D que detecta cuando el jugador está cerca de la cama.")]
    public Collider2D interactTrigger;

    [Header("Capa donde esta el jugador")]
    public LayerMask _playerMask;    

    [SerializeField] KeyCode _key = KeyCode.E;

    [Header(" Manager UI")]
    [SerializeField] BedsManager manager;

    //Cambio de Sprite a cama tendida
    [Header ("Sprite Cama")]
    [SerializeField] Sprite _sprMadeBed;

    SpriteRenderer _sr;
    
    PlayerGrab _player;

    
   

    private void Awake()
    {
        if (interactTrigger != null)
        {
            interactTrigger.isTrigger = true;
            interactTrigger.enabled = true;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (manager == null) manager = FindFirstObjectByType<BedsManager>();
        manager?.RegisterBed(this);

        _sr = GetComponent<SpriteRenderer>();

        if (anim == null) anim = GetComponent<Animator>();

        _player = FindFirstObjectByType<PlayerGrab>();
    }

    private void Update()
    {
        MakeBed();
    }

    void MakeBed()
    {
        bool _keyTask = Input.GetKey(_key);

        if (_isMade) return;
        if (_player.IsCarryingSomething) return;

        if (_keyTask && DetectPlayerInArea())
        {
            _isMade = true;
            //anim.CrossFade(_animationName, _timeChangeAnimation);
            manager?.OnBedMade(this);
            Debug.Log("Cama Hecha");
        }

    }

    bool DetectPlayerInArea()
    {
        if (interactTrigger == null) return false;

        var filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = _playerMask,
            useTriggers = true
        };

        Collider2D[] hits = new Collider2D[16];
        int count = interactTrigger.Overlap(filter, hits);

        for (int i = 0; i < count; i++)
        {
            if (hits[i] != null && hits[i].GetComponent<PlayerGrab>() != null)
                return true;
        }

        return false;
    }

    public void FinishBed() => _sr.sprite = _sprMadeBed;

}
