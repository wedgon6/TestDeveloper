using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.UI
{
	public abstract class GameWindow : MonoBehaviour
	{
		[SerializeField] protected Image _goalBar = null;
		[SerializeField] protected TMP_Text _goalText = null;

		private bool _isInit = false;
		protected bool IsInit => _isInit;

		public virtual void Show(int totalOrdersServed, int ordersTarget)
		{
			if ( !_isInit )
			{
				Init();
			}

			_goalText.text = $"{totalOrdersServed}/{ordersTarget}";
			_goalBar.fillAmount = Mathf.Clamp01((float)totalOrdersServed / ordersTarget);
			gameObject.SetActive(true);
		}

		protected abstract void Init();
	}
}