using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Resources;

namespace WoTMapWPF.Graphics
{
    internal class TextureLoader
    {
        public static readonly Dictionary<string, int> LoadedTextures = new();

        public static int LoadTexture(string path, out int width, out int height)
        {
            width = 0; height = 0;
            if (LoadedTextures.ContainsKey(path))
                return LoadedTextures[path];
            if (path == null || !File.Exists(path))
                return 0;
            try
            {
                Bitmap textureBitmap = new Bitmap(path);
                width = textureBitmap.Width;
                height = textureBitmap.Height;
                BitmapData TextureData =
                        textureBitmap.LockBits(
                        new Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.GenTextures(1, out int textureId);
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textureBitmap.Width, textureBitmap.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, TextureData.Scan0);
                textureBitmap.UnlockBits(TextureData);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                LoadedTextures[path] = textureId;
                return textureId;
            }
            catch
            {
                return 0;
            }
        }

        public static int LoadTexture(Uri uri, out int width, out int height)
        {
            width = 0; height = 0;
            if (uri == null || uri.OriginalString == null)
                return 0;
            if (LoadedTextures.ContainsKey(uri.OriginalString))
                return LoadedTextures[uri.OriginalString];
            try
            {
                StreamResourceInfo sri = App.GetResourceStream(uri);
                if (sri == null)
                    return 0;
                using (Stream stream = sri.Stream)
                {
                    Bitmap textureBitmap = new Bitmap(stream);
                    width = textureBitmap.Width;
                    height = textureBitmap.Height;
                    BitmapData TextureData =
                            textureBitmap.LockBits(
                            new Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
                            ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.GenTextures(1, out int textureId);
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textureBitmap.Width, textureBitmap.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, TextureData.Scan0);
                    textureBitmap.UnlockBits(TextureData);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    LoadedTextures[uri.OriginalString] = textureId;
                    return textureId;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static int LoadTexture(string path)
        {
            return LoadTexture(path, out int w, out int h);
        }

        public static int LoadTexture(Uri uri)
        {
            return LoadTexture(uri, out int w, out int h);
        }

        public static void DeleteTexture(string path)
        {
            if (LoadedTextures.ContainsKey(path))
            {
                int textureId = LoadedTextures[path];
                GL.DeleteTexture(textureId);
                LoadedTextures.Remove(path);
            }
        }
    }
}
