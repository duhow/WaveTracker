﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WaveTracker.Tracker;
using WaveTracker.Rendering;
using WaveTracker.Audio;

namespace WaveTracker.UI
{
    public class EnvelopeEditor : Clickable
    {
        Envelope envelope;
        Scrollbar scrollbar;
        int envelopeType;
        int playbackStep;
        bool isPlaying;
        Textbox envText;
        Texture2D tex;
        int arpRange = 120;
        int arpHeight = 6;
        NumberBox envLength;
        public int lastEnvType;
        int columnWidth => (envelope.values.Count == 0 ? 0 : Math.Clamp(445 / envelope.values.Count, 1, 48));
        public EnvelopeEditor(int x, int y, Texture2D tex, Element parent)
        {
            this.tex = tex;
            this.x = x;
            this.y = y;
            envText = new Textbox("", -1, 239, 535, 535, this);
            envText.isPartOfInternalDialog = true;
            envText.maxLength = 256;
            scrollbar = new Scrollbar(44, 20, 489, 200, this);
            scrollbar.isPartOfInternalDialog = true;
            scrollbar.coarseStepAmount = 2;
            envLength = new NumberBox("Length", -1, 223, 74, 38, this);
            envLength.isPartOfInternalDialog = true;
            envLength.bDown.isPartOfInternalDialog = true;
            envLength.bUp.isPartOfInternalDialog = true;
            envLength.SetValueLimits(0, 220);
            SetParent(parent);
            ResetScrollbar();

        }

        public void ResetScrollbar()
        {
            scrollbar.scrollValue = arpRange - 16;
        }

        int xPositionOfColumn(int index) { return 46 + index * columnWidth; }
        int yPositionOfValue(int value)
        {
            if (envelopeType == 0 || envelopeType == 3)
            {
                return 21 + (99 - value) * 2;
            }
            else if (envelopeType == 1) // arp
            {
                return 21 + (arpRange - value - scrollbar.scrollValue) * arpHeight;
            }
            else  // pitch
            {
                return 20 + (99 - value);
            }
        }

        public void Update(bool cooldownDone)
        {
            envLength.Value = envelope.values.Count;
            envLength.Update();
            if (envLength.ValueWasChanged)
            {
                if (lastEnvType == envelopeType && cooldownDone)
                    envelope.isActive = true;
                while (envLength.Value < envelope.values.Count)
                    envelope.values.RemoveAt(envelope.values.Count - 1);
                while (envLength.Value > envelope.values.Count)
                    envelope.values.Add(0);
            }
            scrollbar.Update();
            if (envelopeType == 0)
                scrollbar.SetSize(1, 2);
            if (envelopeType == 1)
                scrollbar.SetSize(arpRange * 2 - 1, 200 / arpHeight);
            if (envelopeType == 2)
                scrollbar.SetSize(1, 2);
            if (envelopeType == 3)
                scrollbar.SetSize(1, 2);
            envText.Text = envelope.ToString();
            envText.Update();
            if (envText.ValueWasChanged)
            {
                if (lastEnvType == envelopeType && cooldownDone)
                    envelope.isActive = true;
                envelope.loadFromString(envText.Text);
            }
            envText.Text = envelope.ToString();

            if (PointIsInCanvas(LastClickPos))
            {
                if (Input.GetClick(KeyModifier.None))
                {
                    int x = CanvasMouseBlockClamped().X;

                    envelope.values[Math.Clamp(x, 0, envelope.values.Count - 1)] = CanvasMouseBlockClamped().Y;
                    envelope.isActive = true;
                }
            }
            if (Input.GetClickDown(KeyModifier.None))
            {
                if (MouseEnvelopeY == 1 && MouseEnvelopeX >= 1 && MouseEnvelopeX < envelope.values.Count)
                {
                    if (envelope.releaseIndex != MouseEnvelopeX - 1)
                        envelope.releaseIndex = MouseEnvelopeX - 1;
                    else
                        envelope.releaseIndex = -1;
                }
                if (MouseEnvelopeY == 0 && MouseEnvelopeX >= 0 && MouseEnvelopeX < envelope.values.Count)
                {
                    if (envelope.loopIndex != MouseEnvelopeX)
                        envelope.loopIndex = MouseEnvelopeX;
                    else
                        envelope.loopIndex = -1;
                }
            }
        }

        public Point CanvasMouseBlock()
        {
            int x = (int)Math.Floor((MouseX - 46) / (float)columnWidth);
            int y = 0;
            if (envelopeType == 0 || envelopeType == 3) // vol / wave
            {
                y = 99 - (MouseY - 21) / 2;
            }
            else if (envelopeType == 1) // arp
            {
                y = arpRange - ((MouseY - 21) / arpHeight) - scrollbar.scrollValue;
            }
            else // pitch
            {
                y = 98 - (MouseY - 21);
            }
            return new Point(x, y);
        }

        public Point CanvasMouseBlockClamped()
        {

            int mX = Math.Clamp(MouseX, 45, envelope.values.Count * columnWidth + 41);
            int mY = Math.Clamp(MouseY, 20, 219);

            int x = (int)Math.Floor((mX - 46) / (float)columnWidth);
            int y = 0;

            if (envelopeType == 0 || envelopeType == 3)
            {
                y = 99 - (mY - 21) / 2;
            }
            else if (envelopeType == 1) // arp
            {
                if (mY > 218)
                    mY = 218;
                y = arpRange - ((mY - 21) / arpHeight) - scrollbar.scrollValue;
            }
            else // pitch
            {
                y = 98 - (mY - 21);
            }
            return new Point(x, y);
        }

        public void SetEnvelope(Envelope envelope, int envelopeType)
        {
            if (lastEnvType != this.envelopeType)
                lastEnvType = this.envelopeType;
            this.envelope = envelope;
            this.envelopeType = envelopeType;
        }
        public void Draw()
        {

            bool hovering = false;
            if (envelope != null)
            {
                if (envelopeType == 2)
                    scrollbar.Draw();
                if (envelope.values.Count > 0)
                {
                    if (envelopeType == 0 || envelopeType == 3)
                    {
                        for (int i = 0; i < envelope.values.Count; ++i)
                        {
                            if (i % 2 == 0)
                                DrawSprite(tex, xPositionOfColumn(i), 21, columnWidth, 199, new Rectangle(623, 0, 1, 199));
                            else
                                DrawSprite(tex, xPositionOfColumn(i), 21, columnWidth, 199, new Rectangle(624, 0, 1, 199));

                            if (CanvasMouseBlock().X == i && CanvasMouseBlock().Y > envelope.values[i] && PointIsInCanvas(new Point(MouseX, MouseY)))
                                DrawMouseBlock(new Color(64, 73, 115));


                            if (CanvasMouseBlock().X == i && CanvasMouseBlock().Y <= envelope.values[i] && PointIsInCanvas(new Point(MouseX, MouseY)))
                            {
                                hovering = true;
                                DrawBlock(i, envelope.values[i], new Color(193, 222, 235), true);
                            }
                            else if (playbackStep == i && isPlaying)
                                DrawBlock(i, envelope.values[i], new Color(209, 244, 205), true);

                            else
                                DrawBlock(i, envelope.values[i], Color.White, true);

                        }


                    }
                    if (envelopeType == 1) // arp
                    {
                        for (int i = 0; i < envelope.values.Count; ++i)
                        {
                            if (i % 2 == 0)
                            {
                                DrawSprite(tex, xPositionOfColumn(i), 21, columnWidth, 199, new Rectangle(625, 0, 1, 199));
                                for (int j = arpRange; j > -arpRange; j--)
                                {
                                    if (j % 12 == 0)
                                        DrawBlock(i, j, new Color(31, 36, 63), false);
                                }
                            }
                            else
                            {
                                DrawSprite(tex, xPositionOfColumn(i), 21, columnWidth, 199, new Rectangle(626, 0, 1, 199));
                                for (int j = arpRange; j > -arpRange; j--)
                                {
                                    if (j % 12 == 0)
                                        DrawBlock(i, j, new Color(42, 51, 83), false);
                                }
                            }
                            if (CanvasMouseBlock().X == i && PointIsInCanvas(new Point(MouseX, MouseY)))
                                DrawMouseBlock(new Color(64, 73, 115));

                            if (CanvasMouseBlock().X == i && CanvasMouseBlock().Y == envelope.values[i])
                            {
                                hovering = true;
                                DrawBlock(i, envelope.values[i], new Color(193, 222, 235), true);
                            }
                            else if (playbackStep == i && isPlaying)
                                DrawBlock(i, envelope.values[i], new Color(209, 244, 205), true);
                            else
                                DrawBlock(i, envelope.values[i], Color.White, true);
                        }
                    }
                    else if (envelopeType == 2) // pitch
                    {
                        for (int i = 0; i < envelope.values.Count; ++i)
                        {
                            if (i % 2 == 0)
                            {
                                DrawSprite(tex, xPositionOfColumn(i), 20, columnWidth, 199, new Rectangle(627, 0, 1, 199));

                            }
                            else
                                DrawSprite(tex, xPositionOfColumn(i), 20, columnWidth, 199, new Rectangle(628, 0, 1, 199));

                            Point mouse = CanvasMouseBlock();
                            if (mouse.X == i && PointIsInCanvas(new Point(MouseX, MouseY)))
                                DrawMouseBlock(new Color(64, 73, 115));



                            if (envelope.values[i] >= 0 && CanvasMouseBlock().X == i && CanvasMouseBlock().Y <= envelope.values[i] && PointIsInCanvas(new Point(MouseX, MouseY)) && CanvasMouseBlock().Y >= 0)
                            {
                                hovering = true;
                                DrawBlock(i, envelope.values[i], new Color(193, 222, 235), true);
                            }
                            else if (envelope.values[i] < 0 && CanvasMouseBlock().X == i && CanvasMouseBlock().Y > envelope.values[i] && PointIsInCanvas(new Point(MouseX, MouseY)) && CanvasMouseBlock().Y <= 0)
                            {
                                hovering = true;
                                DrawBlock(i, envelope.values[i], new Color(193, 222, 235), true);
                            }
                            else if (playbackStep == i && isPlaying)
                                DrawBlock(i, envelope.values[i], new Color(209, 244, 205), true);
                            else
                                DrawBlock(i, envelope.values[i], Color.White, true);

                        }
                    }
                    string s = (int)(envelope.values.Count * (1000f / Game1.currentSong.tickRate)) + " ms ";
                    if (hovering)
                    {
                        s += "(" + CanvasMouseBlock().X + ", " + CanvasMouseBlock().Y + ")";

                    }
                    Write(s, 90, 226, ButtonColors.Round.backgroundColor);
                    #region draw loop/release
                    if (MouseEnvelopeY == 1 && MouseEnvelopeX >= 1 && MouseEnvelopeX < envelope.values.Count)
                    {
                        DrawSprite(tex, xPositionOfColumn(MouseEnvelopeX) - 1, 10, new Rectangle(578, 51, 40, 9));
                    }
                    if (envelope.HasRelease)
                    {
                        if (MouseEnvelopeY == 1 && MouseEnvelopeX - 1 == envelope.releaseIndex)
                        { DrawSprite(tex, xPositionOfColumn(MouseEnvelopeX) - 1, 10, new Rectangle(578, 69, 40, 9)); }
                        else { DrawSprite(tex, xPositionOfColumn(envelope.releaseIndex + 1) - 1, 10, new Rectangle(578, 60, 40, 9)); }
                        DrawRect(xPositionOfColumn(envelope.releaseIndex + 1), 19, 1, 201, new Color(255, 137, 51));
                    }
                    if (MouseEnvelopeY == 0 && MouseEnvelopeX >= 0 && MouseEnvelopeX < envelope.values.Count)
                    {
                        DrawSprite(tex, xPositionOfColumn(MouseEnvelopeX) - 2, 0, new Rectangle(578, 24, 40, 9));
                    }
                    if (envelope.HasLoop)
                    {
                        if (MouseEnvelopeY == 0 && MouseEnvelopeX == envelope.loopIndex)
                            DrawSprite(tex, xPositionOfColumn(envelope.loopIndex) - 2, 0, new Rectangle(578, 42, 40, 9));
                        else
                            DrawSprite(tex, xPositionOfColumn(envelope.loopIndex) - 2, 0, new Rectangle(578, 33, 40, 9));
                        DrawRect(xPositionOfColumn(envelope.loopIndex) - 1, 9, 1, 211, new Color(99, 171, 63));
                    }
                    #endregion
                    switch (envelopeType)
                    {
                        case 0:
                        case 3:
                            Write(" 99", 29, 20, Color.White);
                            Write(" 00", 29, 213, Color.White);
                            break;
                        case 1:
                            string valUpper = "" + (arpRange - scrollbar.scrollValue);
                            string valLower = "" + (arpRange - scrollbar.scrollValue - (200 / arpHeight) + 1);
                            while (valUpper.Length < 3)
                                valUpper = " " + valUpper;
                            while (valLower.Length < 3)
                                valLower = " " + valLower;
                            Write(valUpper, 29, 20, Color.White);
                            Write(valLower, 29, 213, Color.White);
                            break;
                        case 2:
                            Write(" 99", 29, 20, Color.White);
                            Write("-100", 29, 213, Color.White);
                            break;
                    }
                }
                envText.Draw();
                envLength.Draw();
            }
        }
        void DrawMouseBlock(Color c)
        {
            DrawBlock(CanvasMouseBlockClamped().X, CanvasMouseBlockClamped().Y, c, false);
        }

        public int MouseEnvelopeX
        {

            get { return (int)Math.Floor((MouseX - 46) / (float)columnWidth); }
        }
        public int MouseEnvelopeY
        {
            get
            {
                if (MouseY >= 0 && MouseY <= 8)
                    return 0;
                if (MouseY >= 10 && MouseY <= 18)
                    return 1;
                return -1;

            }
        }


        bool PointIsInCanvas(Point p)
        {
            if (p.Y > 20 && p.Y < 219)
                if (p.X > 40 && p.X < envelope.values.Count * columnWidth + 44)
                    return true;
            return false;
        }
        void DrawBlock(int i, int val, Color c, bool shadow)
        {
            if (envelopeType == 0 || envelopeType == 3)
            {
                DrawRect(xPositionOfColumn(i), yPositionOfValue(val), columnWidth, val * 2 + 1, c);
                if (shadow)
                    DrawRect(xPositionOfColumn(i) + columnWidth - 1, yPositionOfValue(val), 1, val * 2 + 1, Color.LightGray);
            }
            if (envelopeType == 1)
            {
                if (yPositionOfValue(val) > 20 && yPositionOfValue(val) < 219)
                {
                    DrawRect(xPositionOfColumn(i), yPositionOfValue(val), columnWidth, arpHeight, c);
                    if (shadow)
                        DrawRect(xPositionOfColumn(i) + columnWidth - 1, yPositionOfValue(val), 1, arpHeight, Color.LightGray);
                }
            }
            if (envelopeType == 2) // pitch
            {
                int height = val;
                int y = 0;
                if (val > 0)
                    height++;
                if (val <= 0)
                {
                    y++;
                    height--;
                }
                DrawRect(xPositionOfColumn(i), yPositionOfValue(val) + y, columnWidth, height, c);
                if (shadow)
                    DrawRect(xPositionOfColumn(i) + columnWidth - 1, yPositionOfValue(val) + y, 1, height, Color.LightGray);
            }
        }

        public void EditEnvelope(Envelope envelope, int envelopeType, Channel previewChannel, EnvelopePlayer playback, bool cooldownDone)
        {
            SetEnvelope(envelope, envelopeType);
            playbackStep = playback.step;
            isPlaying = !playback.envelopeEnded;
            scrollbar.isPartOfInternalDialog = true;
            Update(cooldownDone);
        }
    }
}
