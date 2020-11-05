TOP=../..

include $(TOP)/Make.config

export MSBuildEnableWorkloadResolver=true

# This tells NuGet to use the nupkgs we're building locally,
# and to put any extracted packages in the 'packages' directory (to not clutter up ~/.nuget/packages)
NuGet.config: $(TOP)/NuGet.config Makefile
	$(Q) $(CP) $< $@.tmp
	$(Q) nuget sources add -Name local-dotnet-feed -Source $(abspath $(DOTNET_FEED_DIR)) -ConfigFile $@.tmp
	$(Q) nuget config -Set globalPackagesFolder=$(abspath $(CURDIR)/packages) -Config $@.tmp
	$(Q) nuget config -Set repositorypath=$(abspath $(CURDIR)/packages) -Config $@.tmp
	$(Q) mv $@.tmp $@

# This tells NuGet to use the exact same dotnet version we've configured in Make.config
global.json: $(TOP)/global6.json
	$(CP) $< $@

../bgen/global.json: global.json
	$(Q) $(CP) $< $@

../bgen/NuGet.config: NuGet.config
	$(Q) $(CP) $< $@

TARGETS += \
	NuGet.config \
	global.json \
	../bgen/NuGet.config \
	../bgen/global.json \

all-local:: $(TARGETS)

export MSBuildEnableWorkloadResolver=true
b: $(TARGETS)
	cp global.json NuGet.config ../bindings-test/dotnet
	$(DOTNET6) build ../bindings-test/dotnet/iOS/bindings-test.csproj

DEVNAME=c0814eeda0e071c4ed07f194ce9582674259fc4b
test: $(TARGETS)
	$(DOTNET6) build MySingleView/*.csproj /p:RuntimeIdentifier=ios-arm64 /bl /v:diag
	mlaunch --installdev MySingleView/bin/Debug/net6.0-ios/ios-arm64/MySingleView.app --devname "$(DEVNAME)"
	mlaunch --launchdev MySingleView/bin/Debug/net6.0-ios/ios-arm64/MySingleView.app --wait-for-unlock --wait-for-exit --devname "$(DEVNAME)"
	sync-crash-reports.sh "$(DEVNAME)"

sim:
	$(DOTNET6) build MySingleView/*.csproj /bl /v:diag
	launch-in-sim MySingleView/bin/Debug/net6.0-ios/ios-x64/MySingleView.app

mac: $(TARGETS)
	$(DOTNET6) build MyCocoaApp/*.csproj /bl /v:diag
	./MyCocoaApp/bin/foo
