﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

/// <summary>
/// FoodController for Slither - AR codelab.
/// </summary>
/// <remarks>
/// See the codelab for the complete narrative of what
/// this class does.
/// </remarks>
public class FoodController : MonoBehaviour
{
    // Plane to spawn the food objects on.
    private DetectedPlane detectedPlane;
    // The current food instance or null.
    private GameObject foodInstance;
    // Age in seconds of the food instance.
    private float foodAge;
    // Max age of a food before destroying.
    private readonly float maxAge = 10f;
    // Array of models to use when create a food instance.
    public GameObject[] foodModels;

    // Update is called once per frame
    void Update()
    {
        if (detectedPlane == null)
        {
            return;
        }

        if (detectedPlane.TrackingState != TrackingState.Tracking)
        {
            return;
        }

        // Check for the plane being subsumed
        // If the plane has been subsumed switch attachment to the subsuming plane.
        while (detectedPlane.SubsumedBy != null)
        {
            detectedPlane = detectedPlane.SubsumedBy;
        }

        if (foodInstance == null || foodInstance.activeSelf == false)
        {
            SpawnFoodInstance();
            return;
        }

        // Increment the age and destroy if expired.
        foodAge += Time.deltaTime;
        if (foodAge >= maxAge)
        {
            DestroyObject(foodInstance);
            foodInstance = null;
        }
    }

    /// <summary>
    /// Spawns the food instance.
    /// </summary>
    private void SpawnFoodInstance()
    {
        GameObject foodItem = foodModels[Random.Range(0, foodModels.Length)];

        // Pick a location.  This is done by selecting a vertex at random and then
        // a random point between it and the center of the plane.
        List<Vector3> vertices = new List<Vector3>();
        detectedPlane.GetBoundaryPolygon(vertices);
        Vector3 pt = vertices[Random.Range(0, vertices.Count)];
        float dist = Random.Range(0.05f, 1f);
        Vector3 position = Vector3.Lerp(pt, detectedPlane.CenterPose.position, dist);
        // Move the object above the plane.
        position.y += .05f;

        // Create an ARCore anchor for this position.
        Anchor anchor = detectedPlane.CreateAnchor(new Pose(position, Quaternion.identity));

        // Create the instance.
        foodInstance = Instantiate(foodItem, position, Quaternion.identity, anchor.transform);

        // Set the tag - IMPORTANT: make sure the tag is defined in the Tag Editor.
        foodInstance.tag = "food";

        foodInstance.transform.localScale = new Vector3(.025f, .025f, .025f);
        foodInstance.transform.SetParent(anchor.transform);
        foodAge = 0;

        foodInstance.AddComponent<FoodMotion>();
    }

    public void SetSelectedPlane(DetectedPlane selectedPlane)
    {
        detectedPlane = selectedPlane;
    }
}