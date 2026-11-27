using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    [SerializeField] private Renderer flashRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    public event Action<float> OnHealthChanged;

    private Color originalColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (flashRenderer == null) flashRenderer = GetComponentInChildren<Renderer>();
        if (flashRenderer != null) originalColor = flashRenderer.material.color;
    }
    public void TakeDamage(float damage)
    {
        if (damage <= 0 || currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - Mathf.RoundToInt(damage));
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);

        if (currentHealth == 0)
            Die();

        if (flashRenderer != null)
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
        Debug.Log("Player Died!!");
        SceneManager.LoadScene("GameHub_MainMenu");

    }
}
