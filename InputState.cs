using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
	// Token: 0x02000158 RID: 344
	public class InputState
	{
		// Token: 0x060008A0 RID: 2208 RVA: 0x00091718 File Offset: 0x0008F918
		public InputState()
		{
			this.CurrentKeyboardStates = new KeyboardState[4];
			this.CurrentGamePadStates = new GamePadState[4];
			this.LastKeyboardStates = new KeyboardState[4];
			this.LastGamePadStates = new GamePadState[4];
			this.GamePadWasConnected = new bool[4];
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0009176C File Offset: 0x0008F96C
		public void Update()
		{
			for (int i = 0; i < 4; i++)
			{
				this.LastKeyboardStates[i] = this.CurrentKeyboardStates[i];
				this.LastGamePadStates[i] = this.CurrentGamePadStates[i];
				this.CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
				this.CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
				if (this.CurrentGamePadStates[i].IsConnected)
				{
					this.GamePadWasConnected[i] = true;
				}
			}
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x00091828 File Offset: 0x0008FA28
		public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			bool result;
			if (controllingPlayer != null)
			{
				playerIndex = controllingPlayer.Value;
				int num = (int)playerIndex;
				result = (this.CurrentKeyboardStates[num].IsKeyDown(key) && this.LastKeyboardStates[num].IsKeyUp(key));
			}
			else
			{
				result = (this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.One), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Two), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Three), out playerIndex) || this.IsNewKeyPress(key, new PlayerIndex?(PlayerIndex.Four), out playerIndex));
			}
			return result;
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x000918C8 File Offset: 0x0008FAC8
		public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			bool result;
			if (controllingPlayer != null)
			{
				playerIndex = controllingPlayer.Value;
				int num = (int)playerIndex;
				result = (this.CurrentGamePadStates[num].IsButtonDown(button) && this.LastGamePadStates[num].IsButtonUp(button));
			}
			else
			{
				result = (this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.One), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Two), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Three), out playerIndex) || this.IsNewButtonPress(button, new PlayerIndex?(PlayerIndex.Four), out playerIndex));
			}
			return result;
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x00091968 File Offset: 0x0008FB68
		public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			return this.IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) || this.IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x000919B0 File Offset: 0x0008FBB0
		public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			return this.IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
		}

		// Token: 0x060008A6 RID: 2214 RVA: 0x000919EC File Offset: 0x0008FBEC
		public bool IsMenuUp(PlayerIndex? controllingPlayer)
		{
			PlayerIndex playerIndex;
			return this.IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) || this.IsNewKeyPress(Keys.W, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x00091A38 File Offset: 0x0008FC38
		public bool IsMenuDown(PlayerIndex? controllingPlayer)
		{
			PlayerIndex playerIndex;
			return this.IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) || this.IsNewKeyPress(Keys.D, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x00091A84 File Offset: 0x0008FC84
		public bool IsPauseGame(PlayerIndex? controllingPlayer)
		{
			PlayerIndex playerIndex;
			return this.IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) || this.IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
		}

		// Token: 0x04000A0B RID: 2571
		public const int MaxInputs = 4;

		// Token: 0x04000A0C RID: 2572
		public KeyboardState[] CurrentKeyboardStates;

		// Token: 0x04000A0D RID: 2573
		public GamePadState[] CurrentGamePadStates;

		// Token: 0x04000A0E RID: 2574
		public KeyboardState[] LastKeyboardStates;

		// Token: 0x04000A0F RID: 2575
		public GamePadState[] LastGamePadStates;

		// Token: 0x04000A10 RID: 2576
		public readonly bool[] GamePadWasConnected;

		// Token: 0x04000A11 RID: 2577
		public static InputState Empty = new InputState();
	}
}
