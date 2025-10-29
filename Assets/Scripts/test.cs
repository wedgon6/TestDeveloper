using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class test : MonoBehaviour
{
	private void Start()
	{
		FindBrokenComponents();
	}

	[ContextMenu("Find Broken UI Components")]
	void FindBrokenComponents()
	{
		// Ищем все компоненты UI
		Graphic[] graphics = FindObjectsOfType<Graphic>(true);
		EventTrigger[] triggers = FindObjectsOfType<EventTrigger>(true);

		Debug.Log($"Checking {graphics.Length} Graphic components...");
		foreach ( var graphic in graphics )
		{
			if ( graphic == null )
			{
				Debug.LogError("Found broken Graphic component!");
				continue;
			}

			// Проверяем связанные компоненты
			if ( graphic.gameObject == null )
			{
				Debug.LogError($"Graphic component on destroyed GameObject: {graphic.GetInstanceID()}");
			}
		}

		Debug.Log($"Checking {triggers.Length} EventTrigger components...");
		foreach ( var trigger in triggers )
		{
			if ( trigger == null )
			{
				Debug.LogError("Found broken EventTrigger component!");
			}
		}
	}
}
