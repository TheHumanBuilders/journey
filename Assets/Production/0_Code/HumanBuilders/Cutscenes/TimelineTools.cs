﻿

using UnityEngine;
using UnityEngine.Playables;

namespace HumanBuilders {
  /// <summary>
  /// Functions that can be used by any Track Mixer.
  /// </summary>
  public static class TimelineTools {

    /// <summary>
    /// Get information for a clip from the track's playable.
    /// </summary>
    /// <param name="playable">The track's playable</param>
    /// <param name="clipIndex">The index of the clip to get</param>
    /// <typeparam name="T">The type of the playable behaviour.</typeparam>
    /// <returns>Clip information for the clip at the index "clipIndex".</returns>
    public static T GetClipInfo<T>(Playable playable, int clipIndex) where T : class, IPlayableBehaviour, new() => ((ScriptPlayable<T>)playable.GetInput(clipIndex)).GetBehaviour();
    
    /// <summary>
    /// Determines whether or not this track should be playing a single clip at full weight.
    /// </summary>
    /// <param name="playable">The track playable</param>
    /// <param name="clipIndex">The output index of the clip</param>
    /// <returns>True if only one clip is active on the track at the current frame.</returns>
    public static bool IsSingleClipPlaying(Playable playable, out int clipIndex) {
      for (int i = 0; i < playable.GetInputCount(); i++) {
        if (playable.GetInputWeight(i) == 1f) {
          clipIndex = i;
          return true;
        }
      }

      clipIndex = -1;
      return false;
    }


    /// <summary>
    /// Get the two clips that need to be mixed together for the current frame.
    /// </summary>
    /// <param name="playable">The track's playable.</param>
    /// <param name="clipIndexA">The output index for the first clip. Use with
    /// TimelineTools.GetPlayableBehaviour to get the corresponding clip.</param>
    /// <param name="clipIndexB">The output index for the second clip. Use with
    /// TimelineTools.GetPlayableBehaviour to get the corresponding clip.</param>
    public static void FindClipsToMix(Playable playable, out int clipIndexA, out int clipIndexB) {
      for (int i = 0; i < playable.GetInputCount(); i++) {
        float weight = playable.GetInputWeight(i);

        if (weight > 0 && weight < 1f) {
          clipIndexA = i;
          clipIndexB = i+1;
          return;
        }
      }

      clipIndexA = -1;
      clipIndexB = -1;
    }


    /// <summary>
    /// Whether or not the indices for the desired clips are valid.
    /// </summary>
    /// <param name="clipIndexA">The index of the first clip</param>
    /// <param name="clipIndexB">The index of the second clip.</param>
    /// <returns>True if both indices for the desired clips are valid.</returns>
    public static bool ClipsAreValid(int clipIndexA, int clipIndexB) => clipIndexA >= 0 && clipIndexB > 0 && clipIndexA != clipIndexB;

  }
}