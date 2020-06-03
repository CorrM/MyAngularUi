using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static MAU.MyAngularUi;

namespace MAU.Core
{
	public abstract class MauApp
	{
		protected MauApp()
		{
			// ReSharper disable once VirtualMemberCallInConstructor
			InitComponents();
			// Register all MauComponents
			RegisterComponents();
		}

		protected abstract void InitComponents();

		private void RegisterComponents()
		{
			// Get all MauComponents
			List<MauComponent> mauComponents = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(mauField => mauField.FieldType.IsSubclassOf(typeof(MauComponent)))
				.Select(mauField => (MauComponent)mauField.GetValue(this))
				.ToList();

			// Register components
			foreach (MauComponent mauComponent in mauComponents.Where(e => e != null))
			{
				if (MyAngularUi.IsComponentRegistered(mauComponent.MauId))
					throw new Exception("MauComponent with same mauId was registered.");

				MyAngularUi.RegisterComponent(mauComponent).GetAwaiter().GetResult();
			}

			// If all components are sent there data to angular side so,
			// Alert angular .Net is Ready
			if (MyAngularUi.GetAllComponents().All(c => c.Value.AngularSent))
				MyAngularUi.SendRequest(string.Empty, RequestType.DotNetReady, null).GetAwaiter().GetResult();
		}
	}
}
