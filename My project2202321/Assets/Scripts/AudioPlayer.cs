using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [Tooltip("List of available AudioCollection assets to choose from")]
    public List<AudioCollection> collections = new List<AudioCollection>();

    [Tooltip("Index of the selected AudioCollection in the 'collections' list")]
    public int collectionIndex;

    [Tooltip("Index of the clip inside the selected AudioCollection's ClipCollection")]
    public int clipIndex;

    // Optional: keep track of the currently requested clip
    private AudioClip currentClip;

    // Expose the current clip as read-only so the field is used (prevents editor warnings)
    public AudioClip CurrentClip => currentClip;

    // Returns the AudioClip currently selected by collectionIndex and clipIndex.
    public AudioClip GetSelectedClip()
    {
        if (collections == null || collections.Count == 0) return null;
        if (collectionIndex < 0 || collectionIndex >= collections.Count) return null;
        var col = collections[collectionIndex];
        if (col == null || col.ClipCollection == null || col.ClipCollection.Count == 0) return null;
        if (clipIndex < 0 || clipIndex >= col.ClipCollection.Count) return null;
        return col.ClipCollection[clipIndex];
    }

    // Play the selected clip through the AudioManager singleton
    public void PlaySelected()
    {
        var clip = GetSelectedClip();
        currentClip = clip;
        if (clip == null)
        {
            Debug.LogWarning("AudioPlayer: no clip selected to play.");
            return;
        }
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioPlayer: AudioManager.Instance is null. Make sure an AudioManager exists in the scene.");
            return;
        }
        AudioManager.Instance.PlaySound(clip);
    }

    public void Stop()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.StopSound();
    }

    public void Pause()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PauseSound();
    }

    public void Resume()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.ResumeSound();
    }

    // Set the clip index (and optionally apply immediately when in Play Mode)
    public void SetClipIndex(int index)
    {
        clipIndex = index;
        if (Application.isPlaying)
        {
            // Apply immediately
            PlaySelected();
        }
    }

    // Convenience: set collection index
    public void SetCollectionIndex(int index)
    {
        collectionIndex = index;
        if (Application.isPlaying)
        {
            PlaySelected();
        }
    }
}
