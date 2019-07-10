using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    interface IMayRequirePosition
    {
        NeededCoordinateSpace RequiresPosition();
    }

    public static class MayRequirePositionExtensions
    {
        public static NeededCoordinateSpace RequiresPosition(this ISlot slot)
        {
            var mayRequirePosition = slot as IMayRequirePosition;
            return mayRequirePosition != null ? mayRequirePosition.RequiresPosition() : NeededCoordinateSpace.None;
        }
    }
}
