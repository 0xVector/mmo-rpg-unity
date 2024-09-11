namespace Camera
{
    using UnityEngine;

    public class Follow : MonoBehaviour
    {
        public Transform target;
        public float smoothTime = 0.3f;
        public Vector3 offset;

        Vector3 velocity = Vector3.zero;

        void FixedUpdate()
        {
            var targetPos = target != null ? target.position : Vector3.zero;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos + offset, ref velocity, smoothTime);
        }
    }
}