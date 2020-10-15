﻿using System.Threading.Tasks;
using Microsoft.DotNet.XHarness.iOS.Shared.Hardware;
using Xharness.Tasks;

namespace Xharness.TestTasks {
	public interface IRunSimulatorTask : IRunXITask<ISimulatorDevice> {

		Task<IAcquiredResource> AcquireResourceAsync ();
	}
}
