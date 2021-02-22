using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;

public class QuestionBlocks : MonoBehaviour
{

    public float bounceHeight = 0.5f; //bloðun yükseleceði yükseklik
    public float bounceSpeed = 4f;    //bloðun yükselirken kazanacaðý hýz

    public float coinMoveSpeed = 8f;
    public float coinMoveHeight = 3f;
    public float coinFallDistance = 2f;

    private Vector2 originalPosition;  //geri dönerken eski pozsiyonu yani orjinal pozisoynuna ihtiyacýmýz var bu yüzden bunu depoluyoruz

    public Sprite emptyBlockSprite;

    private bool canBounce = true;    //bloðun yükselip yükselmediðini kontrol ediyoruz

    void Start()
    {
        originalPosition = transform.localPosition;  // bloðun ilk pozisyonu

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

    void ChangeSprite()
    {
        GetComponent<Animator>().enabled = false;

        GetComponent<SpriteRenderer>().sprite = emptyBlockSprite;
    }

    void PresentCoin()
    {
        GameObject spiningCoin = (GameObject)Instantiate(Resources.Load("Prefabs/Spinning_Coin", typeof(GameObject)));
        spiningCoin.transform.SetParent(this.transform.parent);
        spiningCoin.transform.localPosition = new Vector2(originalPosition.x, originalPosition.y + 1);
        StartCoroutine(MoveCoin(spiningCoin));
    }

    IEnumerator Bounce()
    {

        ChangeSprite();

        PresentCoin();

        while (true)
        {
            transform.localPosition = new Vector2(transform.localPosition.x,
                transform.localPosition.y + bounceSpeed * Time.deltaTime);

            if (transform.localPosition.y >= originalPosition.y + bounceHeight)
            {
                break;
            }

            yield return null;
        }

        while (true)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - bounceSpeed * Time.deltaTime);

            if (transform.localPosition.y <= originalPosition.y)
            {
                transform.localPosition = originalPosition;
                break;
            }

            yield return null;
        }
    }

    IEnumerator MoveCoin(GameObject coin)
    {
        while (true)
        {
            coin.transform.localPosition = new Vector2(coin.transform.localPosition.x,
                coin.transform.localPosition.y + coinMoveSpeed * Time.deltaTime);

            if (coin.transform.localPosition.y >= originalPosition.y + coinMoveHeight + 1)
            {
                break;
            }

            yield return null;
        }

        while (true)
        {
            coin.transform.localPosition = new Vector2(coin.transform.localPosition.x,
                coin.transform.localPosition.y - coinMoveSpeed * Time.deltaTime);

            if (coin.transform.localPosition.y <= originalPosition.y + coinFallDistance + 1)
            {
                Destroy(coin.gameObject);
                break;
            }

            yield return null;
        }
    }
}
