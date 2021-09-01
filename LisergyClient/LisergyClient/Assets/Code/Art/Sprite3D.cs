using Game;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Sprite3D : MonoBehaviour
{
    private static Dictionary<string, AnimationClip> _animations = new Dictionary<string, AnimationClip>();

    public static readonly int FACE = 7;

    public static readonly int[] JUMP = new int[] { 0 };
    public static readonly int[] WALK = new int[] { 0, 1, 0, 2 };
    public static readonly int[] ATTACK = new int[] { 9, 10, 8, 11, 12 };
    public static readonly int[] HURT = new int[] { 3 };

    public int DefaultFrame { get; set; }

    public Camera m_Camera;
    public bool amActive = false;
    public Sprite[] Sprites;

    private SpriteRenderer _renderer;
    private int _currentFrame = 0;
    private int[] _currentAnimation;
    private bool _looping = false;
    private int _delayMS = 0;
    private DateTime _nextFrame;
    private DateTime _endTime;

    private void Start()
    {
        _renderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (_currentAnimation != null && DateTime.Now > _nextFrame)
        {
            if(DateTime.Now > _endTime)
            {
                Log.Debug($"[Anim] Defaulting Frame {DefaultFrame} for {this.gameObject.name}");
                _currentAnimation = null;
                _renderer.sprite = Sprites[DefaultFrame];
                return;
            }

            if (_currentFrame < _currentAnimation.Length)
            {
                _renderer.sprite = Sprites[_currentAnimation[_currentFrame]];
                Log.Debug($"[Anim] Playing Animation Frame {_currentFrame} for {this.gameObject.name}");
                _nextFrame = DateTime.Now + TimeSpan.FromMilliseconds(_delayMS);
                _currentFrame++;
            }
            if (_currentFrame == _currentAnimation.Length)
            {
                if (_looping) _currentFrame = 0;
                else _currentAnimation = null;
            }  
        }
    }
    
    public void PlayAnimation(int [] animation, bool loop, int delayMS, float playForSeconds=-1)
    {
        Log.Debug($"[Anim] Starting animation {animation} for {this.gameObject.name}");
        _looping = loop;
        _currentAnimation = animation;
        _currentFrame = 0;
        this. _delayMS = delayMS;
        _nextFrame = DateTime.Now;
        if (playForSeconds != -1)
            _endTime = DateTime.Now + TimeSpan.FromSeconds(playForSeconds);
        else
            _endTime = DateTime.MaxValue;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
    }
}