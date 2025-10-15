using UniRx;

namespace RentTycoon.Renovation
{
    public class GarbageRemoveModel
    {
        public ReactiveCommand OnUpdated = new ReactiveCommand();
        
        public void Updated()
        {
            OnUpdated?.Execute();
        }
    }
}