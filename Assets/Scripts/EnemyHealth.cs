using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    [SerializeField] private Renderer flashRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    public event Action<float> OnHealthChanged;


    [SerializeField] private int shieldRaiseThreshold = 20;
    [SerializeField] private Transform shieldVisual;
    [SerializeField] private float blockAngle = 0.3f;
    
    
    
    
    private Color originalColor;
    private Coroutine flashRoutine;
    private bool shieldUp = false;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (flashRenderer == null) flashRenderer = GetComponentInChildren<Renderer>();
        if (flashRenderer != null) originalColor = flashRenderer.material.color;
        if (shieldVisual != null) shieldVisual.gameObject.SetActive(false);
    }
    public void TakeDamage(float damage, Vector3 hitDirection)
    {
        if (damage <= 0 || currentHealth <= 0) return;

        if (shieldUp)
        {
            float dot = Vector3.Dot(transform.forward.normalized, -hitDirection);
            if(dot > blockAngle)
            {
                return;
            }
        }

        currentHealth = Mathf.Max(0, currentHealth - Mathf.RoundToInt(damage));
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);

        if (currentHealth <= shieldRaiseThreshold && !shieldUp)
        {
            shieldUp = true;
            if (shieldVisual != null) shieldVisual.gameObject.SetActive(true);

            SwatAI ai = GetComponent<SwatAI>();
            if (ai != null) ai.SetShieldState(true);
        }

        if(currentHealth == 0)
            Die();

        if(flashRenderer != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(Flash());
        }
    }

    private IEnumerator Flash()
    {
        flashRenderer.material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        flashRenderer.material.color = originalColor;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
