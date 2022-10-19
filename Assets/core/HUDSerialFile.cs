namespace GameHUD
{
    [System.Serializable]
    public class SpriteInfo
    {
        public string Name;

        public float yMax;
        public float xMax;
        public float yMin;
        public float xMin;
        public int Width;
        public int Height;
         [System.NonSerialized]
        public int MatIndex;
        [System.NonSerialized]
        public UnityEngine.Material Mat;
       
    }
}
