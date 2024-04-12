using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using static WoTMapWPF.PathNode;

namespace WoTMapWPF.Graphics
{
    public class Scene
    {
        public const float VERTICAL_UNITS = 100.0f;
        private bool initDone;
        private Path path;
        private int vpMatrixUniformLocation;
        private int textureModeUniformLocation;
        private int fixedColorUniformLocation;
        private Matrix4 viewMatrix = Matrix4.Identity;
        private Matrix4 projectionMatrix = Matrix4.Identity;
        private RouteMarker normalMarker = new RouteMarker(RouteMarker.MarkerType.Normal);
        private RouteMarker normalSelectedMarker = new RouteMarker(RouteMarker.MarkerType.NormalSelected);
        private RouteMarker endMarker = new RouteMarker(RouteMarker.MarkerType.End);
        private RouteMarker endSelectedMarker = new RouteMarker(RouteMarker.MarkerType.EndSelected);
        private int moveMarkerIndex = -1;

        public Scene(GLWpfControl glc, Path path)
        {
            initDone = false;
            GLControl = glc;
            this.path = path;
            ResetCamera();
        }

        public GLWpfControl GLControl { get; private set; }
        public int ModelMatrixULoc { get; private set; }
        public float AspectRatio { get; set; }
        public float PixelsPerUnit { get => (float)GLControl.ActualHeight * Scale / VERTICAL_UNITS; }
        public float CameraTranslateX { get; private set; }
        public float CameraTranslateY { get; private set; }
        public int CameraZoomLevel { get; private set; }
        public double ZoomBase { get; private set; } = 1.2;
        public float Scale { get; private set; }

        public void Paint()
        {
            if (initDone)
                Draw();
            else
                Init();
        }

        public void Resize()
        {
            float widthP = (float)GLControl.ActualWidth;
            float heightP = (float)GLControl.ActualHeight;
            GL.Viewport(0, 0, (int)GLControl.Width, (int)GLControl.Height);
            AspectRatio = widthP / heightP;
            float halfWU = AspectRatio * VERTICAL_UNITS / 2f;
            float halfHU = VERTICAL_UNITS / 2f;
            projectionMatrix = Matrix4.CreateOrthographicOffCenter(-halfWU, halfWU, -halfHU, halfHU, -0.1f, 1f);
        }

        public void Zoom(bool zoomIn, double mouseX, double mouseY)
        {
            (float xu, float yu) = MousePositionToGlCoord(mouseX, mouseY);
            if (zoomIn)
            {
                ++CameraZoomLevel;
                CameraTranslateX += (xu - CameraTranslateX) * (1 - 1 / (float)ZoomBase);
                CameraTranslateY += (yu - CameraTranslateY) * (1 - 1 / (float)ZoomBase);
            }
            else
            {
                --CameraZoomLevel;
                if (CameraZoomLevel < 0)
                {
                    CameraZoomLevel = 0;
                    return;
                }
                CameraTranslateX -= (xu - CameraTranslateX) * ((float)ZoomBase - 1);
                CameraTranslateY -= (yu - CameraTranslateY) * ((float)ZoomBase - 1);
            }
            Scale = (float)Math.Pow(ZoomBase, CameraZoomLevel);
            ComputeViewMatrix();
        }

        public void MoveCamera(double xPixels, double yPixels)
        {
            float xu = (float)xPixels / PixelsPerUnit;
            float yu = (float)yPixels / PixelsPerUnit;
            CameraTranslateX -= xu;
            CameraTranslateY -= yu;
            ComputeViewMatrix();
        }

        public void ResetCamera()
        {
            CameraTranslateX = 0;
            CameraTranslateY = 0;
            CameraZoomLevel = 0;
            Scale = 1;
            ComputeViewMatrix();
        }

        public void AddMarker(double mouseX, double mouseY)
        {
            (float xu, float yu) = MousePositionToGlCoord(mouseX, mouseY);
            if (IsLocationOnMap(xu, yu))
            {
                int markerIndex;
                GLPosition position = new GLPosition(xu, yu);
                if (path.SelectedIndex >= 0 && path.SelectedIndex < path.Nodes.Count)
                    markerIndex = path.SelectedIndex + 1;
                else
                    markerIndex = path.Nodes.Count;
                path.Nodes.Insert(markerIndex, new PathNode { Position = position });
                //select the new marker
                path.SelectedIndex = markerIndex;
            }
            moveMarkerIndex = -1;
        }

        public void HandleClick(double mouseX, double mouseY)
        {
            //select nearby marker
            (float xu, float yu) = MousePositionToGlCoord(mouseX, mouseY);
            int selected = GetNearbyMarkerIndex(xu, yu);
            if (selected >= 0)
                path.SelectedIndex = selected;
            //report move finished if it's the case
            if (moveMarkerIndex >= 0)
            {
                moveMarkerIndex = -1;
                path.OnMoveFinished();
            }
        }

        public void HandleMouseLeave()
        {
            //clear move flag
            moveMarkerIndex = -1;
        }

        public void MoveMarker(double mouseX, double mouseY, double xPixels, double yPixels)
        {
            (float xu, float yu) = MousePositionToGlCoord(mouseX, mouseY);
            int nearby;
            if (moveMarkerIndex >= 0 && moveMarkerIndex < path.Nodes.Count)
                nearby = moveMarkerIndex;
            else
                nearby = GetNearbyMarkerIndex(xu, yu);
            if (nearby >= 0)
            {
                float xuMov = (float)xPixels / PixelsPerUnit + path.Nodes[nearby].Position.X;
                float yuMov = (float)yPixels / PixelsPerUnit + path.Nodes[nearby].Position.Y;
                if (IsLocationOnMap(xuMov, yuMov))
                {
                    GLPosition newPosition = new GLPosition(xuMov, yuMov);
                    path.Nodes[nearby].Position = newPosition;
                }
                moveMarkerIndex = nearby;
            }
        }

        public void SetPath(Path path)
        {
            this.path = path;
        }

        public void ChangeShaderFixedColor(byte r, byte g, byte b, byte a)
        {
            GL.Uniform4(fixedColorUniformLocation, r / 255f, g / 255f, b / 255f, a / 255f);
        }

        private void Draw()
        {
            Matrix4 vpMatrix = viewMatrix * projectionMatrix;
            GL.UniformMatrix4(vpMatrixUniformLocation, false, ref vpMatrix);
            SolidColorBrush brush = (SolidColorBrush)App.Current.Resources["ThemeColorVibrant"];
            Color clearColor = brush.Color;
            GL.ClearColor(clearColor.R / 255f, clearColor.G / 255f, clearColor.B / 255f, clearColor.A / 255f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            short lineStipplePattern = (short)App.Current.Resources["LineStipplePattern"];
            int lineStippleFactor = (int)App.Current.Resources["LineStippleFactor"];
            double lineWidth = (double)App.Current.Resources["LineWidth"];
            GL.LineWidth((float)lineWidth);
            GL.LineStipple(lineStippleFactor, lineStipplePattern);

            GL.Uniform1(textureModeUniformLocation, 1);
            Map.Instance?.Draw(this);

            if (path != null && path.Nodes.Count > 0)
            {
                //draw path lines
                if (path.Nodes.Count > 1)
                {
                    SolidColorBrush lineBrush = (SolidColorBrush)Application.Current.Resources["DashedPathColor"];
                    Color lineColor = lineBrush.Color;
                    GL.Uniform1(textureModeUniformLocation, 0);
                    GL.Uniform4(fixedColorUniformLocation, lineColor.R / 255f, lineColor.G / 255f, lineColor.B / 255f, 1.0f);
                    GL.Begin(PrimitiveType.LineStrip);
                    for (int i = path.Nodes.Count - 1; i >= 0; --i)
                    {
                        GL.Vertex3(path.Nodes[i].Position.X, path.Nodes[i].Position.Y, 0);
                    }
                    GL.End();
                }
                //draw pin markers
                RouteMarker markerToDraw;
                GL.Uniform1(textureModeUniformLocation, 2);
                for (int i = 0; i < path.Nodes.Count - 1; ++i)
                {
                    if (i == path.SelectedIndex)
                        markerToDraw = normalSelectedMarker;
                    else
                        markerToDraw = normalMarker;
                    markerToDraw.PosX = path.Nodes[i].Position.X;
                    markerToDraw.PosY = path.Nodes[i].Position.Y;
                    markerToDraw.Draw(this);
                }
                if (path.SelectedIndex == path.Nodes.Count - 1)
                    markerToDraw = endSelectedMarker;
                else
                    markerToDraw = endMarker;
                markerToDraw.PosX = path.Nodes.Last().Position.X;
                markerToDraw.PosY = path.Nodes.Last().Position.Y;
                markerToDraw.Draw(this);
            }
        }

        private void Init()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.LineStipple);
            GL.Enable(EnableCap.LineSmooth);

            string ff = ReadShaderString("basic_shader.frag");
            string vv = ReadShaderString("basic_shader.vert");
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, ff);
            GL.CompileShader(fragmentShader);
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vv);
            GL.CompileShader(vertexShader);

            string vinfo = GL.GetShaderInfoLog(vertexShader);
            string finfo = GL.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(vinfo) || !string.IsNullOrWhiteSpace(finfo))
                throw new Exception("Shader compile exception");

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            GL.UseProgram(shaderProgram);
            ModelMatrixULoc = GL.GetUniformLocation(shaderProgram, "model_matrix");
            vpMatrixUniformLocation = GL.GetUniformLocation(shaderProgram, "vp_matrix");
            textureModeUniformLocation = GL.GetUniformLocation(shaderProgram, "texture_mode");
            fixedColorUniformLocation = GL.GetUniformLocation(shaderProgram, "fixed_color");

            Resize();

            initDone = true;
        }

        private void ComputeViewMatrix()
        {
            Matrix4 translateM = Matrix4.CreateTranslation(-CameraTranslateX, -CameraTranslateY, 0f);
            Matrix4 scaleM = Matrix4.CreateScale(Scale);
            viewMatrix = translateM * scaleM;
        }

        private string ReadShaderString(string fileName)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WoTMapWPF.Graphics.Shaders." + fileName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private (float glX, float glY) MousePositionToGlCoord(double mouseX, double mouseY)
        {
            float yu = VERTICAL_UNITS / 2;
            float xu = -VERTICAL_UNITS * AspectRatio / 2;
            yu = yu / Scale - (float)mouseY / PixelsPerUnit + CameraTranslateY;
            xu = xu / Scale + (float)mouseX / PixelsPerUnit + CameraTranslateX;
            return (xu, yu);
        }

        private bool IsLocationOnMap(float x, float y)
        {
            float halfMapH = VERTICAL_UNITS / 2;
            float halfMapW = halfMapH * Map.Instance.AspectRatio;
            if (x >= -halfMapW && x <= halfMapW && y >= -halfMapH && y <= halfMapH)
                return true;
            else
                return false;
        }

        private int GetNearbyMarkerIndex(float x, float y)
        {
            double pinSize = (double)App.Current.Resources["PinSize"];
            float markerSize = (float)pinSize / PixelsPerUnit;
            int selected = -1;
            for (int i = path.Nodes.Count - 1; i >= 0; --i)
            {
                //translate marker position to compensate for offset texture (the point is at the center but the icon is shown above)
                float xm = path.Nodes[i].Position.X;
                float ym = path.Nodes[i].Position.Y + markerSize / 4;
                if ((x - xm) * (x - xm) + (y - ym) * (y - ym) < markerSize * markerSize / 16)
                {
                    selected = i;
                    break;
                }
            }
            return selected;
        }
    }
}
