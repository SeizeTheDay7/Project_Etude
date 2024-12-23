using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDirector : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private float titleFadeDuration;
    [SerializeField] private float PressAnyKeyFadeDuration;
    [SerializeField] private GameObject canvas;
    [SerializeField] private CanvasRenderer Title;
    [SerializeField] private CanvasRenderer PressAnyKey;
    [SerializeField] private GameObject middle;
    [SerializeField] private float orthTargetSize;
    [SerializeField] private float orthSmoothTime;
    private bool game_start = false;
    private float velocity = 0.0f;
    SpriteRenderer playerSprite;


    // Start is called before the first frame update
    void Start()
    {
        playerSprite = player.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // PressAnyKey.SetAlpha(Mathf.PingPong(Time.time * blinkSpeed, 1)); // Alpha 값을 0과 1 사이로 반복
        if (Input.anyKeyDown && !game_start)
        {
            StartCoroutine(TitleFadeOut());
            StartCoroutine(PressAnykeyFadeOut());
        }
        if (!game_start)
        {
            PressAnyKey.SetAlpha(Mathf.PingPong(Time.time * 1f, 0.75f) + 0.25f); // Alpha 값을 0.5와 1 사이로 반복
        }
    }

    private IEnumerator TitleFadeOut()
    {
        game_start = true;
        yield return new WaitForSeconds(1.5f); // 1초 대기
        float elapsedTime = 0f;
        player.SetActive(true);

        while (elapsedTime < titleFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Title.SetAlpha(Mathf.SmoothStep(1f, 0f, elapsedTime / titleFadeDuration)); // 초반에 급하게 사라지다가 후반에 느리게 사라지게                                                                           
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // 0.5초 대기

        // 완전히 투명해진 뒤 오브젝트 비활성화
        canvas.SetActive(false);
        elapsedTime = 0f;

        while (elapsedTime < titleFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // 플레이어 sprite renderer alpha 서서히 증가시키기
            if (playerSprite != null)
            {
                playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, Mathf.SmoothStep(0f, 1f, elapsedTime / titleFadeDuration));
            }
            yield return null;
        }

        virtualCamera.Follow = middle.transform;
        StartCoroutine(SmoothOrthographicSizeChange());
    }

    private IEnumerator PressAnykeyFadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < PressAnyKeyFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            PressAnyKey.SetAlpha(Mathf.Lerp(1f, 0f, elapsedTime / PressAnyKeyFadeDuration)); // 서서히 Alpha 값을 0으로
            yield return null;
        }
    }

    private IEnumerator SmoothOrthographicSizeChange()
    {
        while (Mathf.Abs(virtualCamera.m_Lens.OrthographicSize - orthTargetSize) > 0.01f)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(
                virtualCamera.m_Lens.OrthographicSize,
                orthTargetSize,
                ref velocity,
                orthSmoothTime
            );
            yield return null;
        }
        // 리듬 오프셋 UI 활성화

        // 방향키 UI 게임 오브젝트 활성화

        // 플레이어 스크립트 활성화
    }

}
