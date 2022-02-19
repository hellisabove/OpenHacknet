using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Hacknet.Effects
{
	// Token: 0x020000AA RID: 170
	public class AudioVisualizer
	{
		// Token: 0x0600037C RID: 892 RVA: 0x00034EA4 File Offset: 0x000330A4
		public void Draw(Rectangle bounds, SpriteBatch sb)
		{
			if (OS.currentInstance != null)
			{
				if (SettingsLoader.ShouldDrawMusicVis)
				{
					if (this.samplesHistory == null)
					{
						this.samplesHistory = new List<ReadOnlyCollection<float>>();
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
						this.samplesHistory.Add(new ReadOnlyCollection<float>(new float[0]));
					}
					if (MediaPlayer.State == MediaState.Playing)
					{
						if (OS.currentInstance != null && OS.currentInstance.lastGameTime != null)
						{
							this.SecondsSinceLastDataUpdate += OS.currentInstance.lastGameTime.ElapsedGameTime.TotalSeconds;
							if (this.SecondsSinceLastDataUpdate >= 0.041666666666666664)
							{
								MediaPlayer.IsVisualizationEnabled = true;
								try
								{
									MediaPlayer.GetVisualizationData(this.visData);
									this.SecondsSinceLastDataUpdate = 0.0;
								}
								catch (OverflowException)
								{
								}
								catch (NullReferenceException)
								{
								}
								catch (IndexOutOfRangeException)
								{
								}
								catch (Exception)
								{
								}
							}
						}
					}
					if (this.visData != null)
					{
						List<float> list = new List<float>(this.visData.Samples.Count);
						for (int i = 0; i < this.visData.Samples.Count; i++)
						{
							float num = Math.Max(0f, Math.Abs(this.visData.Samples[i]));
							if (num > 1f)
							{
								num = 0f;
							}
							list.Add(num);
						}
						ReadOnlyCollection<float> readOnlyCollection = new ReadOnlyCollection<float>(list);
						if (this.previousSamples == null)
						{
							this.previousSamples = this.visData.Samples;
						}
						float num2 = (float)bounds.Height / (float)readOnlyCollection.Count;
						Vector2 vector = new Vector2((float)bounds.X, (float)bounds.Y);
						float scale = 0.2f;
						for (int i = 0; i < readOnlyCollection.Count; i++)
						{
							int index = i;
							float num3 = Math.Abs(readOnlyCollection[index]);
							sb.Draw(Utils.white, new Rectangle((int)vector.X, (int)vector.Y, (int)((float)bounds.Width * num3), Math.Max(1, (int)num2)), OS.currentInstance.highlightColor * (0.2f + num3 / 2f) * scale);
							vector.Y += num2;
						}
						this.samplesHistory.Add(new ReadOnlyCollection<float>(this.visData.Samples.ToArray<float>()));
						this.samplesHistory.RemoveAt(0);
						for (int j = 0; j < this.samplesHistory.Count; j++)
						{
							float num4 = (float)j / (float)(this.samplesHistory.Count - 1);
							vector.Y = (float)bounds.Y;
							for (int i = 0; i < this.samplesHistory[j].Count; i++)
							{
								Color value = (j >= this.samplesHistory.Count - 1) ? (Utils.AddativeWhite * 0.7f) : OS.currentInstance.highlightColor;
								sb.Draw(Utils.white, new Vector2((float)bounds.X + (this.samplesHistory[j][i] * ((float)bounds.Width / 4f) + (float)bounds.Width * 0.75f) + (float)(j * 2) - (float)this.samplesHistory.Count, vector.Y), null, value * 0.6f * (0.01f + num4) * 0.4f);
								vector.Y += num2;
							}
						}
					}
				}
			}
		}

		// Token: 0x040003F7 RID: 1015
		private VisualizationData visData = new VisualizationData();

		// Token: 0x040003F8 RID: 1016
		private List<ReadOnlyCollection<float>> samplesHistory;

		// Token: 0x040003F9 RID: 1017
		private ReadOnlyCollection<float> previousSamples = null;

		// Token: 0x040003FA RID: 1018
		private double SecondsSinceLastDataUpdate = 0.0;
	}
}
