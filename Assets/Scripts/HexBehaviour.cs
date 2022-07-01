using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBehaviour : MonoBehaviour
{
    // Hex durumunu tutacak state enum'ý
    public enum HexState { Idle, Falling, Rotating };

    public HexState state = HexState.Idle;

    public Hex hexReference;

    [SerializeField]
    private Vector3 targetPosition;

    public float moveTime = 1.0f;
    private float moveT = 0.0f;

    [Space]
    private Quaternion targetRotation;
    [SerializeField]
    private Vector3 targetRot;
    public float rotationTime = 1.0f;


    private BoardManager boardManager;
    private GameManager gameManager;

    public ParticleSystem particles;
    void Awake()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        boardManager = BoardManager.Instance;
        gameManager = GameManager.Instance;
        particles = GetComponent<ParticleSystem>();

        waitForEndOfFrame = new WaitForEndOfFrame();
    }
    WaitForEndOfFrame waitForEndOfFrame;

    // Ýki durum var
    // Birincisi: aþaðý doðru düþüþ
    // Ýkincisi: Döndürme
    // Düþmek Ýçin Sadece Hareket Ettirme ve Tahtayý eþleþmeler için kontrol ettirme
    // Döndürmek Ýçin ++
    // Hedefe gittikten sonra panoyu kontrol etmek
    // Eðer eþleþme varsa GM'de Durdur ve Hareket Sayýsýný artýr
    // Eþleþme yoksa noktayý tekrar çevirin ve tahtayý 2 kez daha kontrol ettirme
    // 3 döndürmeden sonra eþleþme olmazsa, dur, GM'de Hareket Sayýsýný ARTIRMAYIN

    // Ýki farklý durum için 2 Fonksiyon geçersiz kýlma kullanýldý
    // Dot.Rotate'de kullanýlan B iþlevi
    // Falling kullanýlan A fonksiyonunda

    // Nesneleri taþýmak için eþyordamlarý kullanmak performans için korkunç
    // Gelecekte kullanma

    #region Function A
    public void SetPosition(Vector3 pos)
    {
        // if (moving)
        // return;
        targetPosition = pos;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        state = HexState.Falling;
        moveT = 0.0f;

        while (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveT);
            moveT += Time.deltaTime / moveTime;
            yield return waitForEndOfFrame;//new WaitForEndOfFrame();
        }
        MoveEnded();
    }

    private void MoveEnded()
    {
        state = HexState.Idle;
        bool isThereAnyMatch = boardManager.RefreshMatchingHexes();
    }
    #endregion

    #region Function B
    public void SetPosition(Vector3 pos, DotBehaviour dotB)
    {
        // if (moving)
        // return;
        targetPosition = pos;
        StartCoroutine(MoveB());
    }

    private IEnumerator MoveB()
    {
        state = HexState.Rotating;

        moveT = 0.0f;
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveT);
            moveT += Time.deltaTime / moveTime;
            yield return waitForEndOfFrame;//new WaitForEndOfFrame();
        }
        MoveEndedB();
    }

    private void MoveEndedB()
    {
        state = HexState.Idle;

        bool isThereAnyMatch = boardManager.RefreshMatchingHexes();
        if (!isThereAnyMatch)
        {
            //Artýþ olmazsa gameManager.movementCount
            if (gameManager.rotationCount < 3)
            {
                if (gameManager.lastRotationDirection)
                {
                    gameManager.OnClockWiseSwipe();
                }
                else
                {
                    gameManager.OnCounterClockWiseSwipe();
                }
            }
        }
        else
        {
            gameManager.rotationCount = 3;
            gameManager.IncrementMovementCount();
            foreach (BombBehaviour bb in boardManager.bombsReferences)
            {
                bb.DecrementCountDown();
            }
        }
    }
    #endregion


    private bool CheckDownIsEmpty()
    {
        if (hexReference.y == 0)
            return false;
        if (boardManager.board[hexReference.x, hexReference.y - 1] == null)
        {
            return true;
        }
        return false;
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }

}

