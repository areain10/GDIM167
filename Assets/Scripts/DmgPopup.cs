using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DmgPopup : MonoBehaviour
{
    public static DmgPopup Create(Vector3 position, int dmgAmt, specialTypes special)
    {
        Transform damagePopupTransform = Instantiate(GameAssets.i.DmgPopup, position, Quaternion.identity);

        DmgPopup damagePopup = damagePopupTransform.GetComponent<DmgPopup>();
        damagePopup.Setup(dmgAmt,position,special);

        return damagePopup;
    }


    private TextMeshPro textMesh;
    private float fadeOutTimer;
    private Color textColor;
    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }
    public void Setup(int dmgAmt,Vector3 position, specialTypes special)
    {
        textMesh.SetText(dmgAmt.ToString());

        switch (special)
        {
            case specialTypes.DAMAGE: textColor = Color.red; break;
                case specialTypes.HEAL: textColor = Color.green; break;
        }
        textMesh.color = textColor;
        fadeOutTimer = 0.5f;
        transform.position = position;
    }

    private void Update()
    {
        float moveYSpeed = 1f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        fadeOutTimer -= Time.deltaTime;
        if(fadeOutTimer < 0)
        {
            float fadeOutSpeed = 3f;
            textColor.a -= fadeOutSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
