using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance { get; set; }

    public GameObject bulletImpactEffectPrefab;

    public GameObject hitMarkerRed;
    public GameObject hitMarkerWhite;

    public GameObject bloodScreen;

    private PlayerSystem playerSystem;
    private RawImage bloodRawImage;

    void Start()
    {
        bloodScreen.SetActive(true);
        bloodScreen.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        bloodRawImage = bloodScreen.GetComponent<RawImage>();

        playerSystem = GameObject.FindGameObjectWithTag("PlayerTag").GetComponent<PlayerSystem>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (playerSystem.currentHealth < playerSystem.maxHealth * 0.25)
        {             
            bloodRawImage.color = new Color(bloodRawImage.color.r, bloodRawImage.color.g, bloodRawImage.color.b, 1);
        }
        else if (playerSystem.currentHealth < playerSystem.maxHealth * 0.5)
        {
            bloodRawImage.color = new Color(bloodRawImage.color.r, bloodRawImage.color.g, bloodRawImage.color.b, 0.5f);
        }
        else if (playerSystem.currentHealth < playerSystem.maxHealth)
        {
            bloodRawImage.color = new Color(bloodRawImage.color.r, bloodRawImage.color.g, bloodRawImage.color.b, 0.25f);
        }
        else if (playerSystem.currentHealth == playerSystem.maxHealth)
        {
            bloodRawImage.color = new Color(bloodRawImage.color.r, bloodRawImage.color.g, bloodRawImage.color.b, 0);
        }
    }

    public void ActiveHitMarkerRed()
    {
        hitMarkerRed.SetActive(true);
        StartCoroutine(DisableHitMarker(hitMarkerRed));
    }

    public void ActiveHitMarkerWhite()
    {
        hitMarkerWhite.SetActive(true);
        StartCoroutine(DisableHitMarker(hitMarkerWhite));
    }

    public IEnumerator DisableHitMarker(GameObject hitMarker)
    {
        yield return new WaitForSeconds(0.5f);
        hitMarker.SetActive(false);
    }
}
