using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance { get; set; }

    public GameObject bulletImpactEffectPrefab;

    public GameObject hitMarkerRed;

    public GameObject hitMarkerWhite;

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
