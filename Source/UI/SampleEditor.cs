﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveTracker.Tracker;
using WaveTracker.Audio;

namespace WaveTracker.UI {
    public class SampleEditor : Element {
        /// <summary>
        /// The sample to edit
        /// </summary>
        public Sample Sample { get; set; }

        MouseRegion waveformRegion;
        int mouseSampleIndex;
        int mouseSampleIndexClamped;
        int selectionStartIndex;
        int selectionEndIndex;
        bool SelectionIsActive { get; set; }
        int SelectionMin => Math.Min(selectionStartIndex, selectionEndIndex);
        int SelectionMax => Math.Max(selectionStartIndex, selectionEndIndex);
        NumberBox baseKey;
        NumberBox fineTune;
        Button importSample;
        Dropdown resamplingMode;

        Dropdown loopMode;
        NumberBox loopPoint;
        SampleBrowser browser;
        Button normalize, reverse, fadeIn, fadeOut, amplifyUp, amplifyDown, invert, mixToMono;
        CheckboxLabeled showInVisualizer;
        int lastMouseHoverSample;

        public SampleEditor(int x, int y, Element parent) {
            this.x = x;
            this.y = y;
            SetParent(parent);
            waveformRegion = new MouseRegion(0, 10, 568, 175, this);

            importSample = new Button("Import Sample    ", 0, 188, this);
            importSample.SetTooltip("", "Import an audio file into the instrument");

            resamplingMode = new Dropdown(242, 237, this);
            resamplingMode.SetMenuItems(new string[] { "Harsh (None)", "Smooth (Linear)", "Mix (None + Linear)" });

            baseKey = new NumberBox("Base Key", 0, 220, 100, 56, this);
            baseKey.SetValueLimits(12, 131);
            baseKey.DisplayMode = NumberBox.NumberDisplayMode.Note;
            baseKey.SetTooltip("", "The note where the sample is played at its original speed");

            fineTune = new NumberBox("Fine tune", 0, 237, 100, 56, this);
            fineTune.SetValueLimits(-199, 199);
            fineTune.DisplayMode = NumberBox.NumberDisplayMode.PlusMinus;
            fineTune.SetTooltip("", "Slightly adjust the pitch of the sample, in cents");

            loopPoint = new NumberBox("Loop position (samples)", 154, 203, 183, 80, this);
            loopPoint.SetTooltip("", "Set the position in audio samples where the sound loops back to");

            loopMode = new Dropdown(282, 188, this, false);
            loopMode.SetMenuItems(new string[] { "One-shot", "Forward", "Ping-pong" });

            int buttonsX = 357;
            int buttonsY = 188;
            int buttonsWidth = 50;
            normalize = new Button("Normalize", buttonsX, buttonsY, buttonsWidth, this);
            buttonsX += buttonsWidth + 1;
            fadeIn = new Button("Fade in", buttonsX, buttonsY, buttonsWidth, this);
            buttonsX += buttonsWidth + 1;
            amplifyUp = new Button("Amplify+", buttonsX, buttonsY, buttonsWidth, this);
            buttonsX += buttonsWidth + 1;
            invert = new Button("Invert", buttonsX, buttonsY, buttonsWidth, this);
            buttonsX = 357;
            buttonsY += 14;
            reverse = new Button("Reverse", buttonsX, buttonsY, buttonsWidth, this);
            buttonsX += buttonsWidth + 1;
            fadeOut = new Button("Fade out", buttonsX, buttonsY, buttonsWidth, this);
            buttonsX += buttonsWidth + 1;
            amplifyDown = new Button("Amplify-", buttonsX, buttonsY, buttonsWidth, this);
            buttonsX += buttonsWidth + 1;
            mixToMono = new Button("To mono", buttonsX, buttonsY, buttonsWidth, this);

            showInVisualizer = new CheckboxLabeled("Show in visualizer", 480, 245, 88, this);
            showInVisualizer.ShowCheckboxOnRight = true;
            browser = new SampleBrowser();
        }

        public void Update() {
            if (waveformRegion.IsHovered) {
                mouseSampleIndex = (int)(waveformRegion.MouseXClamped * (Sample.Length - 1) + 0.5f);
            }
            else {
                mouseSampleIndex = -1;
            }
            mouseSampleIndexClamped = (int)(waveformRegion.MouseXClamped * (Sample.Length - 1) + 0.5f);
            if (InFocus) {
                lastMouseHoverSample = mouseSampleIndexClamped;
            }
            if (!browser.InFocus && Input.focusTimer > 2) {
                if (Input.CurrentModifier == KeyModifier.None && Sample.Length > 0) {
                    if (waveformRegion.DidClickInRegionM(KeyModifier.None)) {
                        if (waveformRegion.ClickedDown) {
                            selectionStartIndex = mouseSampleIndexClamped;
                        }
                        selectionEndIndex = mouseSampleIndexClamped;
                        SelectionIsActive = selectionStartIndex != selectionEndIndex;
                    }
                    if (waveformRegion.Clicked) {
                        if (selectionStartIndex == selectionEndIndex) {
                            SelectionIsActive = false;
                        }
                    }
                    if (waveformRegion.RightClicked) {
                        ContextMenu.Open(new Menu(new MenuItemBase[] {
                            new MenuOption("Set loop point", SetLoopPoint),
                            null,
                            new MenuOption("Cut", Cut, SelectionIsActive),
                            null,
                            new SubMenu("Tools", new MenuItemBase[] {
                                new MenuOption("Reverse", Reverse),
                                new MenuOption("Normalize", Normalize),
                                new MenuOption("Invert", Invert),
                                new MenuOption("Fade In", FadeIn),
                                new MenuOption("Fade Out", FadeOut),
                                new MenuOption("Amplify+", AmplifyUp),
                                new MenuOption("Amplify-", AmplifyDown),
                                new MenuOption("Mix to mono", Sample.MixToMono, Sample.IsStereo),
                            }),
                            null,
                            new MenuOption("Save as...", Sample.SaveToDisk),
                        }));
                    }
                }
                if (waveformRegion.DidClickInRegionM(KeyModifier.Shift)) {
                    SetLoopPoint();
                }

                if (importSample.Clicked) {
                    browser.Open(this);
                    selectionStartIndex = 0;
                    selectionEndIndex = 0;
                    SelectionIsActive = false;
                }

                baseKey.Value = Sample.BaseKey;
                baseKey.Update();
                if (baseKey.ValueWasChangedInternally) {
                    Sample.SetBaseKey(baseKey.Value);
                }
                fineTune.Value = Sample.Detune;
                fineTune.Update();
                if (fineTune.ValueWasChangedInternally) {
                    Sample.SetDetune(fineTune.Value);
                }
                resamplingMode.Value = (int)Sample.resampleMode;
                resamplingMode.Update();
                if (resamplingMode.ValueWasChangedInternally) {
                    Sample.resampleMode = (ResamplingMode)resamplingMode.Value;
                }
                loopMode.Value = (int)Sample.loopType;
                loopMode.Update();
                if (loopMode.ValueWasChangedInternally) {
                    Sample.loopType = (Sample.LoopType)loopMode.Value;
                }
                loopPoint.enabled = Sample.loopType != Sample.LoopType.OneShot;
                loopPoint.Value = Sample.loopPoint;
                loopPoint.Update();
                if (loopPoint.ValueWasChangedInternally) {
                    Sample.loopPoint = loopPoint.Value;
                }

                if (normalize.Clicked) {
                    Normalize();
                }
                if (fadeIn.Clicked) {
                    FadeIn();
                }
                if (fadeOut.Clicked) {
                    FadeOut();
                }
                if (amplifyUp.Clicked) {
                    AmplifyUp();
                }
                if (amplifyDown.Clicked) {
                    AmplifyDown();
                }
                if (invert.Clicked) {
                    Invert();
                }
                if (reverse.Clicked) {
                    Invert();
                }
                if (mixToMono.Clicked) {
                    Sample.MixToMono();
                }
                showInVisualizer.Value = Sample.useInVisualization;
                showInVisualizer.Update();
                Sample.useInVisualization = showInVisualizer.Value;



            }
            browser.Update();
        }

        void Cut() {
            Sample.Cut(SelectionMin, SelectionMax);
            SelectionIsActive = false;
        }

        void Normalize() {
            if (SelectionIsActive)
                Sample.Normalize(SelectionMin, SelectionMax);
            else
                Sample.Normalize();
        }
        void FadeIn() {
            if (SelectionIsActive)
                Sample.FadeIn(SelectionMin, SelectionMax);
            else
                Sample.FadeIn();
        }
        void FadeOut() {
            if (SelectionIsActive)
                Sample.FadeOut(SelectionMin, SelectionMax);
            else
                Sample.FadeOut();
        }
        void Invert() {
            if (SelectionIsActive)
                Sample.Invert(SelectionMin, SelectionMax);
            else
                Sample.Invert();
        }

        void AmplifyUp() {
            if (SelectionIsActive)
                Sample.Amplify(1.1f, SelectionMin, SelectionMax);
            else
                Sample.Amplify(1.1f);
        }
        void AmplifyDown() {
            if (SelectionIsActive)
                Sample.Amplify(0.9f, SelectionMin, SelectionMax);
            else
                Sample.Amplify(0.9f);
        }

        void Reverse() {
            if (SelectionIsActive)
                Sample.Reverse(SelectionMin, SelectionMax);
            else
                Sample.Reverse();
        }

        void SetLoopPoint() {
            Sample.loopPoint = lastMouseHoverSample;
            if (Sample.loopType == Sample.LoopType.OneShot) Sample.loopType = Sample.LoopType.Forward;
        }



        public void Draw() {
            if (Sample == null) {
                return;
            }

            Write(Sample.Length + " samples", waveformRegion.x, waveformRegion.y - 9, UIColors.label);
            WriteRightAlign((Sample.Length / (float)AudioEngine.SAMPLE_RATE).ToString("F5") + " seconds", waveformRegion.x + waveformRegion.width, waveformRegion.y - 9, UIColors.label);

            if (Sample.IsStereo && Sample.Length > 0) {
                DrawWaveform(waveformRegion.x, waveformRegion.y, Sample.sampleDataAccessL, waveformRegion.width, waveformRegion.height / 2);
                DrawWaveform(waveformRegion.x, waveformRegion.y + waveformRegion.height / 2 + 1, Sample.sampleDataAccessR, waveformRegion.width, waveformRegion.height / 2);
            }
            else {
                DrawWaveform(waveformRegion.x, waveformRegion.y, Sample.sampleDataAccessL, waveformRegion.width, waveformRegion.height);
            }
            //Write("[" + mouseSampleIndex + "]", waveformRegion.x, waveformRegion.y + waveformRegion.height + 15, UIColors.label);
            importSample.Draw();
            DrawSprite(importSample.x + importSample.width - 14, importSample.y + (importSample.IsPressed ? 3 : 2), new Rectangle(72, 81, 12, 9));
            loopMode.Draw();
            WriteRightAlign("Loop Mode", loopMode.x - 4, loopMode.y + 4, UIColors.label);
            if (Sample.loopType != Sample.LoopType.OneShot) {

                loopPoint.Draw();
            }
            baseKey.Draw();
            fineTune.Draw();
            resamplingMode.Draw();
            WriteRightAlign("Resampling Mode", resamplingMode.x - 4, resamplingMode.y + 4, UIColors.label);
            loopMode.Draw();
            DrawRect(348, 188, 1, 66, UIColors.label);
            normalize.Draw();
            invert.Draw();
            mixToMono.Draw();
            fadeIn.Draw();
            fadeOut.Draw();
            amplifyDown.Draw();
            amplifyUp.Draw();
            reverse.Draw();
            browser.Draw();
            showInVisualizer.Draw();
        }

        void DrawWaveform(int x, int y, short[] data, int width, int height) {
            int startY = y + height / 2;
            int nextSampleIndex;
            int sampleIndex;
            Color sampleColor = new Color(207, 117, 43);
            DrawRect(x, y, width, height, UIColors.black);

            if (data.Length > 0) {
                if (SelectionIsActive) {
                    int x1 = GetXPositionOfSample(SelectionMin, data.Length, width);
                    int x2 = GetXPositionOfSample(SelectionMax + 1, data.Length, width);
                    DrawRect(x + x1, y, x2 - x1, height, Helpers.Alpha(UIColors.selection, 128));

                }
                int loopPosition = GetXPositionOfSample(Sample.loopPoint, data.Length, width);
                if (Sample.loopType != Sample.LoopType.OneShot) {
                    DrawRect(x + loopPosition, y, width - loopPosition, height, Helpers.Alpha(Color.Yellow, 50));
                }

                for (int i = 0; i < width - 1; i++) {
                    sampleIndex = i * data.Length / width;
                    nextSampleIndex = (i + 1) * data.Length / width;
                    float min = 1;
                    float max = -1;
                    float average = 0;
                    for (int j = sampleIndex; j <= nextSampleIndex; ++j) {
                        float value = data[j] / (float)short.MaxValue;
                        average += MathF.Abs(value);
                        if (value < min) min = value;
                        if (value > max) max = value;
                    }
                    average /= sampleIndex - nextSampleIndex + 1;
                    int rectStart = (int)(max * height / 2);
                    int rectEnd = (int)(min * height / 2);
                    int avgStart = (int)(average * height / 2);
                    int avgEnd = (int)(average * height / -2);
                    if (SelectionIsActive && sampleIndex + 1 >= SelectionMin && nextSampleIndex <= SelectionMax) {
                        DrawRect(x + i, startY - rectStart, 1, rectStart - rectEnd + 1, Color.LightGray);
                    }
                    else {
                        DrawRect(x + i, startY - rectStart, 1, rectStart - rectEnd + 1, sampleColor);
                    }
                    avgStart = Math.Clamp(avgStart, rectEnd, rectStart);
                    avgEnd = Math.Clamp(avgEnd, rectEnd, rectStart);
                    DrawRect(x + i, startY - avgStart, 1, avgStart - avgEnd + 1, Helpers.Alpha(Color.White, 90));
                }


                if (Sample.loopType != Sample.LoopType.OneShot) {
                    DrawRect(x + loopPosition, y, 1, height, Color.Yellow);
                }
                if (Sample.currentPlaybackPosition < data.Length && Audio.ChannelManager.previewChannel.IsPlaying) {
                    DrawRect(x + GetXPositionOfSample(Sample.currentPlaybackPosition, data.Length, width), y, 1, height, Color.Aqua);
                }
                if (mouseSampleIndex > 0) {
                    DrawRect(x + GetXPositionOfSample(mouseSampleIndex, data.Length, width), y, 1, height, Helpers.Alpha(Color.White, 128));
                }
                else if (!InFocus && !browser.InFocus) {
                    DrawRect(x + GetXPositionOfSample(lastMouseHoverSample, data.Length, width), y, 1, height, Helpers.Alpha(Color.White, 64));
                }
            }
        }

        int GetXPositionOfSample(int sampleIndex, int dataLength, int maxWidth) {
            return (int)((float)sampleIndex / dataLength * maxWidth);
        }
    }
}