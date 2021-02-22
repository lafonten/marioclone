using UnityEngine;

public class Player : MonoBehaviour
{
    public float jumpVelocity;   //zýplama hýzý
    public float bounceVelocity;     // sýçrama hýzý
    public Vector2 velocity;           // hýz
    public float gravity;               // yerçekimi yani bu proje için düþme hýzý
    public LayerMask wallMask;          // duvarlarý tanýma tagi
    public LayerMask floorMask;            // zemini tanýma tagi

    private bool walk, walk_left, walk_right, jump;  // karakterin yürüme zýplama durumlarý 

    public enum PlayerState  // karakterin o anki durumu. enum kullanma sebebimiz karakterin durumlar arasý geçiþ yaptýðýnýnda animasyonlarý ve hareket durumlarýný etkin edip etmeyeceðimize karar vermek
    {
        jumpining,  // zýplama
        idle,       // boþta durma
        walking,    // yürüme
        bouncing    // sýçarama
    } 

    private PlayerState playerState = PlayerState.idle;  // enumu default olarak boþta durmaya yani idle pozisyonuna ayarladýk

    private bool grounded = false;   // karakterin zýplama yaptýðýnda yerde mi yoksa þuan havada mý olduðunu kontrol ediyoruz

    private bool bounce = false;     // karakterin düþmaný öldürdükten sonra sýçrama halinde mi yoksa sýçrama bittimi kontrolünü yapýyoruz

    void Start()
    {

    }


    void Update()
    {
        CheckPlayerInput();  //kullanýcýnýn bastýðý tuþlarý kontrol ediyoruz

        UpdatePlayerPosition();   // karakterin bulunduðu pozisyonu güncelliyoruz sürekli

        UpdateAnimationStates();   // animasyon durumlarýný güncelliyoruz
    }

    void UpdatePlayerPosition()
    {

        Vector3 position = transform.localPosition;  // karakterin o anki bulunduðu pozisyonu alýyoruz
        Vector3 scale = transform.localScale;          // karakterin o anki baktýðý yönü alýyoruz

        if (walk)  // karkater eðer hareket halindeyse bu satýr çalýþýyor
        {
            if (walk_left)  // karakter sola doðru gidiyorsa burasý çalýþýyor 
            {
                position.x -= velocity.x * Time.deltaTime;  // karakter sola doðru gidiyor
                scale.x = -1;  // karakter sola doðru gittiði için yüzü sola dönüyor
            }

            if (walk_right)  // karakter saða gitmesi gerekiyorsa burasý çalýþýyor
            {
                position.x += velocity.x * Time.deltaTime;  // karakterin saða gitmesini saðlýyor
                scale.x = 1;  // karakterin yüzünü saða çeviriyor ve default olarak karakter saða bakýyor
            }

            position = CheckWallRays(position, scale.x);  // karakterin üzerinde bulunan raycastler ile duvarlara gelip gelmediðini kontrol ediyoruz eðer karater bir duvara engele denk gelirse hareket etmiyor duruyor

        }

        if (jump & playerState != PlayerState.jumpining)  // karakterin zýplamasýný istediðimizde çalýþýyor fakat karakter zaten zýplýyor durumunda deðilse
        {
            playerState = PlayerState.jumpining;  // karakteri zýplama durumunda olduðunu belirtiyoruz
             
            velocity = new Vector2(velocity.x, jumpVelocity);  // karakteri zýplatýyoruz
        }

        if (playerState == PlayerState.jumpining)  // karakter eðer zýplama halindeyse
        {
            position.y += velocity.y * Time.deltaTime;  // karaktere yukarý doðru bir hýz belirtiyoruz

            velocity.y -= gravity * Time.deltaTime;   // ve karakter düþmesi için bir yerçekmi adý altýnda bir hýz belirtiyoruz ve karakter bu hýza göre düþüyor
        }

        if (bounce && playerState != PlayerState.bouncing)  // karakterin sýçrama durumda olup olmadýðýný kontrol ediyoruz ve eðer sýçramasý gerekiyorsa fakat zaten sýçramadýysa bu kod satýrý çalýþýyor
        {
            playerState = PlayerState.bouncing;  // karakterin þuanda sýçrama durumunda olduðunu belirtiyoruz

            velocity = new Vector2(velocity.x, bounceVelocity);  // karaktere bir sýçrama hýzý veriyoruz
        }

        if (playerState == PlayerState.bouncing) // eðer karakter þuanda sýçrama durumundaysa karakteri geri yere indiriyoruz
        {
            position.y += velocity.y * Time.deltaTime; // karakterin sýçra hýzýný belirtiyoruz

            velocity.y -= gravity * Time.deltaTime;    // karakterin yerçekimi hýzý ile düþmesini saðlýyoruz
        }

        if (velocity.y <= 0)  // karakterin hýzý 0 ve aþaðýdaysa bu karakterin zemine temas ettiði anlamýna gelir
        {
            position = CheckFloorRays(position);  // karakterin zemine temas edip etmediðini kontrol ediyoruz
        }

        if (velocity.y >= 0)  // karakterin y hýzý 0 dan büyükse veya eþitse karakterin yükselmesini kontrol ediyoruz
        {
            position = CheckCeilingRays(position); // karakterin yüksekliði kontrol ediliyor
        }

        transform.localPosition = position; // karakterin iþlemler sonucundaki pozisyonu referans alýnýyor
        transform.localScale = scale;   // karkaterin iþlemler sonucunda bulunduðu yön referans alýnýyor
    }

    void UpdateAnimationStates()  // animasyonlarýn ne zaman çalýþýp ne zaman duracaðýný kontrol edip güncelliyoruz
    {
        if (grounded && !walk  && !bounce)  // eðer karakter yerdeyse fakat yürümüyor ve sýçramýyorsa karakter idle pozisyonundadýr ve idle animasyonu devreye girer
        {
            GetComponent<Animator>().SetBool("isJumping",false); // fakata zýplama animasyonu çalýþmaz
            GetComponent<Animator>().SetBool("isRunning",false); // ve koþma animasyonuda çalýþmaz 

        }

        if (grounded && walk) // karakter hem zeminde hemde yürüyorsa yürüme animasyonu çalýþýr
        {
            GetComponent<Animator>().SetBool("isJumping",false);  // karakter þuanda zýplamadýðý için zýplama animasyonu çalýþmaz
            GetComponent<Animator>().SetBool("isRunning",true);  // karakter yürüdüðü için yürüme animasyonu çalýþýr 
        }

        if (playerState == PlayerState.jumpining) // eðer karakterin girdiði durum zýplama ise zýplama animasyonu çalýþýr
        {
            GetComponent<Animator>().SetBool("isJumping",true);  // karakterin zýplama animasyonu devreye girer
            GetComponent<Animator>().SetBool("isRunning",false);  // karakterin yürüme animasyonu eðer aktifse deaktif olur
        }
    }

    void CheckPlayerInput()  // kullanýcýnýn bastýðý tuþlar kontrol edilir
    {
        bool input_left = Input.GetKey(KeyCode.LeftArrow); // eðer kullanýcý sol oka basarsa karakter sol tuþ girdisi doðru kabul edilir
        bool input_right = Input.GetKey(KeyCode.RightArrow);// eðer kullanýcý sað oka basarsa karakter sað tuþ girdisi doðru kabul edilir
        bool input_space = Input.GetKeyDown(KeyCode.Space);// eðer kullanýcý space'e basarsa karakter space tuþ girdisi doðru kabul edilir

        walk = input_left || input_right; // eðer sað veya sol input doðruysa karakter yürüyor demektir

        walk_left = input_left && !input_right;  // eðer sað input yanlýþ sol input doðruysa karakter sola yürümesi gerekir

        walk_right = input_right && !input_left;  // eðer sað input doðru sol input yanlýþsa karakterin saða yürümesi gerekir

        jump = input_space;   // eðer space inputu doðruysa karakterin zýplamasý gerekir
    }

    Vector3 CheckWallRays(Vector3 position, float direction) // karakterin duvarlala temasý kontrol edilir
    {
        Vector2 originTop = new Vector2(position.x + direction * 0.4f, position.y + 1f - 0.2f); 
        // karakterin üst bölgesinin konumu girilir, burda direction karakterin baktýðý yöndür positionda karakterin bulunduðu pozisyondur
        // origin top raycastte konumlanacak ýþýnýn karakterin neresnde olacaýðý belirtir karkaterin boyu 2f tir karakterin boyunun yarýsýndan 2 eksik ve yönüne baðlý olarak 0.4f uzaklýkta konumlanýr
        
        Vector2 originMiddle = new Vector2(position.x + direction * 0.4f, position.y);
        // karakterin orta bölgesinin konumu girilir, burda direction karakterin baktýðý yöndür positionda karakterin bulunduðu pozisyondur
        // origin middle raycastte konumlanacak ýþýnýn karakterin neresnde olacaýðý belirtir karkaterin boyu 2f tir karakterin boyunun ortasýna bakar ve yönüne baðlý olarak 0.4f uzaklýkta konumlanýr
        
        Vector2 originBottom = new Vector2(position.x + direction * 0.4f, position.y - 1f + 0.2f);
        // karakterin alt bölgesinin konumu girilir, burda direction karakterin baktýðý yöndür positionda karakterin bulunduðu pozisyondur
        // origin bottom raycastte konumlanacak ýþýnýn karakterin neresnde olacaýðý belirtir karkaterin boyu 2f tir karakterin boyunun yarýsýndan 2 fazla fakat yarýsýný çýkartýrýzki aþaðýda konumlansýn ve yönüne baðlý olarak 0.4f uzaklýkta konumlanýr


        // karakterin belli pozisyonuna ve yönüne baðlý olarak hýzýna göre belli mesafe önünde duvarý kontrolü yapacak bir raycastlerdir
        RaycastHit2D wallTop =
            Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallMiddle =
            Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallBottom =
            Physics2D.Raycast(originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        // eðer karakter herhangi bir þeye temas ederse karakterin durulmasý için directionla çarpýlan bir hýz komutu girilir bu komut sayesinde tuþa basýlsa bile ters yöne gidilmediði sürece karakter durur
        if (wallTop.collider != null || wallBottom.collider != null || wallMiddle.collider != null)
        {
            position.x -= velocity.x * Time.deltaTime * direction;
        }

        return position; // komut çaðrýldýðýnda sürekli verilen pozisyon bilgisi gönderilir
    }

    Vector3 CheckFloorRays(Vector3 position) // zemin kontrolü
    {
        // zemin kontrolü için karakterin saðýndan solundan ve ortasýndan 3 ayrý raycast çizdirilir ve böylece hem arkasýnda kalan hem önünde hemde altýnda bulunduðu zemin kontrol edilir

        Vector2 originLeft = new Vector2(position.x - 0.5f + 0.2f, position.y - 1f);
        // burda origin left ile bulunduðu pozisyonun merkez noktasýný alýr ve ondan 0.5 birim çýkartýr ve 0,2 birim ekler böylece pozisyonu sol sýnýra yakýn olur ve ayný zamanda zemin kontrolü yapmak için y ekseninde ortasýndan 1 birim aþaðý alýnýr
        
        Vector2 originMiddle = new Vector2(position.x, position.y - 1f);
        // burda origin left ile bulunduðu pozisyonun merkez noktasýný alýr ve ondan 0.5 birim çýkartýr ve 0,2 birim ekler böylece pozisyonu sol sýnýra yakýn olur ve ayný zamanda zemin kontrolü yapmak için y ekseninde ortasýndan 1 birim aþaðý alýnýr
        
        Vector2 originRight = new Vector2(position.x + 0.5f - 0.2f, position.y - 1f);
        // burda origin left ile bulunduðu pozisyonun merkez noktasýný alýr ve ondan 0.5 birim çýkartýr ve 0,2 birim ekler böylece pozisyonu sol sýnýra yakýn olur ve ayný zamanda zemin kontrolü yapmak için y ekseninde ortasýndan 1 birim aþaðý alýnýr

        //yukarýda zaten vektörün konumunu aldýk artýk sadece aldýðýmýz konumlara göre down vektörünü çizdirip bulunacaðý mesafeyi hýza göre ayarlayýp kontrol edeceði layermask'ý seçmek kaldý
        RaycastHit2D floorLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorMiddle =
            Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorRight = Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        //zemine deðip deðmediðini kontrol ediyor 
        if (floorLeft.collider != null || floorRight.collider != null || floorMiddle.collider != null)
        {
            //deðen raycasting hangi yönde olduðunu kontrol ediyoruz
            RaycastHit2D hitRay = floorRight;

            if (floorLeft)
            {
                hitRay = floorLeft;
            }
            else if (floorMiddle)
            {
                hitRay = floorMiddle;
            }
            else if (floorRight)
            {
                hitRay = floorRight;
            }
            //enemy e deðip deðmediðimize bakýyoruz
            if (hitRay.collider.tag == "Enemy")
            {
                bounce = true; //enemye deðidiðimizde eðer üsten deðdiysek sýçrama yapýyoruz

                hitRay.collider.GetComponent<EnemyAI>().Crush();// sýçramadan önce enemy crush oluyor
            }

            playerState = PlayerState.idle; // karakterimizi idle pozisyonuna döndürüyor

            grounded = true; //yerle temas ettiðimiz doðrulanýyor

            velocity.y = 0; //y ekseninde hýzýmý 0 a sabitleniyor yani yere geri iniyirouz

            position.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + 1; // kontrolü ve zeminin yerini saptadýktan sonra zeminin pozisyonunu belirtiyoruz
        }
        else
        {
            if (playerState != PlayerState.jumpining)
            {
                Fall();
            }
        }

        return position;
    }

    Vector3 CheckCeilingRays(Vector3 position)
    {
        Vector2 originLeft = new Vector2(position.x - 0.5f + 0.2f, position.y + 1f);
        Vector2 originMiddle = new Vector2(position.x, position.y + 1f);
        Vector2 originRight = new Vector2(position.x + 0.5f - 0.2f, position.y + 1f);

        RaycastHit2D ceilLeft = Physics2D.Raycast(originLeft, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilMiddle = Physics2D.Raycast(originMiddle, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilRight = Physics2D.Raycast(originRight, Vector2.up, velocity.y * Time.deltaTime, floorMask);

        if (ceilLeft.collider != null || ceilMiddle.collider != null || ceilRight.collider != null)
        {
            RaycastHit2D hitRay = ceilLeft;
            if (ceilLeft)
            {
                hitRay = ceilLeft;
            }else if (ceilMiddle)
            {
                hitRay = ceilMiddle;
            }else if (ceilRight)
            {
                hitRay = ceilRight;
            }

            if (hitRay.collider.tag == "QuestionBlock")
            {
                hitRay.collider.GetComponent<QuestionBlocks>().QuestionBlockBounce();
            }

            position.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y / 2 - 1;
            Fall();
        }

        return position;
    }

    void Fall()
    {
        velocity.y = 0;

        playerState = PlayerState.jumpining;

        bounce = false;

        grounded = false;
    }
}
