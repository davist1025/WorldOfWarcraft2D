using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Nez.ECS.Headless
{
	public class ContentManagerHeadless : ContentManager
	{
		// todo: runtime loaders?

		public ContentManagerHeadless(IServiceProvider serviceProvider) : base(serviceProvider, "Data")
		{
		}

		/// <summary>
		/// loads an asset on a background thread with optional callback for when it is loaded. The callback will occur on the main thread.
		/// </summary>
		/// <param name="assetName">Asset name.</param>
		/// <param name="onLoaded">On loaded.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void LoadAsync<T>(string assetName, Action<T> onLoaded = null)
		{
			var syncContext = SynchronizationContext.Current;
			Task.Run(() =>
			{
				var asset = Load<T>(assetName);

				// if we have a callback do it on the main thread
				if (onLoaded != null)
				{
					syncContext.Post(d => { onLoaded(asset); }, null);
				}
			});
		}

		/// <summary>
		/// checks to see if an asset with assetName is loaded
		/// </summary>
		/// <returns><c>true</c> if this instance is asset loaded the specified assetName; otherwise, <c>false</c>.</returns>
		/// <param name="assetName">Asset name.</param>
		public bool IsAssetLoaded(string assetName) => LoadedAssets.ContainsKey(assetName);

	}
}
