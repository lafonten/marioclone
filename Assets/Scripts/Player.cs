using UnityEngine;

public class Player : MonoBehaviour
{
    public float jumpVelocity;   //z�plama h�z�
    public float bounceVelocity;     // s��rama h�z�
    public Vector2 velocity;           // h�z
    public float gravity;               // yer�ekimi yani bu proje i�in d��me h�z�
    public LayerMask wallMask;          // duvarlar� tan�ma tagi
    public LayerMask floorMask;            // zemini tan�ma tagi

    private bool walk, walk_left, walk_right, jump;  // karakterin y�r�me z�plama durumlar� 

    public enum PlayerState  // karakterin o anki durumu. enum kullanma sebebimiz karakterin durumlar aras� ge�i� yapt���n�nda animasyonlar� ve hareket durumlar�n� etkin edip etmeyece�imize karar vermek
    {
        jumpining,  // z�plama
        idle,       // bo�ta durma
        walking,    // y�r�me
        bouncing    // s��arama
    } 

    private PlayerState playerState = PlayerState.idle;  // enumu default olarak bo�ta durmaya yani idle pozisyonuna ayarlad�k

    private bool grounded = false;   // karakterin z�plama yapt���nda yerde mi yoksa �uan havada m� oldu�unu kontrol ediyoruz

    private bool bounce = false;     // karakterin d��man� �ld�rd�kten sonra s��rama halinde mi yoksa s��rama bittimi kontrol�n� yap�yoruz

    void Start()
    {

    }


    void Update()
    {
        CheckPlayerInput();  //kullan�c�n�n bast��� tu�lar� kontrol ediyoruz

        UpdatePlayerPosition();   // karakterin bulundu�u pozisyonu g�ncelliyoruz s�rekli

        UpdateAnimationStates();   // animasyon durumlar�n� g�ncelliyoruz
    }

    void UpdatePlayerPosition()
    {

        Vector3 position = transform.localPosition;  // karakterin o anki bulundu�u pozisyonu al�yoruz
        Vector3 scale = transform.localScale;          // karakterin o anki bakt��� y�n� al�yoruz

        if (walk)  // karkater e�er hareket halindeyse bu sat�r �al���yor
        {
            if (walk_left)  // karakter sola do�ru gidiyorsa buras� �al���yor 
            {
                position.x -= velocity.x * Time.deltaTime;  // karakter sola do�ru gidiyor
                scale.x = -1;  // karakter sola do�ru gitti�i i�in y�z� sola d�n�yor
            }

            if (walk_right)  // karakter sa�a gitmesi gerekiyorsa buras� �al���yor
            {
                position.x += velocity.x * Time.deltaTime;  // karakterin sa�a gitmesini sa�l�yor
                scale.x = 1;  // karakterin y�z�n� sa�a �eviriyor ve default olarak karakter sa�a bak�yor
            }

            position = CheckWallRays(position, scale.x);  // karakterin �zerinde bulunan raycastler ile duvarlara gelip gelmedi�ini kontrol ediyoruz e�er karater bir duvara engele denk gelirse hareket etmiyor duruyor

        }

        if (jump & playerState != PlayerState.jumpining)  // karakterin z�plamas�n� istedi�imizde �al���yor fakat karakter zaten z�pl�yor durumunda de�ilse
        {
            playerState = PlayerState.jumpining;  // karakteri z�plama durumunda oldu�unu belirtiyoruz
             
            velocity = new Vector2(velocity.x, jumpVelocity);  // karakteri z�plat�yoruz
        }

        if (playerState == PlayerState.jumpining)  // karakter e�er z�plama halindeyse
        {
            position.y += velocity.y * Time.deltaTime;  // karaktere yukar� do�ru bir h�z belirtiyoruz

            velocity.y -= gravity * Time.deltaTime;   // ve karakter d��mesi i�in bir yer�ekmi ad� alt�nda bir h�z belirtiyoruz ve karakter bu h�za g�re d���yor
        }

        if (bounce && playerState != PlayerState.bouncing)  // karakterin s��rama durumda olup olmad���n� kontrol ediyoruz ve e�er s��ramas� gerekiyorsa fakat zaten s��ramad�ysa bu kod sat�r� �al���yor
        {
            playerState = PlayerState.bouncing;  // karakterin �uanda s��rama durumunda oldu�unu belirtiyoruz

            velocity = new Vector2(velocity.x, bounceVelocity);  // karaktere bir s��rama h�z� veriyoruz
        }

        if (playerState == PlayerState.bouncing) // e�er karakter �uanda s��rama durumundaysa karakteri geri yere indiriyoruz
        {
            position.y += velocity.y * Time.deltaTime; // karakterin s��ra h�z�n� belirtiyoruz

            velocity.y -= gravity * Time.deltaTime;    // karakterin yer�ekimi h�z� ile d��mesini sa�l�yoruz
        }

        if (velocity.y <= 0)  // karakterin h�z� 0 ve a�a��daysa bu karakterin zemine temas etti�i anlam�na gelir
        {
            position = CheckFloorRays(position);  // karakterin zemine temas edip etmedi�ini kontrol ediyoruz
        }

        if (velocity.y >= 0)  // karakterin y h�z� 0 dan b�y�kse veya e�itse karakterin y�kselmesini kontrol ediyoruz
        {
            position = CheckCeilingRays(position); // karakterin y�ksekli�i kontrol ediliyor
        }

        transform.localPosition = position; // karakterin i�lemler sonucundaki pozisyonu referans al�n�yor
        transform.localScale = scale;   // karkaterin i�lemler sonucunda bulundu�u y�n referans al�n�yor
    }

    void UpdateAnimationStates()  // animasyonlar�n ne zaman �al���p ne zaman duraca��n� kontrol edip g�ncelliyoruz
    {
        if (grounded && !walk  && !bounce)  // e�er karakter yerdeyse fakat y�r�m�yor ve s��ram�yorsa karakter idle pozisyonundad�r ve idle animasyonu devreye girer
        {
            GetComponent<Animator>().SetBool("isJumping",false); // fakata z�plama animasyonu �al��maz
            GetComponent<Animator>().SetBool("isRunning",false); // ve ko�ma animasyonuda �al��maz 

        }

        if (grounded && walk) // karakter hem zeminde hemde y�r�yorsa y�r�me animasyonu �al���r
        {
            GetComponent<Animator>().SetBool("isJumping",false);  // karakter �uanda z�plamad��� i�in z�plama animasyonu �al��maz
            GetComponent<Animator>().SetBool("isRunning",true);  // karakter y�r�d��� i�in y�r�me animasyonu �al���r 
        }

        if (playerState == PlayerState.jumpining) // e�er karakterin girdi�i durum z�plama ise z�plama animasyonu �al���r
        {
            GetComponent<Animator>().SetBool("isJumping",true);  // karakterin z�plama animasyonu devreye girer
            GetComponent<Animator>().SetBool("isRunning",false);  // karakterin y�r�me animasyonu e�er aktifse deaktif olur
        }
    }

    void CheckPlayerInput()  // kullan�c�n�n bast��� tu�lar kontrol edilir
    {
        bool input_left = Input.GetKey(KeyCode.LeftArrow); // e�er kullan�c� sol oka basarsa karakter sol tu� girdisi do�ru kabul edilir
        bool input_right = Input.GetKey(KeyCode.RightArrow);// e�er kullan�c� sa� oka basarsa karakter sa� tu� girdisi do�ru kabul edilir
        bool input_space = Input.GetKeyDown(KeyCode.Space);// e�er kullan�c� space'e basarsa karakter space tu� girdisi do�ru kabul edilir

        walk = input_left || input_right; // e�er sa� veya sol input do�ruysa karakter y�r�yor demektir

        walk_left = input_left && !input_right;  // e�er sa� input yanl�� sol input do�ruysa karakter sola y�r�mesi gerekir

        walk_right = input_right && !input_left;  // e�er sa� input do�ru sol input yanl��sa karakterin sa�a y�r�mesi gerekir

        jump = input_space;   // e�er space inputu do�ruysa karakterin z�plamas� gerekir
    }

    Vector3 CheckWallRays(Vector3 position, float direction) // karakterin duvarlala temas� kontrol edilir
    {
        Vector2 originTop = new Vector2(position.x + direction * 0.4f, position.y + 1f - 0.2f); 
        // karakterin �st b�lgesinin konumu girilir, burda direction karakterin bakt��� y�nd�r positionda karakterin bulundu�u pozisyondur
        // origin top raycastte konumlanacak ���n�n karakterin neresnde olaca��� belirtir karkaterin boyu 2f tir karakterin boyunun yar�s�ndan 2 eksik ve y�n�ne ba�l� olarak 0.4f uzakl�kta konumlan�r
        
        Vector2 originMiddle = new Vector2(position.x + direction * 0.4f, position.y);
        // karakterin orta b�lgesinin konumu girilir, burda direction karakterin bakt��� y�nd�r positionda karakterin bulundu�u pozisyondur
        // origin middle raycastte konumlanacak ���n�n karakterin neresnde olaca��� belirtir karkaterin boyu 2f tir karakterin boyunun ortas�na bakar ve y�n�ne ba�l� olarak 0.4f uzakl�kta konumlan�r
        
        Vector2 originBottom = new Vector2(position.x + direction * 0.4f, position.y - 1f + 0.2f);
        // karakterin alt b�lgesinin konumu girilir, burda direction karakterin bakt��� y�nd�r positionda karakterin bulundu�u pozisyondur
        // origin bottom raycastte konumlanacak ���n�n karakterin neresnde olaca��� belirtir karkaterin boyu 2f tir karakterin boyunun yar�s�ndan 2 fazla fakat yar�s�n� ��kart�r�zki a�a��da konumlans�n ve y�n�ne ba�l� olarak 0.4f uzakl�kta konumlan�r


        // karakterin belli pozisyonuna ve y�n�ne ba�l� olarak h�z�na g�re belli mesafe �n�nde duvar� kontrol� yapacak bir raycastlerdir
        RaycastHit2D wallTop =
            Physics2D.Raycast(originTop, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallMiddle =
            Physics2D.Raycast(originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallBottom =
            Physics2D.Raycast(originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

        // e�er karakter herhangi bir �eye temas ederse karakterin durulmas� i�in directionla �arp�lan bir h�z komutu girilir bu komut sayesinde tu�a bas�lsa bile ters y�ne gidilmedi�i s�rece karakter durur
        if (wallTop.collider != null || wallBottom.collider != null || wallMiddle.collider != null)
        {
            position.x -= velocity.x * Time.deltaTime * direction;
        }

        return position; // komut �a�r�ld���nda s�rekli verilen pozisyon bilgisi g�nderilir
    }

    Vector3 CheckFloorRays(Vector3 position) // zemin kontrol�
    {
        // zemin kontrol� i�in karakterin sa��ndan solundan ve ortas�ndan 3 ayr� raycast �izdirilir ve b�ylece hem arkas�nda kalan hem �n�nde hemde alt�nda bulundu�u zemin kontrol edilir

        Vector2 originLeft = new Vector2(position.x - 0.5f + 0.2f, position.y - 1f);
        // burda origin left ile bulundu�u pozisyonun merkez noktas�n� al�r ve ondan 0.5 birim ��kart�r ve 0,2 birim ekler b�ylece pozisyonu sol s�n�ra yak�n olur ve ayn� zamanda zemin kontrol� yapmak i�in y ekseninde ortas�ndan 1 birim a�a�� al�n�r
        
        Vector2 originMiddle = new Vector2(position.x, position.y - 1f);
        // burda origin left ile bulundu�u pozisyonun merkez noktas�n� al�r ve ondan 0.5 birim ��kart�r ve 0,2 birim ekler b�ylece pozisyonu sol s�n�ra yak�n olur ve ayn� zamanda zemin kontrol� yapmak i�in y ekseninde ortas�ndan 1 birim a�a�� al�n�r
        
        Vector2 originRight = new Vector2(position.x + 0.5f - 0.2f, position.y - 1f);
        // burda origin left ile bulundu�u pozisyonun merkez noktas�n� al�r ve ondan 0.5 birim ��kart�r ve 0,2 birim ekler b�ylece pozisyonu sol s�n�ra yak�n olur ve ayn� zamanda zemin kontrol� yapmak i�in y ekseninde ortas�ndan 1 birim a�a�� al�n�r

        //yukar�da zaten vekt�r�n konumunu ald�k art�k sadece ald���m�z konumlara g�re down vekt�r�n� �izdirip bulunaca�� mesafeyi h�za g�re ayarlay�p kontrol edece�i layermask'� se�mek kald�
        RaycastHit2D floorLeft = Physics2D.Raycast(originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorMiddle =
            Physics2D.Raycast(originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D floorRight = Physics2D.Raycast(originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

        //zemine de�ip de�medi�ini kontrol ediyor 
        if (floorLeft.collider != null || floorRight.collider != null || floorMiddle.collider != null)
        {
            //de�en raycasting hangi y�nde oldu�unu kontrol ediyoruz
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
            //enemy e de�ip de�medi�imize bak�yoruz
            if (hitRay.collider.tag == "Enemy")
            {
                bounce = true; //enemye de�idi�imizde e�er �sten de�diysek s��rama yap�yoruz

                hitRay.collider.GetComponent<EnemyAI>().Crush();// s��ramadan �nce enemy crush oluyor
            }

            playerState = PlayerState.idle; // karakterimizi idle pozisyonuna d�nd�r�yor

            grounded = true; //yerle temas etti�imiz do�rulan�yor

            velocity.y = 0; //y ekseninde h�z�m� 0 a sabitleniyor yani yere geri iniyirouz

            position.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + 1; // kontrol� ve zeminin yerini saptad�ktan sonra zeminin pozisyonunu belirtiyoruz
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
