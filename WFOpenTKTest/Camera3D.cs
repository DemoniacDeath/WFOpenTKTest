using System;
using OpenTK.Graphics.OpenGL;

namespace WFOpenTKTest
{
    class Camera3D
    {
        public enum Direction
        {
            Forward,
            Backward,
            Left,
            Right,
            Up,
            Down
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public Camera3D(float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Update()
        {
            GL.Translate(-X, -Y, -Z);
            GL.Rotate(RotationX, 1, 0, 0);
            GL.Rotate(RotationY, 0, 1, 0);
            GL.Rotate(RotationZ, 0, 0, 1);
        }
    }
}
