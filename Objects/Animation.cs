namespace Talos.Objects
{
    internal record Animation
    {
        internal int TargetID { get; set; }
        internal int SourceID { get; set; }
        internal ushort TargetAnimation { get; set; }
        internal ushort SourceAnimation { get; set; }
        internal short AnimationSpeed { get; set; }

        internal Animation(int targetID, int sourceID, ushort targetAnimation, ushort sourceAnimation, short animationSpeed)
        {
            TargetID = targetID;
            SourceID = sourceID;
            TargetAnimation = targetAnimation;
            SourceAnimation = sourceAnimation;
            AnimationSpeed = animationSpeed;
        }
    }
}
