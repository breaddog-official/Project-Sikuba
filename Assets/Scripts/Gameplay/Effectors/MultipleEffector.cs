using UnityEngine;
using System;

namespace Scripts.Gameplay
{
    public class MultipleEffector : Effector
    {
        [SerializeField] protected Effector[] effectors;

        protected override void PlayEffect()
        {
            foreach (var effector in effectors)
            {
                try
                {
                    effector.Play();
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);
                    continue;
                }
            }
        }
    }
}
