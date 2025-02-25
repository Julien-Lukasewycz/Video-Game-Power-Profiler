using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Profiling;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;

namespace PowerProfiler.Editor
{
    internal static class ProfilerDataAnalyzer
    {
        private static List<string> Files = new();
        private static int FileIndex;
        private static string Path;
        private static bool HeaderWritten;
        private static int FrameIndex;
        
        [MenuItem("Window/Analysis/Analyze Recorded Profiler Data", false, 50)]
        private static void Analyze()
        {
            Path = $"Benchmark/ProfilerUtilizationData {DateTime.Now:dd-MM-yyyy HH-mm-ss}.csv";
            Files = Directory.EnumerateFiles("Benchmark/").Where(file => file.Contains("ProfilerData") && file.Contains(".raw")).ToList();
            Files.Sort();
            FileIndex = 0;
            HeaderWritten = false;
            FrameIndex = 0;
            Profiler.maxUsedMemory = int.MaxValue;
            ProfilerDriver.profileLoaded += AnalyzeFile;
            ProfilerDriver.LoadProfile(Files[FileIndex], false);
        }

        private static async void AnalyzeFile()
        {
            Dictionary<ushort, string> categoryIdToName = new();
            Dictionary<string, float> categoryUtilization = new();
            List<ProfilerCategoryInfo> categories = new();
            for (int i = ProfilerDriver.firstFrameIndex; i <= ProfilerDriver.lastFrameIndex; i++)
            {
                using (HierarchyFrameDataView hierarchyFrameDataView = ProfilerDriver.GetHierarchyFrameDataView(i, 0, HierarchyFrameDataView.ViewModes.InvertHierarchy, HierarchyFrameDataView.columnDontSort, false))
                {
                    hierarchyFrameDataView.GetAllCategories(categories);
                    if (!HeaderWritten)
                    {
                        string header = "Frame Index;Frame Time;Frame FPS";
                        foreach (ProfilerCategoryInfo category in categories)
                        {
                            header += $";{category.name}";
                        }
                        header += "\r\n";
                        await File.AppendAllTextAsync(Path, header);
                        HeaderWritten = true;
                    }
                    foreach (ProfilerCategoryInfo category in categories)
                    {
                        categoryIdToName.Add(category.id, category.name);
                        categoryUtilization.Add(category.name, 0);
                    }
                    List<int> childrenIds = new();
                    hierarchyFrameDataView.GetItemChildren(0, childrenIds);
                    Queue<int> childrenToVisit = new();
                    foreach (int childId in childrenIds)
                    {
                        childrenToVisit.Enqueue(childId);
                    }
                    while (childrenToVisit.Count > 0)
                    {
                        int childId = childrenToVisit.Dequeue();
                        ushort categoryId = hierarchyFrameDataView.GetItemCategoryIndex(childId);
                        if (categoryIdToName[categoryId] == "Internal")
                        {
                            List<int> newChildrenToVisit = new();
                            hierarchyFrameDataView.GetItemChildren(childId, newChildrenToVisit);
                            foreach (int newChildId in newChildrenToVisit)
                            {
                                childrenToVisit.Enqueue(newChildId);
                            }
                        }
                        else
                        {
                            categoryUtilization[categoryIdToName[categoryId]] += hierarchyFrameDataView.GetItemColumnDataAsFloat(childId, HierarchyFrameDataView.columnSelfPercent);
                        }
                    }
                    FrameIndex++;
                    string content = $"{FrameIndex};{hierarchyFrameDataView.frameTimeMs:F3};{hierarchyFrameDataView.frameFps:F2}";
                    foreach (KeyValuePair<string, float> utilization in categoryUtilization)
                    {
                        content += $";{utilization.Value:F2}";
                    }
                    content += "\r\n";
                    await File.AppendAllTextAsync(Path, content);
                    categoryIdToName.Clear();
                    categoryUtilization.Clear();
                    categories.Clear();
                }
            }
            FileIndex++;
            if (FileIndex < Files.Count)
            {
                ProfilerDriver.LoadProfile(Files[FileIndex], false);
            }
            else
            {
                Debug.Log("All Profiler Data was analyzed.");
            }
        }
    }
}
