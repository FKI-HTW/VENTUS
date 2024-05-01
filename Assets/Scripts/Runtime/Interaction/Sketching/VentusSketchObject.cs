using System.Collections.Generic;
using UnityEngine;
using VRSketchingGeometry.Splines;
using VRSketchingGeometry.Meshing;
using VRSketchingGeometry.SketchObjectManagement;

namespace VENTUS.Interaction.Sketching
{
    
    /// <summary>
    /// Provides methods to interact with a line game object in the scene.
    /// </summary>
    /// <remarks>Original author: tterpi</remarks>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class VentusSketchObject : SketchObject
    {
        /// <summary>
        /// The instance of the smoothly interpolated Catmul-Rom spline mesh
        /// </summary>
        protected SplineMesh SplineMesh;

        /// <summary>
        /// Linierly interpolated spline for displaying a segment only two control points
        /// </summary>
        protected SplineMesh LinearSplineMesh;

        /// <summary>
        /// Object to be displayed for a line of a single control point
        /// </summary>
        [SerializeField]
#pragma warning disable CS0649
        protected GameObject sphereObject;
#pragma warning restore CS0649

        /// <summary>
        /// The minimal distance a new control point has to have to the last control point.
        /// This is used by addControlPointContinuous.
        /// </summary>
        public float minimumControlPointDistance = 0.01f;

        protected float lineDiameter = 0.01f;
        private int InterpolationSteps = 20;

        protected override void Awake()
        {
            base.Awake();
            
            SplineMesh = this.MakeSplineMesh(InterpolationSteps, Vector3.one * lineDiameter);
            LinearSplineMesh = new SplineMesh(new LinearInterpolationSpline(), Vector3.one * lineDiameter);
        }

        /// <summary>
        /// Adds a control point to the spline if it is far enough away from the previous control point.
        /// The distance is controlled by minimumControlPointDistance.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>True if the control point was added.</returns>
        public bool AddControlPointContinuous(Vector3 point)
        {
            //Check that new control point is far enough away from previous control point
            if (SplineMesh.GetNumberOfControlPoints() == 0 ||
                (transform.InverseTransformPoint(point) - SplineMesh.GetControlPoints()[SplineMesh.GetNumberOfControlPoints() - 1]).magnitude > minimumControlPointDistance)
            {
                AddControlPoint(point);
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public virtual void SetLineDiameter(float diameter)
        {
            this.lineDiameter = diameter;

            if (SplineMesh == null || LinearSplineMesh == null)
            {
                return;
            }

            Mesh smoothMesh = SplineMesh.SetCrossSectionScale(Vector3.one * diameter);
            Mesh linearMesh = LinearSplineMesh.SetCrossSectionScale(Vector3.one * diameter);

            Mesh newMesh = smoothMesh ?? linearMesh;

            sphereObject.transform.localScale = Vector3.one * diameter / sphereObject.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;

            ChooseDisplayMethod(newMesh);
        }
        
        /// <summary>
        /// Adds a control point to the end of the spline.
        /// </summary>
        /// <param name="point"></param>
        private void AddControlPoint(Vector3 point)
        {
            //Transform the new control point from world to local space of sketch object
            Vector3 transformedPoint = transform.InverseTransformPoint(point);
            Mesh newMesh = SplineMesh.AddControlPoint(transformedPoint);
            ChooseDisplayMethod(newMesh);
        }
        
        /// <summary>
        /// Factory method for instantiating a SplineMesh.
        /// </summary>
        /// <remarks>This can be overridden to easily change the Spline and TubeMesh used for creating this line.</remarks>
        /// <param name="interpolationSteps"></param>
        /// <param name="lineDiameter"></param>
        /// <returns></returns>
        protected virtual SplineMesh MakeSplineMesh(int interpolationSteps, Vector3 lineDiameter)
        {
            return new SplineMesh(new KochanekBartelsSpline(interpolationSteps), lineDiameter);
        }

        /// <summary>
        /// Determines how to display the spline depending on the number of control points that are present.
        /// </summary>
        protected virtual void ChooseDisplayMethod(Mesh newMesh)
        {
            sphereObject.SetActive(false);
            if (SplineMesh.GetNumberOfControlPoints() == 0)
            {
                UpdateSceneMesh(null);
            }
            else if (SplineMesh.GetNumberOfControlPoints() == 1)
            {
                //display sphere if there is only one control point
                sphereObject.SetActive(true);
                sphereObject.transform.localPosition = SplineMesh.GetControlPoints()[0];
                //update collider
                UpdateSceneMesh(null);
            }
            else if (SplineMesh.GetNumberOfControlPoints() == 2)
            {
                //display linearly interpolated segment if there are two control points
                List<Vector3> controlPoints = SplineMesh.GetControlPoints();
                //set the two control points
                UpdateSceneMesh(LinearSplineMesh.SetControlPoints(controlPoints.ToArray()));
            }
            else
            {
                UpdateSceneMesh(newMesh);
            }
        }
    }
}
