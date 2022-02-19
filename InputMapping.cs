using System;
using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
	// Token: 0x02000126 RID: 294
	public class InputMapping
	{
		// Token: 0x060006EF RID: 1775 RVA: 0x00072214 File Offset: 0x00070414
		public static InputStates getStatesFromKeys(KeyboardState keys, GamePadState pad, GamePadThumbSticks sticks)
		{
			InputStates inputStates = InputMapping.ret;
			bool flag = 0 == 0;
			if (keys.IsKeyDown(Keys.Right) || pad.DPad.Right == ButtonState.Pressed)
			{
				InputMapping.ret.movement = 1f;
			}
			else if (keys.IsKeyDown(Keys.Left) || pad.DPad.Left == ButtonState.Pressed)
			{
				InputMapping.ret.movement = -1f;
			}
			else if (sticks.Left.X != 0f)
			{
				InputMapping.ret.movement = sticks.Left.X;
			}
			else
			{
				InputMapping.ret.movement = 0f;
			}
			if (pad.Buttons.A == ButtonState.Pressed || keys.IsKeyDown(Keys.Up))
			{
				InputMapping.ret.jumping = true;
			}
			else
			{
				InputMapping.ret.jumping = false;
			}
			if (pad.Buttons.B == ButtonState.Pressed || keys.IsKeyDown(Keys.RightShift))
			{
				InputMapping.ret.useItem = true;
			}
			else
			{
				InputMapping.ret.useItem = false;
			}
			if (keys.IsKeyDown(Keys.D) || pad.DPad.Right == ButtonState.Pressed)
			{
				InputMapping.ret.wmovement = 1f;
			}
			else if (keys.IsKeyDown(Keys.A) || pad.DPad.Left == ButtonState.Pressed)
			{
				InputMapping.ret.wmovement = -1f;
			}
			else if (sticks.Left.X != 0f)
			{
				InputMapping.ret.wmovement = sticks.Left.X;
			}
			else
			{
				InputMapping.ret.wmovement = 0f;
			}
			if (pad.Buttons.A == ButtonState.Pressed || keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Space) || pad.Triggers.Left > 0f)
			{
				InputMapping.ret.wjumping = true;
			}
			else
			{
				InputMapping.ret.wjumping = false;
			}
			if (pad.Buttons.B == ButtonState.Pressed || keys.IsKeyDown(Keys.LeftShift))
			{
				InputMapping.ret.wuseItem = true;
			}
			else
			{
				InputMapping.ret.wuseItem = false;
			}
			InputMapping.lastCalculatedState = InputMapping.ret;
			return InputMapping.ret;
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x000724C4 File Offset: 0x000706C4
		public static InputMap getMapFromKeys(KeyboardState keys, GamePadState pad)
		{
			InputMapping.map.last = InputMapping.lastCalculatedState;
			InputMapping.map.now = InputMapping.getStatesFromKeys(keys, pad, pad.ThumbSticks);
			return InputMapping.map;
		}

		// Token: 0x040007C4 RID: 1988
		private static InputStates ret;

		// Token: 0x040007C5 RID: 1989
		private static InputMap map;

		// Token: 0x040007C6 RID: 1990
		private static InputStates lastCalculatedState;
	}
}
