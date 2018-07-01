﻿using System.Collections;
using UnityEngine;

public class DoomEnemy : Enemy
{
    [SerializeField] private GameObject enemyBullet;
    [SerializeField] private Transform shootTransform;
    [SerializeField] private float burstFireRate = 100;
    /// <summary> Wiggling the shoot position of the bullets (so that not all bullets spawn at the same point) </summary>
    [SerializeField] private float wiggleShootOffset = 0.15f;
    [SerializeField] private float projectileSpeed = 10;
    [SerializeField] private int burstSize = 7;
    [SerializeField] private float timeBetweenShotsInBurst;


    protected override void Awake() {
        base.Awake();
        if (shootTransform == null) shootTransform = transform.Find("shootPosition");
    }

    public override void Attack() {
        base.Attack();
        m_Attacking = true;
        if (!m_CanAttack) return;
        StartCoroutine(FireBurst());
    }

    private IEnumerator FireBurst() {
        timeBetweenShotsInBurst = 60.0f / burstFireRate;
        var i = 0;
        while (i++ < burstSize) {
            // set x velocity to 0
            rb.velocity = new Vector2(0, rb.velocity.y);

            _anim.SetTrigger("Attack");
            Shoot(GameManager.Player.transform.position - shootTransform.position);
            if (!health.IsDead)
                yield return new WaitForSeconds(timeBetweenShotsInBurst);
        }
        m_Attacking = false;
    }

    private void Shoot(Vector2 shootDirection) {
        if (health.IsDead) return;
        shootDirection.Normalize();
        Vector2 randomWiggler = (new Vector2(-shootDirection.y, shootDirection.x)).normalized * UnityEngine.Random.Range(-wiggleShootOffset, wiggleShootOffset);
        Vector2 shootPosition = new Vector2(shootTransform.position.x, shootTransform.position.y) + randomWiggler;

        GameObject projectile = Instantiate(enemyBullet, shootPosition, Quaternion.LookRotation(shootDirection), this.transform);
        projectile.GetComponent<Rigidbody2D>().velocity = shootDirection.normalized * projectileSpeed;

        audioSource.PlayOneShot(attackSound, UnityEngine.Random.Range(0.7f, 1f));
    }
}