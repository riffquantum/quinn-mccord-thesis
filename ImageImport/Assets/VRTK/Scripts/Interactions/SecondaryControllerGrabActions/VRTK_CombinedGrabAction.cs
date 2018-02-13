 //Combine Axis Scale and Control Direction Grab
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    ///Combines functionality of the Control Direction Grab and the Axis Scale Grab
    /// </summary>
    /// <remarks>
    /// For an object to correctly be rotated it must be created with the front of the object pointing down the z-axis (forward) and the upwards of the object pointing up the y-axis (up). 
    ///
    /// It's not possible to control the direction of an interactable object with a `Fixed_Joint` as the joint fixes the rotation of the object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and scale it by grabbing and pulling with the second controller.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Secondary Controller Grab Actions/VRTK_CombinedGrabAction")]

    public class VRTK_CombinedGrabAction : VRTK_BaseGrabAction
    {
        //Axis Scale vars
        [Tooltip("The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.")]
        public float ungrabDistance = 1f;
        [Tooltip("If checked the current X Axis of the object won't be scaled")]
        public bool lockXAxis = false;
        [Tooltip("If checked the current Y Axis of the object won't be scaled")]
        public bool lockYAxis = false;
        [Tooltip("If checked the current Z Axis of the object won't be scaled")]
        public bool lockZAxis = false;
        [Tooltip("If checked all the axes will be scaled together (unless locked)")]
        public bool uniformScaling = false;
        [Tooltip("The smallest scaling factor that will be used")]
        public float minScalingFactor = 0.1f;
        [Tooltip("The maximum scaling factor allowed")]
        public float maxScalingFactor = 10f;
        protected Vector3 initialScale;
        protected Vector3 minScale;
        protected Vector3 maxScale;
        protected float initalLength;
        protected float initialScaleFactor;

        //Control Dir vars
        //[Tooltip("The distance the secondary controller must move away from the original grab position before the secondary controller auto ungrabs the object.")]
        //public float ungrabDistance = 1f;
        [Tooltip("The speed in which the object will snap back to it's original rotation when the secondary controller stops grabbing it. `0` for instant snap, `infinity` for no snap back.")]
        public float releaseSnapSpeed = 0.1f;
        [Tooltip("Prevent the secondary controller rotating the grabbed object through it's z-axis.")]
        public bool lockZRotation = true;

        protected Vector3 initialPosition;
        protected Quaternion initialRotation;
        protected Quaternion releaseRotation;
        protected Vector3 initialGrabbingVector;
        protected Vector3 pivotPoint;
        protected GameObject pivotSphere;
        protected Coroutine snappingOnRelease;

        /// <summary>
        /// The Initalise method is used to set up the state of the secondary action when the object is initially grabbed by a secondary controller.
        /// </summary>
        /// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary controller.</param>
        /// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary controller.</param>
        /// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary controller.</param>
        /// <param name="primaryGrabPoint">The point on the object where the primary controller initially grabbed the object.</param>
        /// <param name="secondaryGrabPoint">The point on the object where the secondary controller initially grabbed the object.</param>
     
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
            initialGrabbingVector = secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position;
            // attach point on the object for rotation:
            pivotPoint = transform.InverseTransformPoint(primaryGrabbingObject.transform.position);
            pivotSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pivotSphere.transform.parent = this.gameObject.transform;
            pivotSphere.transform.localPosition = pivotPoint;
            pivotSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            //Axis Scale
            initialScale = currentGrabbdObject.transform.localScale;
            minScale = initialScale * minScalingFactor;
            maxScale = initialScale * maxScalingFactor;
            initalLength = initialGrabbingVector.magnitude;
            initialScaleFactor = currentGrabbdObject.transform.localScale.x / initalLength;
            //Control Dir
            initialPosition = currentGrabbdObject.transform.localPosition;
            initialRotation = currentGrabbdObject.transform.localRotation;
            
            StopRealignOnRelease();

        }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary controller.
        /// </summary>
        public override void ProcessUpdate()
        {
            base.ProcessUpdate();
            CheckForceStopDistance(ungrabDistance);
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary controller and influences the rotation of the object.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            base.ProcessFixedUpdate();
            if (initialised)
            {
                AimObject();
            }
            
            if (initialised)
            {
                if (uniformScaling)
                {
                    UniformScale();
                }
                else
                {
                    NonUniformScale();
                }
            }
        }


        /************************************
         * CONTROL GRAB OVERRIDES
         * **********************************/

        /// <summary>
        /// The ResetAction method is used to reset the secondary action when the object is no longer grabbed by a secondary controller.
        /// </summary>
        public override void ResetAction()
        {
            releaseRotation = transform.localRotation;
            if (!grabbedObject.grabAttachMechanicScript.precisionGrab)
            {
                if (releaseSnapSpeed < float.MaxValue && releaseSnapSpeed > 0)
                {
                    snappingOnRelease = StartCoroutine(RealignOnRelease());
                }
                else if (releaseSnapSpeed == 0)
                {
                    transform.localRotation = initialRotation;
                }
            }
            Destroy(pivotSphere);
            pivotSphere = null;
            base.ResetAction();
        }

        /// <summary>
        /// The OnDropAction method is executed when the current grabbed object is dropped and can be used up to clean up any secondary grab actions.
        /// </summary>
        public override void OnDropAction()
        {
            base.OnDropAction();
            StopRealignOnRelease();
        }

        

        protected virtual void StopRealignOnRelease()
        {
            if (snappingOnRelease != null)
            {
                StopCoroutine(snappingOnRelease);
            }
            snappingOnRelease = null;
        }

        protected virtual IEnumerator RealignOnRelease()
        {
            var elapsedTime = 0f;

            while (elapsedTime < releaseSnapSpeed)
            {
                transform.localRotation = Quaternion.Lerp(releaseRotation, initialRotation, (elapsedTime / releaseSnapSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.localRotation = initialRotation;
            transform.localPosition = initialPosition;
        }

        protected virtual void AimObject()
        {
            if (lockZRotation)
            {
                ZLockedAim();
            }
            else
            {
                Vector3 currentGrabbingVector = secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position;
                Quaternion rotChange = Quaternion.FromToRotation(initialGrabbingVector, currentGrabbingVector);
                
                //need to rotate around the point where we grabbed the object:
                //pivotPoint = transform.InverseTransformPoint(primaryGrabbingObject.transform.position);
                //pivotSphere.transform.localPosition = pivotPoint;
                Vector3 scaledPivot = pivotPoint;
                scaledPivot.Scale(transform.localScale);
                // first subtract the rotation point then add it back:
                transform.localPosition += (transform.rotation * scaledPivot);
                transform.localRotation = rotChange * initialRotation;
                transform.localPosition -= (transform.rotation * scaledPivot);

                /*
                //creates a sphere with rays to show the rotation
                if (true)
                {
                    Debug.DrawRay(transform.position, transform.rotation * Vector3.up, Color.black);
                    Debug.DrawRay(transform.position, transform.rotation * Vector3.right, Color.black);
                    Debug.DrawRay(transform.position, transform.rotation * Vector3.forward, Color.black);

                    Debug.DrawRay(transform.position + (transform.rotation * scaledPivot), transform.rotation * Vector3.up, Color.green);
                    Debug.DrawRay(transform.position + (transform.rotation * scaledPivot), transform.rotation * Vector3.right, Color.red);
                    Debug.DrawRay(transform.position + (transform.rotation * scaledPivot), transform.rotation * Vector3.forward, Color.blue);
                }
                */
            }

            if (grabbedObject.grabAttachMechanicScript.precisionGrab)
            {
                //transform.Translate(primaryGrabbingObject.controllerAttachPoint.transform.position - primaryInitialGrabPoint.position, Space.World);
            }
        }

        protected virtual void ZLockedAim()
        {
            Vector3 forward = (secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position).normalized;

            // calculate rightLocked rotation
            Quaternion rightLocked = Quaternion.LookRotation(forward, Vector3.Cross(-primaryGrabbingObject.transform.right, forward).normalized);

            // delta from current rotation to the rightLocked rotation
            Quaternion rightLockedDelta = Quaternion.Inverse(grabbedObject.transform.rotation) * rightLocked;

            float rightLockedAngle;
            Vector3 rightLockedAxis;

            // forward direction and roll
            rightLockedDelta.ToAngleAxis(out rightLockedAngle, out rightLockedAxis);

            if (rightLockedAngle > 180f)
            {
                // remap ranges from 0-360 to -180 to 180
                rightLockedAngle -= 360f;
            }

            // make any negative values into positive values;
            rightLockedAngle = Mathf.Abs(rightLockedAngle);

            // calculate upLocked rotation
            Quaternion upLocked = Quaternion.LookRotation(forward, primaryGrabbingObject.transform.forward);

            // delta from current rotation to the upLocked rotation
            Quaternion upLockedDelta = Quaternion.Inverse(grabbedObject.transform.rotation) * upLocked;

            float upLockedAngle;
            Vector3 upLockedAxis;

            // forward direction and roll
            upLockedDelta.ToAngleAxis(out upLockedAngle, out upLockedAxis);

            // remap ranges from 0-360 to -180 to 180
            if (upLockedAngle > 180f)
            {
                upLockedAngle -= 360f;
            }

            // make any negative values into positive values;
            upLockedAngle = Mathf.Abs(upLockedAngle);

            // assign the one that involves less change to roll
            grabbedObject.transform.rotation = (upLockedAngle < rightLockedAngle ? upLocked : rightLocked);
        }

        /************************************
         * AXIS SCALE OVERRIDES
         * **********************************/
         
        protected virtual void ApplyScale(Vector3 newScale)
        {
            Vector3 existingScale = grabbedObject.transform.localScale;

            float finalScaleX = (lockXAxis ? existingScale.x : newScale.x);
            float finalScaleY = (lockYAxis ? existingScale.y : newScale.y);
            float finalScaleZ = (lockZAxis ? existingScale.z : newScale.z);

            if (finalScaleX > 0 && finalScaleY > 0 && finalScaleZ > 0)
            {
                Vector3 scaledPivot = pivotPoint;
                scaledPivot.Scale(transform.localScale);
                transform.localPosition += (transform.rotation * scaledPivot);
                grabbedObject.transform.localScale = ClampedFinalScale(finalScaleX, finalScaleY, finalScaleZ);
                scaledPivot = pivotPoint;
                scaledPivot.Scale(transform.localScale);
                transform.localPosition -= (transform.rotation * scaledPivot);
            }
        }

        protected Vector3 ClampedFinalScale(float scaleX, float scaleY, float scaleZ)
        {
            scaleX = Mathf.Clamp(scaleX, minScale.x, maxScale.x);
            scaleY = Mathf.Clamp(scaleY, minScale.y, maxScale.y);
            scaleZ = Mathf.Clamp(scaleZ, minScale.z, maxScale.z);
            return new Vector3(scaleX, scaleY, scaleZ);
        }

        protected virtual void NonUniformScale()
        {
            Vector3 initialRotatedPosition = grabbedObject.transform.rotation * grabbedObject.transform.position;
            Vector3 initialSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryInitialGrabPoint.position;
            Vector3 currentSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryGrabbingObject.transform.position;

            float newScaleX = CalculateAxisScale(initialRotatedPosition.x, initialSecondGrabRotatedPosition.x, currentSecondGrabRotatedPosition.x);
            float newScaleY = CalculateAxisScale(initialRotatedPosition.y, initialSecondGrabRotatedPosition.y, currentSecondGrabRotatedPosition.y);
            float newScaleZ = CalculateAxisScale(initialRotatedPosition.z, initialSecondGrabRotatedPosition.z, currentSecondGrabRotatedPosition.z);

            var newScale = new Vector3(newScaleX, newScaleY, newScaleZ) + initialScale;
            ApplyScale(newScale);
        }

        protected virtual void UniformScale()
        {
            float currentLength = (secondaryGrabbingObject.transform.position - primaryGrabbingObject.transform.position).magnitude;
            float adjustedScale = initialScaleFactor * currentLength;

            var newScale = new Vector3(adjustedScale, adjustedScale, adjustedScale);
            ApplyScale(newScale);
        }

        protected virtual float CalculateAxisScale(float centerPosition, float initialPosition, float currentPosition)
        {
            float distance = currentPosition - initialPosition;
            distance = (centerPosition < initialPosition ? distance : -distance);
            return distance;
        }
    
}
}