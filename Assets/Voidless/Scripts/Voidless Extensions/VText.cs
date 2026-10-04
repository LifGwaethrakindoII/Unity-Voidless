using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voidless.REMaker
{
	public static class VText
	{
		/*private static float EstimateTextWidth(Font font, string additionalText, int fontSize)
	    {
	        CharacterInfo charInfo;
	        font.GetCharacterInfo(additionalText[0], out charInfo, fontSize);
	        return charInfo.advance * additionalText.Length + estimatedAdditionalWidthPerCharacter * (additionalText.Length - 1);
	    }*/

	    /// <summary>Displays Text as dialogue into a UI Text.</summary>
	    /// <param name="_text">UI Text's reference.</param>
	    /// <param name="_dialogue">Dialogue to display.</param>
	    /// <param name="_typingCooldown">Cooldown applied at each typed character.</param>
	    /// <param name="onDisplayDone">Optional callback invoked when the dialogue has been displayed [null by default].</param>
	    public static IEnumerator DisplayDialogue(this Text _text, string _dialogue, float _typingCooldown, Action onDisplayDone = null)
	    {
	    	if(string.IsNullOrEmpty(_dialogue))
	    	{
	    		if(onDisplayDone != null) onDisplayDone();
	    		yield break;
	    	}

	    	int length = _dialogue.Length;
	    	StringBuilder builder = new StringBuilder();
	    	SecondsDelayWait wait = new SecondsDelayWait(_typingCooldown);

	    	for(int i = 0; i < length; i++)
	    	{
	    		builder.Append(_dialogue[i]);
	    		_text.text = builder.ToString();
	    		while(wait.MoveNext()) yield return null;
	    		wait.Reset();
	    	}

	    	if(onDisplayDone != null) onDisplayDone();
	    }


	}
}