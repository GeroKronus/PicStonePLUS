using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PicStonePlus.Models;

namespace PicStonePlus.Processing
{
    /// <summary>
    /// Pós-produção de imagem portada do PicStone (FrmPrincipal.PosProducao).
    /// 5 estágios na ordem exata: HSL, ColorMatrix, Sombras, Gamma, Tonalidade.
    /// </summary>
    public static class PostProduction
    {
        /// <summary>
        /// Verifica se o preset tem algum valor de pós-produção diferente de zero.
        /// </summary>
        public static bool HasPostProduction(MaterialPreset preset)
        {
            if (preset == null) return false;
            return preset.Brilho != 0 || preset.Contraste != 0 || preset.Sombras != 0 ||
                   preset.Vermelho != 0 || preset.Verde != 0 || preset.Azul != 0 ||
                   preset.Saturacao != 0 || preset.Matiz != 0 ||
                   preset.Gama != 0 || preset.Tonalidade != 0;
        }

        /// <summary>
        /// Aplica os 5 estágios de pós-produção à imagem (idêntico ao PicStone).
        /// </summary>
        public static Bitmap Apply(Image source, MaterialPreset preset)
        {
            if (!HasPostProduction(preset))
                return new Bitmap(source);

            int varMatiz = preset.Matiz;
            if (varMatiz < 0) varMatiz = 360 + varMatiz;

            decimal saturation = preset.Saturacao * 3.0m;
            decimal lightness = 0.0m;
            decimal hue = varMatiz;

            const double c1o60 = 1.0 / 60.0;
            const double c1o255 = 1.0 / 255.0;

            // Copia a imagem original
            Bitmap result = new Bitmap(source);
            result.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            // ═══════════════════════════════════════════════════
            // ESTÁGIO 1: HSL pixel-a-pixel (Matiz + Saturação)
            // ═══════════════════════════════════════════════════
            if (preset.Matiz != 0 || preset.Saturacao != 0)
            {
                BitmapData bmpData = null;
                try
                {
                    bmpData = result.LockBits(
                        new Rectangle(0, 0, result.Width, result.Height),
                        ImageLockMode.ReadWrite, source.PixelFormat);
                    int pixelBytes = Image.GetPixelFormatSize(source.PixelFormat) / 8;
                    IntPtr ptr = bmpData.Scan0;
                    int size = bmpData.Stride * result.Height;
                    byte[] pixels = new byte[size];

                    Marshal.Copy(ptr, pixels, 0, size);

                    for (int row = 0; row < result.Height; row++)
                    {
                        for (int col = 0; col < result.Width; col++)
                        {
                            int index = (row * bmpData.Stride) + (col * pixelBytes);
                            double R = pixels[index + 2];
                            double G = pixels[index + 1];
                            double B = pixels[index + 0];
                            double min = Math.Min(R, Math.Min(G, B));
                            double max = Math.Max(R, Math.Max(G, B));
                            double dif = max - min;
                            double sum = max + min;
                            double L = 0.5 * sum;
                            double H = 0.0;
                            double S = 0.0;
                            double f1 = 0.0, f2 = G - B;

                            if (G > R)
                            {
                                if (G > B)
                                {
                                    max = G; f1 = 120.0; f2 = B - R;
                                }
                            }
                            if (B > R && B > G)
                            {
                                max = B; f1 = 240.0; f2 = R - G;
                            }

                            if (dif == 0)
                            {
                                H = 0.0; S = 0.0;
                            }
                            else
                            {
                                if (L < 127.5)
                                    S = 255.0 * dif / sum;
                                else
                                    S = 255.0 * dif / (510.0 - sum);

                                H = (f1 + 60.0 * f2 / dif);
                                if (H < 0.0) H += 360.0;
                                if (H >= 360.0) H -= 360.0;
                            }

                            // Aplicar transformações
                            H = H + (double)hue;
                            if (H >= 360.0) H -= 360.0;
                            S = S + (127 * (double)saturation / 100);
                            if (S < 0.0) S = 0.0;
                            if (S > 255.0) S = 255.0;
                            L = L + (127 * (double)lightness / 100);
                            if (L < 0.0) L = 0.0;
                            if (L > 255.0) L = 255.0;

                            // HSL → RGB
                            if (S == 0)
                            {
                                R = L; G = L; B = L;
                            }
                            else
                            {
                                double v2;
                                if (L < 127.5)
                                    v2 = c1o255 * L * (255 + S);
                                else
                                    v2 = L + S - c1o255 * S * L;

                                double v1 = 2 * L - v2;

                                // Red
                                double H1 = H + 120.0;
                                if (H1 >= 360.0) H1 -= 360.0;
                                R = GetColorComponent(v1, v2, v2 - v1, H1, c1o60);

                                // Green
                                H1 = H;
                                G = GetColorComponent(v1, v2, v2 - v1, H1, c1o60);

                                // Blue
                                H1 = H - 120.0;
                                if (H1 < 0.0) H1 += 360.0;
                                B = GetColorComponent(v1, v2, v2 - v1, H1, c1o60);
                            }

                            pixels[index + 2] = (byte)Math.Min(Math.Max(R, 0), 255);
                            pixels[index + 1] = (byte)Math.Min(Math.Max(G, 0), 255);
                            pixels[index + 0] = (byte)Math.Min(Math.Max(B, 0), 255);
                        }
                    }

                    Marshal.Copy(pixels, 0, ptr, size);
                }
                finally
                {
                    if (bmpData != null)
                        result.UnlockBits(bmpData);
                }
            }

            // ═══════════════════════════════════════════════════
            // ESTÁGIO 2: ColorMatrix (Contraste + Brilho + R/G/B)
            // ═══════════════════════════════════════════════════
            if (preset.Contraste != 0 || preset.Brilho != 0 ||
                preset.Vermelho != 0 || preset.Verde != 0 || preset.Azul != 0)
            {
                decimal stepContraste = 0.05m;
                decimal stepBrilho = 0.025m;
                decimal stepCor = 0.01m;

                float contraste = (float)((preset.Contraste * stepContraste) + 1);
                float brilho = (float)(preset.Brilho * stepBrilho);

                float[][] colorMatrixVal = new float[][] {
                    new float[] { contraste + (float)(preset.Vermelho * stepCor), 0, 0, 0, 0 },
                    new float[] { 0, contraste + (float)(preset.Verde * stepCor), 0, 0, 0 },
                    new float[] { 0, 0, contraste + (float)(preset.Azul * stepCor), 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { brilho, brilho, brilho, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixVal);
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    using (Graphics gr = Graphics.FromImage(result))
                    {
                        gr.DrawImage(result,
                            new Rectangle(0, 0, result.Width, result.Height),
                            0, 0, result.Width, result.Height,
                            GraphicsUnit.Pixel, ia);
                    }
                }
            }

            // ═══════════════════════════════════════════════════
            // ESTÁGIO 3: ColorMatrix Sombras
            // ═══════════════════════════════════════════════════
            if (preset.Sombras != 0)
            {
                decimal stepContraste = 0.05m;
                decimal stepBrilho = 0.025m;
                decimal stepCor = 0.01m;

                float contrasteSombras = (float)(((decimal)preset.Sombras / 2) * stepContraste + 1);
                float brilhoSombras = (float)((-preset.Sombras) * stepBrilho);

                float[][] colorMatrixVal = new float[][] {
                    new float[] { contrasteSombras + (float)(preset.Vermelho * stepCor), 0, 0, 0, 0 },
                    new float[] { 0, contrasteSombras + (float)(preset.Verde * stepCor), 0, 0, 0 },
                    new float[] { 0, 0, contrasteSombras + (float)(preset.Azul * stepCor), 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { brilhoSombras, brilhoSombras, brilhoSombras, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixVal);
                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    using (Graphics gr = Graphics.FromImage(result))
                    {
                        gr.DrawImage(result,
                            new Rectangle(0, 0, result.Width, result.Height),
                            0, 0, result.Width, result.Height,
                            GraphicsUnit.Pixel, ia);
                    }
                }
            }

            // ═══════════════════════════════════════════════════
            // ESTÁGIO 4: Gamma (LockBits)
            // ═══════════════════════════════════════════════════
            if (preset.Gama != 0)
            {
                double gammaValue;
                if (preset.Gama > 0)
                    gammaValue = 1.0 - (preset.Gama / 50.0) * 0.7;
                else
                    gammaValue = 1.0 - (preset.Gama / 50.0) * 2.0;

                gammaValue = Math.Max(0.3, Math.Min(gammaValue, 3.0));

                if (Math.Abs(gammaValue - 1.0) > 0.01)
                {
                    BitmapData bmpGamaData = null;
                    try
                    {
                        bmpGamaData = result.LockBits(
                            new Rectangle(0, 0, result.Width, result.Height),
                            ImageLockMode.ReadWrite, result.PixelFormat);
                        int pixelBytesGama = Image.GetPixelFormatSize(result.PixelFormat) / 8;
                        IntPtr ptrGama = bmpGamaData.Scan0;
                        int sizeGama = bmpGamaData.Stride * result.Height;
                        byte[] pixelsGama = new byte[sizeGama];

                        Marshal.Copy(ptrGama, pixelsGama, 0, sizeGama);

                        for (int i = 0; i < sizeGama; i += pixelBytesGama)
                        {
                            pixelsGama[i + 0] = (byte)Math.Min(255, Math.Max(0,
                                255 * Math.Pow(pixelsGama[i + 0] / 255.0, gammaValue)));
                            pixelsGama[i + 1] = (byte)Math.Min(255, Math.Max(0,
                                255 * Math.Pow(pixelsGama[i + 1] / 255.0, gammaValue)));
                            pixelsGama[i + 2] = (byte)Math.Min(255, Math.Max(0,
                                255 * Math.Pow(pixelsGama[i + 2] / 255.0, gammaValue)));
                        }

                        Marshal.Copy(pixelsGama, 0, ptrGama, sizeGama);
                    }
                    finally
                    {
                        if (bmpGamaData != null)
                            result.UnlockBits(bmpGamaData);
                    }
                }
            }

            // ═══════════════════════════════════════════════════
            // ESTÁGIO 5: Tonalidade (LockBits) - só pixels com L < 128
            // ═══════════════════════════════════════════════════
            if (preset.Tonalidade != 0)
            {
                BitmapData bmpTonalData = null;
                try
                {
                    bmpTonalData = result.LockBits(
                        new Rectangle(0, 0, result.Width, result.Height),
                        ImageLockMode.ReadWrite, result.PixelFormat);
                    int pixelBytesTonal = Image.GetPixelFormatSize(result.PixelFormat) / 8;
                    IntPtr ptrTonal = bmpTonalData.Scan0;
                    int sizeTonal = bmpTonalData.Stride * result.Height;
                    byte[] pixelsTonal = new byte[sizeTonal];
                    Marshal.Copy(ptrTonal, pixelsTonal, 0, sizeTonal);

                    double fator = 1.0 + (preset.Tonalidade / 200.0);
                    for (int row = 0; row < result.Height; row++)
                    {
                        for (int col = 0; col < result.Width; col++)
                        {
                            int index = (row * bmpTonalData.Stride) + (col * pixelBytesTonal);
                            double R = pixelsTonal[index + 2];
                            double G = pixelsTonal[index + 1];
                            double B = pixelsTonal[index + 0];

                            double min = Math.Min(R, Math.Min(G, B));
                            double max = Math.Max(R, Math.Max(G, B));
                            double sum = max + min;
                            double L = 0.5 * sum;

                            if (L < 128)
                            {
                                L = Math.Max(0, Math.Min(255, L * fator));
                                double dif = max - min;
                                double S;
                                double H;
                                double f1 = 0.0, f2 = G - B;

                                if (G > R)
                                {
                                    if (G > B)
                                    {
                                        max = G; f1 = 120.0; f2 = B - R;
                                    }
                                }
                                if (B > R && B > G)
                                {
                                    max = B; f1 = 240.0; f2 = R - G;
                                }

                                if (dif == 0)
                                {
                                    H = 0.0; S = 0.0;
                                }
                                else
                                {
                                    if (L < 127.5)
                                        S = 255.0 * dif / sum;
                                    else
                                        S = 255.0 * dif / (510.0 - sum);

                                    H = (f1 + 60.0 * f2 / dif);
                                    if (H < 0.0) H += 360.0;
                                    if (H >= 360.0) H -= 360.0;
                                }

                                double v2;
                                if (L < 127.5)
                                    v2 = (L / 255.0) * (255 + S);
                                else
                                    v2 = L + S - (S * L / 255.0);

                                double v1 = 2 * L - v2;
                                double c1o60local = 1.0 / 60.0;

                                // Red
                                double H1 = H + 120.0;
                                if (H1 >= 360.0) H1 -= 360.0;
                                R = GetColorComponent(v1, v2, v2 - v1, H1, c1o60local);

                                // Green
                                H1 = H;
                                G = GetColorComponent(v1, v2, v2 - v1, H1, c1o60local);

                                // Blue
                                H1 = H - 120.0;
                                if (H1 < 0.0) H1 += 360.0;
                                B = GetColorComponent(v1, v2, v2 - v1, H1, c1o60local);

                                pixelsTonal[index + 2] = (byte)Math.Min(Math.Max(R, 0), 255);
                                pixelsTonal[index + 1] = (byte)Math.Min(Math.Max(G, 0), 255);
                                pixelsTonal[index + 0] = (byte)Math.Min(Math.Max(B, 0), 255);
                            }
                        }
                    }

                    Marshal.Copy(pixelsTonal, 0, ptrTonal, sizeTonal);
                }
                finally
                {
                    if (bmpTonalData != null)
                        result.UnlockBits(bmpTonalData);
                }
            }

            return result;
        }

        /// <summary>
        /// Helper HSL→RGB (idêntico ao PicStone GetColorComponent).
        /// </summary>
        private static double GetColorComponent(double v1, double v2, double v3, double H, double c1o60)
        {
            if (H < 0) H += 360;
            if (H < 60.0)
                return v1 + v3 * H * c1o60;
            else if (H < 180.0)
                return v2;
            else if (H < 240.0)
                return v1 + v3 * (4 - H * c1o60);
            else
                return v1;
        }
    }
}
