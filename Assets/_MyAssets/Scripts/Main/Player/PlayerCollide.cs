using UnityEngine;
using UnityEngine.Events;

namespace Main.Player
{
    public sealed class PlayerCollide : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collision> onCollisionEnter;
        [SerializeField] private UnityEvent<Collider> onTriggerEnter;

        private void OnCollisionEnter(Collision collision)
        {
            onCollisionEnter.Invoke(collision);
        }

        private void OnTriggerEnter(Collider collider)
        {
            onTriggerEnter.Invoke(collider);
        }
    }
}