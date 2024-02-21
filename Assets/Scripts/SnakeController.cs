using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float bodySpeed;
    [SerializeField] private float steerSpeed;
    [SerializeField] private GameObject bodyPrefab;
    public GameManager gameManager;

    private int gap = 10;
    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();
    public StickController MoveStick;
    public AudioSource audioSource;
    public AudioClip foodSound;
    void Start()
    {

        //positionHistory.Insert(0, transform.position);
        InvokeRepeating("UpdatePositionHistory", 0f, 0.01f);
        bigSnake();
       bigSnake();

    }
    void Awake()
    {
        if (MoveStick != null)
        {
            MoveStick.StickChanged += MoveStick_StickChanged;
        }
    }

    private Vector2 MoveStickPos = Vector2.zero;

    private void MoveStick_StickChanged(object sender, StickEventArgs e)
    {
        MoveStickPos = e.Position;
    }
    void Update()
    {
        float h = Mathf.Abs(MoveStickPos.x) > Mathf.Abs(Input.GetAxis("Horizontal")) ? MoveStickPos.x : Input.GetAxis("Horizontal");
        float v = Mathf.Abs(MoveStickPos.y) > Mathf.Abs(Input.GetAxis("Vertical")) ? MoveStickPos.y : Input.GetAxis("Vertical");
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up * h * steerSpeed * Time.deltaTime);
        UpdateBodyParts();

    }

    private void UpdateBodyParts()
    {
        int index = 0;
        foreach (GameObject body in bodyParts)
        {
            Vector3 point = positionHistory[Math.Min(index * gap, positionHistory.Count - 1)];
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * bodySpeed * Time.deltaTime;

            body.transform.LookAt(point);

            index++;
        }
    }


    void UpdatePositionHistory()
    {
        Debug.Log("UpdatePositionHistory");
        // Añadir la posición actual al inicio de la lista
        positionHistory.Insert(0, transform.position);

        // Si la lista excede el número máximo de posiciones, elimina la última
        if (positionHistory.Count > 500)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }
    private void bigSnake()
    {
        GameObject body = Instantiate(bodyPrefab);
        bodyParts.Add(body);
        audioSource.PlayOneShot(foodSound);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Body"))
        {
            gameManager.EndGame();
        }
        if (other.CompareTag("food"))
        {
            bigSnake();
            Destroy(other.gameObject);
            gameManager.AddScore(1);
            gameManager.GenerateFood();
        }
    }

}
