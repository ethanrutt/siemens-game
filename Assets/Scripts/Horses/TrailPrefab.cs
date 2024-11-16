using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrailPrefab : MonoBehaviour
{
    public GameObject trailPrefab; // The prefab for the trail
    public float spawnRate = 0.1f; // Time interval between each new trail spawn
    public float fadeTime = 0.5f;  // How long the trail lasts before it fades away
    private float nextTimeToSpawn = 0f; // To track when to spawn the next trail piece

    private Queue<GameObject> trailPool = new Queue<GameObject>(); // Object pool for trail pieces
    public int poolSize = 10; // Number of trail pieces to keep in the pool (consider reducing this if it's too high)
    public Vector3 trailOffset = new Vector3(0, -0.5f, 0); // Offset to position the trail behind the horse

    void Start()
    {
        // Preload trail pieces into the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject trail = Instantiate(trailPrefab, transform.position, Quaternion.identity, transform.parent);
            trail.SetActive(false); // Start inactive
            trailPool.Enqueue(trail);
        }
    }

    void Update()
    {
        // Check if enough time has passed to spawn a new trail piece
        if (Time.time >= nextTimeToSpawn)
        {
            // Only spawn a new trail if there are any available in the pool
            if (trailPool.Count > 0)
            {
                GameObject trail = trailPool.Dequeue();
                trail.SetActive(true); // Activate the trail piece
                trail.transform.position = transform.position + trailOffset; // Position it at the horse's location with offset

                // Start fading and shrinking the trail piece
                StartCoroutine(FadeAndShrinkTrail(trail));

                // Update the time to spawn the next trail piece
                nextTimeToSpawn = Time.time + spawnRate;
            }
        }
    }

    // Coroutine to handle the fading and shrinking of the trail piece
    IEnumerator FadeAndShrinkTrail(GameObject trail)
    {
        Image trailImage = trail.GetComponent<Image>();
        float alpha = 1f; // Start with full opacity

        // Gradually fade out the trail image
        while (alpha > 0)
        {
            alpha -= Time.deltaTime / fadeTime;
            trailImage.color = new Color(trailImage.color.r, trailImage.color.g, trailImage.color.b, alpha);

            // Optionally, reduce the size to simulate shrinking
            trail.transform.localScale *= 0.95f;  // Shrink the trail over time

            yield return null;
        }

        // After the trail fades, deactivate it and return it to the pool
        trail.SetActive(false);
        trailPool.Enqueue(trail); // Return the trail to the pool
    }
}
