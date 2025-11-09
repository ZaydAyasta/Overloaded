using Unity.VisualScripting;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    [SerializeField] KeyCode _key = KeyCode.G;
    [SerializeField] Transform _transform; // destino

    Transform _player;
    bool _indoor = false;
    Animator anim;

    PlayerGrab _play;

    private void Start()
    {
        anim = GetComponent<Animator>();
        _player = FindAnyObjectByType<PlayerGrab>().transform;
        _play = FindAnyObjectByType<PlayerGrab>();
    }

    private void Update()
    {
        if (_indoor && Input.GetKeyDown(_key))
        {
            _indoor = false;        
            anim.CrossFade("puerta", 0.01f);   
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _indoor = true;
            _play._isInDoor = true;
            anim.CrossFade("LockDoor", 0.01f);
            Debug.Log("in");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _indoor = false;
            _play._isInDoor = false;
            Debug.Log("out");
        }
    }

    public bool GetContact() { return _indoor; }
    public void EnterDoor() { _player.position = _transform.position; }
}
