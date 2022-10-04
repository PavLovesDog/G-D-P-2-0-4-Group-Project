using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{ 
    public class LOD_Handler : MonoBehaviour
    {
        [Header("SphereCastAll variables")]
        public LayerMask layerMask; // used to ignore all colliders we wish not to collide with
        public float searchRadius;
        public float currentHitDistance; // both variables used in single sphere cast
        public float maxDistance;        // both variables used in single sphere cast
        
        [Header("SphereCast variables")]
        public GameObject currentHitObject;

        private Vector3 origin;
        private Vector3 direction;

        [SerializeField]
        List<GameObject> allEnvironmentObjects = new List<GameObject>();

        [SerializeField]
        List<GameObject> hitEnvironementObjects = new List<GameObject>();


        // Start is called before the first frame update
        void Start()
        {
            #region Find Trees
            //find all the trees in the scene
            GameObject[] allLODTrees = GameObject.FindGameObjectsWithTag("Tree");

            //add trees to environment list
            foreach(GameObject tree in allLODTrees)
            {
                allEnvironmentObjects.Add(tree);
            }
            #endregion

            #region Find Stones
            //find all the stones in the scene
            GameObject[] allLODStones = GameObject.FindGameObjectsWithTag("Stone");

            //add stones to environment list
            foreach (GameObject stone in allLODStones)
            {
                allEnvironmentObjects.Add(stone);
            }
            #endregion

            //FIND OTHER ENVIRONMENT OBJECTS HERE.....
        }

        void Update()
        {
            //track position
            origin = transform.position + new Vector3(0, 2, 0); // offset to shoot from belly
            direction = transform.forward;
        }

        //Peform pyhysics calcs in fixed pdate for smoother operations
        private void FixedUpdate()
        {
            SphereCastAllRay();
        }

        // Function to detect environment and enable or disable high/low polygon models
        // dependent on player distance to objects
        public void SphereCastAllRay()
        {
            // creat objects to hold references to mesh scripts
            Low_Poly_Model hitObjectLowPoly;
            High_Poly_Model hitObjectHighPoly;

            currentHitDistance = maxDistance;
            hitEnvironementObjects.Clear(); // clear list becfore cast
            RaycastHit[] hits = Physics.SphereCastAll(origin, searchRadius, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);

            foreach (RaycastHit hit in hits)
            {
                hitEnvironementObjects.Add(hit.transform.gameObject);
                currentHitDistance = hit.distance; // NOTE this will make the draw gizmo only draw thwe wire sphere based on the distance of the last tree that ran..
            }

            //adjust models for all Environment LOD objects
            for (int i = 0; i < allEnvironmentObjects.Count; i++)
            {
                // find the objects high poly model and Activate!
                hitObjectHighPoly = allEnvironmentObjects[i].GetComponentInChildren<High_Poly_Model>();
                hitObjectHighPoly.Deactivate();

                // find the objects Low poly model and Deactivate!
                hitObjectLowPoly = allEnvironmentObjects[i].GetComponentInChildren<Low_Poly_Model>();
                hitObjectLowPoly.Activate();
            }

            // WITHIN SEARCH RADIUS
            //adjust models for environment LOD objects HIT
            if (hitEnvironementObjects != null)
            {
                for (int i = 0; i < hitEnvironementObjects.Count; i++)
                {
                    // find the objects high poly model and Activate!
                    hitObjectHighPoly = hitEnvironementObjects[i].GetComponentInChildren<High_Poly_Model>();
                    hitObjectHighPoly.Activate();

                    // find the objects Low poly model and Deactivate!
                    hitObjectLowPoly = hitEnvironementObjects[i].GetComponentInChildren<Low_Poly_Model>();
                    hitObjectLowPoly.Deactivate();
                }
            }
        }

        //USE THIS RAY CAST FOR ENEMIES!
        // function to shoot a single ray and record its data
        public void SphereCastRay()
        {
            RaycastHit hit;
            if (Physics.SphereCast(origin, searchRadius, direction, out hit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                if (hit.transform.gameObject != null)
                {
                    currentHitObject = hit.transform.gameObject;
                    Debug.Log(hit.transform.gameObject.name);
                    currentHitDistance = hit.distance;
                }

            }
            else // no hit
            {
                currentHitDistance = maxDistance;
                currentHitObject = null;
            }
        }

        //function to draw gizmos!
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Debug.DrawLine(origin, origin + direction * currentHitDistance);
            Gizmos.DrawWireSphere(origin + direction * currentHitDistance, searchRadius);
        }

    }
}
