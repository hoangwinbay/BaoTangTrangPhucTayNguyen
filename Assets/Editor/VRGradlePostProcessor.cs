using System.IO;
using UnityEditor.Android;
using UnityEngine;

namespace MuseumVR.Editor
{
    public class VRGradlePostProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            Debug.Log($"[VRGradlePostProcessor] Tự động tối ưu Gradle tại: {path}");

            string gradleRoot = path;
            if (Path.GetFileName(gradleRoot) == "unityLibrary")
            {
                gradleRoot = Path.GetDirectoryName(gradleRoot);
            }

            string unityLibraryGradle = Path.Combine(gradleRoot, "unityLibrary", "build.gradle");
            string launcherGradle = Path.Combine(gradleRoot, "launcher", "build.gradle");
            string gradleProperties = Path.Combine(gradleRoot, "gradle.properties");

            PatchBuildGradle(unityLibraryGradle);
            PatchBuildGradle(launcherGradle);

            if (File.Exists(gradleProperties))
            {
                string props = File.ReadAllText(gradleProperties);
                if (!props.Contains("android.enableR8.fullMode"))
                {
                    props += "\nandroid.enableR8.fullMode=false\n";
                    File.WriteAllText(gradleProperties, props);
                }
            }
        }

        private static void PatchBuildGradle(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string content = File.ReadAllText(filePath);

            if (content.Contains("lint {"))
            {
                content = content.Replace("lint {", "lint {\n        checkReleaseBuilds false\n        checkDependencies false\n        ignoreTestSources true");
            }

            string zip64Block = @"
afterEvaluate {
    tasks.matching { it.name.contains('Lint') || it.name.contains('lint') }.configureEach {
        enabled = false
    }
    tasks.withType(Zip).configureEach {
        zip64 = true
    }
}
";
            if (!content.Contains("zip64 = true"))
            {
                content += "\n" + zip64Block;
            }

            File.WriteAllText(filePath, content);
            Debug.Log($"[VRGradlePostProcessor] Đã kích hoạt zip64 và tắt Lint cho: {Path.GetFileName(Path.GetDirectoryName(filePath))}/build.gradle");
        }
    }
}
