using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE {
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler InputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;
        
        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]

        float rotationSpeed = 10;

        // Start is called before the first frame update
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            InputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
        }

        public void Update()
        {
            float delta = Time.deltaTime;

            InputHandler.TickInput(delta);

            moveDirection = cameraObject.forward * InputHandler.vertical;
            moveDirection += cameraObject.right * InputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(InputHandler.moveAmount, 0);

            if(animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }


        #region Movement
        Vector3 normalVector;
        Vector3 targetPositionl;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = InputHandler.moveAmount;

            targetDir = cameraObject.forward * InputHandler.vertical;
            targetDir += cameraObject.right * InputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if(targetDir == Vector3.zero)
            {
                targetDir = myTransform.forward;
            }

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        #endregion
    }
}