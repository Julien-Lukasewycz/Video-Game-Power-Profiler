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
        private static int FrameIndex;
        private static readonly Dictionary<string, int> CategoryMapping = new()
        {
            { "Virtual Texturing", 0 },
            { "Audio Update Job", 1 },
            { "Ai", 6 },
            { "Loading", 8 },
            { "UI Layout", 3 },
            { "Scripts", 7 },
            { "Other", 9 },
            { "Input", 12 },
            { "Text", 3 },
            { "Lighting", 4 },
            { "Physics", 2 },
            { "Particles", 5 },
            { "UI Details", 3 },
            { "Debug", 7 },
            { "GC", 9 },
            { "Burst Jobs", 7 },
            { "GPU", 0 },
            { "Director", 11 },
            { "Vr", 0 },
            { "Animation", 11 },
            { "Overhead", 9 },
            { "Managed Jobs", 7 },
            { "PlayerLoop", 7 },
            { "Audio Job", 1 },
            { "Network", 8 },
            { "Build Interface", 9 },
            { "FileIO", 8 },
            { "Physics2D", 2 },
            { "Video", 10 },
            { "Render", 0 },
            { "Audio", 1 },
            { "VSync", 0 },
            { "Jobs", 7 },
            { "Gui", 3 },
            { "Internal", 9 },
            { "UI Render", 3 },
            { "VFX", 5 },
            { "Memory", 9 },
            { "Network Operations", 8 }
        };
        // Rendering = 0
        // Audio = 1
        // Physics = 2
        // UI = 3
        // Lighting = 4
        // VFX = 5
        // AI = 6
        // Scripting = 7
        // IO = 8
        // Other = 9
        // Video = 10
        // Animation = 11
        // Input = 12
        
        [MenuItem("Window/Analysis/Analyze Recorded Profiler Data", false, 50)]
        private static async void Analyze()
        {
            Path = $"Benchmark/ProfilerUtilizationData {DateTime.Now:dd-MM-yyyy HH-mm-ss}.csv";
            Files = Directory.EnumerateFiles("Benchmark/").Where(file => file.Contains("ProfilerData") && file.Contains(".raw")).ToList();
            Files.Sort();
            FileIndex = 0;
            FrameIndex = 0;
            Profiler.maxUsedMemory = int.MaxValue;
            ProfilerDriver.profileLoaded += AnalyzeFile;
            const string header = "Frame Index;Frame Time;Frame FPS;Rendering;Audio;Physics;UI;Lighting;VFX;AI;Scripting;IO;Other;Video;Animation;Input\r\n";
            await File.AppendAllTextAsync(Path, header);
            ProfilerDriver.LoadProfile(Files[FileIndex], false);
        }

        private static async void AnalyzeFile()
        {
            Dictionary<int, string> categoryIdToName = new();
            float[] categoryUtilization = new float[13];
            List<ProfilerCategoryInfo> categories = new();
            for (int i = ProfilerDriver.firstFrameIndex; i <= ProfilerDriver.lastFrameIndex; i++)
            {
                using (HierarchyFrameDataView hierarchyFrameDataView = ProfilerDriver.GetHierarchyFrameDataView(i, 0, HierarchyFrameDataView.ViewModes.MergeSamplesWithTheSameName, HierarchyFrameDataView.columnDontSort, false))
                {
                    hierarchyFrameDataView.GetAllCategories(categories);
                    foreach (ProfilerCategoryInfo category in categories)
                    {
                        categoryIdToName.Add(category.id, category.name);
                    }
                    List<int> childrenIds = new();
                    hierarchyFrameDataView.GetItemChildren(hierarchyFrameDataView.GetRootItemID(), childrenIds);
                    Queue<Vector2Int> childrenToVisit = new();
                    foreach (int childId in childrenIds)
                    {
                        childrenToVisit.Enqueue(new Vector2Int(childId, 9));
                    }
                    while (childrenToVisit.Count > 0)
                    {
                        Vector2Int childId = childrenToVisit.Dequeue();
                        int categoryId = hierarchyFrameDataView.GetItemCategoryIndex(childId.x);
                        if (categoryIdToName[categoryId] == "Internal")
                        {
                            categoryId = childId.y;
                        }
                        string categoryName = categoryIdToName[categoryId];
                        bool hasMapping = CategoryMapping.TryGetValue(categoryName, out categoryId);
                        if (!hasMapping)
                        {
                            categoryId = 9;
                            Debug.LogWarning($"Missing Category Mapping for {categoryName}");
                        }
                        categoryUtilization[categoryId] += hierarchyFrameDataView.GetItemColumnDataAsFloat(childId.x, HierarchyFrameDataView.columnSelfPercent);
                        //else
                       // {
                            /*int parent = childId;
                            do
                            {
                                List<int> parents = new();
                                hierarchyFrameDataView.GetItemAncestors(parent, parents);
                                if (parents.Count == 0)
                                {
                                    parent = categoryId;
                                    break;
                                }
                                parent = parents[0];
                            } while (categoryIdToName[hierarchyFrameDataView.GetItemCategoryIndex(parent)] != "Internal");
                            string categoryName = categoryIdToName[hierarchyFrameDataView.GetItemCategoryIndex(parent)];
                            bool hasMapping = CategoryMapping.TryGetValue(categoryName, out int categoryIndex);
                            if (!hasMapping)
                            {
                                categoryIndex = 9;
                            }
                            categoryUtilization[categoryIndex] += hierarchyFrameDataView.GetItemColumnDataAsFloat(childId, HierarchyFrameDataView.columnSelfPercent);*/
                        //}
                        /*else
                        {
                            List<int> parents = new();
                            hierarchyFrameDataView.GetItemAncestors(childId, parents);
                            foreach (int parent in parents)
                            {
                                parentsToVisit.Enqueue(parent);
                            }
                        }*/
                        List<int> children = new();
                        hierarchyFrameDataView.GetItemChildren(childId.x, children);
                        foreach (int child in children)
                        {
                            if (categoryIdToName[hierarchyFrameDataView.GetItemCategoryIndex(childId.x)] == "Internal")
                            {
                                childrenToVisit.Enqueue(new Vector2Int(child, childId.y));
                            }
                            else
                            {
                                childrenToVisit.Enqueue(new Vector2Int(child, hierarchyFrameDataView.GetItemCategoryIndex(childId.x)));
                            }
                        }
                    }
                    /*List<int> leafs = new();
                    while (childrenToVisit.Count > 0)
                    {
                        int childId = childrenToVisit.Dequeue();
                        List<int> children = new();
                        hierarchyFrameDataView.GetItemChildren(childId, children);
                        if (children.Count == 0)
                        {
                            leafs.Add(childId);
                        }
                        else
                        {
                            foreach (int child in children)
                            {
                                childrenToVisit.Enqueue(child);
                            }
                        }
                    }
                    childrenToVisit.Clear();
                    foreach (int leaf in leafs)
                    {
                        childrenToVisit.Enqueue(leaf);
                        Debug.Log(hierarchyFrameDataView.GetItemName(leaf));
                    }*/
                    /*while (parentsToVisit.Count > 0)
                    {
                        int parentId = parentsToVisit.Dequeue();
                        ushort categoryId = hierarchyFrameDataView.GetItemCategoryIndex(parentId);
                        if (categoryIdToName[categoryId] == "Internal")
                        {
                            List<int> newParentsToVisit = new();
                            hierarchyFrameDataView.GetItemAncestors(parentId, newParentsToVisit);
                            foreach (int newParentId in newParentsToVisit)
                            {
                                parentsToVisit.Enqueue(newParentId);
                            }
                        }
                        else
                        {
                            string categoryName = categoryIdToName[categoryId];
                            bool hasMapping = CategoryMapping.TryGetValue(categoryName, out int categoryIndex);
                            if (!hasMapping)
                            {
                                categoryIndex = 9;
                                Debug.LogWarning($"Missing Category Mapping for {categoryName}");
                            }
                            categoryUtilization[categoryIndex] += hierarchyFrameDataView.GetItemColumnDataAsFloat(parentId, HierarchyFrameDataView.columnSelfPercent);
                        }
                    }*/
                    FrameIndex++;
                    string content = $"{FrameIndex};{hierarchyFrameDataView.frameTimeMs:F3};{hierarchyFrameDataView.frameFps:F2}";
                    foreach (float utilization in categoryUtilization)
                    {
                        content += $";{utilization:F2}";
                    }
                    content += "\r\n";
                    await File.AppendAllTextAsync(Path, content);
                    categoryIdToName.Clear();
                    categoryUtilization = new float[13];
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
