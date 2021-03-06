﻿using Avalonia.Media.Imaging;
using HelperClasses.Gm1Converter;
using System;
using System.Drawing;

namespace Files.Gm1Converter
{
    class Palette
    {
        public readonly static int paletteSize = 5120;

        #region Variables

        private byte[] arrayPaletteByte = new byte[5120];
        private ushort[] arrayPalette = new ushort[2560];
        private WriteableBitmap bitmap;
        #endregion

        #region Construtor
        public Palette(byte[] array)
        {

            Buffer.BlockCopy(array, GM1FileHeader.fileHeaderSize, arrayPaletteByte, 0, paletteSize);

            for (int i = 0; i < arrayPalette.Length; i++)
            {
                this.arrayPalette[i] = BitConverter.ToUInt16(arrayPaletteByte, i * 2);
            }

            PalleteToImG();
        }

        #endregion

        #region GetterSetter

        public ushort[] ArrayPalette { get => arrayPalette; set => arrayPalette = value; }
        public byte[] ArrayPaletteByte { get => arrayPaletteByte; set => arrayPaletteByte = value; }
        public WriteableBitmap Bitmap { get => bitmap; set => bitmap = value; }



        #endregion

        #region Methods

        private unsafe void PalleteToImG()
        {
            int y = 0;
            int x = 0;
            byte r, g, b;
            int pixelGroesse = 16;
            int height = 40 * pixelGroesse;//40*8
            int width =  65 * pixelGroesse;//65*8
  
            bitmap = new WriteableBitmap(new Avalonia.PixelSize(width, height), new Avalonia.Vector(100, 100), Avalonia.Platform.PixelFormat.Bgra8888);// Bgra8888 is device-native and much faster.
            var reihe = new UInt32[65];
          
            using (var buf = bitmap.Lock())
            {
                for (int i = 0; i < arrayPalette.Length; i++)
                {
                    if (i % 65 == 0 && i != 0)
                    {
                        y++;
                        x = 0;
                        for (int k = 0; k < pixelGroesse - 1; k++)
                        {
                            for (int z = 0; z < 65; z++)
                            {
                                for (int t = 0; t < pixelGroesse; t++)
                                {
                                    
                                    var ptr = (uint*)buf.Address;
                                    ptr += (uint)((width * y) + x);
                                    *ptr = reihe[z];
                                    x++;
                                }
                            }
                            y++;
                            x = 0;

                        }

                    }
                    Utility.ReadColor(arrayPalette[i], out r, out g, out b);
                  
                        for (int k = 0; k < pixelGroesse; k++)
                        {
                            UInt32 colorByte = (UInt32)(b | (g << 8) | (r << 16) | (255 << 24));
                        reihe[i%65] = colorByte;
                            var ptr = (uint*)buf.Address;
                            ptr += (uint)((width * y) + x);
                            *ptr = colorByte;
                        x++;
                       
                        }


                    if (i == 2560 - 1)
                    {
                        y++;
                        x = 0;
                        for (int k = 0; k < pixelGroesse - 1; k++)
                        {
                            for (int z = 0; z < 25; z++)
                            {
                                for (int t = 0; t < pixelGroesse; t++)
                                {

                                    var ptr = (uint*)buf.Address;
                                    ptr += (uint)((width * y) + x);
                                    *ptr = reihe[z];
                                    x++;
                                }
                            }
                            y++;
                            x = 0;

                        }

                    }


                }
            }

        }

        #endregion

    }
}
