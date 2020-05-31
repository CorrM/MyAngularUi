using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static MAU.MyAngularUi;

namespace MAU.Core
{
	public abstract class MauComponent
	{
		#region [ Internal Props ]

		internal bool AngularSent { get; private set; }

		#endregion

		#region [ Public Props ]

		public string ComponentName { get; }

		#endregion

		protected MauComponent(string componentName)
		{
			ComponentName = componentName;

			// ReSharper disable once VirtualMemberCallInConstructor
			InitElements();
			// Register all MauElements
			RegisterElements();
			// Register this MauComponent, it's just for .Net side
			MyAngularUi.RegisterComponents(this);
		}

		protected abstract void InitElements();

		private void RegisterElements()
		{
			// Get all mauElement
			List<MauElement> mauElements = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(mauField => mauField.FieldType.IsSubclassOf(typeof(MauElement)))
				.Select(mauField => (MauElement)mauField.GetValue(this))
				.ToList();

			// Register them
			foreach (MauElement element in mauElements.Where(e => e != null))
			{
				if (MyAngularUi.IsElementRegistered(element.MauId))
					throw new Exception("MauElement with same mauId was registered.");

				MyAngularUi.RegisterElement(element);
			}

			AngularSent = true;

			// If all components are sent there data to angular side so,
			// Alert angular .Net is Ready
			if (MyAngularUi.GetAllComponents().All(c => c.AngularSent))
				MyAngularUi.SendRequest(string.Empty, RequestType.DotNetReady, null).GetAwaiter().GetResult();
		}
	}
}
