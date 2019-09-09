﻿#if !__WATCHOS__
using System;
using System.Threading;
using Network;
using CoreFoundation;
using Foundation;

using NUnit.Framework;


namespace monotouchtest.Network
{
	[TestFixture]
	[Preserve(AllMembers = true)]
	public class NWPathMonitorTest
	{

		NWPathMonitor monitor;

		[TestFixtureSetUp]
		public void Init()
		{
			TestRuntime.AssertXcodeVersion(10, 0);
			monitor = new NWPathMonitor();
		}

		[Test]
		public void StatusPropertyTest()
		{
			Assert.That(monitor.CurrentPath, Is.Null, "'CurrentPath' property should be null");

			NWPath finalPath = null;
			bool isPathUpdated = false;

			TestRuntime.RunAsync(DateTime.Now.AddSeconds(30), async () =>
			{

				monitor.SnapshotHandler = ((path) =>
				{
					if (path != null)
					{
						finalPath = monitor.CurrentPath;
						isPathUpdated = true;
					}

				});

				var q = new DispatchQueue(label: "monitor");
				monitor.SetQueue(q);
				monitor.Start();

			}, () => isPathUpdated);

			Assert.That(finalPath, Is.Not.Null, "'CurrentPath' property should not be null");
		}

		[Test]
		public void PathIsAlwaysUpdatedWithNewHandlerTest()
		{
			NWPath oldPath = monitor.CurrentPath;
			NWPath newPath = monitor.CurrentPath;
			bool isOldPathSet = false;
			bool isNewPathSet = false;
			var cbEvent = new AutoResetEvent(false);

			TestRuntime.RunAsync(DateTime.Now.AddSeconds(30), async () =>
			{

				monitor.SnapshotHandler = ((path) =>
				{
					if (path != null)
					{
						oldPath = monitor.CurrentPath;
						isOldPathSet = true;
						Console.WriteLine("oldPath: " + oldPath);
						cbEvent.Set();
					}
				});

				var q = new DispatchQueue(label: "monitor");
				monitor.SetQueue(q);
				monitor.Start();

			}, () => isOldPathSet);


			TestRuntime.RunAsync(DateTime.Now.AddSeconds(30), async () =>
			{
				cbEvent.WaitOne();
				monitor.SnapshotHandler = ((path) =>
				{
					if (path != null)
					{
						newPath = monitor.CurrentPath;
						Console.WriteLine("newPath: " + newPath);
						isNewPathSet = true;
					}

				});

				var q = new DispatchQueue(label: "monitor");
				monitor.SetQueue(q);
				monitor.Start();
			}, () => isNewPathSet);

			Assert.AreNotEqual(oldPath, newPath, "'CurrentPath' wasn't updated when a new SnapshotHandler was assigned");
		}

		[TearDown]
		public void TearDown()
		{
			monitor?.Dispose();
		}

	}
}
#endif