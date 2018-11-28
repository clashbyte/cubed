using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Audio {

	/// <summary>
	/// Internal audio effect processor
	/// </summary>
	internal class AudioEffects {

		/// <summary>
		/// Parent audio system
		/// </summary>
		AudioSystem system;

		/// <summary>
		/// Effects extension
		/// </summary>
		EffectsExtension efx;

		/// <summary>
		/// Effect
		/// </summary>
		uint effect;

		/// <summary>
		/// Auxiliary slot
		/// </summary>
		uint auxSlot;

		/// <summary>
		/// Initializing audio effects
		/// </summary>
		public AudioEffects(AudioSystem parent) {
			system = parent;
			efx = new EffectsExtension();

			// Initializing effect
			effect = (uint)efx.GenEffect();
			
			// Initializing AUX slot
			auxSlot = (uint)efx.GenAuxiliaryEffectSlot();
			efx.BindEffectToAuxiliarySlot(auxSlot, effect);

			EffectsExtension.EaxReverb frev = EffectsExtension.ReverbPresets.Chapel;
			EffectsExtension.EfxEaxReverb rev = new EffectsExtension.EfxEaxReverb();
			EffectsExtension.GetEaxFromEfxEax(ref frev, out rev);

			efx.Effect(effect, EfxEffecti.EffectType, (int)EfxEffectType.Reverb);
			efx.Effect(effect, EfxEffectf.ReverbDensity, rev.Density);
			efx.Effect(effect, EfxEffectf.ReverbDiffusion, rev.Diffusion);
			efx.Effect(effect, EfxEffectf.ReverbGain, rev.Gain);
			efx.Effect(effect, EfxEffectf.ReverbGainHF, rev.GainHF);
			efx.Effect(effect, EfxEffectf.ReverbDecayTime, rev.DecayTime);
			efx.Effect(effect, EfxEffectf.ReverbDecayHFRatio, rev.DecayHFRatio);
			efx.Effect(effect, EfxEffectf.ReverbReflectionsGain, rev.ReflectionsGain);
			efx.Effect(effect, EfxEffectf.ReverbReflectionsDelay, rev.ReflectionsDelay);
			efx.Effect(effect, EfxEffectf.ReverbLateReverbGain, rev.LateReverbGain);
			efx.Effect(effect, EfxEffectf.ReverbLateReverbDelay, rev.LateReverbDelay);
			efx.Effect(effect, EfxEffectf.ReverbAirAbsorptionGainHF, rev.AirAbsorptionGainHF);
			efx.Effect(effect, EfxEffectf.ReverbRoomRolloffFactor, rev.RoomRolloffFactor);
			efx.Effect(effect, EfxEffecti.ReverbDecayHFLimit, rev.DecayHFLimit);
			efx.BindEffectToAuxiliarySlot(auxSlot, effect);

		}

		/// <summary>
		/// Full update
		/// </summary>
		public void Update() {
			
		}

		/// <summary>
		/// Source updating
		/// </summary>
		/// <param name="src"></param>
		public void SourceUpdate(int src) {

			//efx.BindSourceToAuxiliarySlot((uint)src, auxSlot, 0, (uint)EfxFilterType.Null);

		}
	}
}
