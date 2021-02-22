using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                //takip edilecek hedefin yani karakterin konumu
    public Transform leftBounds;            //kameranýn gideceði max sol sýnýrý
    public Transform rightBounds;           //kameranýn gideceði max sað sýnýrý

    public float smoothDampTime = 0.5f;             //kamera karakteri takip ederken smooth olmasý için bekleme süresi koyuyoruz
    private Vector3 smoothDampVelocity = Vector3.zero;          //merkez konuma çizilen vektör (0,0,0)

    private float cameraWidth, cameraHeight, levelMinX, levelMaxX;   //kameranýn özellikleri - geniþlik - yükseklik - maximum gidebileceði kenar - minimum gidebileceði kenar
     
    void Start()
    {
        cameraHeight = Camera.main.orthographicSize * 2;        //kameranýn yüksekliði
        cameraWidth = cameraHeight* Camera.main.aspect;         //kameranýn geniþliði

        float leftBoundsWidth = leftBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;       //sol sýnýr
        float rightBoundsWidth = rightBounds.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;     //sað sýnýr

        levelMinX = leftBounds.position.x + leftBoundsWidth + (cameraWidth / 2);        //maximum gideceði sol
        levelMaxX = rightBounds.position.x - rightBoundsWidth - (cameraWidth / 2);      //maximum gideceði sað

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
