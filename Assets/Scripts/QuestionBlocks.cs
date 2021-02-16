using System.Collections;
using UnityEngine;

public class QuestionBlocks : MonoBehaviour
{

    public float bounceHeight = 0.5f;
    public float bounceSpeed = 4f;

    private Vector2 originalPosition;

    private bool canBounce = true;

    void Start()
    {
        originalPosition = transform.localPosition;

    }

    
    void Update()
    {
        
    }

    public void QuestionBlockBounce()
    {
        if (canBounce)
        {
            canBounce = false;
            StartCoroutine(Bounce());
        }
    }

    IEnumerator Bounce()
    {

    }
}
