using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;

namespace WFOpenTKTest
{
    class Main : GameWindow
    {
        private Camera3D camera;

        private RenderObject world;

        private bool mouseLook;

        const float size = 3.0f;
        const float padding = 0.1f;

        public Main() : base(800, 600)
        {
            Title = "OpenTK Test";
            mouseLook = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            camera = new Camera3D(0, 0, 30);

            world = new RenderObject(Vector3.Zero);
            Dictionary<Color, Vector3> sides = new Dictionary<Color, Vector3>();
            sides.Add(Color.Red, new Vector3(1, 0, 0));
            sides.Add(Color.Orange, new Vector3(-1, 0, 0));
            sides.Add(Color.Yellow, new Vector3(0, 1, 0));
            sides.Add(Color.White, new Vector3(0, -1, 0));
            sides.Add(Color.Blue, new Vector3(0, 0, 1));
            sides.Add(Color.Green, new Vector3(0, 0, -1));

            foreach (KeyValuePair<Color, Vector3> side in sides)
            {
                Vector3 position = new Vector3(
                    (1.5f * size + padding) * side.Value.X,
                    (1.5f * size + padding) * side.Value.Y,
                    (1.5f * size + padding) * side.Value.Z
                );

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        Vector3[] vertices = new Vector3[4];

                        for (int c = 0; c <= 3; c++)
                        {
                            float a = i * (size + padding) + size / 2 * ((c == 0 || c == 3) ? 1 : -1);
                            float b = j * (size + padding) + size / 2 * ((c < 2) ? 1 : -1);
                            float x = (side.Value.X == 0) ? a : 0;
                            float y = (side.Value.Y != 0) ? 0 : (side.Value.X == 0) ? b : a;
                            float z = (side.Value.Z == 0) ? b : 0;
                            vertices[c] = new Vector3(x, y, z);
                        }

                        world.Children.Add(
                            new RenderObject(
                                position,
                                side.Key,
                                vertices
                            )
                        );
                    }
                }
            }
            this.MouseMove += new EventHandler<OpenTK.Input.MouseMoveEventArgs>((object sender, OpenTK.Input.MouseMoveEventArgs _e) =>
            {
                if (mouseLook)
                {
                    int dX = _e.XDelta;
                    int dY = _e.YDelta;

                    camera.RotationX += (float)dY / 5.0f;
                    camera.RotationY += (float)dX / 5.0f;
                }
            });
            this.MouseDown += new EventHandler<MouseButtonEventArgs>((object sender, MouseButtonEventArgs _e) =>
            {
                if (_e.Button == MouseButton.Left)
                    mouseLook = true;
                if (_e.Button == MouseButton.Right)
                {
                    MouseRay ray = new MouseRay(_e.Position);
                    RenderObject hitObject = world.FindHit(ray.Start, ray.End);
                    if (hitObject != null)
                        hitObject.Color = Color.Black;

                    world.Children.Add(new RenderObject(
                        Vector3.Zero,
                        Color.Purple,
                        new Vector3[] {
                            ray.Start,
                            ray.End,
                        }
                    ));
                }
            });
            this.MouseUp += new EventHandler<MouseButtonEventArgs>((object sender, MouseButtonEventArgs _e) =>
            {
                if (_e.Button == MouseButton.Left)
                    mouseLook = false;
            });
            this.MouseWheel += new EventHandler<MouseWheelEventArgs>((object sender, MouseWheelEventArgs _e) =>
            {
                camera.Z -= _e.Delta;
                if (camera.Z < 15)
                    camera.Z = 15;
                if (camera.Z > 100)
                    camera.Z = 100;
            });
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            Matrix4 matrix = OpenTK.Matrix4.CreatePerspectiveFieldOfView(1.0f, (Width / (float)Height), 1.0f, 10000.0f);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref matrix);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            KeyboardState state = OpenTK.Input.Keyboard.GetState();
            if (state[Key.Q])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            camera.Update();

            world.Render();

            SwapBuffers();
        }
    }
}
