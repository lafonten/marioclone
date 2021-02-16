using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform leftBounds;
    public Transform rightBounds;

    public float smoothDampTime = 0.5f;
    private Vector3 smoothDampVelocity = Vector3.zero;

    private float cameraWidth, cameraHeight, levelMinX, levelMaxX;


    void Start()
    {
        cameraHeight = Camera.main.orthographicSize * 2;
        cameraWidth = cameraHeight* Camera.main.aspect;

        float leftBoundsWidth = leftBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        float rightBoundsWidth = rightBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;

        levelMinX = leftBounds.position.x + leftBoundsWidth + (cameraWidth / 2);
        levelMaxX = rightBounds.position.x - rightBoundsWidth - (cameraWidth / 2);

    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            float targetX = Mathf.Max(levelMinX, Mathf.Min(levelMaxX, target.position.x));
            
            float x = Mathf.SmoothDamp(transform.position.x, targetX,ref smoothDampVelocity.x, smoothDampTime);

            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
