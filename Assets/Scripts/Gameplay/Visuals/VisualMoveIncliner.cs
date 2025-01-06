using Scripts.Extensions;
using Scripts.Gameplay.Abillities;
using Scripts.MonoCache;
using UnityEngine;

namespace Scripts.Gameplay.Visuals
{
    public class VisualMoveIncliner : Visual<AbillityMove>, IMonoCacheLateUpdate
    {
        [SerializeField] protected Transform entityTransform;
        [SerializeField] protected Transform bone;
        [Space]
        [SerializeField] protected float inclineAngle = 1f;
        [SerializeField] protected float inclineSpeed = 1f;
        [SerializeField] protected GenVector3<bool> inclineAxis = new(true, true, true);
        [SerializeField] protected GenVector3<bool> invertAxis;
        
        protected Vector3 moveVector;


        public Behaviour Behaviour => this;



        private void Awake()
        {
            MonoCacher.Registrate(this);
        }

        protected virtual void OnEnable()
        {
            if (Abillity != null)
            {
                Abillity.OnMove += OnMove;
                Abillity.OnStopMove += OnStopMove;
            } 
        }

        protected virtual void OnDisable()
        {
            if (Abillity != null)
            {
                Abillity.OnMove -= OnMove;
                Abillity.OnStopMove -= OnStopMove;
            }
        }

        public void LateUpdateCached()
        {
            ApplyBoneRotation();
        }



        protected virtual void OnMove(Vector3 vector)
        {
            moveVector = vector.normalized;
        }

        protected virtual void OnStopMove()
        {
            moveVector = Vector3.zero;
        }



        protected virtual void ApplyBoneRotation()
        {
            // Same as TransformDirection
            Vector3 worldVector = entityTransform.rotation * moveVector;

            print($"MoveVector: {moveVector} \nWorldVector: {worldVector}");
            worldVector = Vector3.Scale(worldVector, inclineAxis.ToInteger(FalsePresentation.Zero));
            worldVector = Vector3.Scale(worldVector, invertAxis.ToInteger(FalsePresentation.MinusOne));
            worldVector *= inclineAngle;

            ReplaceFromMatrix(ref worldVector, bone.rotation.eulerAngles, inclineAxis, true);

            bone.rotation = Quaternion.Lerp(bone.rotation, Quaternion.Euler(worldVector), Time.deltaTime * inclineSpeed);
        }

        protected static void ReplaceFromMatrix(ref Vector3 vector, Vector3 replaceVector, GenVector3<bool> matrix, bool invertMatrix = false)
        {
            if (invertMatrix ? !matrix.x : matrix.x) vector.x = replaceVector.x;
            if (invertMatrix ? !matrix.y : matrix.y) vector.y = replaceVector.y;
            if (invertMatrix ? !matrix.z : matrix.z) vector.z = replaceVector.z;
        }
    }
}
