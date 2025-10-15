using UniRx;
using UnityEngine;

namespace RentTycoon.Renovation
{
    public class GrassRenovationWorldStep : RenovationWorldStep
    {
        public ReactiveCommand OnCompleted { get; set; }

        public override void CompleteStep()
        {
            base.CompleteStep();
            Debug.Log("GrassStep completed.");
        }
    }
}