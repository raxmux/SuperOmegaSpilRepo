using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTrans : MonoBehaviour
{
    [SerializeField] Image transitionScreen;


    public void Fade()
    {
        StartCoroutine(FadeEnumerator());
    }
    public IEnumerator FadeEnumerator()
    {
        bool waitasecbro = true;
        bool faded = false;
        byte alpha = 0;
        bool fadein = false;

        while (faded == false)
        {
            transitionScreen.GetComponent<Image>().color = new Color32(255,255,255,alpha);



            if (waitasecbro == true && alpha == 255)
            {
                yield return new WaitForSeconds(0.2f);
                waitasecbro = false;
            }

            if (alpha == 255 && fadein == false)
            {
                fadein = true;
            }

            if (alpha < 255 && fadein == false)
            {
                alpha += 15;
            }

            if (fadein == true && alpha == 0)
            {
                faded = true;
            }

            if (fadein == true && waitasecbro == false)
            {
                alpha -= 15;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
}
