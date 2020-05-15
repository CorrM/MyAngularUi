using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MAU.Core
{
	public abstract class MauComponent
	{
		#region [ Public Props ]

		public string ComponentName { get; }

		#endregion

		protected MauComponent()
		{
			ComponentName = this.GetType().Name;

			// ReSharper disable once VirtualMemberCallInConstructor
			InitElements();
			// Register all MauElements
			RegisterComponents();
		}

		protected abstract void InitElements();
		private void RegisterComponents()
		{
			// Get all mauElement
			List<MauElement> mauElements = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(mauField => mauField.FieldType.IsSubclassOf(typeof(MauElement)))
				.Select(mauField => (MauElement)mauField.GetValue(this))
				.ToList();

			// Register them
			foreach (MauElement element in mauElements.Where(e => e != null))
			{
				if (MyAngularUi.IsMauRegistered(element.MauId))
					throw new Exception("MauElement with same mauId was registered.");

				MyAngularUi.RegisterMau(element);
			}
		}
	}
}
