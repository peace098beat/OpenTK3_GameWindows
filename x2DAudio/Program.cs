﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace x2DAudio
{
    /// <summary>
    /// 2D GameWindow Audio サンプル
    /// </summary>
    class Program : OpenTK.GameWindow
    {

        OpenTK.Audio.AudioCapture AudioCapture;
        private int[] wavebuffer;
        private bool RotationRight = true;

        // ワールド座標の外枠
        System.Drawing.Rectangle GlobalArea = new System.Drawing.Rectangle(-500, -500, 1000, 1000);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Program(int width, int height, OpenTK.Graphics.GraphicsMode mode, string title) : base(width, height, mode, title)
        {
            IList<string> devices = OpenTK.Audio.AudioCapture.AvailableDevices;
            foreach (string device in devices)
            {
                Console.WriteLine(device);
            }

            // オーディオキャプチャ
            this.AudioCapture = new OpenTK.Audio.AudioCapture();
            this.AudioCapture.Start();

            // デバッグ
            Console.WriteLine("[INFO] " + this.AudioCapture.AvailableSamples);
            Console.WriteLine("[INFO] " + this.AudioCapture.CurrentDevice);
            Console.WriteLine("[INFO] " + this.AudioCapture.CurrentError);
            Console.WriteLine("[INFO] " + this.AudioCapture.SampleFormat);
            Console.WriteLine("[INFO] " + this.AudioCapture.SampleFrequency);
            Console.WriteLine("[INFO] Start Audio Capture");

        }

        /// <summary>
        /// メイン関数
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Antialias Mode
            using (var game = new Program(800, 600, new OpenTK.Graphics.GraphicsMode(32, 0, 0, 4), "gamewidow"))
            {
                game.Run();
            }
        }

        /// <summary>
        /// ClearColorを設定
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // バッファの初期化
            wavebuffer = new int[4096];

            // Set the clear color to blue
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);



            base.OnLoad(e);
        }

        int Hue1024 = 0;
        float Sat = 0;
        float Value = 0;

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            int x = e.X;
            int y = e.Y;

            Sat = x / (float)(this.Width);
            Value = y / (float)(this.Height);
            float h = Hue1024 / 1024f;
            //var c = OpenTK.Graphics.Color4.FromHsv(new Vector4(h, s, v, 1));
            var c = OpenTK.Graphics.Color4.FromHsv(new Vector4(h, Sat, Value, 1));
            GL.ClearColor(c);

        }

        /// <summary>
        /// リサイズ
        /// (座標について) ワールド座標 -> 正規化デバイス座標 -> デバイス座標系(viewport)
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            this.Title = this.ClientRectangle.ToString();

            /* デバイス座標系での、ビューポートの位置とサイズを指定 */
            GL.Viewport(0, 0, this.Width, this.Height);

            /* 変換行列の初期化 */
            GL.LoadIdentity();

            /* ワールド座標系を切り取る */
            //GL.Ortho(GlobalArea.Left, GlobalArea.Right, GlobalArea.Bottom, GlobalArea.Top, -1, 1);
            GL.Ortho(GlobalArea.Left, GlobalArea.Right, GlobalArea.Top, GlobalArea.Bottom, -1, 1); // Y軸 Flip

            base.OnResize(e);
        }

        /// <summary>
        /// キーボードショートカット
        /// </summary>
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == OpenTK.Input.Key.Escape)
            {
                this.WindowState = WindowState.Normal;
                //this.Exit();
            }

            if (e.Key == OpenTK.Input.Key.F11)
            {
                if (this.WindowState == WindowState.Fullscreen)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Fullscreen;
            }

            if (e.Key == OpenTK.Input.Key.Right)
            {
                // 右回転
                GL.Rotate(-1, 0, 0, 1);
            }

            if (e.Key == OpenTK.Input.Key.Left) { }
        }


        /// <summary>
        /// Called when the frame is updated.
        /// </summary>
        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Hue1024++;

            float h = Hue1024 / 1024f;
            var c = OpenTK.Graphics.Color4.FromHsv(new Vector4(h, Sat, Value, 1));
            GL.ClearColor(c);



            // バッファの初期化
            int sampleCount = 256;
            int bufferLength = sampleCount * 2;

            wavebuffer = new int[sampleCount];

            // バッファのコピー (1024byte = 512sample)
            this.AudioCapture.ReadSamples(wavebuffer, bufferLength);


            // Rotation Right On
            if (this.RotationRight == true)
            {
                //GL.Rotate(-1, 0, 0, 1);
            }


        }


        /// <summary>
        /// Vertex(x,y)の座標はグローバル座標
        /// </summary>
        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Begin(PrimitiveType.LineStrip);
            {
                GL.LineWidth(1);

                GL.Color4(new OpenTK.Graphics.Color4(30, 30, 30, 255));

                GL.Vertex2(GlobalArea.Left, GlobalArea.Top);
                GL.Vertex2(GlobalArea.Right, GlobalArea.Top);
                GL.Vertex2(GlobalArea.Right, GlobalArea.Bottom);
                GL.Vertex2(GlobalArea.Left, GlobalArea.Bottom);

                GL.Vertex2(GlobalArea.Right, GlobalArea.Top);
                GL.Vertex2(GlobalArea.Left, GlobalArea.Top);
                GL.Vertex2(GlobalArea.Right, GlobalArea.Bottom);
            }
            GL.End();

            //  waveform
            GL.Begin(PrimitiveType.Points);
            {
                //GL.LineWidth(0.001f);

                GL.Color4(OpenTK.Graphics.Color4.White);

                int N = wavebuffer.Length;

                GL.Vertex2(this.GlobalArea.Left, 0);

                for (int i = 0; i < N; i++)
                {
                    int v = wavebuffer[i];

                    float x0 = this.GlobalArea.Left;
                    float x1 = this.GlobalArea.Right;
                    float dx = (x1 - x0) / (N - 1.0f);
                    float x = (i * dx) + x0;


                    float y0 = this.GlobalArea.Bottom;
                    float y1 = this.GlobalArea.Top;
                    float dy = (float)(y1 - y0);
                    float r = (float)v / Int32.MaxValue;
                    float y = (float)v / Int32.MaxValue * dy;

                    y = Math.Abs(y);
                    y -= this.GlobalArea.Bottom;


                    // 色


                    // 上半分
                    GL.Vertex2(x, y);
                    // 下半分
                    //GL.Vertex2(x, -1*y);
                }

                GL.Vertex2(this.GlobalArea.Right, 0);


            }
            GL.End();


            this.SwapBuffers();
            base.OnRenderFrame(e);
        }


    }
}
