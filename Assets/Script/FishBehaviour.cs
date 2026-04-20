using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FishBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float minSpeed = 1f;
    public float maxSpeed = 2f;
    private float speed;

    [Header("Hunger")]
    public float hungerLevel = 100f;
    public float hungerDecreaseRate = 5f;

    public float hungerTimer = 0f;
    public bool isOnCooldown = false;
    public float hungerCooldown = 10f;
    private bool isEating = false;

    [Header("Detection")]
    public float detectionRadius = 5f;

    [Header("Fear")]
    bool isFear = false;
    Vector3 fearDirection;
    float fearTimer = 0f;

    public float fleeDuration = 2f;
    public float fleeSpeedMultiplier = 2.5f;

    private Transform targetFood;
    private Vector3 randomTarget;

    private float changeDirTimer; 
    
    Vector3 currentDirection;
    Vector3 targetDirection;

    bool isTurning = false;
    public float turnThreshold = 20f;

    private SpriteRenderer spriteRenderer;

    public bool isDie;
    void Start()
    {
        hungerCooldown = ConfigManager.Instance.config.hungerCooldown;
        minSpeed = ConfigManager.Instance.config.fishMinSpeed;
        maxSpeed = ConfigManager.Instance.config.fishMaxSpeed;
        if (hungerCooldown <= 0)
        {
            hungerCooldown = 10;
        }
        if (minSpeed <= 0)
        {
            minSpeed = 1;
        }
        if (maxSpeed <= 0)
        {
            maxSpeed = 2;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        speed = Random.Range(minSpeed, maxSpeed);

        PickRandomTarget();

        currentDirection = (randomTarget - transform.position).normalized;
        targetDirection = currentDirection;
    }

    void Update()
    {
        if (isDie)
        {
            return;
        }

        if (isFear)
        {
            HandleFear();
            return;
        }

        HandleHunger();

        if (isEating)
        {
            SeekFood();
        }
        else
        {
            Swim();
        }


        UpdateColorByHungerLevel();

        if (isEating && targetFood == null)
        {
            ClearFoodState();
        }
    }

    void HandleHunger()
    {
        if (isOnCooldown)
        {
            hungerTimer += Time.deltaTime;

            if (hungerTimer >= hungerCooldown)
            {
                isOnCooldown = false;
                hungerTimer = 0f;
            }

            return;
        }

        hungerLevel -= hungerDecreaseRate * Time.deltaTime;
        hungerLevel = Mathf.Clamp(hungerLevel, 0, 100);

        if (hungerLevel <= 0 && !isEating)
        {
            FindNearestFood();
        }
    }

    void FindNearestFood()
    {
        Food[] foods = GameObject.FindObjectsOfType<Food>();

        float closest = detectionRadius;
        Transform nearest = null;

        foreach (var f in foods)
        {
            float dist = Vector3.Distance(transform.position, f.transform.position);

            if (dist < closest)
            {
                closest = dist;
                nearest = f.transform;
            }
        }

        if (nearest != null)
        {
            Food food = nearest.GetComponent<Food>();

            if (food.takenBy == null)
            {
                food.takenBy = gameObject;
                targetFood = nearest;
                isEating = true;
            }
        }
    }

    void SeekFood()
    {
        if (targetFood == null || !targetFood.gameObject.activeInHierarchy)
        {
            ClearFoodState();
            return;
        }

        Food food = targetFood.GetComponent<Food>();

        if (food.takenBy != gameObject)
        {
            ClearFoodState();
            return;
        }

        MoveTowards(targetFood.position);

        if (Vector3.Distance(transform.position, targetFood.position) < 0.6f)
        {
            FoodObjectPooling.Instance.RemoveFood(food.foodIndex);

            GameManager.Instance.AddScore(10);
            hungerLevel = 100;

            ClearFoodState();

            isOnCooldown = true;
            hungerTimer = 0f;
        }
    }
    void ClearFoodState()
    {
        if (targetFood != null)
        {
            Food food = targetFood.GetComponent<Food>();

            if (food != null && food.takenBy == gameObject)
                food.takenBy = null;
        }

        targetFood = null;
        isEating = false;
    }
    void Swim()
    {
        changeDirTimer -= Time.deltaTime;

        if (changeDirTimer <= 0)
        {
            PickRandomTarget();
        }

        MoveTowards(randomTarget);
    }

    void PickRandomTarget()
    {
        randomTarget = SpawnManager.Instance.spawnArea.GetRandomPosition();

        Vector3 dir = (randomTarget - transform.position);
        dir.y *= 0.5f;

        targetDirection = dir.normalized;

        changeDirTimer = Random.Range(2f, 5f);
    }
    Vector3 lockedDirection;

    void MoveTowards(Vector3 target)
    {
        Vector3 desiredDirection = (target - transform.position).normalized;

        if (!isTurning)
        {
            float angle = Vector3.Angle(currentDirection, desiredDirection);

            if (angle > turnThreshold)
            {
                isTurning = true;
                lockedDirection = desiredDirection;
            }
            else
            {
                targetDirection = desiredDirection;
            }
        }

        if (isTurning)
        {
            currentDirection = Vector3.Lerp(currentDirection, lockedDirection, 5f * Time.deltaTime);

            RotateToDirection();

            if (Vector3.Angle(currentDirection, lockedDirection) < 3f)
            {
                isTurning = false;
                targetDirection = lockedDirection;
            }

            return;
        }

        currentDirection = Vector3.Lerp(currentDirection, targetDirection, 3f * Time.deltaTime);

        Vector3 newPos = transform.position + currentDirection * speed * Time.deltaTime;

        Vector3 clamped = SpawnManager.Instance.spawnArea.ClampToScreen(newPos);

        bool hitX = newPos.x != clamped.x;
        bool hitY = newPos.y != clamped.y;

        if (hitX || hitY)
        {
            Vector3 dir = currentDirection;

            if (hitX) dir.x *= -1;
            if (hitY) dir.y *= -1;

            isTurning = true;
            lockedDirection = dir.normalized;
        }

        transform.position = clamped;

        RotateToDirection();
    }
    void RotateToDirection()
    {
        if (currentDirection == Vector3.zero) return;

        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0, angle, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 5f * Time.deltaTime);
    }

    public void TriggerFear(Vector3 from)
    {
        isFear = true;
        fearDirection = (transform.position - from).normalized;
        fearTimer = fleeDuration;
        isEating = false;
    }

    void UpdateColorByHungerLevel()
    {
        float normalized = 1f - (hungerLevel / 100f);                                            
        float t = Mathf.Clamp01((10f - hungerLevel) / 10f);
        float hue = 0.33f;
        float saturation = t;
        float value = 1f;
        Color color = Color.HSVToRGB(hue, saturation, value);

        spriteRenderer.color = color;
    }

    void HandleFear()
    {
        fearTimer -= Time.deltaTime;

        float fleeSpeed = speed * fleeSpeedMultiplier;
        Vector3 newPos = transform.position + fearDirection * fleeSpeed * Time.deltaTime;
        Vector3 clamped = SpawnManager.Instance.spawnArea.ClampToScreen(newPos);
        if (newPos != clamped)
        {
            Vector3 dir = fearDirection;

            if (newPos.x != clamped.x) dir.x *= -1;
            if (newPos.y != clamped.y) dir.y *= -1;

            fearDirection = dir.normalized;
        }

        transform.position = clamped;

        currentDirection = Vector3.Lerp(currentDirection, fearDirection, 6f * Time.deltaTime);
        RotateToDirection();
        spriteRenderer.color = Color.red;

        if (fearTimer <= 0f)
        {
            spriteRenderer.color = Color.white;
            isFear = false;
            PickRandomTarget();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<TrashBehaviour>())
        {
            if (hungerLevel <= 0)
            {
                isDie = true;
                StartCoroutine(DeathRoutine());
            }
        }
    }

    IEnumerator DeathRoutine()
    {
        spriteRenderer.color = Color.black;
        float duration = 1f;
        float t = 0f;

        bool startFlip = spriteRenderer.flipY;
        bool endFlip = !startFlip;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            if (lerp > 0.5f)
                spriteRenderer.flipX = endFlip;

            yield return null;
        }
        SpawnManager.Instance.RemoveFish(this.gameObject);
    }
}