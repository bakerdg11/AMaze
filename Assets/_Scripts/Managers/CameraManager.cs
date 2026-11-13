using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField, UnityEngine.Range(30f, 90f)] private float cameraAngle = 60f;
    [SerializeField, UnityEngine.Range(20f, 80f)] private float cameraFieldOfView = 45f;
    [SerializeField, UnityEngine.Range(0f, 0.4f)] private float verticalScreenPadding = 0.15f;
    [SerializeField, UnityEngine.Range(0f, 0.4f)] private float horizontalScreenPadding = 0.05f;

    public void AdjustCameraToLevel(GameObject level)
    {
        if (mainCamera == null || level == null) return;

        Renderer[] renderers = level.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found on the current level!");
            return;
        }

        Bounds levelBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            levelBounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 center = levelBounds.center;

        // Set up perspective camera
        mainCamera.orthographic = false;
        mainCamera.fieldOfView = cameraFieldOfView;
        mainCamera.transform.rotation = Quaternion.Euler(cameraAngle, 0f, 0f);

        // Calculate usable screen space
        float usableHeight = 1f - (verticalScreenPadding * 2f);
        float usableWidth = 1f - (horizontalScreenPadding * 2f);

        // Calculate camera field of view
        float verticalFOV = mainCamera.fieldOfView * Mathf.Deg2Rad;
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(verticalFOV / 2f) * mainCamera.aspect);

        // Calculate the distance required to fit the entire level
        float requiredDistance = 0f;

        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 corner = center + Vector3.Scale(levelBounds.extents, new Vector3(x, y, z));

                    Vector3 relativeCorner = corner - center;

                    Vector3 cameraSpaceCorner = Quaternion.Inverse(mainCamera.transform.rotation) * relativeCorner;

                    float verticalDistance = Mathf.Abs(cameraSpaceCorner.y) / (Mathf.Tan(verticalFOV / 2f) * usableHeight);
                    float horizontalDistance = Mathf.Abs(cameraSpaceCorner.x) / (Mathf.Tan(horizontalFOV / 2f) * usableWidth);

                    float cornerDistance = Mathf.Max(verticalDistance, horizontalDistance) + cameraSpaceCorner.z;

                    requiredDistance = Mathf.Max(requiredDistance, cornerDistance);
                }
            }
        }

        // Position camera so the entire maze is visible
        requiredDistance = Mathf.Max(requiredDistance, mainCamera.nearClipPlane + levelBounds.extents.magnitude + 0.1f);

        mainCamera.transform.position = center - mainCamera.transform.forward * requiredDistance;

        Debug.Log($"Camera adjusted to fit level. Distance: {requiredDistance}");
    }
}