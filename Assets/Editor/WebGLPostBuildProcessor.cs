#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class WebGLPostBuildProcessor : IPostprocessBuildWithReport
{
    private const string OldBaseName = "WebGL Builds";
    private const string NewBaseName = "WebGLBuilds";
    private const string Typo = "An unspecified error occured.";
    private const string Corrected = "An unspecified error occurred.";

    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
        {
            return;
        }

        string buildRoot = report.summary.outputPath;
        string buildFolderPath = Path.Combine(buildRoot, "Build");

        RenameWebGLBuildFiles(buildFolderPath);
        FixLoaderTypo(buildFolderPath);
        UpdateGeneratedIndex(buildRoot);
    }

    private static void RenameWebGLBuildFiles(string buildFolderPath)
    {
        if (!Directory.Exists(buildFolderPath))
        {
            return;
        }

        string[] candidateFiles =
        {
            ".data.br",
            ".framework.js.br",
            ".loader.js",
            ".wasm.br"
        };

        foreach (string suffix in candidateFiles)
        {
            string oldPath = Path.Combine(buildFolderPath, OldBaseName + suffix);
            string newPath = Path.Combine(buildFolderPath, NewBaseName + suffix);

            if (File.Exists(oldPath))
            {
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }

                File.Move(oldPath, newPath);
            }
        }
    }

    private static void FixLoaderTypo(string buildFolderPath)
    {
        string loaderPath = Path.Combine(buildFolderPath, NewBaseName + ".loader.js");
        if (!File.Exists(loaderPath))
        {
            loaderPath = Path.Combine(buildFolderPath, OldBaseName + ".loader.js");
        }

        if (!File.Exists(loaderPath))
        {
            return;
        }

        string loaderText = File.ReadAllText(loaderPath);
        if (!loaderText.Contains(Typo))
        {
            return;
        }

        File.WriteAllText(loaderPath, loaderText.Replace(Typo, Corrected));
        Debug.Log($"{nameof(WebGLPostBuildProcessor)}: Corrected loader typo in {loaderPath}.");
    }

    private static void UpdateGeneratedIndex(string buildRoot)
    {
        string indexPath = Path.Combine(buildRoot, "index.html");
        if (!File.Exists(indexPath))
        {
            return;
        }

        string html = File.ReadAllText(indexPath);
        if (!html.Contains(OldBaseName))
        {
            return;
        }

        File.WriteAllText(indexPath, html.Replace(OldBaseName, NewBaseName));
        Debug.Log($"{nameof(WebGLPostBuildProcessor)}: Updated index.html WebGL asset names to '{NewBaseName}'.");
    }
}
#endif
