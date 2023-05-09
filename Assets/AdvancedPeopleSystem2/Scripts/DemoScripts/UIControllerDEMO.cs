using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AdvancedPeopleSystem;
using System.Collections.Generic;

/// <summary>
/// This script was created to demonstrate api, I do not recommend using it in your projects.
/// </summary>
public class UIControllerDEMO : MonoBehaviour
{
    [Space(5)]
    [Header("I do not recommend using it in your projects")]
    [Header("This script was created to demonstrate api")]

    public CharacterCustomization CharacterCustomization;
    [Space(15)]

    public Text playbutton_text;

    public Text bake_text;
    public Text lod_text;

    public Text panelNameText;

    public Slider fatSlider;
    public Slider musclesSlider;
    public Slider thinSlider;

    public Slider slimnessSlider;
    public Slider breastSlider;

    public Slider heightSlider;

    public Slider legSlider;

    public Slider headSizeSlider;

    public Slider headOffsetSlider;

    public Slider[] faceShapeSliders;

    public RectTransform HairPanel;
    public RectTransform BeardPanel;
    public RectTransform ShirtPanel;
    public RectTransform PantsPanel;
    public RectTransform ShoesPanel;
    public RectTransform HatPanel;
    public RectTransform AccessoryPanel;
    public RectTransform BackpackPanel;

    public RectTransform FaceEditPanel;
    public RectTransform BaseEditPanel;

    public RectTransform SkinColorPanel;
    public RectTransform EyeColorPanel;
    public RectTransform HairColorPanel;
    public RectTransform UnderpantsColorPanel;

    public RectTransform EmotionsPanel;

    public RectTransform SavesPanel;
    public RectTransform SavesPanelList;
    public RectTransform SavesPrefab;
    public List<RectTransform> SavesList = new List<RectTransform>();

    public Image SkinColorButtonColor;
    public Image EyeColorButtonColor;
    public Image HairColorButtonColor;
    public Image UnderpantsColorButtonColor;

    public Vector3[] CameraPositionForPanels;
    public Vector3[] CameraEulerForPanels;
    int currentPanelIndex = 0;

    public Camera Camera;

    public RectTransform femaleUI;
    public RectTransform maleUI;

    #region ButtonEvents
    public void SwitchCharacterSettings(string name)
    {
        CharacterCustomization.SwitchCharacterSettings(name);
        if(name == "Male")
        {
            maleUI.gameObject.SetActive(true);
            femaleUI.gameObject.SetActive(false);
        }
        if (name == "Female")
        {
            femaleUI.gameObject.SetActive(true);
            maleUI.gameObject.SetActive(false);
        }
    }
    public void ShowFaceEdit()
    {
        FaceEditPanel.gameObject.SetActive(true);
        BaseEditPanel.gameObject.SetActive(false);
        currentPanelIndex = 1;
        panelNameText.text = "FACE CUSTOMIZER";
    }

    public void ShowBaseEdit()
    {
        FaceEditPanel.gameObject.SetActive(false);
        BaseEditPanel.gameObject.SetActive(true);
        currentPanelIndex = 0;
        panelNameText.text = "BASE CUSTOMIZER";
    }
    
    public void SetFaceShape(int index)
    {
        var faceBlendshape = CharacterCustomization.GetBlendshapeDatasByGroup(CharacterBlendShapeGroup.Face);


        CharacterCustomization.SetBlendshapeValue(true,faceBlendshape[index].type, faceShapeSliders[index].value);
    }

    public void SetHeadOffset()
    {
        slided = true;
        CharacterCustomization.SetBlendshapeValue(true, CharacterBlendShapeType.Head_Offset, headOffsetSlider.value);
    }
    
    public void BodyFat()
    {
        slided = true;
        CharacterCustomization.SetBlendshapeValue(true, CharacterBlendShapeType.Fat, fatSlider.value);
    }
    public void BodyMuscles()
    {
        slided = true;
        CharacterCustomization.SetBlendshapeValue(true, CharacterBlendShapeType.Muscles, musclesSlider.value);
    }
    public void BodyThin()
    {
        slided = true;
        CharacterCustomization.SetBlendshapeValue(true, CharacterBlendShapeType.Thin, thinSlider.value);
    }

    public void BodySlimness()
    {
        slided = true;
        CharacterCustomization.SetBlendshapeValue(true, CharacterBlendShapeType.Slimness, slimnessSlider.value);
    }
    public void BodyBreast()
    {
        CharacterCustomization.SetBlendshapeValue(true, CharacterBlendShapeType.BreastSize, breastSlider.value,
            new string[] { "Chest", "Stomach", "Head" },
            new CharacterElementType[] { CharacterElementType.Shirt }
            );
    }
    public void SetHeight()
    {
        slided = true;
        CharacterCustomization.SetHeight(heightSlider.value);
    }
    public void SetHeadSize()
    {
        slided = true;
        CharacterCustomization.SetHeadSize(headSizeSlider.value);
    }
    int lodIndex;
    public void Lod_Event(int next)
    {
        lodIndex += next;
        if (lodIndex < 0)
            lodIndex = 3;
        if (lodIndex > 3)
            lodIndex = 0;

        lod_text.text = lodIndex.ToString();

        CharacterCustomization.ForceLOD(lodIndex);
    }
    public void SetNewSkinColor(Color color)
    {
        SkinColorButtonColor.color = color;
        CharacterCustomization.SetBodyColor(BodyColorPart.Skin, color);
    }
    public void SetNewEyeColor(Color color)
    {
        EyeColorButtonColor.color = color;
        CharacterCustomization.SetBodyColor(BodyColorPart.Eye, color);
    }
    public void SetNewHairColor(Color color)
    {
        HairColorButtonColor.color = color;
        CharacterCustomization.SetBodyColor(BodyColorPart.Hair, color);
    }
    public void SetNewUnderpantsColor(Color color)
    {
        UnderpantsColorButtonColor.color = color;
        CharacterCustomization.SetBodyColor(BodyColorPart.Underpants, color);
    }
    public void VisibleSkinColorPanel(bool v)
    {
        HideAllPanels();
        SkinColorPanel.gameObject.SetActive(v);
    }
    public void VisibleEyeColorPanel(bool v)
    {
        HideAllPanels();
        EyeColorPanel.gameObject.SetActive(v);
    }
    public void VisibleHairColorPanel(bool v)
    {
        HideAllPanels();
        HairColorPanel.gameObject.SetActive(v);
    }
    public void VisibleUnderpantsColorPanel(bool v)
    {
        HideAllPanels();
        UnderpantsColorPanel.gameObject.SetActive(v);
    }
    public void ShirtPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            ShirtPanel.gameObject.SetActive(false);
        else
            ShirtPanel.gameObject.SetActive(true);
    }
    public void PantsPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            PantsPanel.gameObject.SetActive(false);
        else
            PantsPanel.gameObject.SetActive(true);
    }
    public void ShoesPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            ShoesPanel.gameObject.SetActive(false);
        else
            ShoesPanel.gameObject.SetActive(true);
    }
    public void BackpackPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            BackpackPanel.gameObject.SetActive(false);
        else
            BackpackPanel.gameObject.SetActive(true);
    }
    public void HairPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            HairPanel.gameObject.SetActive(false);
        else
            HairPanel.gameObject.SetActive(true);

        currentPanelIndex = (v) ? 1 : 0;
    }
    public void BeardPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            BeardPanel.gameObject.SetActive(false);
        else
            BeardPanel.gameObject.SetActive(true);

        currentPanelIndex = (v) ? 1 : 0;
    }
    public void HatPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            HatPanel.gameObject.SetActive(false);
        else
            HatPanel.gameObject.SetActive(true);
        currentPanelIndex = (v) ? 1 : 0;
    }
    public void EmotionsPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            EmotionsPanel.gameObject.SetActive(false);
        else
            EmotionsPanel.gameObject.SetActive(true);
        currentPanelIndex = (v) ? 1 : 0;
    }
    public void AccessoryPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            AccessoryPanel.gameObject.SetActive(false);
        else
            AccessoryPanel.gameObject.SetActive(true);
        currentPanelIndex = (v) ? 1 : 0;
    }

    public void SavesPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
        {
            SavesPanel.gameObject.SetActive(false);
            foreach(var save in SavesList)
            {
                Destroy(save.gameObject);
            }
            SavesList.Clear();
        }
        else
        {
            var saves = CharacterCustomization.GetSavedCharacterDatas();
            for (int i = 0; i < saves.Count;i++)
            {
                var savePrefab = Instantiate(SavesPrefab, SavesPanelList);
                int index = i;
                savePrefab.GetComponent<Button>().onClick.AddListener(() => SaveSelect(index));
                savePrefab.GetComponentInChildren<Text>().text = string.Format("({0}) {1}",index,saves[i].name);
                SavesList.Add(savePrefab);
            }
            SavesPanel.gameObject.SetActive(true);
        }
    }
    public void SaveSelect(int index)
    {
        var saves = CharacterCustomization.GetSavedCharacterDatas();
        CharacterCustomization.ApplySavedCharacterData(saves[index]);
    }
    public void EmotionsChange_Event(int index)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.uiObject.SetActive(false);
            GameManager.instance.FreeCursor(false);
            GameManager.instance.mycontroller.blendShapeIndex = index;
        }
        var anim = CharacterCustomization.Settings.characterAnimationPresets[index];
        if (anim != null)
            CharacterCustomization.PlayBlendshapeAnimation(anim.name, 2f);
    }
    public void HairChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Hair, index);
    }
    public void BeardChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Beard, index);
    }
    public void ShirtChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Shirt, index);
    }
    public void PantsChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Pants, index);
    }
    public void ShoesChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Shoes, index);
    }
    public void BackpackChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Item1, index);
    }
    public void HatChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Hat, index);
    }
    public void AccessoryChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(CharacterElementType.Accessory, index);
    }
    public void HideAllPanels()
    {
        
        if (SkinColorPanel != null)
            SkinColorPanel.gameObject.SetActive(false);
        if (EyeColorPanel != null)
            EyeColorPanel.gameObject.SetActive(false);
        if (HairColorPanel != null)
            HairColorPanel.gameObject.SetActive(false);
        if (UnderpantsColorPanel != null)
            UnderpantsColorPanel.gameObject.SetActive(false);
        if (EmotionsPanel != null)
            EmotionsPanel.gameObject.SetActive(false);
        if (BeardPanel != null)
            BeardPanel.gameObject.SetActive(false);
        if (HairPanel != null)
            HairPanel.gameObject.SetActive(false);
        if (ShirtPanel != null)
            ShirtPanel.gameObject.SetActive(false);
        if (PantsPanel != null)
            PantsPanel.gameObject.SetActive(false);
        if (ShoesPanel != null)
            ShoesPanel.gameObject.SetActive(false);
        if (BackpackPanel != null)
            BackpackPanel.gameObject.SetActive(false);
        if (HatPanel != null)
            HatPanel.gameObject.SetActive(false);
        if (AccessoryPanel != null)
            AccessoryPanel.gameObject.SetActive(false);
        if (SavesPanel != null)
            SavesPanel.gameObject.SetActive(false);

        currentPanelIndex = 0;
    }
    public void SaveToFile()
    {
        CharacterCustomization.SaveCharacterToFile(CharacterCustomizationSetup.CharacterFileSaveFormat.Json);
    }
    public void ClearFromFile()
    {
        SavesPanel.gameObject.SetActive(false);
        CharacterCustomization.ClearSavedData();
    }
    public void Randimize()
    {
        CharacterCustomization.Randomize();
    }
    bool walk_active = false;

    public Animator dummyanimator;
    public void PlayAnim()
    {
        walk_active = !walk_active;

        dummyanimator.SetFloat("Speed", walk_active?1.5f:0);
        dummyanimator.SetFloat("MotionSpeed", walk_active ? 1.5f : 0);

        playbutton_text.text = (walk_active) ? "STOP" : "PLAY";
    }
    #endregion

    bool canvasVisible = true;
    bool slided = false;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && GameManager.IngameScene())
        {
            canvasVisible = !canvasVisible;

            //GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>().enabled = canvasVisible;
        }

        if(SkinColorButtonColor!=null)
            SkinColorButtonColor.color = CharacterCustomization.GetBodyColor(BodyColorPart.Skin);
        if (EyeColorButtonColor != null)
            EyeColorButtonColor.color = CharacterCustomization.GetBodyColor(BodyColorPart.Eye);
        if (HairColorButtonColor != null)
            HairColorButtonColor.color = CharacterCustomization.GetBodyColor(BodyColorPart.Hair);
        if (UnderpantsColorButtonColor != null)
            UnderpantsColorButtonColor.color = CharacterCustomization.GetBodyColor(BodyColorPart.Underpants);
        if(!slided)
        {
            if (fatSlider != null)
                fatSlider.value = CharacterCustomization.fat;
            if (musclesSlider != null)
                musclesSlider.value = CharacterCustomization.breastsize;
            if (thinSlider != null)
                thinSlider.value = CharacterCustomization.slimness;
            if (slimnessSlider != null)
                slimnessSlider.value = CharacterCustomization.slimness;
            if (heightSlider != null)
                heightSlider.value = CharacterCustomization.heightValue;
            if (headSizeSlider != null)
                headSizeSlider.value = CharacterCustomization.headSizeValue;
            if (headOffsetSlider != null)
                headOffsetSlider.value = CharacterCustomization.headOffset;
        }
        

        //Camera.transform.position = Vector3.Lerp(Camera.transform.position, CameraPositionForPanels[currentPanelIndex], Time.deltaTime * 5);
        //Camera.transform.eulerAngles = Vector3.Lerp(Camera.transform.eulerAngles, CameraEulerForPanels[currentPanelIndex], Time.deltaTime * 5);
    }
}
