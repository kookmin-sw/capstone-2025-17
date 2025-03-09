using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// View 
public class ProfileUI : MonoBehaviour
{
    [SerializeField]
    GameObject characterRender;
    [SerializeField]
    MaterialStorage materials;

    SkinnedMeshRenderer sm_renderer;

    // ��ġ����ŷ ��ư
    public Button randomBtn;
    public Button createBtn;
    public Button joinBtn;

    // ������ ���� ��ư
    public Button optionBtn;
    public Button nicknameBtn;
    public Button nextBtn;
    public Button prevBtn;

    public TMP_InputField nicknameInp;
    public TextMeshProUGUI nicknameTMP;

    private void Awake()
    {
        sm_renderer = characterRender.GetComponent<SkinnedMeshRenderer>();
    }

    // M ���� -> V ������Ʈ
    public void UpdateCharacterUI(int characterId)
    {
        sm_renderer.material = materials.GetMesh(characterId);
    }

    public void UpdateNicknameUI(string nickname)
    {
        nicknameTMP.text = nickname;
    }
}
