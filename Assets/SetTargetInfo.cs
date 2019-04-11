using UnityEngine;

public class SetTargetInfo : MonoBehaviour
{
    public UnityEngine.UI.Text target_name;
    public UnityEngine.UI.Text target_rating;
    public UnityEngine.UI.Text width;
    
    public void SetTargetInfoText(string target_nametext, string target_ratingtext, string widthtext)
    {
        target_name.text = target_nametext;
        target_rating.text = target_ratingtext;
        width.text = widthtext;
    }
}
