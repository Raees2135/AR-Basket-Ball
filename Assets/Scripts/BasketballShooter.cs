using UnityEngine;

public class BasketballShooter : MonoBehaviour
{
    public GameObject basketballPrefab;
    public Transform spawnPoint;
    public LineRenderer trajectoryLineRenderer;
    public float baseShootForce = 100f;
    public int maxLives = 5;
    public Transform basketTransform; // Reference to the basket or hoop

    private GameObject spawnedBasketball;
    private Vector2 startTouchPosition, endTouchPosition;
    private bool isSwipe = false;
    public int currentLives;
    public int score = 0;

    public HoopScore hoopScore;

    void Start()
    {
        currentLives = maxLives;
        trajectoryLineRenderer.enabled = false; // Hide trajectory line initially
    }

    void Update()
    {
        HandleSwipe();
        UpdateTrajectoryLine(); // Update the trajectory line if basketball is ready
    }

    public GameObject SpawnedBasketball
    {
        get { return spawnedBasketball; }
    }

    public void SpawnBasketball()
    {
        if (currentLives > 0 && spawnedBasketball == null)
        {
            spawnedBasketball = Instantiate(basketballPrefab, spawnPoint.position, spawnPoint.rotation);

            Rigidbody rb = spawnedBasketball.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void HandleSwipe()
    {
        if (Input.touchCount > 0 && spawnedBasketball != null)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                isSwipe = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwipe)
            {
                endTouchPosition = touch.position;
                Vector2 swipeDirection = endTouchPosition - startTouchPosition;

                if (swipeDirection.magnitude > 100f)
                {
                    ShootBasketball(swipeDirection);
                }

                isSwipe = false;
            }
        }

        if (Input.GetMouseButtonDown(0) && spawnedBasketball != null)
        {
            startTouchPosition = Input.mousePosition;
            isSwipe = true;
        }
        else if (Input.GetMouseButtonUp(0) && isSwipe)
        {
            endTouchPosition = Input.mousePosition;
            Vector2 swipeDirection = endTouchPosition - startTouchPosition;

            if (swipeDirection.magnitude > 100f)
            {
                ShootBasketball(swipeDirection);
            }

            isSwipe = false;
        }
    }

    // Method to calculate arc velocity for an accurate trajectory
    Vector3 CalculateArcVelocity(Vector3 target, float angle)
    {
        Vector3 dir = target - spawnPoint.position; // Direction to the target
        float h = dir.y; // Height difference
        dir.y = 0; // Horizontal direction only
        float dist = dir.magnitude; // Horizontal distance
        float a = angle * Mathf.Deg2Rad; // Convert angle to radians
        dir.y = dist * Mathf.Tan(a); // Set dir.y based on angle
        dist += h / Mathf.Tan(a); // Adjust distance for height

        // Calculate the velocity magnitude
        float velocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * dir.normalized; // Return velocity vector
    }

    // Visualize the predicted trajectory
    void UpdateTrajectoryLine()
    {
        if (spawnedBasketball != null)
        {
            trajectoryLineRenderer.enabled = true;

            // Calculate swipe strength and apply the same adjustments as in the shooting method
            float swipeStrength = (endTouchPosition - startTouchPosition).magnitude / Screen.height;
            swipeStrength = Mathf.Clamp(swipeStrength, 0.1f, 0.5f);
            swipeStrength = Mathf.Pow(swipeStrength, 2f); // Apply the power curve

            // Calculate the target position
            Vector3 targetPosition = basketTransform.position;

            // Calculate dynamic angle and arc velocity for the trajectory
            float angle = Mathf.Lerp(60f, 35f, swipeStrength); // Adjust angle based on swipe strength
            Vector3 arcVelocity = CalculateArcVelocity(targetPosition, angle);

            // Draw the trajectory line based on the calculated velocity
            DrawTrajectoryLine(spawnPoint.position, arcVelocity * swipeStrength * baseShootForce);
        }
        else
        {
            trajectoryLineRenderer.enabled = false;
        }
    }

    // Draw the trajectory using LineRenderer
    void DrawTrajectoryLine(Vector3 startPoint, Vector3 velocity)
    {
        int lineSegmentCount = 30; // Number of points to draw
        float timeStep = 0.1f; // Time interval between each point
        trajectoryLineRenderer.positionCount = lineSegmentCount;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float time = i * timeStep;
            Vector3 point = startPoint + velocity * time + 0.5f * Physics.gravity * time * time; // Calculate point along the curve
            trajectoryLineRenderer.SetPosition(i, point);
        }
    }

    void ShootBasketball(Vector2 swipeDirection)
    {
        if (spawnedBasketball != null)
        {
            Rigidbody rb = spawnedBasketball.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            // Calculate swipe strength
            float swipeStrength = swipeDirection.magnitude / Screen.height; // Normalize based on screen height for better scaling
            swipeStrength = Mathf.Clamp(swipeStrength, 0.1f, 0.5f); // Lower maximum strength for more realistic throws

            // Apply a power curve to make the strength feel more natural
            swipeStrength = Mathf.Pow(swipeStrength, 2f); // Makes small swipes weaker and long swipes stronger, but with diminishing returns

            // Calculate the target position based on basket position
            Vector3 targetPosition = basketTransform.position;

            // Use a fixed angle for now to prevent overly high throws
            float angle = Mathf.Lerp(60f, 35f, swipeStrength); // Dynamic angle based on strength

            // Calculate arc velocity based on target position and dynamic angle
            Vector3 arcVelocity = CalculateArcVelocity(targetPosition, angle);

            // Apply the velocity scaled by swipe strength
            rb.velocity = arcVelocity * swipeStrength * baseShootForce;

            // Destroy the basketball after 5 seconds
            Destroy(spawnedBasketball, 5f);

            // Invoke the scoring check after 3 seconds
            Invoke("CheckIfScored", 3f);
        }
    }

    void CheckIfScored()
    {
        if (spawnedBasketball == null || !HasScored())
        {
            currentLives--;
            Debug.Log("Missed! Lives left: " + currentLives);
        }

        spawnedBasketball = null; // Allow spawning a new ball

        if (currentLives > 0)
        {
            // Do not automatically spawn a new basketball here
        }
        else
        {
            ResetScore(); // Reset score when lives are finished
        }
    }

    bool HasScored()
    {
        // Implement scoring logic here
        return false;
    }

    void ResetScore()
    {
        score = 0;
        currentLives = maxLives;
        Debug.Log("Score reset to zero. Lives reset to max.");
    }

    public void IncreaseScore()
    {
        score++;
        Debug.Log("Score: " + score);
    }
}
