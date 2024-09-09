namespace FlyEngine
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewWeights", menuName = "Weights", order = 1)]
    public class Weight : ScriptableObject
    {
        public int dirtWeight;
        public int roadWeight;
        public int mountainWeight;
        public int streamWeight;
    }
}