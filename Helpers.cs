﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveTracker.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace WaveTracker
{
    public class Helpers
    {
        /// <summary>
        /// Returns the name of a note value.
        /// <br></br>
        /// 48 returns "C-4", 51 returns "D#4"<br></br>-1 returns "...", -2 returns "OFF"
        /// </summary>
        /// <param name="noteNum"></param>
        /// <returns></returns>
        public static string GetNoteName(int noteNum)
        {
            if (noteNum == -3) { return "OFF"; }
            if (noteNum == -2) { return "REL"; }
            if (noteNum < 0) { return "..."; }
            int noteWithinOctave = noteNum % 12;
            int octave = noteNum / 12;
            string noteName = "";
            switch (noteWithinOctave)
            {
                case 0:
                    noteName = "C-";
                    break;
                case 1:
                    noteName = "C#";
                    break;
                case 2:
                    noteName = "D-";
                    break;
                case 3:
                    noteName = "D#";
                    break;
                case 4:
                    noteName = "E-";
                    break;
                case 5:
                    noteName = "F-";
                    break;
                case 6:
                    noteName = "F#";
                    break;
                case 7:
                    noteName = "G-";
                    break;
                case 8:
                    noteName = "G#";
                    break;
                case 9:
                    noteName = "A-";
                    break;
                case 10:
                    noteName = "A#";
                    break;
                case 11:
                    noteName = "B-";
                    break;
            }
            return noteName + octave;
        }



        public static string GetEffectCharacter(int num)
        {
            return num switch
            {
                0 => "0",
                1 => "1",
                2 => "2",
                3 => "3",
                4 => "4",
                7 => "7",
                8 => "8",
                9 => "9",
                10 => "Q",
                11 => "R",
                20 => "C",
                21 => "B",
                22 => "D",
                14 => "P",
                15 => "F",
                16 => "V",
                17 => "G",
                18 => "S",
                _ => "-",
            };
        }

        public static int getWidthOfText(string text)
        {
            int ret = 0;
            string chars5 = "WX&%#@~YM«»mw";
            string chars3 = "kI 1-crtT<>{}+\"^\\/?=";
            string chars2 = "(),[]*j";
            string chars1 = "li.'!";

            foreach (char c in text)
            {

                if (chars5.Contains(c))
                    ret += 6;
                else if (chars3.Contains(c))
                    ret += 4;
                else if (chars2.Contains(c))
                    ret += 3;
                else if (chars1.Contains(c))
                    ret += 2;
                else
                    ret += 5;
            }
            return ret - 1;
        }


        public static string FlushString(string original)
        {
            string alphabet = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()_+-={}[]\\|'\":;?/>.<,~`©àèìòùÀÈÌÒÙáéíóúýÁÉÍÓÚÝâêîôûÂÊÎÔÛãñõÃÑÕäëïöüÿÄËÏÖÜŸæçÇ";
            string ret = "";
            foreach (char c in original)
            {
                if (alphabet.Contains(c))
                    ret += c;
            }
            return ret;
        }
        public static bool isEffectHexadecimal(int effectNum)
        {
            return new List<int> { 0, 4, 10, 11, 7 }.Contains(effectNum);
        }

        public static float NoteToFrequency(float noteNum)
        {
            return (float)Math.Pow(2, (noteNum - 57) / 12.0) * 440;

        }

        public static Color Alpha(Color c, int a)
        {
            return new Color(c.R, c.G, c.B, a);
        }

        public static Color LerpColor(Color a, Color b, float amt)
        {
            Color c = Color.White;
            c.R = (byte)(a.R + (b.R - a.R) * amt);
            c.G = (byte)(a.G + (b.G - a.G) * amt);
            c.B = (byte)(a.B + (b.B - a.B) * amt);
            return c;
        }

        public static int GetPianoInput(int currentOctave)
        {

            if (Input.GetKey(Keys.Z, KeyModifier.None)) return currentOctave * 12 + 0;
            if (Input.GetKey(Keys.S, KeyModifier.None)) return currentOctave * 12 + 1;
            if (Input.GetKey(Keys.X, KeyModifier.None)) return currentOctave * 12 + 2;
            if (Input.GetKey(Keys.D, KeyModifier.None)) return currentOctave * 12 + 3;
            if (Input.GetKey(Keys.C, KeyModifier.None)) return currentOctave * 12 + 4;
            if (Input.GetKey(Keys.V, KeyModifier.None)) return currentOctave * 12 + 5;
            if (Input.GetKey(Keys.G, KeyModifier.None)) return currentOctave * 12 + 6;
            if (Input.GetKey(Keys.B, KeyModifier.None)) return currentOctave * 12 + 7;
            if (Input.GetKey(Keys.H, KeyModifier.None)) return currentOctave * 12 + 8;
            if (Input.GetKey(Keys.N, KeyModifier.None)) return currentOctave * 12 + 9;
            if (Input.GetKey(Keys.J, KeyModifier.None)) return currentOctave * 12 + 10;
            if (Input.GetKey(Keys.M, KeyModifier.None)) return currentOctave * 12 + 11;
            if (Input.GetKey(Keys.OemComma, KeyModifier.None)) return currentOctave * 12 + 12;
            if (Input.GetKey(Keys.L, KeyModifier.None)) return currentOctave * 12 + 13;
            if (Input.GetKey(Keys.OemPeriod, KeyModifier.None)) return currentOctave * 12 + 14;
            if (Input.GetKey(Keys.OemSemicolon, KeyModifier.None)) return currentOctave * 12 + 15;
            if (Input.GetKey(Keys.OemQuestion, KeyModifier.None)) return currentOctave * 12 + 16;
            if (Input.GetKey(Keys.Q, KeyModifier.None)) return currentOctave * 12 + 12;
            if (Input.GetKey(Keys.D2, KeyModifier.None)) return currentOctave * 12 + 13;
            if (Input.GetKey(Keys.W, KeyModifier.None)) return currentOctave * 12 + 14;
            if (Input.GetKey(Keys.D3, KeyModifier.None)) return currentOctave * 12 + 15;
            if (Input.GetKey(Keys.E, KeyModifier.None)) return currentOctave * 12 + 16;
            if (Input.GetKey(Keys.R, KeyModifier.None)) return currentOctave * 12 + 17;
            if (Input.GetKey(Keys.D5, KeyModifier.None)) return currentOctave * 12 + 18;
            if (Input.GetKey(Keys.T, KeyModifier.None)) return currentOctave * 12 + 19;
            if (Input.GetKey(Keys.D6, KeyModifier.None)) return currentOctave * 12 + 20;
            if (Input.GetKey(Keys.Y, KeyModifier.None)) return currentOctave * 12 + 21;
            if (Input.GetKey(Keys.D7, KeyModifier.None)) return currentOctave * 12 + 22;
            if (Input.GetKey(Keys.U, KeyModifier.None)) return currentOctave * 12 + 23;
            if (Input.GetKey(Keys.I, KeyModifier.None)) return currentOctave * 12 + 24;
            if (Input.GetKey(Keys.D9, KeyModifier.None)) return currentOctave * 12 + 25;
            if (Input.GetKey(Keys.O, KeyModifier.None)) return currentOctave * 12 + 26;
            if (Input.GetKey(Keys.D0, KeyModifier.None)) return currentOctave * 12 + 27;
            if (Input.GetKey(Keys.P, KeyModifier.None)) return currentOctave * 12 + 28;
            return -1;
        }
        public static int GetPianoInputDown(int currentOctave)
        {

            if (Input.GetKeyDown(Keys.Z, KeyModifier.None)) return currentOctave * 12 + 0;
            if (Input.GetKeyDown(Keys.S, KeyModifier.None)) return currentOctave * 12 + 1;
            if (Input.GetKeyDown(Keys.X, KeyModifier.None)) return currentOctave * 12 + 2;
            if (Input.GetKeyDown(Keys.D, KeyModifier.None)) return currentOctave * 12 + 3;
            if (Input.GetKeyDown(Keys.C, KeyModifier.None)) return currentOctave * 12 + 4;
            if (Input.GetKeyDown(Keys.V, KeyModifier.None)) return currentOctave * 12 + 5;
            if (Input.GetKeyDown(Keys.G, KeyModifier.None)) return currentOctave * 12 + 6;
            if (Input.GetKeyDown(Keys.B, KeyModifier.None)) return currentOctave * 12 + 7;
            if (Input.GetKeyDown(Keys.H, KeyModifier.None)) return currentOctave * 12 + 8;
            if (Input.GetKeyDown(Keys.N, KeyModifier.None)) return currentOctave * 12 + 9;
            if (Input.GetKeyDown(Keys.J, KeyModifier.None)) return currentOctave * 12 + 10;
            if (Input.GetKeyDown(Keys.M, KeyModifier.None)) return currentOctave * 12 + 11;
            if (Input.GetKeyDown(Keys.OemComma, KeyModifier.None)) return currentOctave * 12 + 12;
            if (Input.GetKeyDown(Keys.L, KeyModifier.None)) return currentOctave * 12 + 13;
            if (Input.GetKeyDown(Keys.OemPeriod, KeyModifier.None)) return currentOctave * 12 + 14;
            if (Input.GetKeyDown(Keys.OemSemicolon, KeyModifier.None)) return currentOctave * 12 + 15;
            if (Input.GetKeyDown(Keys.OemQuestion, KeyModifier.None)) return currentOctave * 12 + 16;
            if (Input.GetKeyDown(Keys.Q, KeyModifier.None)) return currentOctave * 12 + 12;
            if (Input.GetKeyDown(Keys.D2, KeyModifier.None)) return currentOctave * 12 + 13;
            if (Input.GetKeyDown(Keys.W, KeyModifier.None)) return currentOctave * 12 + 14;
            if (Input.GetKeyDown(Keys.D3, KeyModifier.None)) return currentOctave * 12 + 15;
            if (Input.GetKeyDown(Keys.E, KeyModifier.None)) return currentOctave * 12 + 16;
            if (Input.GetKeyDown(Keys.R, KeyModifier.None)) return currentOctave * 12 + 17;
            if (Input.GetKeyDown(Keys.D5, KeyModifier.None)) return currentOctave * 12 + 18;
            if (Input.GetKeyDown(Keys.T, KeyModifier.None)) return currentOctave * 12 + 19;
            if (Input.GetKeyDown(Keys.D6, KeyModifier.None)) return currentOctave * 12 + 20;
            if (Input.GetKeyDown(Keys.Y, KeyModifier.None)) return currentOctave * 12 + 21;
            if (Input.GetKeyDown(Keys.D7, KeyModifier.None)) return currentOctave * 12 + 22;
            if (Input.GetKeyDown(Keys.U, KeyModifier.None)) return currentOctave * 12 + 23;
            if (Input.GetKeyDown(Keys.I, KeyModifier.None)) return currentOctave * 12 + 24;
            if (Input.GetKeyDown(Keys.D9, KeyModifier.None)) return currentOctave * 12 + 25;
            if (Input.GetKeyDown(Keys.O, KeyModifier.None)) return currentOctave * 12 + 26;
            if (Input.GetKeyDown(Keys.D0, KeyModifier.None)) return currentOctave * 12 + 27;
            if (Input.GetKeyDown(Keys.P, KeyModifier.None)) return currentOctave * 12 + 28;
            return -1;
        }
        public static int GetPianoInputUp(int currentOctave)
        {

            if (Input.GetKeyUp(Keys.Z, KeyModifier.None)) return currentOctave * 12 + 0;
            if (Input.GetKeyUp(Keys.S, KeyModifier.None)) return currentOctave * 12 + 1;
            if (Input.GetKeyUp(Keys.X, KeyModifier.None)) return currentOctave * 12 + 2;
            if (Input.GetKeyUp(Keys.D, KeyModifier.None)) return currentOctave * 12 + 3;
            if (Input.GetKeyUp(Keys.C, KeyModifier.None)) return currentOctave * 12 + 4;
            if (Input.GetKeyUp(Keys.V, KeyModifier.None)) return currentOctave * 12 + 5;
            if (Input.GetKeyUp(Keys.G, KeyModifier.None)) return currentOctave * 12 + 6;
            if (Input.GetKeyUp(Keys.B, KeyModifier.None)) return currentOctave * 12 + 7;
            if (Input.GetKeyUp(Keys.H, KeyModifier.None)) return currentOctave * 12 + 8;
            if (Input.GetKeyUp(Keys.N, KeyModifier.None)) return currentOctave * 12 + 9;
            if (Input.GetKeyUp(Keys.J, KeyModifier.None)) return currentOctave * 12 + 10;
            if (Input.GetKeyUp(Keys.M, KeyModifier.None)) return currentOctave * 12 + 11;
            if (Input.GetKeyUp(Keys.OemComma, KeyModifier.None)) return currentOctave * 12 + 12;
            if (Input.GetKeyUp(Keys.L, KeyModifier.None)) return currentOctave * 12 + 13;
            if (Input.GetKeyUp(Keys.OemPeriod, KeyModifier.None)) return currentOctave * 12 + 14;
            if (Input.GetKeyUp(Keys.OemSemicolon, KeyModifier.None)) return currentOctave * 12 + 15;
            if (Input.GetKeyUp(Keys.OemQuestion, KeyModifier.None)) return currentOctave * 12 + 16;
            if (Input.GetKeyUp(Keys.Q, KeyModifier.None)) return currentOctave * 12 + 12;
            if (Input.GetKeyUp(Keys.D2, KeyModifier.None)) return currentOctave * 12 + 13;
            if (Input.GetKeyUp(Keys.W, KeyModifier.None)) return currentOctave * 12 + 14;
            if (Input.GetKeyUp(Keys.D3, KeyModifier.None)) return currentOctave * 12 + 15;
            if (Input.GetKeyUp(Keys.E, KeyModifier.None)) return currentOctave * 12 + 16;
            if (Input.GetKeyUp(Keys.R, KeyModifier.None)) return currentOctave * 12 + 17;
            if (Input.GetKeyUp(Keys.D5, KeyModifier.None)) return currentOctave * 12 + 18;
            if (Input.GetKeyUp(Keys.T, KeyModifier.None)) return currentOctave * 12 + 19;
            if (Input.GetKeyUp(Keys.D6, KeyModifier.None)) return currentOctave * 12 + 20;
            if (Input.GetKeyUp(Keys.Y, KeyModifier.None)) return currentOctave * 12 + 21;
            if (Input.GetKeyUp(Keys.D7, KeyModifier.None)) return currentOctave * 12 + 22;
            if (Input.GetKeyUp(Keys.U, KeyModifier.None)) return currentOctave * 12 + 23;
            if (Input.GetKeyUp(Keys.I, KeyModifier.None)) return currentOctave * 12 + 24;
            if (Input.GetKeyUp(Keys.D9, KeyModifier.None)) return currentOctave * 12 + 25;
            if (Input.GetKeyUp(Keys.O, KeyModifier.None)) return currentOctave * 12 + 26;
            if (Input.GetKeyUp(Keys.D0, KeyModifier.None)) return currentOctave * 12 + 27;
            if (Input.GetKeyUp(Keys.P, KeyModifier.None)) return currentOctave * 12 + 28;
            return -1;
        }

        public static bool isNoteBlackKey(int noteNum)
        {
            switch (noteNum % 12)
            {
                case 0:
                case 2:
                case 4:
                case 5:
                case 7:
                case 9:
                case 11:
                    return false;
            }
            return true;
        }

        public static bool readWav(string filepath, out List<float> L, out List<float> R)
        {
            L = new List<float>();
            R = new List<float>();
            try
            {
                AudioFileReader Nreader = new AudioFileReader(filepath);
                ISampleProvider isp = Nreader.ToSampleProvider();
                float[] buffer = new float[Nreader.Length / 2];
                isp.Read(buffer, 0, buffer.Length);
                bool mono = isp.WaveFormat.Channels == 1;
                bool zero = true;
                for (int s = 0, v = 0; v < buffer.Length; s++)
                {
                    if (s > 44100 * 60)
                        break;
                    if (!zero)
                    {
                        L.Add(buffer[v++]);
                        R.Add(buffer[v++]);
                    }
                    else
                    {
                        if (buffer[v++] != 0)
                            zero = false;
                        if (buffer[v++] != 0)
                            zero = false;
                    }
                }
                if (mono)
                    R.Clear();
                return true;
            }
            catch
            {
                L.Add(0f);
                R.Add(0f);
                return false;
            }
        }

        public int FileNextInt(FileStream stream)
        {
            byte[] arr = new byte[4];
            for (int i = 0; i < 4; ++i)
            {
                arr[i] = (byte)stream.ReadByte();
            }
            return BitConverter.ToInt32(arr);
        }

        public short FileNextShort(FileStream stream)
        {
            byte[] arr = new byte[2];
            for (int i = 0; i < 4; ++i)
            {
                arr[i] = (byte)stream.ReadByte();
            }
            return BitConverter.ToInt16(arr);
        }

        public string FileNextString(FileStream stream)
        {
            int length = FileNextInt(stream);
            byte[] arr = new byte[2];
            string ret = "";
            for (int i = 0; i < length; ++i)
            {
                ret += BitConverter.ToChar(new byte[2] { (byte)stream.ReadByte(), (byte)stream.ReadByte() });
            }
            return ret;
        }
    }

}
