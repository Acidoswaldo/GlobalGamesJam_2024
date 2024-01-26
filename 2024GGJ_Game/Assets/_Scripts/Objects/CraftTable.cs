using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTable : MonoBehaviour
{
    [SerializeField] List<GameObject> clothesPrefabs;
    private int currentClothIndex = 0;
    private bool playerInRange = false;
    [SerializeField] PlayerController playerController;
    private bool isCrafting = false;
    private float craftingTime = 2.0f; 

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q) && !isCrafting)
        {
            StartCoroutine(CraftClothes());
            Debug.Log("Crafting");
        }

        if (isCrafting && playerController != null && !IsPlayerCloseEnough())
        {
            StopAllCoroutines();
            isCrafting = false;
        }
    }

    private IEnumerator CraftClothes()
    {
        isCrafting = true;
        float elapsedTime = 0f;

        while (elapsedTime < craftingTime)
        {
            if (!IsPlayerCloseEnough()) // Check if the player has moved away
            {
                Debug.Log("Crafting Interrupted");
                isCrafting = false;
                yield break; // Exit the coroutine if the player moves away
            }

            elapsedTime += Time.deltaTime; // Increment the time
            yield return null; // Wait for the next frame
        }

        if (currentClothIndex < clothesPrefabs.Count)
        {
            Instantiate(clothesPrefabs[currentClothIndex], playerController.transform.position + Vector3.left, Quaternion.identity);
            currentClothIndex++;
            Debug.Log("Created");
        }

        isCrafting = false;
    }

    private bool IsPlayerCloseEnough()
    {
        return Vector3.Distance(transform.position, playerController.transform.position) < 10.0f; 
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            playerController = collision.gameObject.GetComponent<PlayerController>();
            Debug.Log("Enter");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            playerController = null;
        }
    }
}