using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                //takip edilecek hedefin yani karakterin konumu
    public Transform leftBounds;            //kameran�n gidece�i max sol s�n�r�
    public Transform rightBounds;           //kameran�n gidece�i max sa� s�n�r�

    public float smoothDampTime = 0.5f;             //kamera karakteri takip ederken smooth olmas� i�in bekleme s�resi koyuyoruz
    private Vector3 smoothDampVelocity = Vector3.zero;          //merkez konuma �izilen vekt�r (0,0,0)

    private float cameraWidth, cameraHeight, levelMinX, levelMaxX;   //kameran�n �zellikleri - geni�lik - y�kseklik - maximum gidebilece�i kenar - minimum gidebilece�i kenar
     
    void Start()
    {
        cameraHeight = Camera.main.orthographicSize * 2;        //kameran�n y�ksekli�i
        cameraWidth = cameraHeight* Camera.main.aspect;         //kameran�n geni�li�i

        float leftBoundsWidth = leftBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;       //sol s�n�r
        float rightBoundsWidth = rightBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;     //sa� s�n�r

        levelMinX = leftBounds.position.x + leftBoundsWidth + (cameraWidth / 2);        //maximum gidece�i sol
        levelMaxX = rightBounds.position.x - rightBoundsWidth - (cameraWidth / 2);      //maximum gidece�i sa�

    }

    
    void Update()
    {
        if (target)  //hedef obje
        {
            float targetX = Mathf.Max(levelMinX, Mathf.Min(levelMaxX, target.position.x));
            
            float x = Mathf.SmoothDamp(transform.position.x, targetX,ref smoothDampVelocity.x, smoothDampTime);

            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
