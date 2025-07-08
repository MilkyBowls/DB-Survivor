using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Data")]
    public PlayerCharacterData characterData;
    private CharacterTransformation currentTransformation;
    private int currentTransformationIndex = -1; // -1 = base form

    [Header("Visuals & Components")]
    public Transform visualTransform;
    public Transform enemyTargetPoint;
    public Transform auraVisual;
    public Transform saibamanLatchPoint;
    public AuraController auraController;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private PlayerControls controls;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 visualBasePosition;
    private CameraFollow cameraFollow;

    public bool isLatched = false;
    public bool isCharging = false;
    public bool isDead { get; private set; } = false;
    private bool isTransforming = false;
    private bool isFacingLeft = false;

    [Header("Ki System")]
    public float currentKi;
    private float kiRegenTimer = 0f;

    [Header("Health System")]
    public int currentHealth;

    [Header("UI")]
    public Slider kiBar;
    public Slider healthBar;
    public TextMeshProUGUI kiBarText;
    public TextMeshProUGUI healthBarText;

    [Header("Magnet Settings")]
    public float baseMagnetRadius = 2f;
    public float maxMagnetRadius = 8f;
    public float radiusGrowSpeed = 5f;
    private float currentMagnetRadius = 0f;

    private float moveSpeed;
    private float damage;
    private int kiRegenRate;
    private int maxKi;
    private int maxHealth;

    [Header("Weapon")]
    public Transform firePoint;

    [Header("Collectibles")]
    public LayerMask collectibleLayer;

    [Header("Dash Settings")]
    public float baseDashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public bool allowDashInvincibility = true;
    public AnimationCurve dashAccelerationCurve = AnimationCurve.Linear(0, 1, 1, 1);

    private bool isDashing = false;
    private bool canDash = true;
    private Coroutine dashRoutine;

    [Header("Afterimage Effect")]
    public GameObject afterimagePrefab;
    public float afterimageInterval = 0.05f;
    public int afterimageCount = 6;

    [Header("Movement Dynamics")]
    public float acceleration = 20f;
    public float deceleration = 30f;

    // Input buffering
    private float dashBufferTime = 0.15f;
    private float dashBufferTimer = 0f;

    [Header("Melee Settings")]
    public float meleeRange = 1.5f;
    public float meleeCooldown = 0.75f;
    public LayerMask enemyLayer;
    private float meleeCooldownTimer = 0f;
    private bool isMeleeAttacking = false;

    [SerializeField] private int maxComboCount = 3;
    [SerializeField] private float comboResetTime = 1.2f;
    private int currentComboIndex = 0;
    private float comboTimer = 0f;
    private bool isInCombo = false;
    private bool bufferedAttack = false;

    [Header("Hit Effects")]
    public GameObject hitEffectPrefab;

    [Header("Finisher Effects")]
    public GameObject finisherTrailEffect;
    public GameObject finisherImpactFlash;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        controls.Player.Move.performed += ctx => { if (!isCharging) moveInput = ctx.ReadValue<Vector2>(); };
        controls.Player.Move.canceled += ctx => { if (!isCharging) moveInput = Vector2.zero; };

        controls.Player.Charge.performed += ctx => StartCharge();
        controls.Player.Charge.canceled += ctx => StopCharge();

        controls.Player.DeTransform.performed += OnDeTransform;
        controls.Player.Transform.performed += ctx => TryTransform();

        controls.Player.MaxTransform.performed += ctx => MaxTransform();
        controls.Player.BaseForm.performed += ctx => ForceRevertToBaseForm();

        controls.Player.Dash.performed += ctx => TryDash();

        controls.Player.Attack.performed += ctx => TryManualAttack();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (!isCharging)
            moveInput = ctx.ReadValue<Vector2>();
    }

    void Start()
    {
        if (characterData == null)
        {
            Debug.LogError("No PlayerCharacterData assigned!");
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        animator = visualTransform.GetComponent<Animator>();
        spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
        visualBasePosition = visualTransform.localPosition;
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        moveSpeed = characterData.movementSpeed;
        damage = characterData.damage;
        maxHealth = (int)characterData.maxHealth;
        currentHealth = maxHealth;
        maxKi = (int)characterData.maxKiCapacity;
        currentKi = maxKi;
        kiRegenRate = (int)characterData.kiRegenRate;

        animator.runtimeAnimatorController = characterData.animatorController;

        if (characterData.startingWeaponPrefab != null)
        {
            Instantiate(characterData.startingWeaponPrefab, transform.position, Quaternion.identity, transform);
        }

        currentMagnetRadius = baseMagnetRadius;
        UpdateUI();
    }

    void Update()
    {
        if (!isDead && !isTransforming)
        {
            if (isCharging && currentKi < maxKi)
            {
                kiRegenTimer += Time.deltaTime;
                if (kiRegenTimer >= 1f)
                {
                    currentKi += kiRegenRate;
                    currentKi = Mathf.Min(currentKi, maxKi);
                    kiRegenTimer = 0f;
                    UpdateUI();
                }
            }

            if (currentTransformation != null && currentKi <= 0)
            {
                currentTransformationIndex = -1;
                RevertToBaseForm();
                UpdateUI();
            }
        }

        if (isCharging && currentMagnetRadius < maxMagnetRadius)
        {
            currentMagnetRadius += radiusGrowSpeed * Time.deltaTime;
            currentMagnetRadius = Mathf.Min(currentMagnetRadius, maxMagnetRadius);
        }

        if (dashBufferTimer > 0)
        {
            dashBufferTimer -= Time.deltaTime;
            if (canDash && !isDashing && !isDead && !isCharging && moveInput != Vector2.zero)
            {
                TryDash();
                dashBufferTimer = 0;
            }
        }

        if (meleeCooldownTimer > 0f)
            meleeCooldownTimer -= Time.deltaTime;

        if (isInCombo)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                currentComboIndex = 0;
                isInCombo = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead || isTransforming) return;
        if (isDashing) return;

        if (isCharging)
        {
            rb.linearVelocity = Vector2.zero;
            visualTransform.localPosition = visualBasePosition;
            animator.SetInteger("Direction", 0);
            animator.SetFloat("Speed", 0f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentMagnetRadius, collectibleLayer);
            foreach (var hit in hits)
            {
                var collectible = hit.GetComponent<CollectibleObject>();
                if (collectible != null) collectible.StartMagnetize(transform);
            }
            return;
        }

        Vector2 targetVelocity = moveInput.normalized * moveSpeed;
        float accel = (moveInput.sqrMagnitude > 0.1f) ? acceleration : deceleration;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, accel * Time.fixedDeltaTime);

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        visualTransform.localPosition = isMoving
            ? visualBasePosition + new Vector3(0, Mathf.Sin(Time.time * 10f) * 0.15f, 0)
            : visualBasePosition;

        int direction = isMoving ? (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y) ? 1 : 2) : 0;
        animator.SetInteger("Direction", direction);
        animator.SetFloat("Speed", isMoving ? moveInput.sqrMagnitude : 0f);

        if (isLatched || isCharging || isDead || isTransforming) return;

        FaceMouseDirection();
    }

    private void TryManualAttack()
    {
        if (isCharging || isDashing || isLatched || isDead || isTransforming)
            return;

        if (meleeCooldownTimer > 0f)
        {
            bufferedAttack = true;
            return;
        }

        // Scan for closest enemy
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, meleeRange, enemyLayer);

        Transform closest = null;
        float closestDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent<SaibamanEnemy>(out var saibaman) && saibaman.IsExploding)
                continue;

            float dist = Vector2.SqrMagnitude(enemy.transform.position - transform.position);
            if (dist < closestDist)
            {
                closest = enemy.transform;
                closestDist = dist;
            }
        }

        if (closest != null)
        {
            TriggerMeleeAttack(closest);
            meleeCooldownTimer = meleeCooldown;
        }
        else
        {
            animator.SetTrigger("MeleeWhiff");
            meleeCooldownTimer = meleeCooldown;

            // Still track combo timing to allow follow-up
            comboTimer = comboResetTime;
            isInCombo = true;

            // Optional: smooth return from whiff
            StartCoroutine(EndMeleeAfterDelay(0.2f));
        }
    }

    private void TriggerMeleeAttack(Transform target)
    {
        if (isLatched) return;
        if (target.TryGetComponent<SaibamanEnemy>(out var saibaman) && saibaman.IsExploding) return;

        isMeleeAttacking = true;
        spriteRenderer.flipX = target.position.x < transform.position.x;

        Vector2 direction = (target.position - transform.position).normalized;
        StartCoroutine(ShortDash(direction, 0.3f, 5f));

        int animationIndex = (currentComboIndex % 3) + 1;
        string clipName = $"Melee{animationIndex}";
        animator.SetTrigger(clipName);
        StartCoroutine(WaitAndStartEndMelee(target, clipName));

        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(10);
            damageable.ApplyStatusEffect(StatusEffect.Stun, 0.25f);
        }

        StartCoroutine(HitPauseAfterDelay(0.05f, 0.05f));
        CameraFollow.Instance?.TriggerImpulseShake(0.3f, 0.02f, 10f);

        currentComboIndex++;

        if (currentComboIndex >= maxComboCount)
        {
            // After the 3rd hit is complete, THEN start finisher
            StartCoroutine(StartFinisherAfterDelay(clipName));
            currentComboIndex = 0;
            isInCombo = false;
        }
        else
        {
            comboTimer = comboResetTime;
            isInCombo = true;
        }
    }


    private IEnumerator PlayHitEffectAfterDelay(Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, position, Quaternion.identity);
    }

    private IEnumerator EndMeleeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isMeleeAttacking = false;

        if (bufferedAttack)
        {
            bufferedAttack = false;
            TryManualAttack(); // queue follow-up attack
            yield break;
        }

        animator.CrossFade("Idle", 0.1f);
    }

    private IEnumerator ExecuteComboFinisher()
    {
        isMeleeAttacking = true;
        comboTimer = 0;
        isInCombo = false;
        currentComboIndex = 0;
        rb.linearVelocity = Vector2.zero;
        controls.Disable();

        List<Vector3> relativeOffsets = new List<Vector3>
        {
            new Vector3(-1f, 0.5f, 0),
            new Vector3(1f, 0.5f, 0),
            new Vector3(0f, -0.75f, 0)
        };

        List<Transform> targets = GetNearestEnemies(3);

        if (targets.Count < 3 && targets.Count > 0)
        {
            Transform fallback = targets[0];
            while (targets.Count < 3)
                targets.Add(fallback);
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null) continue;

            transform.position = targets[i].position + relativeOffsets[i % relativeOffsets.Count];
            spriteRenderer.flipX = targets[i].position.x < transform.position.x;

            string trigger = $"Melee{i + 1}";
            animator.SetTrigger(trigger);

            if (finisherTrailEffect != null)
            {
                GameObject trail = Instantiate(finisherTrailEffect, transform.position, Quaternion.identity);
                Destroy(trail, 0.083f);
            }

            if (i == 0)
                cameraFollow?.TemporaryZoom(4.5f, 0.2f); // custom method in CameraFollow

            if (i == targets.Count - 1)
            {
                Time.timeScale = 0.3f;
                if (finisherImpactFlash != null)
                    Instantiate(finisherImpactFlash, targets[i].position, Quaternion.identity);
            }

            CameraFollow.Instance?.TriggerImpulseShake(0.4f, 0.02f, 12f);
            StartCoroutine(HitPause(0.05f));

            var damageable = targets[i].GetComponent<IDamageable>();
            damageable?.TakeDamage(10);
            damageable?.ApplyStatusEffect(StatusEffect.Stun, 0.25f);

            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, targets[i].position, Quaternion.identity);

            yield return new WaitForSecondsRealtime(0.35f);

            if (i == targets.Count - 1)
                Time.timeScale = 1f;
        }

        controls.Enable();
        isMeleeAttacking = false;
        animator.CrossFade("Idle", 0.1f);
    }

    private IEnumerator StartFinisherAfterDelay(string clipName)
    {
        yield return null; // wait 1 frame

        float delay = 0.5f; // fallback
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                delay = clip.length;
                break;
            }
        }

        yield return new WaitForSeconds(delay);
        StartCoroutine(ExecuteComboFinisher());
    }

    private List<Transform> GetNearestEnemies(int count)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 6f, enemyLayer);
        return hits
            .Where(c => c != null && c.gameObject.activeInHierarchy)
            .OrderBy(c => Vector2.SqrMagnitude(c.transform.position - transform.position))
            .Select(c => c.transform)
            .Take(count)
            .ToList();
    }

    private IEnumerator ShortDash(Vector2 direction, float duration, float speed)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            rb.linearVelocity = (direction * speed * 0.7f) + (moveInput.normalized * moveSpeed * 0.3f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator HitPause(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
    }

    private IEnumerator HitPauseAfterDelay(float delayBeforePause, float pauseDuration)
    {
        yield return new WaitForSeconds(delayBeforePause);
        yield return HitPause(pauseDuration);
    }

    private IEnumerator WaitAndStartEndMelee(Transform target, string triggerName)
    {
        yield return null; // Wait 1 frame to ensure Animator updates

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length;

        // Delay hit effect
        if (hitEffectPrefab != null)
            StartCoroutine(PlayHitEffectAfterDelay(target.position, clipLength * 0.8f));

        StartCoroutine(EndMeleeAfterDelay(clipLength));
    }


    private void UpdateUI()
    {
        if (kiBar != null) kiBar.value = currentKi;
        if (kiBarText != null) kiBarText.text = $"{currentKi:F1} / {maxKi}";
        if (healthBar != null) healthBar.value = currentHealth;
        if (healthBarText != null) healthBarText.text = $"{currentHealth} / {maxHealth}";
    }

    private void ApplyTransformationStats(CharacterTransformation transformation)
    {
        moveSpeed = characterData.movementSpeed * transformation.speedMultiplier;
        damage = characterData.damage * transformation.damageMultiplier;

        if (transformation.animatorOverride != null)
            animator.runtimeAnimatorController = transformation.animatorOverride;
        if (transformation.transformationPortrait != null)
            spriteRenderer.sprite = transformation.transformationPortrait;

        auraController?.ApplyTransformationAura(transformation);
    }

    private void TryTransform()
    {
        if (isCharging || isLatched || isDead || isTransforming) return;

        int nextIndex = currentTransformationIndex + 1;
        if (nextIndex < characterData.transformations.Count)
        {
            var transformation = characterData.transformations[nextIndex];
            if (currentKi >= transformation.kiCostToTransform)
            {
                currentTransformationIndex = nextIndex;
                currentTransformation = transformation;
                StartCoroutine(PerformTransformation(transformation));
            }
        }
    }

    public void MaxTransform()
    {
        if (isCharging || isLatched || isDead) // Removed isTransforming
        {
            Debug.Log("Blocked: Cannot transform now.");
            return;
        }

        for (int i = characterData.transformations.Count - 1; i >= 0; i--)
        {
            var transformation = characterData.transformations[i];
            Debug.Log($"Checking form {i}: {transformation.formName}, cost: {transformation.kiCostToTransform}, currentKi: {currentKi}");

            if (currentKi >= transformation.kiCostToTransform)
            {
                Debug.Log($"Transforming to: {transformation.formName}");
                currentTransformationIndex = i;
                currentTransformation = transformation;
                StartCoroutine(PerformTransformation(transformation));
                break;
            }
        }
    }



    private void OnDeTransform(InputAction.CallbackContext ctx)
    {
        if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)
            return; // Skip if Shift is pressed — this is likely a ForceRevert call

        if (currentTransformationIndex > 0)
        {
            currentTransformationIndex--;
            var transformation = characterData.transformations[currentTransformationIndex];
            currentTransformation = transformation;
            StartCoroutine(PerformDeTransform(transformation));
        }
        else
        {
            RevertToBaseForm();
        }
    }

    private IEnumerator PerformDeTransform(CharacterTransformation transformation)
    {
        isTransforming = true;

        animator.SetTrigger("Transform");

        yield return new WaitForSecondsRealtime(0f);

        ApplyTransformationStats(transformation); // <-- replaces all manual assignments

        animator.Play("Idle", 0, 0f);
        animator.updateMode = AnimatorUpdateMode.Normal;

        isTransforming = false;
    }



    public void ForceRevertToBaseForm()
    {
        if (currentTransformation == null && currentTransformationIndex == -1)
            return;

        StopAllCoroutines();

        currentTransformation = null;
        currentTransformationIndex = -1;

        moveSpeed = characterData.movementSpeed;
        damage = characterData.damage;

        animator.runtimeAnimatorController = characterData.animatorController;
        animator.SetBool("IsCharging", false);
        animator.Play("Idle", 0, 0f);

        SFXManager.Instance?.Play(SFXManager.Instance.PowerDown);

        auraController?.ClearAuraState();
        if (characterData.baseAuraProfile != null)
            auraController?.ApplyTransformationAura(characterData.baseAuraProfile);

        isCharging = false;
        isTransforming = false;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        cameraFollow?.StopChargingEffects();

        Debug.Log("Forced revert to base form.");
    }


    private IEnumerator PerformTransformation(CharacterTransformation transformation)
    {
        isTransforming = true;

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("Transform");

        CameraFollow.Instance?.TriggerImpulseShake(1f, 0.05f, 25f);
        CameraFollow.Instance?.TemporaryZoom(3.5f, 0.5f);
        Time.timeScale = 0.1f;

        yield return new WaitForSecondsRealtime(1.183f);

        auraController?.FadeInAura(1f);
        Time.timeScale = 1f;

        ApplyTransformationStats(transformation); // <-- replaces all manual assignments

        animator.Play("Idle", 0, 0f);
        animator.updateMode = AnimatorUpdateMode.Normal;

        isTransforming = false;
    }



    private void RevertToBaseForm()
    {
        if (currentTransformation == null && currentTransformationIndex == -1)
            return; // Already base form

        currentTransformation = null;
        currentTransformationIndex = -1;

        moveSpeed = characterData.movementSpeed;
        damage = characterData.damage;

        animator.runtimeAnimatorController = characterData.animatorController;
        auraController?.DisableAura();
        auraController?.ClearAuraState();

        if (characterData.baseAuraProfile != null)
            auraController?.ApplyTransformationAura(characterData.baseAuraProfile);

        SFXManager.Instance?.Play(SFXManager.Instance.PowerDown);

        Debug.Log("Reverted to base form.");
    }

    private bool isSuperSaiyan() => currentTransformation != null;

    public void SetCharacter(PlayerCharacterData data)
    {
        characterData = data;
        Start();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (healthBar != null) healthBar.value = currentHealth;
        if (healthBarText != null) healthBarText.text = $"{currentHealth} / {maxHealth}";

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        StopCharge();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        controls.Player.Disable();
        animator.SetTrigger("Die");
    }

    private void FaceMouseDirection()
    {
        if (isMeleeAttacking)
            return; // don't flip during melee

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0f;
        isFacingLeft = worldMousePos.x < transform.position.x;
        spriteRenderer.flipX = isFacingLeft;
    }

    public void PlayAttackAnimation()
    {
        if (isCharging || isLatched || isDead) return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        float verticalSpeed = moveInput.y;

        if (!isMoving)
            animator.Play("IdleAttack");
        else if (verticalSpeed > 0.1f)
            animator.Play("RiseAttack");
        else
            animator.Play("WalkAttack");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, currentMagnetRadius);
    }

    public bool TrySpendKi(float amount)
    {
        if (isDead || isTransforming || isLatched) return false;

        float cost = isSuperSaiyan() ? amount * currentTransformation.attackKiMultiplier : amount;

        if (currentKi >= cost)
        {
            currentKi -= cost;

            if (isSuperSaiyan() && currentKi <= 0f)
                RevertToBaseForm();

            return true;
        }

        if (isSuperSaiyan())
            RevertToBaseForm();

        return false;
    }


    private void StartCharge()
    {
        if (isCharging || isDead || isTransforming) return;

        isCharging = true;
        currentMagnetRadius = baseMagnetRadius;

        SFXManager.Instance?.StartKiChargeLoop();
        cameraFollow?.StartChargingEffects();
        animator.SetBool("IsCharging", true);
        moveInput = Vector2.zero;

        auraController?.EnableAura(); // Make sure aura visuals and Animator are active
        auraController?.PlayChargeAnimation(true); // Play looping charge animation

        foreach (var collectible in CollectibleObject.ActiveCollectibles)
        {
            if (Vector3.Distance(transform.position, collectible.transform.position) <= currentMagnetRadius)
            {
                collectible.StartMagnetize(transform);
            }
        }
    }


    private void StopCharge()
    {
        if (!isCharging || isDead || isTransforming) return;

        isCharging = false;
        SFXManager.Instance?.StopKiChargeLoop();
        cameraFollow?.StopChargingEffects();
        animator.SetBool("IsCharging", false);
        auraController?.PlayChargeAnimation(false);

        currentMagnetRadius = 0f;
    }

    private void TryDash()
    {
        if (!canDash || isDashing || isDead || isLatched || isTransforming)
        {
            dashBufferTimer = dashBufferTime; // store intent to dash
            return;
        }

        if (moveInput == Vector2.zero)
            return; // Require a direction to dash

        if (dashRoutine != null)
            StopCoroutine(dashRoutine);

        dashRoutine = StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;

        StartCoroutine(SpawnAfterimages());

        Vector2 dashDir = moveInput.normalized;
        float dashEndTime = Time.time + dashDuration;

        CameraFollow.Instance?.TriggerImpulseShake(0.5f, 0.02f, 10f); // Camera shake

        if (allowDashInvincibility)
            gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");

        float dashTimer = 0f;

        while (Time.time < dashEndTime)
        {
            float progress = dashTimer / dashDuration;
            float speedScale = dashAccelerationCurve.Evaluate(progress);

            float finalDashSpeed = baseDashSpeed;
            if (currentTransformation != null)
                finalDashSpeed *= currentTransformation.dashSpeedMultiplier;

            rb.linearVelocity = dashDir * (finalDashSpeed * speedScale);

            dashTimer += Time.deltaTime;
            yield return null;
        }

        if (allowDashInvincibility)
            gameObject.layer = LayerMask.NameToLayer("Player");

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    private IEnumerator SpawnAfterimages()
    {
        for (int i = 0; i < afterimageCount; i++)
        {
            if (afterimagePrefab != null)
            {
                GameObject ghost = Instantiate(afterimagePrefab, visualTransform.position, visualTransform.rotation);
                SpriteRenderer ghostSR = ghost.GetComponent<SpriteRenderer>();
                SpriteRenderer playerSR = visualTransform.GetComponent<SpriteRenderer>();

                if (ghostSR != null && playerSR != null)
                {
                    ghostSR.sprite = playerSR.sprite;
                    ghostSR.flipX = playerSR.flipX;
                    ghostSR.color = new Color(1f, 1f, 1f, 0.4f);
                    ghostSR.sortingLayerID = playerSR.sortingLayerID;
                    ghostSR.sortingOrder = playerSR.sortingOrder - 1;

                    // Step 3: scale stretch for motion
                    Vector3 scale = visualTransform.lossyScale;
                    scale.x *= 1.2f;
                    ghost.transform.localScale = scale;

                    // Step 4: slight offset behind movement
                    Vector3 offset = -((Vector3)moveInput.normalized * 0.1f);
                    ghost.transform.position += offset;
                }
            }

            yield return new WaitForSeconds(afterimageInterval);
        }
    }

}

