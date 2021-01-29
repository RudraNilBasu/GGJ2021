echo "Deleting W:\build\GGJ2021"

del /S /F /Q W:\build\GGJ2021

echo "Building Project"




"C:\Program Files\Unity\Hub\Editor\2020.1.2f1\Editor\Unity.exe" -batchmode -projectPath "W:\Game Jam Games\GGJ2021\GGJ2021" -executeMethod "Build.BuildWin" -logfile "Build.log" -quit
