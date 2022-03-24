# Gravity simulation

This application uses [Newton's law of gravity](https://en.wikipedia.org/wiki/Newton%27s_law_of_universal_gravitation) to simulate the interaction of masses.
<!--
$$ F=G \dfrac{m_1m_2}{r^2} $$
![formula](https://render.githubusercontent.com/render/math?math=F=G\dfrac{m_1m_2}{r^2})
-->

https://user-images.githubusercontent.com/26148013/159098245-1f63e998-75ff-4ebb-9062-077299926989.mp4

## Build from source

1. Install  [.Net Core v5.0](https://dotnet.microsoft.com/download/dotnet/5.0) SDK
2. Clone repository
	``` bash
	git clone https://github.com/IldarGaleev/SimpleSimulations.git
	```
3. Enter project directory 
	``` bash
	cd .\SimpleSimulations\Gravity
	```
4. Restore packages
    ``` bash
    dotnet restore 
    ```
5. Build and run
	``` bash
	dotnet run .\Gravity.csproj
	```

## Command line

| Command                 | Default                 | Description                                                                 |
|------------------------ |:-----------------------:|-----------------------------------------------------------------------------|
| `-c`, `--celestialfile` | [Data/celestial_init.csv](Data/celestial_init.csv) | csv file with initial celestial bodies                                      |
| `-f`, `--fullscreen`    | false                   | Fullscreen mode                                                             |
| `-W`, `--width`         | 800                     | Window width                                                                |
| `-H`, `--height`        | 600                     | Window height                                                               |
| `-t`, `--track`         | false                   | Show objects tracks (also you can press "T" key to toggle this option)      |
| `-i`, `--interaction`   | false                   | Show interaction vectors (also you can press "A" key to toggle this option) |
| `-s`, `--worldsize`     | 1                       | World size in meters                                                        |
| `-o`, `--origin`        | false                   | Display origin point                                                        |
| `--help`                |                         | Display this help screen.                                                   |
| `--version`             |                         | Display version information.                                                |

## Hotkeys

| Key   | Description                    |
|:-----:|--------------------------------|
| `A`   | Toggle displaying vectors      |
| `E`   | Erase simulation objects       |
| `F`   | Selecting an object to track   |
| `M`   | Focus on the heaviest object   |
| `O`   | Toggle displaying origin point |
| `R`   | Add random particles           |
| `T`   | Toggle displaying tracks       |
| `U`   | Stop Tracking an object        |
| `Ctrl + S` | Save the particles to a CSV file under the "Data" path. |
| `1`   | Reset scale                    |
| `Esc` | Close application              |
