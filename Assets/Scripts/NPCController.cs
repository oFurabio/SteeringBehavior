using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour {
    NavMeshAgent agent;
    Transform target;
    Vector3 wanderTarget = Vector3.zero;

    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderDistance = 20f;
    [SerializeField] private float wanderJitter = 1f;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        MeshRenderer cor = GetComponentInChildren<MeshRenderer>();
        cor.material.color = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );
        target = transform;
    }

    private void Update() {
        Wander();
    }

    void Seek(Vector3 targetPos) {
        agent.SetDestination(targetPos);
    }

    void Flee(Vector3 targetPos) {
        Vector3 vetorFuga = transform.position - targetPos;
        agent.SetDestination(transform.position + vetorFuga);
    }

    void Pursuit() {
        Vector3 targetDir = target.transform.position - transform.position;
        
        float relativeHeading = Vector3.Angle(transform.forward, transform.InverseTransformVector(target.transform.forward));
        float angleToTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));
        
        if (angleToTarget > 90 && relativeHeading < 20) {
            Seek(target.position);
            return;
        }

        float speeds = agent.speed;
        float lookAhead = targetDir.magnitude / speeds;

        Vector3 targetPred = target.transform.position + target.transform.forward * lookAhead;
        Seek(targetPred);
    }

    void Evade() {
        Vector3 targetDir = target.transform.position - transform.position;

        float speeds = agent.speed;
        float lookAhead = targetDir.magnitude / speeds;

        Vector3 targetPred = target.transform.position + target.transform.forward * lookAhead;
        Flee(targetPred);
    }

    void Wander() {
        float randomX = Random.Range(-1.0f, 1.0f) * wanderJitter;
        float randomZ = Random.Range(-1.0f, 1.0f) * wanderJitter;
        wanderTarget += new Vector3(randomX, 0f, randomZ);

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0f, 0f, wanderDistance);

        Vector3 targetWorld = transform.TransformVector(targetLocal);
        Seek(targetWorld);
    }

    void Hide() {
        float distance = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        Vector3 chosenDirection = Vector3.zero;
        GameObject chosenGo = null;

        for (int i = 0; i < GameEnvironment.GetInstance.GetEsconderijos().Length; i++) {
            Vector3 hideDir = GameEnvironment.GetInstance.GetEsconderijos()[i].transform.position - target.transform.position;
            Vector3 hidePos = GameEnvironment.GetInstance.GetEsconderijos()[i].transform.position + hideDir.normalized * 10;

            if (Vector3.Distance(transform.position, hidePos) < distance) {
                chosenSpot = hidePos;
                chosenDirection = hideDir;
                chosenGo = GameEnvironment.GetInstance.GetEsconderijos()[i];
                distance = Vector3.Distance(transform.position, hidePos);
            }
        }
        Collider hideCollider = chosenGo.GetComponent<Collider>();
        chosenSpot.y = 0.5f;
        Vector3 chosenDirectionNormalized = new Vector3(chosenDirection.x, 0f, chosenDirection.z).normalized;
        Ray backRay = new Ray(chosenSpot, -chosenDirectionNormalized);
        RaycastHit hit;
        hideCollider.Raycast(backRay, out hit, 100.0f);
        
        Seek(hit.point + chosenDirectionNormalized * 2.0f);
    }

    bool CanSeeTarget() {
        RaycastHit hit;
        Vector3 rayToTarget = target.position - transform.position;
        float lookAngle = Vector3.Angle(rayToTarget, transform.forward);

        if (lookAngle < 60 && Physics.Raycast(transform.position, rayToTarget, out hit))
            if (hit.collider.gameObject.tag == "Pegador")
                return true;
        
        return false;
    }

    bool CanSeeMe() {
        Vector3 rayToTarget = transform.position - target.transform.position;
        float lookAngle = Vector3.Angle(rayToTarget, target.transform.forward);

        if (lookAngle < 60)
            return true;

        return false;
    }

    bool TargetInRange() {
        Vector3 distance = target.position - transform.position;

        if (distance.magnitude < 10)
            return true;

        return false;
    }
}
