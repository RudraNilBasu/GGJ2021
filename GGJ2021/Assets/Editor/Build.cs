using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using System.Runtime.InteropServices; // DLL imports
using UnityEditor.Build.Reporting;
// using UnityEditor.Build.Reporting;

// TODO(@rudra): Move this to a common file
using f32 = System.Single;
using u8  = System.Byte;
using u32 = System.UInt32;
using b32 = System.Boolean;
using v3 = UnityEngine.Vector3;

public class Build : MonoBehaviour
{
    [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
                                                           out long lpPerformanceCount);
    [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
                                                             out long lpFrequency);
    
    [MenuItem("Build/Build Win")]
        public static void BuildWin()
    {
        long freq;   // QueryPerformanceFrequency
        long freq_1; // QueryPerformanceCounter
        long freq_2; // QueryPerformanceCounter
        QueryPerformanceCounter(out freq_1);
        
        BuildPlayerOptions GameBuildPlayerOptions = new BuildPlayerOptions();
        Debug.Log("NOTE(@rudra): This uses hardcoded level names, if you've added new levels, add it here, or automate it!");
        
        GameBuildPlayerOptions.scenes = new [] {"Assets/Scenes/SampleScene.unity"};
        GameBuildPlayerOptions.locationPathName = "W:/build/GGJ2021/TestBuild.exe";
        GameBuildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        GameBuildPlayerOptions.options = BuildOptions.None;
        
        BuildReport GameReport = BuildPipeline.BuildPlayer(GameBuildPlayerOptions);
        BuildSummary GameSummary = GameReport.summary;
        
        QueryPerformanceFrequency(out freq);
        QueryPerformanceCounter(out freq_2);
        Debug.Log("Build Time: " + ((1.0f*(freq_2 - freq_1))/ freq) + " sec");
        
        if (GameSummary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build Succeeded: " + GameSummary.totalSize + " bytes");
        }
        
        if (GameSummary.result == BuildResult.Failed)
        {
            Debug.Log("Build Failed.");
        }
    }
}
