using System;

namespace Hacknet.Effects
{
	// Token: 0x02000052 RID: 82
	public class ActiveEffectsUpdater
	{
		// Token: 0x06000194 RID: 404 RVA: 0x000157D0 File Offset: 0x000139D0
		public void Update(float dt, object osobj)
		{
			OS os = (OS)osobj;
			if (this.themeSwapTimeRemaining > 0f)
			{
				this.themeSwapTimeRemaining -= dt;
				if (this.themeSwapTimeRemaining <= 0f)
				{
					this.CompleteThemeSwap(os);
					return;
				}
				bool flag = Utils.randm(1f) > this.themeSwapTimeRemaining / this.originalThemeSwapTime;
				OSTheme theme = flag ? this.newTheme : this.oldTheme;
				string text = flag ? this.newThemePath : this.oldThemePath;
				ThemeManager.LastLoadedCustomTheme = (flag ? this.newCustomTheme : this.oldCustomTheme);
				if (text != null)
				{
					ThemeManager.switchTheme(os, text);
				}
				else
				{
					ThemeManager.switchTheme(os, theme);
				}
			}
			if (this.ScreenBleedActive)
			{
				this.ScreenBleedTimeLeft -= dt;
				float val = 1f - this.ScreenBleedTimeLeft / this.ScreenBleedStartTime;
				PostProcessor.dangerModePercentComplete = Math.Max(0f, Math.Min(val, 1f));
				PostProcessor.dangerModeEnabled = true;
				if (this.ScreenBleedTimeLeft <= 0f)
				{
					this.ScreenBleedActive = false;
					PostProcessor.dangerModePercentComplete = 0f;
					PostProcessor.dangerModeEnabled = false;
					if (!string.IsNullOrWhiteSpace(this.screenBleedCompleteAction))
					{
						RunnableConditionalActions.LoadIntoOS(this.screenBleedCompleteAction, os);
					}
				}
				else
				{
					OS os2 = os;
					os2.postFXDrawActions = (Action)Delegate.Combine(os2.postFXDrawActions, new Action(delegate()
					{
						TraceDangerSequence.DrawCountdownOverlay(Utils.CheckStringIsTitleRenderable(this.screenBleedTitle) ? GuiData.titlefont : GuiData.font, GuiData.smallfont, os, this.screenBleedTitle, this.screenBleedL1, this.screenBleedL2, this.screenBleedL3);
					}));
				}
			}
		}

		// Token: 0x06000195 RID: 405 RVA: 0x000159B0 File Offset: 0x00013BB0
		public void StartThemeSwitch(float time, OSTheme newTheme, object osobj, string customThemePath = null)
		{
			this.oldTheme = ThemeManager.currentTheme;
			this.oldThemePath = ((this.oldTheme == OSTheme.Custom) ? ThemeManager.LastLoadedCustomThemePath : null);
			this.oldCustomTheme = ((this.oldTheme == OSTheme.Custom) ? ThemeManager.LastLoadedCustomTheme : null);
			if (this.themeSwapTimeRemaining > 0f)
			{
				this.CompleteThemeSwap(osobj);
			}
			this.originalThemeSwapTime = (this.themeSwapTimeRemaining = time);
			this.newTheme = newTheme;
			this.newThemePath = customThemePath;
			try
			{
				if (time <= 0f)
				{
					this.CompleteThemeSwap(osobj);
				}
				else if (this.newThemePath != null)
				{
					ThemeManager.switchTheme(osobj, this.newThemePath);
					this.newCustomTheme = ThemeManager.LastLoadedCustomTheme;
				}
				else
				{
					this.newCustomTheme = null;
				}
			}
			catch (Exception ex)
			{
				time = (this.themeSwapTimeRemaining = 0f);
				throw ex;
			}
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00015AA8 File Offset: 0x00013CA8
		public void CompleteThemeSwap(object osobj)
		{
			OS os = (OS)osobj;
			if (this.newThemePath != null)
			{
				ThemeManager.setThemeOnComputer(os.thisComputer, this.newThemePath);
				ThemeManager.switchTheme(os, this.newThemePath);
			}
			else
			{
				ThemeManager.setThemeOnComputer(os.thisComputer, this.newTheme);
				ThemeManager.switchTheme(os, this.newTheme);
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00015B10 File Offset: 0x00013D10
		public void StartScreenBleed(float time, string title, string line1, string line2, string line3, string completeAction)
		{
			this.ScreenBleedTimeLeft = time;
			this.ScreenBleedStartTime = time;
			this.screenBleedTitle = title;
			this.screenBleedL1 = line1;
			this.screenBleedL2 = line2;
			this.screenBleedL3 = line3;
			this.screenBleedCompleteAction = completeAction;
			this.ScreenBleedActive = true;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00015B5B File Offset: 0x00013D5B
		public void CancelScreenBleedEffect()
		{
			this.ScreenBleedActive = false;
			PostProcessor.dangerModePercentComplete = 0f;
			PostProcessor.dangerModeEnabled = false;
		}

		// Token: 0x04000189 RID: 393
		private bool ScreenBleedActive = false;

		// Token: 0x0400018A RID: 394
		private float ScreenBleedTimeLeft = 0f;

		// Token: 0x0400018B RID: 395
		private float ScreenBleedStartTime = 0f;

		// Token: 0x0400018C RID: 396
		private string screenBleedTitle = "UNKNOWN";

		// Token: 0x0400018D RID: 397
		private string screenBleedL1;

		// Token: 0x0400018E RID: 398
		private string screenBleedL2;

		// Token: 0x0400018F RID: 399
		private string screenBleedL3;

		// Token: 0x04000190 RID: 400
		private string screenBleedCompleteAction = null;

		// Token: 0x04000191 RID: 401
		private OSTheme oldTheme;

		// Token: 0x04000192 RID: 402
		private string oldThemePath;

		// Token: 0x04000193 RID: 403
		private CustomTheme oldCustomTheme;

		// Token: 0x04000194 RID: 404
		private OSTheme newTheme;

		// Token: 0x04000195 RID: 405
		private string newThemePath;

		// Token: 0x04000196 RID: 406
		private CustomTheme newCustomTheme;

		// Token: 0x04000197 RID: 407
		private float themeSwapTimeRemaining;

		// Token: 0x04000198 RID: 408
		private float originalThemeSwapTime;
	}
}
