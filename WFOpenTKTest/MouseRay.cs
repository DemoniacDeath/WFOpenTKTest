using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace WFOpenTKTest
{
    public class MouseRay
    {
        private Vector3 _start;
        private Vector3 _end;

        public Vector3 Start { get { return _start; } }

        public Vector3 End { get { return _end; } }

        public MouseRay(Point mouse)
            : this(mouse.X, mouse.Y)
        {
        }

        public MouseRay(int x, int y)
        {
            int[] viewport = new int[4];
            Matrix4 modelMatrix, projMatrix;

            GL.GetFloat(GetPName.ModelviewMatrix, out modelMatrix);
            GL.GetFloat(GetPName.ProjectionMatrix, out projMatrix);
            GL.GetInteger(GetPName.Viewport, viewport);

            _start = new Vector3(x, y, 0.0f).UnProject(projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
            _end = new Vector3(x, y, 1.0f).UnProject(projMatrix, modelMatrix, new Size(viewport[2], viewport[3]));
        }
    }
}
