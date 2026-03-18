using UnityEngine;

public class CharacterVisual : MonoBehaviour
{
    public static CharacterVisual singleton { get; private set; }

    // the only visual parts in equipment are the helmet, chestplate and weapon
    // only the mesh changes depends on the level/rarity of the equipped part
    // 0 common, 1 rare 2 scarce 3 nonexistent
    [Header("Character Body GFX")]
    [SerializeField] MeshFilter bodyGFX;
    [SerializeField] Mesh[] bodyMeshes;

    [SerializeField] MeshFilter helmetGFX;
    [SerializeField] Mesh[] helmetMeshes;

    [SerializeField] MeshFilter weaponGFX;
    [SerializeField] Mesh[] weaponMeshes;

    [Header("Indicators")]
    [SerializeField] ParticleSystem moveEffect;
    [SerializeField] Transform directionalTargetingIndicator;
    [SerializeField] Transform aerialTargetingIndicator;
    [SerializeField] Transform radiusTargetingIndicator;


    private void Awake()
    {
        singleton = this;
    }
    public void MoveVFX(Vector3 VFXPos)
    {
        ParticleSystem moveEffectTemp = Instantiate(moveEffect, VFXPos,moveEffect.transform.rotation);
    }

    //in case you want to change the size and the color
    public void MoveVFX(Vector3 VFXPos,float scale,Color color)
    {
        ParticleSystem moveEffectTemp = Instantiate(moveEffect, VFXPos ,moveEffect.transform.rotation);
        moveEffectTemp.transform.localScale *= scale;
        ParticleSystem.MainModule moveEffectProperties = moveEffectTemp.main;
        moveEffectProperties.startColor = color;
    }

    public void SetIndicator(RangeIndicatorType indicatorType,Vector3 scaleVector, bool on)
    {
        if (indicatorType == RangeIndicatorType.characterDirectional)
        {
            directionalTargetingIndicator.gameObject.SetActive(on);
        }
        else if (indicatorType == RangeIndicatorType.characterStatic)
        {
            radiusTargetingIndicator.gameObject.SetActive(on);
            radiusTargetingIndicator.localScale = scaleVector;
        }
        else
        {
            aerialTargetingIndicator.gameObject.SetActive(on);
        }
    }

    public void TurnOffIndicators()
    {
        directionalTargetingIndicator.gameObject.SetActive(false);
        aerialTargetingIndicator.gameObject.SetActive(false);
        radiusTargetingIndicator.gameObject.SetActive(false);
    }

    public void VisualizeEquipment(int bodyPart,LootDrop rarity)
    {
        if (bodyPart == 0)
            helmetGFX.mesh = helmetMeshes[(int)rarity];
        else if (bodyPart == 1)
            bodyGFX.mesh = bodyMeshes[(int)rarity];
        else if (bodyPart == 2)
            weaponGFX.mesh = weaponMeshes[(int)rarity];
    }
}
