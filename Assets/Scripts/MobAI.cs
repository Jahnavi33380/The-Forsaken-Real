using System.Collections; 

using UnityEngine;
using UnityEngine.AI;

public class MobAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField]
    private GameObject _target;

    private float lastAttackTime = 0f;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (_agent == null)
        {
            Debug.LogError("NavMeshAgent component missing from this game object");
        }
        _agent.SetDestination(_target.transform.position);
    }

    void Update()
    {
        _agent.SetDestination(_target.transform.position);
    }

    
}