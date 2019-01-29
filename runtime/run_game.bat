rem dotnet build


set ms=64

halite.exe --seed 1547877018 --replay-directory replays/ --no-timeout -vvv --width %ms% --height %ms% "dotnet %cd%\Halite3\bin\Release\netcoreapp2.0\MyBot.dll" "dotnet %cd%\Halite3v32\bin\Release\netcoreapp2.0\MyBot.dll"


pause 

