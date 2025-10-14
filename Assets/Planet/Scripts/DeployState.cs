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
    private readonly CompositeMotionHandle _handles = new();
    private readonly Collider2D[] _hitCollidersCache = new Collider2D[20];
    private readonly PlanetParams _planetParams;
    private readonly IPlayerContext _player;
    private SpriteRenderer _attractionAreaSpriteRenderer = null!;
    private GameObject _attractionAreaView = null!;
    private bool _canAttract;
    private bool _canOrbit;
    private MotionHandle _floatingMotion;
    private SpriteRenderer _orbitAreaSpriteRenderer = null!;
    private GameObject _orbitAreaView = null!;

    [Inject]
    public DeployState(PlanetParams planetParams, PlayerController player)
    {
        _player = player;
        _planetParams = planetParams;
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
        InitSkill(_attractionAreaView, _attractionAreaSpriteRenderer, _planetParams.AttractionRange);

        // 公転範囲表示
        _orbitAreaView = planet.OrbitAreaView;
        _orbitAreaSpriteRenderer = planet.OrbitAreaSpriteRenderer;
        InitSkill(_orbitAreaView, _orbitAreaSpriteRenderer, _planetParams.OrbitalRange);

        // 初期状態は引きつけ可能
        _canAttract = true;

        _canOrbit = true;
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
        _orbitAreaView.SetActive(false);
    }

    /// <summary>
    /// skillの設定初期化
    /// </summary>
    /// <param name="view"></param>
    /// <param name="spriteRenderer"></param>
    /// <param name="range"></param>
    private void InitSkill(GameObject view, SpriteRenderer spriteRenderer, float range)
    {
        float baseRadius = spriteRenderer.sprite.bounds.extents.x;
        if (baseRadius > 0)
        {
            view.transform.localScale = Vector2.one * (range / baseRadius);
        }

        view.SetActive(true);
    }

    /// <summary>
    /// 引きつけ
    /// </summary>
    public void Attract(Vector2 planetPosition)
    {
        if (!_canAttract) return;

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

    public void Orbit(Vector2 planetPosition, bool isOrbiting)
    {
        if (!isOrbiting)
        {
            _handles.Cancel();
            _player.SetCanControl(true);
            return;
        }

        // contactFilterを設定
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player"));
        filter.useTriggers = false;

        _player.SetCanControl(false); // 一時的に操作不可にする
        int size = Physics2D.OverlapCircle(planetPosition, _planetParams.OrbitalRange, filter, _hitCollidersCache);
        for (int i = 0; i < size; i++)
        {
            var hitObject = _hitCollidersCache[i].gameObject;
            Rigidbody2D rb = hitObject.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            float speed = (2f * Mathf.PI * _planetParams.OrbitalRange) / _planetParams.OrbitalPeriod;
            float correctionFactor = 5f;

            float distToPlanet = Vector2.Distance(rb.position, planetPosition);
            Vector2 direction = rb.position - planetPosition;

            // 角度計算をメソッド化
            float baseAngleRad = GetAngleRad(direction);

            // 回転方向判定をメソッド化
            bool isClockwise = IsClockwise(rb, planetPosition);

            float startAngle = isClockwise ? 360f : 0f;
            float endAngle = isClockwise ? 0f : 360f;

            LMotion.Create(startAngle, endAngle, _planetParams.OrbitalPeriod)
                .WithLoops(-1)
                .WithEase(Ease.Linear)
                .Bind(angle =>
                {
                    if (rb == null) return;

                    float radian = angle * Mathf.Deg2Rad + baseAngleRad;

                    // 理想座標
                    Vector2 desiredPosition = planetPosition + new Vector2(
                        distToPlanet * Mathf.Cos(radian),
                        distToPlanet * Mathf.Sin(radian)
                    );

                    // 接線速度
                    Vector2 tangentDirection = new Vector2(-Mathf.Sin(radian), Mathf.Cos(radian));
                    Vector2 tangentialVelocity = tangentDirection * speed;

                    // 補正速度
                    Vector2 toDesiredPosition = desiredPosition - rb.position;
                    Vector2 correctionVelocity = toDesiredPosition * correctionFactor;

                    // 合成速度
                    rb.linearVelocity = tangentialVelocity + correctionVelocity;
                })
                .AddTo(_handles);
        }
    }

    // 方向ベクトルから角度(ラジアン)を取得
    private static float GetAngleRad(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x);
    }

    // プレイヤーの現在位置・速度から回転方向を判定
    private static bool IsClockwise(Rigidbody2D rb, Vector2 planetPosition)
    {
        bool isPlayerUp = rb.linearVelocity.y > 0;
        bool isPlayerPlusX = rb.position.x - planetPosition.x > 0;
        return isPlayerUp ^ isPlayerPlusX;
    }
}