using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.SSMQoLMod.Commands
{
    public static class CameraOffsetCommands
    {
        [Command("camera_offset_x", "Sets horizontal camera offset")]
        public static void CmdCameraOffsetX(float x)
        {
            x *= 48f;
            (Engine.Scene as Level).CameraOffset.X = x;
        }

        [Command("camera_offset_y", "Sets vertical camera offset")]
        public static void CmdCameraOffsetY(float y)
        {
            y *= 32f;
            (Engine.Scene as Level).CameraOffset.Y = y;
        }

        [Command("camera_offset", "Sets camera offset")]
        public static void CmdCameraOffset(float x, float y)
        {
            x *= 48f;
            y *= 32f;
            (Engine.Scene as Level).CameraOffset = new Vector2(x, y);
        }

        [Command("cx", "Sets horizontal camera offset")]
        public static void CmdCX(float x) => CmdCameraOffsetX(x);
        [Command("cy", "Sets vertical camera offset")]
        public static void CmdCY(float y) => CmdCameraOffsetY(y);
        [Command("c", "Sets camera offset")]
        public static void CmdC(float x, float y) => CmdCameraOffset(x, y);
    }
}
