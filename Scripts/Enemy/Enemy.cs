using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class Enemy : MonoBehaviour
{
    [Header("--- Stats ---")]
    public float health;
    public float maxHealth;
    public float attackRange;
    [SerializeField] private float patrolSpeed;
    public float chaseSpeed;
    [SerializeField] private float size;
    public int subLevelID;


    [Header("--- Weapon ---")]
    public int currWeaponId;
    public Weapon weapon;
    [SerializeField] private List<GameObject> weaponPrefab;
    [SerializeField] private List<GameObject> weaponPickUpPrefab;
    [SerializeField] private Transform weaponSlot;
    private bool isLoadingChamber;


    [Header("--- Grab Interaction ---")]
    public bool isGrabbed;
    public GameObject grabPoint;
    public CircleCollider2D enemyCollider;


    [Header("--- Physics ---")]
    private Rigidbody2D rb;
    private Vector2 visionPoint;
    private Vector3 visionPoint3;


    [Header("--- Pathfinding ---")]
    public NavMeshAgent agent;
    public LayerMask layerMask;

    [Header("--- Chasing Player ---")]
    public float chaseLetDownLength;
    public float chaseLetDownTimer;
    Transform playerTransform;

    [Header("--- Detection ---")]
    [SerializeField] private float detectionConeRange;
    [SerializeField] private float detectionConeAngle;
    [SerializeField] private float detectionCircleRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionTimeToAlert;
    [SerializeField] private float detectionTimeToAlertTimer;
    [SerializeField] private float blindAngle = 120f;

    [Header("--- Patroling ---")]
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private Transform currPatrolPoint;
    [SerializeField] private int currPatrolPointIndex = 0;
    private PatrolPoint currentPatrolPointScript;
    private bool isWaitingAtPatrolPoint = false;
    private float waitTimer = 0.0f;

    public List<Transform> tempPatrolPoints;
    private bool tempPatrol;

    [Header("--- Room ---")]
    public Room currRoom = null;


    [Header("--- States ---")]
    public bool isPatroling;
    public bool isSearching;
    public bool isChasingPlayer;
    public bool isAttacking;

    [Header("--- Gameplay Effects ---")]
    public bool isStunned = false;
    public bool isPunched = false;
    [SerializeField] private GameObject upperBody;

    [Header("--- Visual Effects ---")]
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject dummyParticle;
    [SerializeField] private GameObject bloodDeath;

    [Header("--- Body ---")]
    [SerializeField] private GameObject torsoObject;
    [SerializeField] private GameObject legsObject;
    public Animator legsAnimator;
    private HandSkinManger handSkinManager;

    [Header("--- Corpse ---")]
    [SerializeField] private GameObject deadBody;
    [SerializeField] private GameObject headObject;
    [SerializeField] private int headID;
    [SerializeField] private int torsoID;

    [Header("--- Testing ---")]
    public bool isDeactive;
    public bool isDummy;
    [SerializeField] private bool isKatanaDummy;
    [SerializeField] private bool debugDraw;

    [SerializeField] private bool isBodyDeactive = false;

    [Header("--- Dummy ---")]
    public bool wasPunched = false;
    public bool wasKicked = false;
    public bool wasSliced = false;
    //public float axisZRotInit = 0.0f;

    [Header("--- BossFight ---")]
    public bool isAllknowing = false;

    [Header("--- VisualsSpecific")]
    public List<int> possibleTorsos;


    [Header("--- References ---")]
    private CorpseManager corpseManager;



    protected virtual void Start()
    {
        maxHealth = health;

        if (!isDummy) weapon = Instantiate(weaponPrefab[currWeaponId], weaponSlot).GetComponent<Weapon>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<CircleCollider2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        isLoadingChamber = false;

        agent.speed = patrolSpeed;
        transform.localScale *= size;

        if (patrolPoints.Count > 0) {
            currPatrolPoint = patrolPoints[currPatrolPointIndex];
            currentPatrolPointScript = currPatrolPoint.GetComponent<PatrolPoint>(); 
        }
        
        

        if (!isDummy) {
            handSkinManager = GetComponent<HandSkinManger>();
            handSkinManager.InitHandsParts(weapon);
            torsoID = torsoObject.GetComponent<PickBodyPart>().SetTorsoSpecifiedEnemy(possibleTorsos[Random.Range(0, possibleTorsos.Count)]);
            headID = headObject.GetComponent<PickBodyPart>().SetHead();

            legsObject = transform.Find("Legs").gameObject;
            legsAnimator = legsObject.GetComponent<Animator>();
        }


        if (isKatanaDummy) {
            weapon.GetComponent<MeleeWeapon>().damage = 0;
        }

        tempPatrolPoints = new List<Transform>();
        tempPatrol = false;

        corpseManager = GetComponent<CorpseManager>();
    }

    protected virtual void Update()
    {

        if (isDeactive && !isBodyDeactive) HideEnemy(true);
        else if (!isDeactive && isBodyDeactive) HideEnemy(false);


        {
            if (isGrabbed) { enemyCollider.enabled = false; transform.position = grabPoint.transform.position; }
            else { enemyCollider.enabled = true; }
            if (isStunned || isPunched || isDeactive || isGrabbed || isDummy) return;
            enemyCollider.enabled = true;
        }

        if (isAllknowing) {
            detectionTimeToAlertTimer = detectionTimeToAlert;
            chaseLetDownTimer = chaseLetDownLength;

            agent.speed = chaseSpeed;

            ChasePlayer();
            CheckForDetection();
            LegsAnim();
            return;
        }

        if (isPatroling)
        {
            isChasingPlayer = false;
            isAttacking = false;
            Patrol();
            ScanForward();
            ScanAround();
            DetectionTimer();

          
        }
        else if (isChasingPlayer)
        {
            isPatroling = false;
            isSearching = false;
            CheckForDetection();
            ChaseTimer();

        
        }

        LegsAnim();
    }


    public void LegsAnim() 
    {
        {
            if (!agent.isStopped)
            {
                Vector2 moveDirection = agent.velocity.normalized;
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

                if (agent.velocity.magnitude > 0.1f)
                {
                    if (isWaitingAtPatrolPoint)
                    {

                        legsAnimator.SetBool("IsStepping", false);
                        legsAnimator.SetBool("IsWalking", false);
                    }
                    else if (isPatroling)
                    {
                        legsAnimator.SetBool("IsWalking", false);
                        legsAnimator.SetBool("IsStepping", true);
                    }
                    else
                    {
                        legsAnimator.SetBool("IsStepping", false);
                        legsAnimator.SetBool("IsWalking", true);
                    }
                }
                else
                {
                    legsAnimator.SetBool("IsStepping", false);
                    legsAnimator.SetBool("IsWalking", false);
                }
            }
            else
            {
                legsAnimator.SetBool("IsStepping", false);
                legsAnimator.SetBool("IsWalking", false);


            }

            visionPoint = transform.position;
            visionPoint3 = transform.position;
        }

    }

    private void SetChildEnabledRecursive(Transform parent, bool enabled)
    {
        foreach (Transform child in parent)
        {
            SetChildEnabledRecursive(child, enabled); // Recursively call this function for each child
            SpriteRenderer renderer = child.gameObject.GetComponent<SpriteRenderer>();
            if (renderer != null) {
                renderer.enabled = enabled;
            }

            LineRenderer lineRenderer = child.gameObject.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.enabled = enabled;
            }
        }
    }

    private void HideEnemy(bool on)
    {
        enemyCollider.enabled = !on;

        Light2D light = GetComponent<Light2D>();
        if (light != null) {
            light.enabled = !on;
        }

        SetChildEnabledRecursive(transform, !on); // Start recursion from the top-level transform
        isBodyDeactive = on;
    }

    #region Patrol

    private void Patrol()
    {

        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance && !isWaitingAtPatrolPoint)
        {
            SetWaitPatrolTimer();
        }
        else if (isWaitingAtPatrolPoint) {

            if (tempPatrolPoints.Count > 0) {
                waitTimer = 0f;
            }

            waitTimer -= Time.deltaTime * 1f;

            if (waitTimer <= 0) {
                isWaitingAtPatrolPoint = false;
                agent.isStopped = false;
                SetNextPatrolPoint();
            }
        }
        else
        {
            agent.SetDestination(currPatrolPoint.position);
        }

        if (tempPatrol)
        {
            agent.speed = patrolSpeed * 2;
            isSearching = true;
        }
        else {
            agent.speed = patrolSpeed;
            isSearching = false;
        }
    }

    private void SetWaitPatrolTimer() {
        
        

       
        isWaitingAtPatrolPoint = true;
        agent.isStopped = true;

        if (tempPatrol)
        {
            waitTimer = 0f;
        }
        else {
            waitTimer = currentPatrolPointScript.waitAtPoint;
        }
      
       
    }


    private void SetNextPatrolPoint()
    {
        if (tempPatrolPoints.Count > 0 && tempPatrol)
        {

            currPatrolPoint.GetComponentInParent<Room>().roomState = RoomState.Empty;
            tempPatrolPoints.RemoveAt(0);

            if (tempPatrolPoints.Count > 0)
            {
                currPatrolPoint = tempPatrolPoints[0];
                currentPatrolPointScript = currPatrolPoint.GetComponent<PatrolPoint>();
                agent.destination = currPatrolPoint.position;
            }
            else{
                SetNextPatrolPoint();
            }
        }
        else if (tempPatrolPoints.Count > 0) 
        {
            tempPatrol = true;
            currPatrolPoint = tempPatrolPoints[0];
            currentPatrolPointScript = currPatrolPoint.GetComponent<PatrolPoint>();
            agent.destination = currPatrolPoint.position;
        }
        else
        { 
            currPatrolPointIndex = (currPatrolPointIndex + 1) % patrolPoints.Count;
            currPatrolPoint = patrolPoints[currPatrolPointIndex];
            currentPatrolPointScript = currPatrolPoint.GetComponent<PatrolPoint>();
            agent.destination = currPatrolPoint.position;
            tempPatrol = false;
        }
    }

    public virtual void EnterSearchMode() {
        if (isAllknowing || isDummy) return;

        isWaitingAtPatrolPoint = false;
        waitTimer = 0f;

        tempPatrol = true;
        currPatrolPoint = tempPatrolPoints[0];
    }

    #endregion

    #region Detection

    private void ScanForward()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(visionPoint, detectionConeRange, playerLayer);

        foreach (Collider2D hit in hits)
        {
            Vector2 directionToTarget = hit.transform.position - visionPoint3;
            float angle = Vector2.Angle(transform.up, directionToTarget);

            if (angle < detectionConeAngle / 2f)
            {


                gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                RaycastHit2D hitInfo = Physics2D.Raycast(visionPoint, directionToTarget, detectionConeRange, layerMask);
                gameObject.layer = LayerMask.NameToLayer("Enemy");

                if (hitInfo.collider != null && hitInfo.collider == hit)
                {
                    // Player is actually visible
                    Debug.Log("Player Detected!");
                    detectionTimeToAlertTimer += Time.deltaTime * 2f;
                }
                else
                {
                    Debug.Log("Something else obstructs the view: " + hitInfo.collider.name);

                    // Something else obstructs the view
                    detectionTimeToAlertTimer -= Time.deltaTime * 1f;
                }
            }
            else
            {
                // Not in range
                detectionTimeToAlertTimer -= Time.deltaTime * 1f;
            }
        }

        DebugDrawDetectionCone(visionPoint, transform.up, detectionConeRange, detectionConeAngle);
    }


    private void ScanAround()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(visionPoint, detectionCircleRange, playerLayer);

        if (hits.Length == 0)
        {
            detectionTimeToAlertTimer -= Time.deltaTime * 1f;
        }
        else
        {
            foreach (Collider2D hit in hits)
            {
                Vector2 directionToTarget = ((Vector2)hit.transform.position - (Vector2)visionPoint).normalized;

                // Rotate the direction vector by the inverse of the enemy's rotation
                float angleToTarget = Vector2.SignedAngle(transform.up, directionToTarget);

                // Check if the player is within a certain angle range behind the enemy
                if (angleToTarget > -blindAngle && angleToTarget < blindAngle)
                {
                    gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    RaycastHit2D hitInfo = Physics2D.Raycast(visionPoint, directionToTarget, detectionCircleRange, layerMask);
                    gameObject.layer = LayerMask.NameToLayer("Enemy");

                    if (hitInfo.collider != null && hitInfo.collider == hit)
                    {
                        // Player is actually visible
                        //Debug.Log("Player Detected!");
                        detectionTimeToAlertTimer += Time.deltaTime * 4f;
                    }
                    else
                    {
                        // Something else obstructs the view
                        detectionTimeToAlertTimer -= Time.deltaTime * 1f;
                    }
                }
            }
        }

        DebugDrawDetectionCircle();
    }



    private void DetectionTimer()
    {
        detectionTimeToAlertTimer = Mathf.Clamp(detectionTimeToAlertTimer, 0f, detectionTimeToAlert);

        if (detectionTimeToAlertTimer == detectionTimeToAlert)
        {
            isChasingPlayer = true;
            isPatroling = false;

            agent.speed = chaseSpeed;
            chaseLetDownTimer = chaseLetDownLength;
        }
    }

    #endregion

    #region Chase
    public void ChasePlayer()
    {
        //if (agent.hasPath) return;
        isAttacking = false;
        agent.SetDestination(playerTransform.position);
        agent.isStopped = false;
    }


    public void CheckForDetection()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        // Calculate the angle between the forward direction of the enemy and the direction to the player
        float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Create a rotation quaternion around the z-axis
        Quaternion lookRotation = Quaternion.Euler(0f, 0f, angleToPlayer - 90);

        // Smoothly rotate the enemy towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);



        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < 5) {

            // Player detected
            chaseLetDownTimer = chaseLetDownLength;
            CheckForAttack();
            return;
        }

        Vector2 directionToTarget = playerTransform.position - visionPoint3;

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        int layerMaskWithoutFauna;
        if (chaseLetDownTimer >= chaseLetDownLength)
        {
            layerMaskWithoutFauna = layerMask & ~(1 << LayerMask.NameToLayer("Fauna"));
        }
        else {
            layerMaskWithoutFauna = layerMask;
        }

       

        RaycastHit2D hitInfo = Physics2D.Raycast(visionPoint, directionToTarget, distanceToPlayer, layerMaskWithoutFauna);

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        if (hitInfo.collider != null && hitInfo.collider.gameObject == playerTransform.gameObject)
        {
            // Player detected
            chaseLetDownTimer = chaseLetDownLength;
            Debug.DrawLine(visionPoint, hitInfo.point, Color.green); // Draw raycast

            CheckForAttack();
        }
        else
        {
            // Player not detected

            chaseLetDownTimer -= Time.deltaTime * 1f;
         

            Debug.DrawRay(visionPoint, directionToTarget.normalized * distanceToPlayer, Color.gray); // Draw raycast

            ChasePlayer();
        }
    }

    private void CheckForAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;

            // Calculate the angle between the forward direction of the enemy and the direction to the player
            float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            // Create a rotation quaternion around the z-axis
            Quaternion lookRotation = Quaternion.Euler(0f, 0f, angleToPlayer - 90);

            // Smoothly rotate the enemy towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);

            agent.isStopped = true;
            isAttacking = true;
            // Attack the player
            FireWeapon();
           
        }
        else
        {
            
            ChasePlayer();
        }
    }

    private void ChaseTimer()
    {
        if (chaseLetDownTimer <= 0)
        {
            isChasingPlayer = false;
            isPatroling = true;

            agent.speed = patrolSpeed;
            agent.isStopped = false;
        }
    }

    #endregion

    #region Attack
    private void FireWeapon()
    {
        if (currWeaponId == 6 || currWeaponId == 7)
        {
            weapon.gameObject.GetComponent<MeleeWeapon>().Slash(true);
        }
        if (weapon.magCurrent == 0 && currWeaponId == 5)
        {
            weapon.GetComponent<ShortyWeapon>().Reload();
        }
        else if (weapon.magCurrent == 0)
        {
            weapon.Reload();
        }
        else if (isLoadingChamber)
        {
            weapon.Reload();
            isLoadingChamber = true;

            if (weapon.magCurrent == weapon.magCapacity)
            {
                isLoadingChamber = false;
            }
        }
        else if (currWeaponId == 1)
        {
            weapon.GetComponent<ShotgunWeapon>().FireShotgun(true);
        }
        else if (currWeaponId == 5)
        {
            weapon.GetComponent<ShortyWeapon>().FireShorty(1, true);
        }
        else
        {
            weapon.Fire(true);
        }

    }
    #endregion

    #region Health
    private void CheckHealth()
    {
        if (isDummy || isDeactive) return;
        if (health <= 0f)
        {
            Death();
        }
    }
    private void Death()
    {
        if (weaponPickUpPrefab[currWeaponId] != null)
        {
            Instantiate(weaponPickUpPrefab[currWeaponId], (transform.position - (Vector3.up * 2)), Quaternion.identity);
            GameObject bloodObject = Instantiate(bloodDeath, transform.position, Quaternion.identity);
            weaponPickUpPrefab[currWeaponId] = null;
        }

        GameObject.FindWithTag("SubLevelProgres").GetComponent<SubLevelProgres>().SetEnemyDeath(subLevelID);

        GameObject body = Instantiate(deadBody, transform.position, Quaternion.identity);
        body.GetComponent<DeadBody>().SetDamageCounts(corpseManager.GetHeavyCount(), corpseManager.GetLightCount(), corpseManager.GetLimbChance());
        print(corpseManager.GetLightCount());
        body.GetComponent<DeadBody>().InitDeadBody(torsoID, headID);

        if (gameObject!=null)
            Destroy(gameObject);
    }
    private void LateUpdate()
    {
        CheckHealth();
    }

    #endregion

    #region Collisions
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("StaticObject")) {
            Door door = other.GetComponent<Door>();

            if (door != null)
            {
                if (!door.isOpened) {
                    door.ToggleDoor();

                    if (isPatroling) {
                        door.PatrolClose();
                    }
                }

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MeleeWeapon"))
        {
            collision.collider.GetComponentInParent<MeleeWeapon>().PlaySoundHitEffect();
            if (isDummy)
            {
                wasSliced = true;
            }
            else
            {
                if (weapon.isMelee && weapon.isAttacking)
                {
                    return;
                }
            }

            if (isPatroling && !isChasingPlayer)
            {
                Takedown(collision.collider.transform);
            }
            else {
                TakeDamage(collision.collider.GetComponentInParent<MeleeWeapon>().damage, collision.collider.GetComponentInParent<MeleeWeapon>().forceSeconds, collision.collider.transform, false, DamageType.Slash);
            }


        }
    }
    #endregion

    #region CallableFromOutside
    public void SetSubLevelId(int ID)
    {
        subLevelID = ID;
    }

    public void SetGrabPoint(GameObject inputPoint)
    {
        grabPoint = inputPoint;
    }

    public void DetachFromGrabPoint()
    {
        if (grabPoint != null)
        {
            grabPoint.GetComponent<FistsWeapon>().ReleaseObject();
            grabPoint = null;
            enemyCollider.enabled = true;
        }
    }

    public void TakePush(Vector2 weaponPosition, float force, float duration)
    {
        isPunched = true;
        // Stop the NavMeshAgent
        agent.isStopped = true;

        // Calculate the direction vector towards the weapon's position
        Vector2 direction = (weaponPosition - (Vector2)transform.position).normalized;

        // Invert the direction vector
        direction *= -1;

        // Apply force using Rigidbody2D.velocity
        rb.velocity = direction * force;

        // Stop applying the force after a specified duration
        StartCoroutine(StopPush(duration));
    }

    public void TakeStun(float duration)
    {
        if (isStunned) return;

        Vector2 kickBackDirection = -transform.forward;

        StartCoroutine(Stunned(duration));
        StartCoroutine(StopMovement(0.25f));
    }

    public void TakeStun(float duration, float kickForce)
    {
        if (isStunned) return;
        StartCoroutine(Stunned(duration));
    }

    public void TakeDamage(float dmg, float forceSecs, Transform weaponPosition, bool isShotgun, DamageType damageType)
    {
        if (isDummy || isKatanaDummy)
        {
            GameObject bloodObject = Instantiate(dummyParticle, transform.position, Quaternion.identity);
            return;
        }
        else
        {
            GameObject bloodObject = Instantiate(blood, transform.position, Quaternion.identity);
            bloodObject.GetComponent<BloodDestructor>().RotateSelf(isShotgun);
        }
        if (forceSecs != 0f)
        {
            StartCoroutine(StopMovement(forceSecs));
        }


        health -= dmg;

        detectionTimeToAlertTimer = detectionTimeToAlert * 2f;

        corpseManager.CountDamage(damageType);

        if (currRoom != null) {
            currRoom.AlertRoom();
        }
    }

    public void TakeGranade(float dmg, float forceSecs, Transform weaponPosition)
    {
        if (isDummy || isKatanaDummy)
        {
            GameObject bloodObject = Instantiate(dummyParticle, transform.position, Quaternion.identity);
            return;
        }
        else
        {
            GameObject bloodObject = Instantiate(blood, transform.position, Quaternion.identity);
            bloodObject.GetComponent<BloodDestructor>().RotateSelf(weaponPosition.position);
        }
        if (forceSecs != 0f)
        {
            StartCoroutine(StopMovement(forceSecs));
        }


        health -= dmg;

        detectionTimeToAlertTimer = detectionTimeToAlert * 2f;

        if (currRoom != null)
        {
            currRoom.AlertRoom();
        }
    }


    public void Takedown(Transform weaponPosition)
    {
        if (isDummy || isKatanaDummy)
        {
            GameObject bloodObject = Instantiate(dummyParticle, transform.position, Quaternion.identity);
            return;
        }
        else
        {
            GameObject bloodObject = Instantiate(blood, transform.position, Quaternion.identity);
            bloodObject.GetComponent<BloodDestructor>().RotateSelf(false);
        }

        health = -1;
    }
    #endregion

    #region GroupInteraction

    public void GetAlerted() {
        detectionTimeToAlertTimer = detectionTimeToAlert * 2;
    }

    public void Dissalert() {
        tempPatrol = false;
        isChasingPlayer = false;
        isPatroling = true;
        agent.speed = patrolSpeed;
        detectionTimeToAlertTimer = 0f;
        chaseLetDownTimer = 0f;
        tempPatrolPoints.Clear();
    }

    #endregion

    #region Coroutines

    private IEnumerator Stunned(float duration)
    {
        isStunned = true;

        agent.isStopped = true;

        for (int i = 0; i < upperBody.transform.childCount; i++)
        {
            GameObject child = upperBody.transform.GetChild(i).gameObject;
            child.GetComponent<SpriteRenderer>().color = Color.magenta;
        }

        yield return new WaitForSeconds(duration);

        isStunned = false;

        agent.isStopped = false;

        for (int i = 0; i < upperBody.transform.childCount; i++)
        {
            GameObject child = upperBody.transform.GetChild(i).gameObject;
            child.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    public IEnumerator StopMovement(float wait)
    {
        yield return new WaitForSeconds(wait);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
    }
    private IEnumerator StopPush(float duration)
    {
        yield return new WaitForSeconds(duration);

        isPunched = false;
        rb.velocity = Vector2.zero;
        agent.isStopped = false;
    }
    #endregion

    #region Debug
    private void DebugDrawDetectionCone(Vector2 origin, Vector2 direction, float range, float angle)
    {
        if (!debugDraw) return;

        float halfAngle = angle / 2f;
        Vector2 leftRayDirection = Quaternion.Euler(0, 0, -halfAngle) * direction;
        Vector2 rightRayDirection = Quaternion.Euler(0, 0, halfAngle) * direction;

        Vector2 topPoint = origin + direction * range;

        Debug.DrawRay(origin, direction * range, Color.red);
        Debug.DrawRay(origin, leftRayDirection * range, Color.red);
        Debug.DrawRay(origin, rightRayDirection * range, Color.red);

        Debug.DrawLine(origin, origin + direction * range, Color.red);
        Debug.DrawLine(origin, origin + leftRayDirection * range, Color.red);
        Debug.DrawLine(origin, origin + rightRayDirection * range, Color.red);

        Debug.DrawLine(topPoint, origin + leftRayDirection * range, Color.red);
        Debug.DrawLine(topPoint, origin + rightRayDirection * range, Color.red);

        Vector2 upRayDirection = Quaternion.Euler(0, 0, -halfAngle / 2f) * direction;
        Vector2 downRayDirection = Quaternion.Euler(0, 0, halfAngle / 2f) * direction;

        Debug.DrawRay(origin, upRayDirection * range, Color.red);
        Debug.DrawRay(origin, downRayDirection * range, Color.red);

        Vector2 upPoint = origin + upRayDirection * range;
        Vector2 downPoint = origin + downRayDirection * range;

        Debug.DrawLine(topPoint, upPoint, Color.red);
        Debug.DrawLine(topPoint, downPoint, Color.red);

        Debug.DrawLine(origin + leftRayDirection * range, upPoint, Color.red);
        Debug.DrawLine(origin + leftRayDirection * range, downPoint, Color.red);
        Debug.DrawLine(origin + rightRayDirection * range, upPoint, Color.red);
        Debug.DrawLine(origin + rightRayDirection * range, downPoint, Color.red);
    }

    private void DebugDrawDetectionCircle()
    {
        if (!debugDraw) return;
        int linesCount = 124;
        float angleIncrement = 360f / linesCount;

        for (int i = 0; i < linesCount; i++)
        {
            float angle = i * angleIncrement;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * -transform.up; // Direction facing down (-transform.up)
            Vector2 startPoint = visionPoint;
            Vector2 endPoint = startPoint + direction * detectionCircleRange;

            // Calculate the angle between the direction and the forward direction of the enemy
            float angleToDirection = Vector2.SignedAngle(transform.up, direction);

            // Check if the angle falls within the blind spot cone
            if (angleToDirection > -blindAngle && angleToDirection < blindAngle)
            {
                // Draw the blind spot lines in a different color
                Debug.DrawLine(startPoint, endPoint, Color.red);
            }
            else
            {
                // Draw the other lines in the default color
                Debug.DrawLine(startPoint, endPoint, Color.magenta);
            }
        }
    }

        #endregion

    }
