using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x02000053 RID: 83
	public class ExplodingUIElementEffect
	{
		// Token: 0x0600019A RID: 410 RVA: 0x00015BAD File Offset: 0x00013DAD
		public void Init(ContentManager content)
		{
			this.Textures.Add(Utils.white);
			this.Textures.Add(Utils.white);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00015BD4 File Offset: 0x00013DD4
		public void Explode(int particles, Vector2 AngleRange, Vector2 startPos, float minScale, float maxScale, float minSpeed, float maxSpeed, float minFriction, float maxFriction, float minStartFade, float maxStartFade)
		{
			for (int i = 0; i < particles; i++)
			{
				this.Particles.Add(new ExplodingUIElementEffect.ExplosionParticle
				{
					Pos = startPos,
					angle = AngleRange.X + Utils.randm(AngleRange.Y - AngleRange.X),
					fade = minStartFade + Utils.randm(maxStartFade - minStartFade),
					rotation = Utils.randm(6.2831855f),
					rotationRate = Utils.randm(20f) - Utils.randm(20f),
					Size = minScale + Utils.randm(maxScale - minScale),
					speed = minSpeed + Utils.randm(maxSpeed - minSpeed),
					frictionSlowdownPerSec = minFriction + Utils.randm(minFriction - maxFriction),
					textureIndex = Utils.random.Next(this.Textures.Count),
					FlickerOffset = Utils.randm(50f)
				});
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00015CE4 File Offset: 0x00013EE4
		public void Update(float dt)
		{
			for (int i = 0; i < this.Particles.Count; i++)
			{
				ExplodingUIElementEffect.ExplosionParticle value = this.Particles[i];
				value.Pos += Utils.PolarToCartesian(value.angle, value.speed * dt);
				value.speed -= value.frictionSlowdownPerSec * dt;
				if (value.speed <= 0f)
				{
					value.speed = 0f;
				}
				value.rotation += value.rotationRate * dt;
				value.rotationRate -= value.frictionSlowdownPerSec * dt * 0.1f;
				if (value.rotationRate <= 0f)
				{
					value.rotationRate = 0f;
				}
				value.fade -= dt;
				if (value.fade <= 0f)
				{
					this.Particles.RemoveAt(i);
					i--;
				}
				else
				{
					this.Particles[i] = value;
				}
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00015E18 File Offset: 0x00014018
		public void Render(SpriteBatch sb)
		{
			for (int i = 0; i < this.Particles.Count; i++)
			{
				ExplodingUIElementEffect.ExplosionParticle explosionParticle = this.Particles[i];
				Texture2D texture2D = this.Textures[explosionParticle.textureIndex];
				if (texture2D.IsDisposed)
				{
					texture2D = Utils.white;
				}
				Vector2 scale = new Vector2(explosionParticle.Size / (float)texture2D.Width, explosionParticle.Size / (float)texture2D.Width);
				scale = new Vector2(Math.Min(scale.X, scale.Y), Math.Min(scale.X, scale.Y));
				float num = 1f - Math.Min(1f, Math.Max(0f, explosionParticle.fade / 2f));
				FlickeringTextEffect.DrawFlickeringSpriteFull(sb, explosionParticle.Pos, explosionParticle.rotation, scale, texture2D.GetCentreOrigin(), texture2D, explosionParticle.FlickerOffset, explosionParticle.Size, 0f, OS.currentInstance, Utils.AddativeWhite * (explosionParticle.fade / 4f));
			}
		}

		// Token: 0x04000199 RID: 409
		private List<Texture2D> Textures = new List<Texture2D>();

		// Token: 0x0400019A RID: 410
		private List<ExplodingUIElementEffect.ExplosionParticle> Particles = new List<ExplodingUIElementEffect.ExplosionParticle>();

		// Token: 0x02000054 RID: 84
		private struct ExplosionParticle
		{
			// Token: 0x0400019B RID: 411
			public Vector2 Pos;

			// Token: 0x0400019C RID: 412
			public float angle;

			// Token: 0x0400019D RID: 413
			public float speed;

			// Token: 0x0400019E RID: 414
			public float rotation;

			// Token: 0x0400019F RID: 415
			public float rotationRate;

			// Token: 0x040001A0 RID: 416
			public float fade;

			// Token: 0x040001A1 RID: 417
			public int textureIndex;

			// Token: 0x040001A2 RID: 418
			public float Size;

			// Token: 0x040001A3 RID: 419
			public float frictionSlowdownPerSec;

			// Token: 0x040001A4 RID: 420
			public float FlickerOffset;
		}
	}
}
