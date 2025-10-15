using System;
using UnityEngine;
using UniRx;

namespace RentTycoon
{
    public class ButtonCollider : MonoBehaviour
    {
        public ReactiveCommand OnClick = new ReactiveCommand();
        public void OnMouseDown()
        {
            OnClick?.Execute();
        }
    }
}

