using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    [Header("Assign the Global Volume here")]
    [SerializeField] private Volume postProcessVolume;

    [Header("Post Processing Profiles for Different Levels")]
    [SerializeField] private VolumeProfile postProfileMain;
    [SerializeField] private VolumeProfile postProfileLevel2;
    [SerializeField] private VolumeProfile postProfileLevel3;
    [SerializeField] private VolumeProfile postProfileLevel4;

    // ✅ Switch to Main Post-Processing
    public void MainPostProcess()
    {
        if (postProcessVolume != null && postProfileMain != null)
            postProcessVolume.profile = postProfileMain;
    }

    // ✅ Switch to Secondary (Level 2)
    public void SecondaryPostProcess()
    {
        if (postProcessVolume != null && postProfileLevel2 != null)
            postProcessVolume.profile = postProfileLevel2;
    }

    // ✅ Optionally add Level 3 & 4
    public void Level3PostProcess()
    {
        if (postProcessVolume != null && postProfileLevel3 != null)
            postProcessVolume.profile = postProfileLevel3;
    }

    public void Level4PostProcess()
    {
        if (postProcessVolume != null && postProfileLevel4 != null)
            postProcessVolume.profile = postProfileLevel4;
    }
}
