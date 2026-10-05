using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

namespace Voidless
{
public static class VGit
{
    public static readonly string PATH_GITIGNORE;

    /// <summary>VGit's static constructor.</summary>
    static VGit()
    {
        PATH_GITIGNORE = VString.GetProjectPath() + "/.gitignore";
    }

    /// <returns>.gitignore's Content.</returns>
    public static string GetGitIgnoreContent()
    {
        string content = string.Empty;

        try { content = File.ReadAllText(PATH_GITIGNORE); }
        catch(Exception exception) { Debug.LogWarning("[VGit] Catched exception while trying to retreive .gitignore's content: " + exception.Message); }

        return content;
    }

    /// <summary>Adds arguments to .gitignore.</summary>
    /// <param name="args">New arguments. Arguments already contained will be ignored.</param>
    public static void AddToGitIgnore(params string[] args)
    {
        if(args == null || args.Length == 0) return;

        string content = string.Empty;

        try { content = GetGitIgnoreContent(); }
        catch(Exception exception) { return; }

        StringBuilder builder = new StringBuilder();

        foreach(string arg in args)
        {
            if(!content.Contains(arg))
            {
                if(builder.Length == 0) builder.AppendLine();
                builder.AppendLine(arg);
            }
        }

        // No need to add extra arguments...
        if(builder.Length == 0) return;

        StringBuilder gitignoreBuilder = new StringBuilder(content);
        gitignoreBuilder.Append(builder.ToString());
        File.WriteAllText(PATH_GITIGNORE, gitignoreBuilder.ToString());
    }
}
}