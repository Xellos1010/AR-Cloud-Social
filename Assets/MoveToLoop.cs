using UnityEngine;
using System.Collections;

public class MoveToLoop : MonoBehaviour {

    public Vector3 moveFrom;
    public Vector3 moveTo;
    public float moveFromY;
    public float moveToY;
    public float alphaFrom;
    public float alphaTo;

    public float time = .5f;

    RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }
    private RectTransform _rectTransform;

    UnityEngine.UI.RawImage rawImage
    {
        get
        {
            if (_rawImage == null)
                _rawImage = GetComponent<UnityEngine.UI.RawImage>();
            return _rawImage;
        }
    }
    private UnityEngine.UI.RawImage _rawImage;



    void Awake()
    {
        UpdateY(moveFromY);
        UpdateAlpha(alphaFrom);
    }

	// Use this for initialization
	void OnEnable()
    {

	}

    void OnDisable()
    {
        StopTween();
    }	

    public void StopTween()
    {
        //iTween.Stop();
    }

    public void StartTweenMovement()
    {
        //iTween.ValueTo(gameObject, tweenParamsStartMovement());
    }


    public void ResetTweenAlpha()
    {
        //iTween.ValueTo(gameObject, tweenParamsResetAlpha());
    }

    public void UpdateY(float newValue)
    {
        rectTransform.position = new Vector3(rectTransform.position.x,newValue, rectTransform.position.z);
    }

    public void UpdateAlpha(float newValue)
    {
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, newValue);
    }

    public void SetMoveToPosition()
    {
        moveTo = rectTransform.position;
        moveToY = rectTransform.position.y;
    }

    public void SetMoveFromPosition()
    {
        moveFrom = rectTransform.position;
        moveFromY = rectTransform.position.y;
    }

    public void SetAlphaFromValue()
    {
        alphaFrom = rawImage.color.a;
    }

    public void SetAlphaToValue()
    {
        alphaTo = rawImage.color.a;
    }

    public void SetAlphaToAlphaFromValue()
    {
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b,alphaFrom);
    }

    public void SetAlphaToAlphaToValue()
    {
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, alphaTo);
    }

    public void MoveToFromPosition()
    {
        rectTransform.position = moveFrom;
    }
}
