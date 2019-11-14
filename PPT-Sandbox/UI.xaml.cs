﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox {
    public partial class UI {
        private ProcessMemory Game = new ProcessMemory("puyopuyotetris", false);
        public UI() {
            InitializeComponent();

            Version.Text = $"PPT-Sandbox-{Assembly.GetExecutingAssembly().GetName().Version.Minor} by {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName}";

            switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName) {
                case "ko":
                    break;
                    
                case "ja":
                    break;
                    
                default:
                    ScriptsHeader.Header = "      SCRIPTS";
                        General.Text = "General";
                            PentominoVersus.Content = "Pentomino Versus";
                            RemoveLineClearDelay.Content = "Remove Line Clear Delay";
                            UndoHold.Content = "Undo Hold";
                            SecretGradeGarbage.Content = "Secret Grade Garbage";

                        AutoLocking.Text = "Piece Auto-Locking";
                            RemoveAutoLock.Content = "Disable Auto-Locking";
                            TGMAutoLock.Content = "TGM Auto-Locking";

                        TetrisB2B.Text = "Back-to-Back Tetris";
                            TetrisB2BDouble.Content = "Tetris B2B doubles attack";
                            TetrisB2BAdd2.Content = "Tetris B2B adds 2 attack";

                        TspinB2B.Text = "Back-to-Back T-Spin";
                            TspinB2BDouble.Content = "T-Spin B2B doubles attack";
                            TspinB2BAdd2.Content = "T-Spin B2B adds 2 attack";

                    AttackHeader.Header = "ATTACK";
                        TetrisVsTetris.Header = "     Tetris vs Tetris";
                            Attacks.Text = "Attacks";

                            Single.Text = "Single";
                            Double.Text = "Double";
                            Triple.Text = "Triple";
                            Tetris.Text = "Tetris";
                            TetrisPlus.Text = "Tetris Plus";

                            Clear.Text = "Clear";
                            PerfectClear.Text = "Perfect Clear";
                            TSpin.Text = "T-Spin";

                            Combo.Text = "Combos";

                        TetrisVsPuyo.Header = "Tetris vs Puyo";
                        Puyo.Header = "Puyo";

                    GarbageHeader.Header = "GARBAGE";
                        GarbageGeneration.Text = "Garbage Generation";
                            CleanGarbage.Title = "Clean Garbage Chance";
                            GarbageFilled.Title = "Filled Garbage Tile";
                            GarbageEmpty.Title = "Empty Garbage Tile";
                    break;
            }

            Scripts = new Dictionary<Control, Action<bool>>() {
                {PentominoVersus, x => {
                    IntPtr addr = (IntPtr)Game.ReadUInt64(new IntPtr(0x140463F20));

                    if (x) {
                        Game.WriteByte(addr, 5);            //S
                        Game.WriteInt32(addr + 0x24, -2);
                        Game.WriteByte(addr + 0xE0, 5);     //Z
                        Game.WriteByteArray(
                            addr + 0x104,
                            ConvertByteString("01 00 00 00 FF FF FF FF")
                        );
                        Game.WriteByte(addr + 0x1C0, 5);    //J
                        Game.WriteInt32(addr + 0x1E8, -1);
                        Game.WriteByte(addr + 0x2A0, 5);    //L
                        Game.WriteByteArray(
                            addr + 0x2C4,
                            ConvertByteString("FF FF FF FF FF FF FF FF")
                        );
                        Game.WriteByte(addr + 0x380, 5);    //T
                        Game.WriteInt32(addr + 0x3A8, 2);   
                        Game.WriteByte(addr + 0x460, 5);    //O
                        Game.WriteInt32(addr + 0x484, -1);  
                        Game.WriteByte(addr + 0x540, 5);    //I
                        Game.WriteInt32(addr + 0x564, -2);

                    } else {
                        Game.WriteByte(addr, 4);          //S
                        Game.WriteByte(addr + 0xE0, 4);   //Z
                        Game.WriteByte(addr + 0x1C0, 4);  //J
                        Game.WriteByte(addr + 0x2A0, 4);  //L
                        Game.WriteByte(addr + 0x380, 4);  //T
                        Game.WriteByte(addr + 0x460, 4);  //O
                        Game.WriteByte(addr + 0x540, 4);  //I
                    }
                }},
                {RemoveLineClearDelay, x =>
                    Game.WriteByte(
                        new IntPtr(0x1400A26F4),
                        Convert.ToByte(!x)
                    )
                },
                {UndoHold, x => {
                    byte[] write = x
                        ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
                        : new byte[] { 0xFF, 0x83, 0x48, 0x01, 0x00, 0x00 };

                    Game.WriteByteArray(new IntPtr(0x142852508), write);
                    Game.WriteByteArray(new IntPtr(0x14285252F), write);
                }},
                {SecretGradeGarbage, x => {
                    Game.WriteByteArray(
                        new IntPtr(0x14009F480),
                        x
                            ? ConvertByteString("8A 1D 7A 3B 3C 00 80 3D 74 3B 3C 00 00 75 08 FF 05 6B 3B 3C 00 EB 06 FF 0D 63 3B 3C 00 80 3D 5D 3B 3C 00 00 75 10 80 3D 53 3B 3C 00 09 7C 27 C6 05 4B 3B 3C 00 01 80 3D 43 3B 3C 00 00 75 17 C6 05 3B 3B 3C 00 00 80 3D 33 3B 3C 00 0A 72 07 C6 05 2A 3B 3C 00 00 E9 85 3B 61 02")
                            : ConvertByteString("E9 DB 3B 61 02")
                    );

                    Game.WriteByteArray(
                        new IntPtr(0x140061010),
                        x
                            ? ConvertByteString("66 C7 05 E7 1F 40 00 00 00 E9 A2 79 91 01")
                            : ConvertByteString("E9 AB 79 91 01")
                    );
                }},
                {RemoveAutoLock, x => 
                    Game.WriteByte(
                        new IntPtr(0x142853B33),
                        Convert.ToByte(!x) //0 on enable, 1 on disable of script
                    )
                },
                {TGMAutoLock, x => {
                    byte[] write = x
                        ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 } //enable
                        : new byte[] { 0x66, 0x41, 0x89, 0x86, 0x10, 0x01, 0x00, 0x00 };//disable
            
                    Game.WriteByteArray(new IntPtr(0x1400A76CF), write);
                    Game.WriteByteArray(new IntPtr(0x1400A6EE0), write);
                }}
            };
        }

        private byte[] ConvertByteString(string bytes) =>
            bytes.Split(' ').Select(i => Convert.ToByte(i, 16)).ToArray();

        private Dictionary<Control, Action<bool>> Scripts;

        private void CheckBoxHandle(object sender, RoutedEventArgs e) {
            CheckBox source = (CheckBox)sender;
            Scripts[source].Invoke(source.IsChecked == true);
        }

        private void RadioButtonHandle(object sender, RoutedEventArgs e) {
            OptionalRadioButton source = (OptionalRadioButton)sender;
            OptionalRadioButton previous = OptionalRadioButton.GetSelected(source.GroupName);

            if (previous != null) 
                Scripts[previous].Invoke(false);
            
            if (source.IsChecked == true)
                Scripts[source].Invoke(true);
        }
    }
}
