using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    public static TextBox singleton { get; private set; }

    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject dialogueTextParent;
    [SerializeField] Image characterImage;
    [SerializeField] Sprite[] characterExpressions;
    [SerializeField] Image interactedWithImage;
    [SerializeField] float textRate;
    //[SerializeField] AudioClip CharacterSpeechSFX;
    private AudioSource AS;
    private float originalPitch;
    private DialogueText.Dialogue[] dialogue;
    private float timeTillNextChar;
    private float timeTillTextClearAuto;
    private int i;


    private float[] pullAmount;
    private Vector2[] pullOrNot;

    private void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        AS = GetComponent<AudioSource>();
        originalPitch = AS.pitch;

        dialogueText.text = string.Empty;
        //dialogueText.enabled = false;
        dialogueTextParent.SetActive(false);

        
        pullAmount = new float[500];
        pullOrNot = new Vector2[500];
        for (int j =0;j<500;j++)
        {
            pullAmount[j] = Random.Range(-0.25f, -1f);
            pullOrNot[j].x = Random.Range(0, 2);
            pullOrNot[j].y = Random.Range(0, 2);
            
            //change later
            if (pullOrNot[j].x == 0 && pullOrNot[j].y == 0)
            {
                if (Random.Range(0, 2) == 0)
                    pullOrNot[j].x = 1;
                else
                    pullOrNot[j].y = 1;
            }
        }
        

        /*
        pullAmount = new float[dialogueText.textInfo.characterCount];
        pullOrNot = new Vector2[dialogueText.textInfo.characterCount];
        for (int j = 0; j < dialogueText.textInfo.characterCount; j++)
        {
            pullAmount[j] = Random.Range(-0.25f, -1f);
            pullOrNot[j].x = Random.Range(0, 2);
            pullOrNot[j].y = Random.Range(0, 2);
        }
         */
    }

    void Update()
    {
        if (dialogue!= null)
        {
            TextEffect(dialogue[i]);
            if (dialogue[dialogue.Length - 1].text.Length > 0)
            {
                if (dialogue[i].text.Length > 0)
                {
                    if (Time.time > timeTillNextChar)
                    {
                        timeTillNextChar = Time.time + textRate;
                        dialogueText.text += dialogue[i].text.Substring(0, 1);
                        dialogue[i].text = dialogue[i].text.Substring(1);
                        if (dialogue[i].text.Length%2==0)
                        {
                            AS.pitch = Random.Range(originalPitch *0.9f, originalPitch * 1.1f);
                            AS.Play();     
                        }
                    }
                }
                else if (Time.time > timeTillTextClearAuto)
                {
                    dialogueText.text = string.Empty;
                    i++;
                    characterImage.sprite = characterExpressions[(int)dialogue[i].playerExpression];
                    timeTillTextClearAuto = Time.time + dialogue[i].text.Length * textRate * 3.5f; //2.5f
                }
            }
            else if (i == dialogue.Length-1 && Time.time>timeTillTextClearAuto)
            {
                dialogueText.text = string.Empty;
                i = 0;
                characterImage.sprite = characterExpressions[0]; //return to default
                //dialogueText.enabled = false;
                dialogueTextParent.SetActive(false);
            }
        }
    }

    public bool IsDialogueActive()
    {
        if (dialogueTextParent.activeSelf)
            return true;
        return false;
    }

    public void DisplayTexts(DialogueText.Dialogue[] dialogueToDisplay)
    {
        dialogue = new DialogueText.Dialogue[dialogueToDisplay.Length];
        System.Array.Copy(dialogueToDisplay, dialogue, dialogueToDisplay.Length);
        //dialogue = dialogueToDisplay; //this removed the text on the original object, meaning I couldn't interact with it again
        timeTillNextChar = Time.time;
        timeTillTextClearAuto = Time.time + dialogue[0].text.Length * textRate * 3.5f; //2.5f
        //TextEffect(dialogue[0]);
        dialogueText.text = string.Empty;
        //dialogueText.enabled = true;
        dialogueTextParent.SetActive(true);
        characterImage.sprite = characterExpressions[(int)dialogueToDisplay[0].playerExpression];
    }

    public void SkipInteraction()
    {
        if (dialogue[i].text.Length == 0)
            timeTillTextClearAuto = Time.time;
        else
        {
            timeTillTextClearAuto -= dialogue[i].text.Length * textRate * 2;
            dialogueText.text += dialogue[i].text.Substring(0, dialogue[i].text.Length);
            dialogue[i].text = dialogue[i].text.Substring(dialogue[i].text.Length);
        }
    }

    private void TextEffect(DialogueText.Dialogue dialogueEffect)
    {
        if (dialogueEffect.playerExpression == TextEffects.Fear) //surprise -> anger -> fear
        {
            for (int i = 0; i < dialogueText.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = dialogueText.textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] sourceVertices = dialogueText.textInfo.meshInfo[materialIndex].vertices;

                sourceVertices[vertexIndex + 0] += new Vector3(0, pullAmount[i] * Time.deltaTime * 10 * pullOrNot[i].x, 0); //bottom left *15
                sourceVertices[vertexIndex + 1] += new Vector3(0, 0, 0); //top left
                sourceVertices[vertexIndex + 2] += new Vector3(0, 0, 0); // top right
                sourceVertices[vertexIndex + 3] += new Vector3(0, pullAmount[i] * Time.deltaTime * 10 * pullOrNot[i].y, 0); // bottom right * 15

                dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }

        if (dialogueEffect.playerExpression == TextEffects.Surprise)
        {
            for (int i = 0; i < dialogueText.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = dialogueText.textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] sourceVertices = dialogueText.textInfo.meshInfo[materialIndex].vertices;

                float waveOffset = Mathf.Sin(Time.time * 1.5f + i * 0.5f) * 0.35f;

                sourceVertices[vertexIndex + 0] += new Vector3(0, waveOffset * Time.deltaTime * 15, 0);
                sourceVertices[vertexIndex + 1] += new Vector3(0, waveOffset * Time.deltaTime * 15, 0);
                sourceVertices[vertexIndex + 2] += new Vector3(0, waveOffset * Time.deltaTime * 15, 0);
                sourceVertices[vertexIndex + 3] += new Vector3(0, waveOffset * Time.deltaTime * 15, 0);

                dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }

        if (dialogueEffect.playerExpression == TextEffects.Anger)
        {
            for (int i = 0; i < dialogueText.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = dialogueText.textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] sourceVertices = dialogueText.textInfo.meshInfo[materialIndex].vertices;

                sourceVertices[vertexIndex + 0] += new Vector3(-2.5f * Time.deltaTime, -1 * Time.deltaTime, 0); //bottom left
                sourceVertices[vertexIndex + 1] += new Vector3(-2.5f * Time.deltaTime, 1 * Time.deltaTime, 0); //top left
                sourceVertices[vertexIndex + 2] += new Vector3(2.5f * Time.deltaTime, 1 * Time.deltaTime, 0); // top right
                sourceVertices[vertexIndex + 3] += new Vector3(2.5f * Time.deltaTime, -1 * Time.deltaTime, 0); // bottom right

                dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }

        /**
        //testing
        for (int i = 0; i < dialogueText.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = dialogueText.textInfo.characterInfo[i];

            // Skip if the character is a space or not rendered
            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] sourceVertices = dialogueText.textInfo.meshInfo[materialIndex].vertices;

            // Example: Move the character's vertices up and down in a wave pattern
            float waveOffset = Mathf.Sin(Time.time * 1.5f + i * 0.5f) * 0.35f;

            // Apply the offset to the character's four vertices
            //sourceVertices[vertexIndex + 0] += new Vector3(0, waveOffset, 0);
            //sourceVertices[vertexIndex + 1] += new Vector3(0, waveOffset, 0);
            //sourceVertices[vertexIndex + 2] += new Vector3(0, waveOffset, 0);
            //sourceVertices[vertexIndex + 3] += new Vector3(0, waveOffset, 0);



            sourceVertices[vertexIndex + 0] += new Vector3(0, pullAmount[i] * Time.deltaTime * 25 * pullOrNot[i].x, 0); //bottom left
            sourceVertices[vertexIndex + 1] += new Vector3(0, 0, 0); //top left
            sourceVertices[vertexIndex + 2] += new Vector3(0, 0, 0); // top right
            sourceVertices[vertexIndex + 3] += new Vector3(0, pullAmount[i] * Time.deltaTime * 25 * pullOrNot[i].y, 0); // bottom right

            //sourceVertices[vertexIndex + 0] += new Vector3(-2.5f * Time.deltaTime, -1 * Time.deltaTime, 0); //bottom left
            //sourceVertices[vertexIndex + 1] += new Vector3(-2.5f * Time.deltaTime, 1 * Time.deltaTime, 0); //top left
            //sourceVertices[vertexIndex + 2] += new Vector3(2.5f * Time.deltaTime, 1 * Time.deltaTime, 0); // top right
            //sourceVertices[vertexIndex + 3] += new Vector3(2.5f * Time.deltaTime, -1 * Time.deltaTime, 0); // bottom right


            // After modifying all desired vertices in the loop
            dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            // Or for more specific updates:
            // tmpText.UpdateGeometry(tmpText.mesh, 0); // If you only modified the main mesh

        }
        */
    }

    public enum TextEffects
    {
        None,
        Surprise,
        Anger,
        Fear,
        Confusion,
        Love
    }
}
