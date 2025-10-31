using UnityEngine.UI;
using UnityEngine;

public class FindAABBProblem : MonoBehaviour
{
	[ContextMenu("Deep Investigate UI")]
	void DeepInvestigate()
	{
		Debug.Log("=== DEEP UI INVESTIGATION STARTED ===");

		InvestigateCanvases();
		InvestigateLayouts();
		InvestigateRenderers();
		InvestigateInvalidValues();

		Debug.Log("=== INVESTIGATION COMPLETE ===");
	}

	void InvestigateCanvases()
	{
		Canvas[] canvases = FindObjectsOfType<Canvas>(true);
		Debug.Log($"Found {canvases.Length} canvases");

		foreach ( Canvas canvas in canvases )
		{
			// Проверяем компоненты canvas
			CanvasRenderer renderer = canvas.GetComponent<CanvasRenderer>();
			if ( renderer != null )
			{
				CheckRenderer(renderer, canvas.name);
			}

			// Проверяем все дочерние элементы
			Graphic[] graphics = canvas.GetComponentsInChildren<Graphic>(true);
			foreach ( Graphic graphic in graphics )
			{
				CheckGraphic(graphic);
			}
		}
	}

	void InvestigateLayouts()
	{
		// ContentSizeFitter - главный виновник
		ContentSizeFitter[] fitters = FindObjectsOfType<ContentSizeFitter>(true);
		Debug.Log($"Found {fitters.Length} ContentSizeFitter components");

		foreach ( ContentSizeFitter fitter in fitters )
		{
			Debug.Log($"ContentSizeFitter on: {fitter.gameObject.name}");

			RectTransform rect = fitter.GetComponent<RectTransform>();
			if ( rect != null )
			{
				CheckRectTransform(rect, "ContentSizeFitter");
			}
		}

		// Layout Groups
		LayoutGroup[] layouts = FindObjectsOfType<LayoutGroup>(true);
		Debug.Log($"Found {layouts.Length} LayoutGroup components");
	}

	void InvestigateRenderers()
	{
		CanvasRenderer[] renderers = FindObjectsOfType<CanvasRenderer>(true);
		Debug.Log($"Found {renderers.Length} CanvasRenderers");

		foreach ( CanvasRenderer renderer in renderers )
		{
			CheckRenderer(renderer, renderer.gameObject.name);
		}
	}

	void InvestigateInvalidValues()
	{
		// Ищем объекты с невалидными значениями
		RectTransform[] allRects = FindObjectsOfType<RectTransform>(true);
		int problemCount = 0;

		foreach ( RectTransform rect in allRects )
		{
			if ( CheckRectTransform(rect, "Scene Scan") )
			{
				problemCount++;
			}
		}

		Debug.Log($"Found {problemCount} problematic RectTransforms");
	}

	bool CheckRenderer(CanvasRenderer renderer, string context)
	{
		bool hasProblem = false;

		// Проверяем связанный graphic
		Graphic graphic = renderer.GetComponent<Graphic>();
		if ( graphic != null )
		{
			if ( graphic.canvasRenderer == null )
			{
				Debug.LogError($"[{context}] Graphic has null canvasRenderer: {renderer.gameObject.name}", renderer.gameObject);
				hasProblem = true;
			}
		}

		return hasProblem;
	}

	bool CheckGraphic(Graphic graphic)
	{
		bool hasProblem = false;

		try
		{
			// Проверяем rect transform
			RectTransform rect = graphic.rectTransform;
			if ( rect != null )
			{
				if ( CheckRectTransform(rect, graphic.gameObject.name) )
				{
					hasProblem = true;
				}
			}

			// Проверяем материал
			if ( graphic.material != null && graphic.material.name.Contains("NULL") )
			{
				Debug.LogError($"Graphic has NULL material: {graphic.gameObject.name}", graphic.gameObject);
				hasProblem = true;
			}
		}
		catch ( System.Exception e )
		{
			Debug.LogError($"Exception checking graphic {graphic.gameObject.name}: {e.Message}");
			hasProblem = true;
		}

		return hasProblem;
	}

	bool CheckRectTransform(RectTransform rect, string context)
	{
		bool hasProblem = false;

		try
		{
			Rect rectValue = rect.rect;

			// Проверяем на NaN и Infinity
			if ( float.IsNaN(rectValue.width) || float.IsInfinity(rectValue.width) ||
				float.IsNaN(rectValue.height) || float.IsInfinity(rectValue.height) ||
				float.IsNaN(rectValue.x) || float.IsInfinity(rectValue.x) ||
				float.IsNaN(rectValue.y) || float.IsInfinity(rectValue.y) )
			{
				Debug.LogError($"[{context}] Invalid RectTransform values: {rect.gameObject.name} " +
							  $"(w:{rectValue.width}, h:{rectValue.height}, x:{rectValue.x}, y:{rectValue.y})",
							  rect.gameObject);
				hasProblem = true;
			}

			// Проверяем размеры
			if ( rectValue.width < 0 || rectValue.height < 0 )
			{
				Debug.LogWarning($"[{context}] Negative RectTransform size: {rect.gameObject.name}", rect.gameObject);
				hasProblem = true;
			}

			// Проверяем анкоры
			if ( float.IsNaN(rect.anchorMin.x) || float.IsInfinity(rect.anchorMax.y) )
			{
				Debug.LogError($"[{context}] Invalid anchors: {rect.gameObject.name}", rect.gameObject);
				hasProblem = true;
			}
		}
		catch ( System.Exception e )
		{
			Debug.LogError($"[{context}] Exception checking RectTransform {rect.gameObject.name}: {e.Message}");
			hasProblem = true;
		}

		return hasProblem;
	}

	void Update()
	{
		if ( Input.GetKeyDown(KeyCode.F) )
		{
			DeepInvestigate();
		}
	}
}