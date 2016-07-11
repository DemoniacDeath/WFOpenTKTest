using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace WFOpenTKTest
{
    class RenderObject
    {
        public IList<RenderObject> Children { get; private set; }
        public Vector3[] Verticies { get; private set; }
        public Color Color;
        public Vector3 Position;

        public RenderObject(Vector3 position)
        {
            Children = new List<RenderObject>();
            Position = position;
            Color = Color.Empty;
        }

        public RenderObject(Vector3 position, Color color, Vector3[] vectors) : this(position)
        {
            Color = color;
            Verticies = vectors;
        }

        public void Render()
        {
            Render(Vector3.Zero);
        }

        public void Render(Vector3 parentCoordinates)
        {
            Vector3 position = Position + parentCoordinates;
            if (Verticies != null)
            {
                GL.Color3(Color);
                switch (Verticies.Length)
                {
                    case 4:
                        GL.Begin(PrimitiveType.Quads);
                        break;
                    case 3:
                        GL.Begin(PrimitiveType.Triangles);
                        break;
                    case 2:
                    default:
                        GL.Begin(PrimitiveType.Lines);
                        break;
                }
                foreach (Vector3 vector in Verticies)
                {
                    Vector3 transformedVector = vector + position;
                    GL.Vertex3(
                        transformedVector.X,
                        transformedVector.Y,
                        transformedVector.Z);
                }
                GL.End();
            }

            foreach (RenderObject child in Children)
            {
                child.Render(position);
            }
        }

        public RenderObject FindHit(Vector3 start, Vector3 end)
        {
            return FindHit(start, end, Vector3.Zero);
        }

        public RenderObject FindHit(Vector3 start, Vector3 end, Vector3 parentCoordinates)
        {
            Vector3 position = Position + parentCoordinates;

            if (Verticies != null && Verticies.Length == 4)
            {
                Vector3 dS21 = position + (Verticies[1] - Verticies[0]);
                Vector3 dS31 = position + (Verticies[3] - Verticies[0]);
                Vector3 normal = Vector3.Cross(dS21, dS31);

                Vector3 dR = end - start;

                float ndotdR = Vector3.Dot(normal, dR);

                if (Math.Abs(ndotdR) > 0)
                {
                    return this;
                }
            }
            foreach (RenderObject child in Children)
            {
                RenderObject hit = child.FindHit(start, end, position);
                if (hit != null)
                    return hit;
            }
            return null;
        }
    }
}
