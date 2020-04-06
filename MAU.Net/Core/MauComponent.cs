using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MAU.Core
{
	public abstract class MauComponent
	{
		#region [ Public Props ]

		public string ComponentName { get; }

		#endregion

		public MauComponent()
		{
			ComponentName = this.GetType().Name;

			InitElements();
		}

		public abstract void InitElements();
		protected void RegisterComponent()
		{
			// Get all mauElement
			List<MauElement> mauElements = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(mauField => mauField.FieldType.IsSubclassOf(typeof(MauElement)))
				.Select(mauField => (MauElement)mauField.GetValue(this))
				.ToList();

			// Regester them
			foreach (MauElement element in mauElements.Where(e => e != null))
			{
				if (MyAngularUi.MauRegistered(element.MauId))
					throw new Exception("MauElement with same mauId was registered.");

				MyAngularUi.RegisterUi(element);
			}
		}
	}
}
