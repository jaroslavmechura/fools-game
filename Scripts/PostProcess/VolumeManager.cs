using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumeManager : MonoBehaviour
{
    [Header("--- VolumeProfiles ---")]
    [SerializeField] private VolumeProfile basicProfile;
    [SerializeField] private VolumeProfile bulletTimeProfile;
    [SerializeField] private VolumeProfile killProfile;

    private bool isInBulletTime = false;

    private Volume volume;

    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile = basicProfile;
    }

    public void SetVolume(string volumeType, bool isBulletTime) {
        if (isInBulletTime && !isBulletTime && volumeType == "kill") {
            volume.profile = killProfile;
        }
        else if (isInBulletTime && !isBulletTime) return;

        if (volumeType == "basic" && isInBulletTime && !isBulletTime) {
            volume.profile = bulletTimeProfile;
        }

        if (volumeType == "basic")
        {
            if (isBulletTime) isInBulletTime = false;

            volume.profile = basicProfile;
        }
        else if (volumeType == "bulletTime")
        {
            if (isBulletTime) isInBulletTime = true;

            volume.profile = bulletTimeProfile;
        }
        else if (volumeType == "kill")
        {
            volume.profile = killProfile;
        }         
    }
}
