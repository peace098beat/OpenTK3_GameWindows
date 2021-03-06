﻿using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace x2DTraspose
{
    public class Game : OpenTK.GameWindow
    {

        Shader shader;
        int VertexBufferObject;     // VBO
        int VertexArrayObject;      // VAO
        int ElementBufferObject;    // EBO
        Texture texture;

        // Uniform
        private Vector2 mouse;


        float[] vertices =
        {
            // position(x,y,z), Texture Coordinates(u,v)
             1.0f, 1.0f, 0.0f, 1.0f, 1.0f, //0 : top right
             1.0f,-1.0f, 0.0f, 1.0f, 0.0f, //1 : btm right
            -1.0f,-1.0f, 0.0f, 0.0f, 0.0f, //2 : btm left
            -1.0f, 1.0f, 0.0f, 0.0f, 1.0f  //3 : top left
        };

        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public Stopwatch sw=new Stopwatch();

        /// <summary>
        /// Constractor
        /// </summary>
        public Game(int width, int height, string title) : base(width, height, OpenTK.Graphics.GraphicsMode.Default, title) { }

        /// <summary>
        /// OnLoad
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            this.sw.Start();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);


            // ---------- VBO
            this.VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject); // VBO
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw); // Position3


            // ---------- EBO
            this.ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject); // Element indices
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);


            // ---------- Shader
            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();

            // ---------- Texture
            texture = new Texture("container.png");
            texture.Use();


            // ---------- VAO Bind
            this.VertexArrayObject = GL.GenVertexArray(); // VAO
            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferObject);

            // ---------- VAO aPosition

            int vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation); // [Vertex Shader] Enable
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0); // [p,p,p,-,-]

            // ---------- VAO aTexCoord

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation); // [Vertex Shader] Enable
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float)); // [-,-,-,t,t]




            // ---------- 

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(mask: ClearBufferMask.ColorBufferBit);

            //// ---------- Uniform

            //float time = (float)this.sw.Elapsed.TotalMilliseconds;
            //GL.Uniform1(GL.GetUniformLocation(shader.Program, "time"), time);

            //Vector2 mouse = this.mouse;
            //GL.Uniform2(GL.GetUniformLocation(shader.Program, "mouse"), mouse);

            //Vector2 resolution = new Vector2(Width, Height);
            //GL.Uniform2(GL.GetUniformLocation(shader.Program, "resolution"), resolution);

            ////vertices[0] += 0.001f;

            //// ---------- VBO
            ////GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject); // VBO
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw); // Position3
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            // ----------- VAO

            GL.BindVertexArray(VertexArrayObject); // VAO

            // ----------- VAO

            shader.Use();
            texture.Use();
            // ----------- 

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            // ----------- 

            this.Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            this.mouse = new Vector2(e.Mouse.X, this.Height - e.Mouse.Y);

            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            this.mouse = new Vector2(e.Mouse.X, this.Height - e.Mouse.Y);

            base.OnMouseUp(e);
        }

        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {

            OpenTK.Input.KeyboardState input = OpenTK.Input.Keyboard.GetState();

            if (input.IsKeyDown(OpenTK.Input.Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteBuffer(ElementBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);

            shader.Dispose();
            texture.Dispose();
            base.OnUnload(e);
        }
    }
}