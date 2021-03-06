using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace HumanBuilders {
  /// <summary>
  /// This class is a track mixer utilized by PlayerTracks to mix together the player's <see cref="FiniteStateMachine" /> and transform information.
  /// </summary>
  /// <seealso cref="PlayerCharacter"/>
  /// <seealso cref="PlayerCharacterTrack"/>
  /// <seealso cref="PoseClip"/>
  /// <seealso cref="AbsolutePoseClip"/>
  /// <seealso cref="RelativePoseClip"/>
  public class PlayerCharacterTrackMixer : PlayableBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// A cache of state drivers for the finite state machine.
    /// </summary>
    private Dictionary<Type, StateDriver> stateDrivers;

    /// <summary>
    /// How to handle the end of a track.
    /// 
    /// <list type="bullet">
    /// <item> Resume - Resume normal play from where the track put the player. </item>
    /// <item> Freeze - Freeze the player in place where the track put them.</item>
    /// <item> Revert - Move the player back to their original state prior to the track.</item>
    /// </list>
    /// </summary>
    public OutroSetting Outro;

    /// <summary>
    /// A snapshot of the way the player was before the cutscene.
    /// </summary>
    public PlayerSnapshot GraphSnapshot;

    /// <summary>
    /// virtual transform info about the player used to combine both relative
    /// and absolute changes in position.
    /// </summary>
    public PlayerSnapshot VirtualSnapshot;
    #endregion


    #region Playable API
    //-------------------------------------------------------------------------
    // Playable API
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Process a single frame of animation.
    /// </summary>
    /// <param name="playable">The playable this mixer is for.</param>
    /// <param name="info">Extra information about this frame.</param>
    /// <param name="playerData">Data bound to the track that this mixer is on.</param>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
      PlayerCharacter player = playerData != null ? (PlayerCharacter)playerData : GameManager.Player;
      if (player == null) {
        player = GameObject.FindObjectOfType<PlayerCharacter>(true);
      }

      if (TimelineTools.IsSingleClipPlaying(playable, out int clipIndex)) {
        MixSingle(playable, player, clipIndex);

      } else {

        TimelineTools.FindClipsToMix(playable, out int clipIndexA, out int clipIndexB);
        if (TimelineTools.ClipsAreValid(clipIndexA, clipIndexB)) {
          MixMultiple(playable, player, clipIndexA, clipIndexB);
        }          
      }
    }
  
    /// <summary>
    /// Fires when the graph for the timeline begins playing.
    /// </summary>
    public override void OnGraphStart(Playable playable) {
      stateDrivers = new Dictionary<Type, StateDriver>();

      PlayerCharacter player = GameManager.Player != null ? GameManager.Player : GameObject.FindObjectOfType<PlayerCharacter>(true);
      if (player == null) {
        return;
      }

      // Keep the player from changing animation/behavior states.
      player.FSM.Pause();

      // Take a snapshot of where the player started for the sake of potentially
      // restoring them later.
      GraphSnapshot = new PlayerSnapshot(player);

      // During mixing, the virtual snapshot is updated as needed by absolute posing and referenced by
      // relative posing.
      VirtualSnapshot = GraphSnapshot;

      Rigidbody2D rb = player.GetComponentInChildren<Rigidbody2D>(true);
      if (rb != null) {
        rb.gravityScale = 0;
        rb.velocity = Vector3.zero;
      }
    }

    /// <summary>
    /// Fires once the graph for the timeline stops playing.
    /// </summary>
    public override void OnGraphStop(Playable playable) {
      PlayerCharacter player = GameManager.Player != null ? GameManager.Player : GameObject.FindObjectOfType<PlayerCharacter>(true);
      if (player == null) {
        return;
      }

      Rigidbody2D rb = player.GetComponentInChildren<Rigidbody2D>(true);
      if (rb != null) {
        rb.gravityScale = 1;
        rb.velocity = Vector3.zero;
      }

      #if UNITY_EDITOR
      if (Application.isPlaying) {
      #endif

      switch (Outro) {
        case OutroSetting.Resume: {
          // Bring them back to their original animation state, but keep their
          // ending position.
          GraphSnapshot.RestoreState(player);
          GraphSnapshot.RestoreCollider(player);
          GraphSnapshot.RestoreSprite(player);
          player.gameObject.SetActive(true);
          player.FSM.Resume();
          break;
        }
        case OutroSetting.Freeze: {
          // Keep the player's state exactly as is. This is meant be used for
          // situations where back-to-back timelines will be playing and you
          // need to keep the player where they are.
          GraphSnapshot.RestoreCollider(player);
          player.Physics.GravityScale = 0;
          break;
        }
        case OutroSetting.Revert: {
          // Bring them back to their original animation state and position from
          // before the timeline.
          GraphSnapshot.Restore(player);
          player.FSM.Resume();
          break;
        }
      }

      #if UNITY_EDITOR
      } else {
        // Restore transform on de-selecting the timeline asset in-editor.
        // Restore everything except state, since the player's FSM isn't running
        // at edit-time.
        GraphSnapshot.RestoreTransform(player);
        GraphSnapshot.RestoreCollider(player);
        GraphSnapshot.RestoreActive(player);
        GraphSnapshot.RestoreFacing(player);
        GraphSnapshot.RestoreSprite(player);
      } 
      #endif
    }
    #endregion

    #region Clip Mixing
    //-------------------------------------------------------------------------
    // Clip Mixing
    //-------------------------------------------------------------------------

    /// <summary>
    /// Mix a single clip.
    /// </summary>
    /// <param name="playable">The track's playable</param>
    /// <param name="player">The player character</param>
    /// <param name="clipIndex">The index of the clip to mix</param>
    private void MixSingle(Playable playable, PlayerCharacter player, int clipIndex) {
      PoseInfo pose = TimelineTools.GetClipInfo<PoseInfo>(playable, clipIndex);
      
      player.gameObject.SetActive(pose.Active);

      if (PoseTools.IsAbsolute(pose)) { // Absolute
        MixAbsolute(player, (AbsolutePoseInfo)pose);

      } else { // Relative
        VirtualSnapshot.RestoreTransform(player);
        MixRelative(player, (RelativePoseInfo)pose);
      }

      StateDriver driver = GetDriver(pose);
      UpdateFacing(player, pose);
      UpdateState(player, driver, playable, pose);
    }

    /// <summary>
    /// Mix together two active clips based on what type of clips they are.
    /// </summary>
    /// <param name="playable">The track's playable</param>
    /// <param name="player">The player character</param>
    /// <param name="clipIndexA">The index of the first clip to mix</param>
    /// <param name="clipIndexB">The index of the second clip to mix</param>
    private void MixMultiple(Playable playable, PlayerCharacter player, int clipIndexA, int clipIndexB) {
      PoseInfo poseA = TimelineTools.GetClipInfo<PoseInfo>(playable, clipIndexA);
      float weightA = playable.GetInputWeight(clipIndexA);

      PoseInfo poseB = TimelineTools.GetClipInfo<PoseInfo>(playable, clipIndexB);
      float weightB = playable.GetInputWeight(clipIndexB);     

      // Mix together clips based on their typing.
      if (PoseTools.IsAbsolute(poseA) && PoseTools.IsAbsolute(poseB)) {
        MixAbsolute(player, (AbsolutePoseInfo)poseA, weightA);
        MixAbsolute(player, (AbsolutePoseInfo)poseB, weightB);

      } else if (PoseTools.IsRelative(poseA) && PoseTools.IsRelative(poseB)) {
        VirtualSnapshot.RestoreTransform(player);

        RelativePoseInfo mixed = new RelativePoseInfo();
        mixed.Position = poseA.Position*weightA + poseB.Position*weightB;
        mixed.Rotation = poseA.Rotation*weightA + poseB.Rotation*weightB;
        mixed.Scale = poseA.Scale*weightA + poseB.Scale*weightB;

        MixRelative(player, mixed);
      } else {
        MixAbsoluteRelative(player, poseA, weightA, poseB, weightB);
      }

      // Apply player facing and FSM state based on which pose is weighted heavier.
      PoseInfo dominantPose = weightA > weightB ? poseA : poseB;
      player.gameObject.SetActive(dominantPose.Active);
      StateDriver driver = GetDriver(dominantPose);
      UpdateFacing(player, dominantPose);
      UpdateState(player, driver, playable, dominantPose);
    }

    /// <summary>
    /// Apply the pose of an absolute clip.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="pose">The pose to apply</param>
    /// <param name="weight">(optional) The weighting on the pose, used to
    /// interpolate this pose with another if necessary. Default: 1.</param>
    /// <param name="updateVirtualSnapshot">(optional) Whether or not to update the mixer's virtual snapshot
    /// of the player character. When mixing multiple absolute poses, this
    /// action can be saved for the last pose applied. Default: true.</param>
    private void MixAbsolute(PlayerCharacter player, AbsolutePoseInfo pose, float weight = 1f, bool updateVirtualSnapshot = true) {
      PoseTools.MixAbsolute(player, VirtualSnapshot, pose, weight);

      if (updateVirtualSnapshot) {
        VirtualSnapshot = new PlayerSnapshot(player);
      }
    }

    /// <summary>
    /// Apply the pose of a relative clip.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="pose">The pose to apply</param>
    /// <param name="weight">(optional) The weighting on the pose, used to
    /// interpolate this pose with another if necessary. Default: 1.</param>
    private void MixRelative(PlayerCharacter player, RelativePoseInfo pose, float weight = 1f) {
      PoseTools.MixRelative(player, VirtualSnapshot, pose, weight);
    }

    /// <summary>
    /// Mix together a relative and absolute pose. For inputs, which pose is which doesn't matter.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="poseA">The first pose</param>
    /// <param name="weightA">Interpolation weight for the first pose</param>
    /// <param name="poseB">The second pose</param>
    /// <param name="weightB">Interpolation weight for the second pose</param>
    private void MixAbsoluteRelative(PlayerCharacter player, PoseInfo poseA, float weightA, PoseInfo poseB, float weightB) {
      AbsolutePoseInfo absPose;
      RelativePoseInfo relPose;
      float absWeight;
      float relWeight;

      if (PoseTools.IsAbsolute(poseA)) {
        absPose = (AbsolutePoseInfo)poseA;
        relPose = (RelativePoseInfo)poseB;
        absWeight = weightA;
        relWeight = weightB;
      } else {
        absPose = (AbsolutePoseInfo)poseB;
        relPose = (RelativePoseInfo)poseA;
        absWeight = weightB;
        relWeight = weightA;
      }

      MixAbsolute(player, absPose, absWeight);
      VirtualSnapshot.RestoreTransform(player);
      MixRelative(player, relPose, relWeight);
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// Get the state driver for the current state.
    /// </summary>
    /// <param name="pose">The pose clip.</param>
    /// <returns>The state driver for the current state.</returns>
    private StateDriver GetDriver(PoseInfo pose) {
      if (pose == null || pose.State == null) {
        return null;
      }

      if (!stateDrivers.ContainsKey(pose.State)) {
        stateDrivers.Add(pose.State, StateDriver.For(pose.State));
      }
      return stateDrivers[pose.State];
    }

    /// <summary>
    /// Update which way the player is facing if necessary.
    /// </summary>
    /// <param name="player">The player character.</param>
    /// <param name="pose">The target pose for the player.</param>
    private void UpdateFacing(PlayerCharacter player, PoseInfo pose) {
      if (pose.Flipped) {
        player.SetFacing(Facing.Left);
      } else {
        player.SetFacing(Facing.Right);
      }
    }

    /// <summary>
    /// Update the animation/FSM state for the player.
    /// </summary>
    /// <param name="player">The player character.</param>
    /// <param name="driver">The state driver for the target state.</param>
    /// <param name="playable">The playable associated with this clip.</param>
    /// <param name="pose">Information about the player's pose.</param>
    private void UpdateState(PlayerCharacter player, StateDriver driver, Playable playable, PoseInfo pose) {
      if (driver == null) {
        return;
      }

      #if UNITY_EDITOR
      if (Application.isPlaying) {
      #endif

        if (!driver.IsInState(player.FSM)) {
          driver.ForceStateChangeOn(player.FSM);
        }

      #if UNITY_EDITOR
      } else {
        // In-Editor we don't want to actually change the Finite State Machine,
        // so just play the appropriate animation.
        float totalTrackTime = (float)playable.GetTime();
        float currentTimeInClip = totalTrackTime - pose.StartTime;

        driver.SampleClip(player.FSM, currentTimeInClip);
      }
      #endif
    }
    #endregion
  }
}