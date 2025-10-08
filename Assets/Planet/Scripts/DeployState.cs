#nullable enable
using System;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;
using VContainer;
using R3;

/// <summary>
/// planetの設置後の状態
/// </summary>
public class DeployState : IPlanetState
{
    private GameObject _attractionAreaView = null!;
    private SpriteRenderer _attractionAreaSpriteRenderer = null!;
    private MotionHandle _floatingMotion;
    private bool _canAttract;
    private readonly IPlayerContext _player;
    private readonly Collider2D[] _hitCollidersCache = new Collider2D[20];
    private readonly PlanetParams _planetParams;
    
    [Inject]
    public DeployState(PlanetParams planetParams, PlayerController player)
    {
        _player = player;
        _planetParams  = planetParams;
    }
    
    public void Enter(IPlanetContext planet)
    {
        // 浮遊モーション
        _floatingMotion = LMotion
            .Create(planet.PlanetTransform.position.y, planet.PlanetTransform.position.y - 0.2f, 1f)
            .WithEase(Ease.InOutSine)
            .WithLoops(-1, LoopType.Yoyo)
            .BindToPositionY(planet.PlanetTransform)
            .AddTo(planet.PlanetTransform);

        // 引力範囲表示
        _attractionAreaView = planet.AttractionAreaView;
        _attractionAreaSpriteRenderer = planet.AttractionAreaSpriteRenderer;
        float baseRadius = _attractionAreaSpriteRenderer.sprite.bounds.extents.x;
        if (baseRadius > 0)
        {
            _attractionAreaView.transform.localScale = Vector2.one * (_planetParams.AttractionRange / baseRadius);
        }
        _attractionAreaView.SetActive(true);
        
        // 初期状態は引きつけ可能
        _canAttract = true;
    }

    public void Update(IPlanetContext planet, PlanetStateMachine stateMachine)
    {
        // 状態遷移の判定
        if (!planet.IsLaunched) stateMachine.TransitionTo(stateMachine.Follow, planet);
    }

    public void Exit()
    {
        _floatingMotion.Cancel();
        _attractionAreaView.SetActive(false);
    }

    /// <summary>
    /// 引きつけ
    /// </summary>
    public void Attract(Vector2 planetPosition)
    {
        if(!_canAttract) return;
        
        // contactFilterを設定
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player")); 
        filter.useTriggers = false;      
        
        // 引きつけ範囲内にPlayerがいるか判定
        int size = Physics2D.OverlapCircle(planetPosition, _planetParams.AttractionRange, filter, _hitCollidersCache);
        if (size == 1)
        {
            // Playerを引きつける
            Rigidbody2D playerRigidbody = _hitCollidersCache[0].gameObject.GetComponent<Rigidbody2D>();
            Vector2 direction = planetPosition - playerRigidbody.position;
            _player.SetCanControl(false); // 一時的に操作不可にする
            playerRigidbody.AddForce(direction.normalized * _planetParams.AttractionForce, ForceMode2D.Impulse);
            _canAttract = false;
            _attractionAreaView.SetActive(false);
            
            // delayしてから操作可能にする
            Observable.Timer(TimeSpan.FromSeconds(0.5f))
                .Subscribe(_ => _player.SetCanControl(true));
        }
        else if (size >= 2)
        {
            Debug.LogWarning("指定したレイヤーのオブジェクトはPlayerのみです。オブジェクトのレイヤー設定を確認してください");
        }
    }
}