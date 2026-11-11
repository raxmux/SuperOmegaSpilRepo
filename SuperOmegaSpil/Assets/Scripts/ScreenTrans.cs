using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenTrans : MonoBehaviour
{
    [SerializeField] GameObject transitionScreen;


    public void Fade()
    {
        Debug.Log("playeuuu");
        StartCoroutine(FadeEnumerator());
    }
    public IEnumerator FadeEnumerator()
    {
        Debug.Log("plye");
        bool faded = false;
        int alpha = 0;

        while (faded == false)
        {
            bool fadein = false;
            transitionScreen.GetComponent<Image>().tintColor = new Vector4(0,0,0,alpha);
            yield return new WaitForSeconds(0.01f);
            if (alpha < 255 && fadein == false)
            {
                alpha++;
            }

            if (alpha == 255 && fadein == false)
            {
                fadein = true;
            }

            if (fadein == true)
            {
                alpha--;
            }

            if (fadein == true && alpha == 0)
            {
                faded = true;
            }
            
        }
    }
}
