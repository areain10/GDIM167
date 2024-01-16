using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DmgPopup : MonoBehaviour
{
    public static DmgPopup Create(Vector3 position, int dmgAmt)
    {
        Transform damagePopupTransform = Instantiate(GameAssets.i.DmgPopup, position, Quaternion.identity);

        DmgPopup damagePopup = damagePopupTransform.GetComponent<DmgPopup>();
        damagePopup.Setup(dmgAmt);

        return damagePopup;
    }


    private TextMeshPro textMesh;
    private float fadeOutTimer;
    private Color textColor;
    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }
    public void Setup(int dmgAmt)
    {
        textMesh.SetText(dmgAmt.ToString());
        textColor = textMesh.color;
        fadeOutTimer = 0.5f;
    }

    private void Update()
    {
        float moveYSpeed = 5f;
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
