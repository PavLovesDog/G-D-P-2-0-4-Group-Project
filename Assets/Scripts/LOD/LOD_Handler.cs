using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBF
{ 
    public class LOD_Handler : MonoBehaviour
    {
        [Header("SphereCastAll variables")]
        public LayerMask EnvironmentLayerMask; // used to ignore all colliders we wish not to collide with
        public LayerMask ProjectileLayerMask;
        public float searchRadius;
        public float currentHitDistance;
        public float maxDistance;


        private Vector3 origin;
        private Vector3 direction;

        [SerializeField]
        List<GameObject> allEnvironmentObjects = new List<GameObject>(); // list to contain all elements of the chosen layermask
        [SerializeField]
        List<GameObject> hitEnvironementObjects = new List<GameObject>(); // list to contain only elements sphere cast is colliding with

        [SerializeField]
        List<GameObject> allProjectileObjects = new List<GameObject>();
        [SerializeField]
        List<GameObject> hitProjectileObjects = new List<GameObject>();


        void Start()
        {
            /*
             *  Find all tagged gameobjects that will be affected by the LOD handler (i.e models with high poly & low poly versions)
             *  here and add them to the environment objects list. 
             *  See examples for objects tagged "Tree" & "Stone" below.
             */

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

            #region Find Bonfires
            GameObject[] allLODBonfires = GameObject.FindGameObjectsWithTag("Warm Zone"); // NOTE "Warm zone" may trigger other things if more work is doen here!
            foreach ( GameObject bonfire in allLODBonfires)
            {
                allEnvironmentObjects.Add(bonfire);
            }
            #endregion

            //#region Find Projectiles
            //GameObject[] allProjectiles = GameObject.FindGameObjectsWithTag("Projectile"); // NOTE "Warm zone" may trigger other things if more work is doen here!
            //foreach (GameObject rocks in allProjectiles)
            //{
            //    allProjectileObjects.Add(rocks);
            //}
            //#endregion

            //FIND OTHER ENVIRONMENT OBJECTS.....
        }

        void Update()
        {
            //track position
            origin = transform.position + new Vector3(0, 2, 0); // offset to shoot from object midpoint
            direction = transform.forward;

            allProjectileObjects.Clear();
            GameObject[] allProjectiles = GameObject.FindGameObjectsWithTag("Projectile"); // NOTE "Warm zone" may trigger other things if more work is doen here!
            foreach (GameObject rocks in allProjectiles)
            {
                if(rocks != null)
                    allProjectileObjects.Add(rocks);
            }
        }

        //Peform pyhysics calcs in fixed pdate for smoother operations
        private void FixedUpdate()
        {
            DetectLODGameObjectsSphereRayCast();
            DetectProjectilesSphereRayCast();
        }

        // Function to detect environment and enable or disable high/low polygon models
        // dependent on player distance to objects
        public void DetectLODGameObjectsSphereRayCast()
        {
            // creat objects to hold references to mesh scripts
            Low_Poly_Model hitObjectLowPoly;
            High_Poly_Model hitObjectHighPoly;

            currentHitDistance = maxDistance;
            hitEnvironementObjects.Clear(); // clear list becfore cast
            RaycastHit[] hits = Physics.SphereCastAll(origin, searchRadius, direction, maxDistance, EnvironmentLayerMask, QueryTriggerInteraction.UseGlobal); // detect hits

            foreach (RaycastHit hit in hits)
            {
                hitEnvironementObjects.Add(hit.transform.gameObject); // ad 'em to the list!
                currentHitDistance = hit.distance; // NOTE this will make the draw gizmo only draw thwe wire sphere based on the distance of the last tree that ran..
            }

            #region Environment Objects
            // OUTSIDE SEARCH RADIUS
            //adjust models for all Environment LOD objects
            for (int i = 0; i < allEnvironmentObjects.Count; i++)
            {
                // find the objects high poly model and Activate!
                hitObjectHighPoly = allEnvironmentObjects[i].GetComponentInChildren<High_Poly_Model>();
                if (hitObjectHighPoly != null)
                    hitObjectHighPoly.Deactivate();

                // find the objects Low poly model and Deactivate!
                hitObjectLowPoly = allEnvironmentObjects[i].GetComponentInChildren<Low_Poly_Model>();
                if (hitObjectLowPoly != null)
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
                    if(hitObjectHighPoly != null)
                        hitObjectHighPoly.Activate();

                    // find the objects Low poly model and Deactivate!
                    hitObjectLowPoly = hitEnvironementObjects[i].GetComponentInChildren<Low_Poly_Model>();
                    if(hitObjectLowPoly != null)
                        hitObjectLowPoly.Deactivate();
                }
            }
            #endregion
        }

        public void DetectProjectilesSphereRayCast()
        {
            InteractableObject hitInteractableObject = null;

            hitProjectileObjects.Clear();
            RaycastHit[] hits = Physics.SphereCastAll(origin, searchRadius / 1.5f, direction, maxDistance, ProjectileLayerMask, QueryTriggerInteraction.UseGlobal); // detect hits

            foreach (RaycastHit hit in hits)
            {
                if(hit.transform.gameObject != null)
                    hitProjectileObjects.Add(hit.transform.gameObject);
            }

            // ALL projectiles OUTSIDE of search radius
            for (int i = 0; i < allProjectileObjects.Count; i++)
            {
                // find the objects script
                if(allProjectileObjects[i].gameObject != null)
                    hitInteractableObject = allProjectileObjects[i].GetComponent<InteractableObject>();
                if (hitInteractableObject != null)
                    hitInteractableObject.Deactivate();
            }

            // ALL projectiles INSIDE of search radius
            for (int i = 0; i < hitProjectileObjects.Count; i++)
            {
                // find the objects script
                if (hitProjectileObjects[i].gameObject != null)
                    hitInteractableObject = hitProjectileObjects[i].GetComponent<InteractableObject>();
                if (hitInteractableObject != null)
                    hitInteractableObject.Activate();
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
