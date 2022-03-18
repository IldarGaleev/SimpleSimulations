# Swarm intelligence simulation

This application demonstrate [swarm intelligence](https://en.wikipedia.org/wiki/Swarm_intelligence) algorithm

![Demo](../ReadmeAssets/swarmIntelligence_demo_1.mp4)

## Build from source

1. Install  [.Net Core v5.0](https://dotnet.microsoft.com/download/dotnet/5.0) SDK
2. Clone repository
	``` bash
	git clone https://github.com/IldarGaleev/SimpleSimulations.git
	```
3. Enter project directory 
	``` bash
	cd .\SimpleSimulations\SwarmIntelligence
	```
4. Restore packages
    ``` bash
    dotnet restore 
    ```
5. Build and run
	``` bash
	dotnet run .\SwarmIntelligence.csproj
	```

# Used packages

- [OpenTK](https://github.com/opentk/opentk) - C# bindings for OpenGL
- [CommandLineParser](https://github.com/commandlineparser/commandline) - Command Line Parser Library for CLR and NetStandard

