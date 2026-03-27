using System.Collections;
using Meta.XR.Samples;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Meta.XR.InteractionSDK.Samples
{
    [RequireComponent(typeof(AudioSource))]
    public class CollisionHandler : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioClip[] _bounceAudio;
        [SerializeField] private AudioClip _winClip;

        [Header("Portal Settings")]
        [SerializeField] private GameObject _portalPrefab;
        [SerializeField] private bool _toFreezeOnHit;

        private AudioSource _audioSource;
        private PooledThrowable _pooledThrowable;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _pooledThrowable = GetComponent<PooledThrowable>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 1. Check for bouncing on walls/floor
            if (collision.gameObject.CompareTag("Bouncable") || collision.gameObject.CompareTag("Floor"))
            {
                PlayRandomBounceSound();
                SpawnPortal(collision.contacts[0].point);
            }
            // 2. Check for hitting the Goal/Target
            else if (collision.gameObject.CompareTag("Target"))
            {
                PlayWinSound();

                if (_toFreezeOnHit)
                {
                    HitTarget(collision.gameObject);
                }
            }
        }

        private void SpawnPortal(Vector3 position)
        {
            if (_portalPrefab != null)
            {
                GameObject portal = Instantiate(_portalPrefab, position, Quaternion.identity);
                var teleporter = FindObjectOfType<PortalTeleporter>();
                if (teleporter != null) teleporter.SetPortalLocation(portal);
            }
        }

        private void PlayRandomBounceSound()
        {
            if (_bounceAudio != null && _bounceAudio.Length > 0)
            {
                AudioClip clip = _bounceAudio[Random.Range(0, _bounceAudio.Length)];
                _audioSource.PlayOneShot(clip, 0.4f);
            }
        }

        private void PlayWinSound()
        {
            if (_winClip != null)
            {
                _audioSource.PlayOneShot(_winClip, 1.0f);
                Debug.Log("Goal Reached! Playing Win Sound.");
            }
        }

        private void HitTarget(GameObject targetObject)
        {
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
            }
            transform.SetParent(targetObject.transform, true);
        }
    }
}