using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveSpawn : MonoBehaviour
{
    private ParticleSystem particle;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        Messenger.AddListener(GameEvent.ENEMY_QUEUE_DEPLETED, StopParticle);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.ENEMY_QUEUE_DEPLETED, StopParticle);
    }

    private void StopParticle()
    {
        particle.Stop();
    }
}
