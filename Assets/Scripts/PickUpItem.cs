using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItem : MonoBehaviour
{
    [SerializeField]
    private float pickupRange = 6f;

    [SerializeField]
    private GameObject pickupText;

    [SerializeField]
    private LayerMask layerMask;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange, layerMask))
        {
            if (hit.transform.CompareTag("AmmoBox"))
            {
                pickupText.GetComponent<Text>().text = "Press E to pick up ammo";
                pickupText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpAmmo(hit.transform.gameObject);
                }
            }
            else if (hit.transform.CompareTag("Weapon"))
            {
                pickupText.GetComponent<Text>().text = $"Press E to pick up {Regex.Replace(hit.transform.name, @"\([^()]*\)", "")}"; // Remove the (Clone) part from the name with Regex
                pickupText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickUpWeapon(hit.transform.gameObject);
                }
            }
        }
        else
        {
            pickupText.SetActive(false);
        }
    }

    private void PickUpAmmo(GameObject ammoBox)
    {
        Weapon playerWeapon = GameObject.FindGameObjectWithTag("PlayerWeapon").GetComponent<Weapon>();

        if (playerWeapon != null)
        {
            playerWeapon.numberOfBulletsRemaining += playerWeapon.magazineSize;
        }

        Destroy(ammoBox);
    }

    private void PickUpWeapon(GameObject weapon)
    {
        Weapon playerWeapon = GameObject.FindGameObjectWithTag("PlayerWeapon").GetComponent<Weapon>();

        // Drop the current weapon
        GameObject instantiateWeapon = Instantiate(playerWeapon.weaponPrefab, playerWeapon.transform.position, playerWeapon.transform.rotation);
        if (instantiateWeapon.GetComponent<Rigidbody>() == null)
        {
            instantiateWeapon.AddComponent<Rigidbody>();
            instantiateWeapon.GetComponent<Rigidbody>().mass = 50f;
        }
        else
        {
            instantiateWeapon.GetComponent<Rigidbody>().isKinematic = false;
        }
        instantiateWeapon.tag = "Weapon";
        instantiateWeapon.GetComponent<Weapon>().numberOfBulletsRemaining = playerWeapon.numberOfBulletsRemaining;
        instantiateWeapon.GetComponent<Weapon>().magazineBullets = playerWeapon.magazineBullets;

        // Pick up the new weapon
        weapon.transform.SetParent(playerWeapon.transform.parent);
        weapon.transform.position = playerWeapon.transform.position;
        weapon.transform.rotation = playerWeapon.transform.rotation;
        weapon.tag = "PlayerWeapon";
        weapon.GetComponent<Rigidbody>().isKinematic = true;

        // Destroy the old weapon
        Destroy(playerWeapon.gameObject);
    }
}
