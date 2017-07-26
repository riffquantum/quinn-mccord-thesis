// Track Object Position Grab Attach|GrabAttachMechanics|50080
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Track Object Position Grab Attach script doesn't attach the object to the controller via a joint, instead it ensures the object tracks the direction of the controller.
    /// </summary>
    /// <remarks>
    /// Same as Object Grab Attach, but only tracks position, not rotation.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_TrackObjectPositionGrabAttach")]
    public class Custom_TrackObjPositionGrabAttach : VRTK_TrackObjectGrabAttach
    {

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
            Vector3 positionDelta = trackPoint.position - initialAttachPoint.position;

            Vector3 velocityTarget = positionDelta / Time.fixedDeltaTime;
            Vector3 calculatedVelocity = Vector3.MoveTowards(grabbedObjectRigidBody.velocity, velocityTarget, maxDistanceDelta);

            if (velocityLimit == float.PositiveInfinity || calculatedVelocity.sqrMagnitude < velocityLimit)
            {
                grabbedObjectRigidBody.velocity = calculatedVelocity;
            }
        }

    }
}