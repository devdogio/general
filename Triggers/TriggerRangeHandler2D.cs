using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine.Assertions;

namespace Devdog.General
{
    public partial class TriggerRangeHandler2D : MonoBehaviour, ITriggerRangeHandler
    {
        [SerializeField]
        private float _useRange = 10f;
        public float useRange
        {
            get { return _useRange; }
        }

        private readonly List<Player> _playersInRange = new List<Player>();
        private CircleCollider2D _circleCollider;
        private Rigidbody2D _rigidbody2D;
        private TriggerBase _trigger;

        protected void Awake()
        {
            _trigger = GetComponentInParent<TriggerBase>();
            Assert.IsNotNull(_trigger, "TriggerRangeHandler used but no TriggerBase found on object.");

            _rigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
            _rigidbody2D.isKinematic = true;

            _circleCollider = gameObject.GetOrAddComponent<CircleCollider2D>();
            _circleCollider.isTrigger = true;
            _circleCollider.radius = useRange;

            gameObject.layer = 2; // Ignore raycasts
        }

        public IEnumerable<Player> GetPlayersInRange()
        {
            return _playersInRange;
        }

        public bool IsPlayerInRange(Player player)
        {
            return _playersInRange.Contains(player);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.gameObject.GetComponent<Player2D>();
            if (player != null)
            {
                _playersInRange.Add(player);
                _trigger.NotifyCameInRange(player);
            }
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            var player = other.gameObject.GetComponent<Player2D>();
            if (player != null)
            {
                _trigger.NotifyWentOutOfRange(player);
                _playersInRange.Remove(player);
            }
        }
    }
}