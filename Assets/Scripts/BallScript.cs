﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class BallScript : MonoBehaviour
{
    public static BallScript instance;
    public int speedLvl;
    
    private Rigidbody2D rigid2D;
    private SpriteRenderer spriteRenderer;

    private static readonly float[] Speed = {8, 10, 12, 14, 16};
    private static readonly Color Blue = new Color(57f / 255, 196f / 255, 215f / 255);
    private static readonly Color Yellow = new Color(215f / 255, 165f / 255, 57f / 255);

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        rigid2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        rigid2D.velocity = -transform.up * Speed[speedLvl];
    }

    // Update is called once per frame
    private void Update()
    {
        if (Mathf.Abs(transform.position.x) > 4 || Mathf.Abs(transform.position.y) > 6)
            SceneManager.LoadScene("Main");
    }

    private void FixedUpdate()
    {
        var hit = Physics2D.CircleCast(rigid2D.position, .25f, rigid2D.velocity,
            rigid2D.velocity.magnitude * Time.deltaTime);
        GameObject o;
        if (hit.collider != null && (o = hit.collider.gameObject).CompareTag("Brick"))
        {
            var brick = o.GetComponent<BrickScript>();
            if (brick.level < speedLvl)
            {
                rigid2D.isKinematic = true;
                rigid2D.useFullKinematicContacts = true;
                return;
            }
        }
        rigid2D.isKinematic = false;
    }

    public void SpeedUp()
    {
        if (speedLvl < Speed.Length - 1)
        {
            speedLvl++;
        }
        RefreshBallSpeed();
    }

    public void SpeedDown()
    {
        speedLvl = 0;
        RefreshBallSpeed();
    }

    public void RefreshBallSpeed()
    {
        rigid2D.velocity = rigid2D.velocity.normalized * Speed[speedLvl];
        spriteRenderer.color = Color.Lerp(Blue, Yellow, (float) speedLvl / (Speed.Length - 1));
    }

    public bool HitBrick(int brickLevel)
    {
        bool result = true;
        if (speedLvl > brickLevel) speedLvl = speedLvl - brickLevel - 1;
        else if (speedLvl == brickLevel) speedLvl = 0;
        else result = false;
        RefreshBallSpeed();
        return result;
    }
}