using UnityEngine;

public class DestroyAfter : MonoBehaviour {
    ParticleSystem psystem;

    void Start () {
        psystem = GetComponent<ParticleSystem>();
	}

    void Update()
    {
        if (!psystem.isPlaying)
            Destroy(gameObject);
    }
}
