using System.Diagnostics;
using System.IO;
using Fantasy;
using UnityEditor;
using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;

public class LubanTableEditor
{
    [MenuItem("Luban导表/Generate")]
    public static void Generate()
    {
        EditorUtility.DisplayProgressBar("Generate", "Generate", 0.5f);
        var projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string fullPath = Path.GetFullPath(Path.Combine(projectRoot, "../Tools/Luban/gen.bat"));
        Log.Debug(fullPath);
        Process process = new Process();
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = fullPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(fullPath) // 设置工作目录
        };
        process.StartInfo = processStartInfo;

        process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
        process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            EditorUtility.ClearProgressBar();
            process.Close();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to run the .bat file: {e.Message}");
        }
        //Copy();
      
    }

    //[MenuItem("Tools/Luban/CopyToUnity")]
    public static void Copy()
    {
        //var projectRoot = Directory.GetParent(Application.dataPath).FullName;

        // //代码
        // CopyToUnity(Path.GetFullPath(Path.Combine(projectRoot, "../../Tools/Luban/output/code")),
        //     Application.dataPath+"/Scripts/Config");
        // //配置
        // CopyToUnity(Path.GetFullPath(Path.Combine(projectRoot, "../../Tools/Luban/output/data")),
        //     Application.dataPath+"/GameRes/Config/Luban");
    }

    public static void CopyToUnity(string sourcePath, string targetPath)
    {
        if (Directory.Exists(targetPath))
        {
            Directory.Delete(targetPath, true);
        }

        Directory.CreateDirectory(targetPath);
        CopyFilesRecursively(sourcePath, targetPath);
        AssetDatabase.Refresh();
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        Debug.Log(sourcePath);
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}