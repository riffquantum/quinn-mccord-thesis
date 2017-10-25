// Track Object Position Grab Attach|GrabAttachMechanics|50080
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Track Object Position Grab Attach script doesn't attach the object to the controller via a joint, instead it ensures the object tracks the position of the controller.
    /// </summary>
    /// <remarks>
    /// Same as Object Grab Attach, but only tracks position, not rotation.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_TrackObjectPositionGrabAttach")]
    public class Custom_TrackObjPositionGrabAttach : VRTK_TrackObjectGrabAttach
    {
        private GameObject grabbingObject;
        private Vector3 previousGrabbingPosition;

        public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {
            bool result = base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint);
            this.grabbingObject = grabbingObject;
            this.previousGrabbingPosition = grabbingObject.transform.position;
            return result;
        }

        /// <summary>
        /// The ProcessFixedUpdate method is run in every FixedUpdate method on the interactable object. It applies velocity to the object to ensure it is tracking the grabbing object.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            if (!grabbedObject)
            {
                return;
            }

            float maxDistanceDelta = 10f;
            //Vector3 positionDelta = trackPoint.position - (grabbedSnapHandle != null ? grabbedSnapHandle.position : grabbedObject.transform.position);
            //Vector3 positionDelta = trackPoint.position - initialAttachPoint.position;
            Vector3 positionDelta = grabbingObject.transform.position - this.previousGrabbingPosition;
            
            Vector3 velocityTarget = positionDelta / Time.fixedDeltaTime;
            Vector3 calculatedVelocity = Vector3.MoveTowards(grabbedObjectRigidBody.velocity, velocityTarget, maxDistanceDelta);

            if (velocityLimit == float.PositiveInfinity || calculatedVelocity.sqrMagnitude < velocityLimit)
            {
                grabbedObjectRigidBody.velocity = calculatedVelocity;
            }
            /*
            Debug.Log("Grabbing Object: " + grabbingObject.name + " grabbingObject.pos " + grabbingObject.transform.position +
                "\ngrabbedObject: " + grabbedObject.name + " grabbedObject.pos " + grabbedObject.transform.position +
                "\ninitialAttachPoint: " + initialAttachPoint.name + " initialAttachPoint.pos " + initialAttachPoint.position +
                "\ntrackPoint: " + trackPoint.name + " trackpoint.pos " + trackPoint.position +
                "\npositionDelta " + positionDelta +
                "\n controllerAttachPoint " + controllerAttachPoint.name + controllerAttachPoint.position +
                "\n grabbedObjectRigidBody " + grabbedObjectRigidBody.name + grabbedObjectRigidBody.position +
                "\n grabbedSnapHandle " + grabbedSnapHandle
                );
                */
            this.previousGrabbingPosition = grabbingObject.transform.position;
        }

    }
}