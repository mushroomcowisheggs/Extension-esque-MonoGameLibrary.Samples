using System;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Extensions.Input;

namespace MonoGameLibrary.Adapters.MonoGame.Input {
    /// <summary>
    /// Converts between platform‑independent <see cref="KeyCode"/> and MonoGame <see cref="Keys"/>.
    /// </summary>
    public static class KeyCodeConverter {
        public static Keys ToMonoGameKey(KeyCode codeKey) {
            switch (codeKey) {
                case KeyCode.None: return Keys.None;
                case KeyCode.A: return Keys.A;
                case KeyCode.B: return Keys.B;
                case KeyCode.C: return Keys.C;
                case KeyCode.D: return Keys.D;
                case KeyCode.E: return Keys.E;
                case KeyCode.F: return Keys.F;
                case KeyCode.G: return Keys.G;
                case KeyCode.H: return Keys.H;
                case KeyCode.I: return Keys.I;
                case KeyCode.J: return Keys.J;
                case KeyCode.K: return Keys.K;
                case KeyCode.L: return Keys.L;
                case KeyCode.M: return Keys.M;
                case KeyCode.N: return Keys.N;
                case KeyCode.O: return Keys.O;
                case KeyCode.P: return Keys.P;
                case KeyCode.Q: return Keys.Q;
                case KeyCode.R: return Keys.R;
                case KeyCode.S: return Keys.S;
                case KeyCode.T: return Keys.T;
                case KeyCode.U: return Keys.U;
                case KeyCode.V: return Keys.V;
                case KeyCode.W: return Keys.W;
                case KeyCode.X: return Keys.X;
                case KeyCode.Y: return Keys.Y;
                case KeyCode.Z: return Keys.Z;
                case KeyCode.Space: return Keys.Space;
                case KeyCode.Enter: return Keys.Enter;
                case KeyCode.Escape: return Keys.Escape;
                case KeyCode.Tab: return Keys.Tab;
                case KeyCode.Backspace: return Keys.Back;
                case KeyCode.Up: return Keys.Up;
                case KeyCode.Down: return Keys.Down;
                case KeyCode.Left: return Keys.Left;
                case KeyCode.Right: return Keys.Right;
                case KeyCode.F1: return Keys.F1;
                case KeyCode.F2: return Keys.F2;
                case KeyCode.F3: return Keys.F3;
                case KeyCode.F4: return Keys.F4;
                case KeyCode.F5: return Keys.F5;
                case KeyCode.F6: return Keys.F6;
                case KeyCode.F7: return Keys.F7;
                case KeyCode.F8: return Keys.F8;
                case KeyCode.F9: return Keys.F9;
                case KeyCode.F10: return Keys.F10;
                case KeyCode.F11: return Keys.F11;
                case KeyCode.F12: return Keys.F12;
                default: return Keys.None;
            }
        }
        
        public static KeyCode ToKeyCode(Keys keyMonoGame) {
            switch (keyMonoGame) {
                case Keys.None: return KeyCode.None;
                case Keys.A: return KeyCode.A;
                case Keys.B: return KeyCode.B;
                case Keys.C: return KeyCode.C;
                case Keys.D: return KeyCode.D;
                case Keys.E: return KeyCode.E;
                case Keys.F: return KeyCode.F;
                case Keys.G: return KeyCode.G;
                case Keys.H: return KeyCode.H;
                case Keys.I: return KeyCode.I;
                case Keys.J: return KeyCode.J;
                case Keys.K: return KeyCode.K;
                case Keys.L: return KeyCode.L;
                case Keys.M: return KeyCode.M;
                case Keys.N: return KeyCode.N;
                case Keys.O: return KeyCode.O;
                case Keys.P: return KeyCode.P;
                case Keys.Q: return KeyCode.Q;
                case Keys.R: return KeyCode.R;
                case Keys.S: return KeyCode.S;
                case Keys.T: return KeyCode.T;
                case Keys.U: return KeyCode.U;
                case Keys.V: return KeyCode.V;
                case Keys.W: return KeyCode.W;
                case Keys.X: return KeyCode.X;
                case Keys.Y: return KeyCode.Y;
                case Keys.Z: return KeyCode.Z;
                case Keys.Space: return KeyCode.Space;
                case Keys.Enter: return KeyCode.Enter;
                case Keys.Escape: return KeyCode.Escape;
                case Keys.Tab: return KeyCode.Tab;
                case Keys.Back: return KeyCode.Backspace;
                case Keys.Up: return KeyCode.Up;
                case Keys.Down: return KeyCode.Down;
                case Keys.Left: return KeyCode.Left;
                case Keys.Right: return KeyCode.Right;
                case Keys.F1: return KeyCode.F1;
                case Keys.F2: return KeyCode.F2;
                case Keys.F3: return KeyCode.F3;
                case Keys.F4: return KeyCode.F4;
                case Keys.F5: return KeyCode.F5;
                case Keys.F6: return KeyCode.F6;
                case Keys.F7: return KeyCode.F7;
                case Keys.F8: return KeyCode.F8;
                case Keys.F9: return KeyCode.F9;
                case Keys.F10: return KeyCode.F10;
                case Keys.F11: return KeyCode.F11;
                case Keys.F12: return KeyCode.F12;
                default: return KeyCode.None;
            }
        }
    }
}