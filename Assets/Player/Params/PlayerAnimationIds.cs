using UnityEngine;

namespace Orbit.Player
{
    /// <summary>
    /// Animatorのパラメータをhash値として管理する静的クラス
    /// </summary>
    public static class PlayerAnimationIds
    {
        public static readonly int IdleHash = Animator.StringToHash("Idle");
        public static readonly int JumpHash = Animator.StringToHash("Jump");
        public static readonly int WalkHash = Animator.StringToHash("Walk");
        public static readonly int FallHash = Animator.StringToHash("Fall");
        public static readonly int DeathHash = Animator.StringToHash("Death");
    }
}