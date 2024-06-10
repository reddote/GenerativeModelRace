using Dummiesman;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MTLLoader
{
    public List<string> SearchPaths = new List<string>() { "%FileName%_Textures", string.Empty };

    private FileInfo _objFileInfo = null;

    /// <summary>
    /// The texture loading function. Overridable for stream loading purposes.
    /// </summary>
    /// <param name="path">The path supplied by the OBJ file, converted to OS path separation</param>
    /// <param name="isNormalMap">Whether the loader is requesting we convert this into a normal map</param>
    /// <returns>Texture2D if found, or NULL if missing</returns>
    public virtual Texture2D TextureLoadFunction(string path, bool isNormalMap)
    {
        foreach (var searchPath in SearchPaths)
        {
            string processedPath = (_objFileInfo != null)
                ? searchPath.Replace("%FileName%", Path.GetFileNameWithoutExtension(_objFileInfo.Name))
                : searchPath;
            string filePath = Path.Combine(processedPath, path);

            if (File.Exists(filePath))
            {
                Texture2D tex = LoadAndCompressTexture(filePath);

                if (isNormalMap)
                    tex = ImageUtils.ConvertToNormalMap(tex);

                return tex;
            }
        }

        return null;
    }

    private Texture2D LoadAndCompressTexture(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);

        if (tex.LoadImage(fileData))
        {
            tex = CompressTexture(tex);
        }

        return tex;
    }

    private Texture2D CompressTexture(Texture2D texture)
    {
        // Create a render texture in a lower resolution
        RenderTexture rt = RenderTexture.GetTemporary(texture.width / 2, texture.height / 2);
        Graphics.Blit(texture, rt);

        // Read the render texture back into a new texture
        RenderTexture.active = rt;
        Texture2D newTex = new Texture2D(rt.width, rt.height);
        newTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        newTex.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // Compress the texture to a DXT format
        newTex.Compress(true);

        return newTex;
    }

    private Texture2D TryLoadTexture(string texturePath, bool normalMap = false)
    {
        texturePath = texturePath.Replace('\\', Path.DirectorySeparatorChar);
        texturePath = texturePath.Replace('/', Path.DirectorySeparatorChar);

        return TextureLoadFunction(texturePath, normalMap);
    }

    private int GetArgValueCount(string arg)
    {
        switch (arg)
        {
            case "-bm":
            case "-clamp":
            case "-blendu":
            case "-blendv":
            case "-imfchan":
            case "-texres":
                return 1;
            case "-mm":
                return 2;
            case "-o":
            case "-s":
            case "-t":
                return 3;
        }

        return -1;
    }

    private int GetTexNameIndex(string[] components)
    {
        for (int i = 1; i < components.Length; i++)
        {
            var cmpSkip = GetArgValueCount(components[i]);
            if (cmpSkip < 0)
            {
                return i;
            }

            i += cmpSkip;
        }

        return -1;
    }

    private float GetArgValue(string[] components, string arg, float fallback = 1f)
    {
        string argLower = arg.ToLower();
        for (int i = 1; i < components.Length - 1; i++)
        {
            var cmp = components[i].ToLower();
            if (argLower == cmp)
            {
                return OBJLoaderHelper.FastFloatParse(components[i + 1]);
            }
        }

        return fallback;
    }

    private string GetTexPathFromMapStatement(string processedLine, string[] splitLine)
    {
        int texNameCmpIdx = GetTexNameIndex(splitLine);
        if (texNameCmpIdx < 0)
        {
            Debug.LogError($"texNameCmpIdx < 0 on line {processedLine}. Texture not loaded.");
            return null;
        }

        int texNameIdx = processedLine.IndexOf(splitLine[texNameCmpIdx]);
        string texturePath = processedLine.Substring(texNameIdx);

        return texturePath;
    }

    /// <summary>
    /// Loads a *.mtl file
    /// </summary>
    /// <param name="input">The input stream from the MTL file</param>
    /// <returns>Dictionary containing loaded materials</returns>
    public Dictionary<string, Material> Load(Stream input)
    {
        var inputReader = new StreamReader(input);
        Dictionary<string, Material> mtlDict = new Dictionary<string, Material>();
        Material currentMaterial = null;

        while (!inputReader.EndOfStream)
        {
            string line = inputReader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string processedLine = line.Clean();
            string[] splitLine = processedLine.Split(' ');

            if (splitLine.Length < 2 || processedLine[0] == '#')
                continue;

            if (splitLine[0] == "newmtl")
            {
                string materialName = processedLine.Substring(7);
                var newMtl = new Material(Shader.Find("Standard (Specular setup)")) { name = materialName };
                mtlDict[materialName] = newMtl;
                currentMaterial = newMtl;
                continue;
            }

            if (currentMaterial == null)
                continue;

            switch (splitLine[0])
            {
                case "Kd":
                case "kd":
                    var currentColor = currentMaterial.GetColor("_Color");
                    var kdColor = OBJLoaderHelper.ColorFromStrArray(splitLine);
                    currentMaterial.SetColor("_Color", new Color(kdColor.r, kdColor.g, kdColor.b, currentColor.a));
                    break;

                case "map_Kd":
                case "map_kd":
                    string texturePath = GetTexPathFromMapStatement(processedLine, splitLine);
                    if (texturePath != null)
                    {
                        var KdTexture = TryLoadTexture(texturePath);
                        currentMaterial.SetTexture("_MainTex", KdTexture);
                        if (KdTexture != null &&
                            (KdTexture.format == TextureFormat.DXT5 || KdTexture.format == TextureFormat.ARGB32))
                        {
                            OBJLoaderHelper.EnableMaterialTransparency(currentMaterial);
                        }

                        if (Path.GetExtension(texturePath).ToLower() == ".dds")
                        {
                            currentMaterial.mainTextureScale = new Vector2(1f, -1f);
                        }
                    }
                    break;

                case "map_Bump":
                case "map_bump":
                    texturePath = GetTexPathFromMapStatement(processedLine, splitLine);
                    if (texturePath != null)
                    {
                        var bumpTexture = TryLoadTexture(texturePath, true);
                        float bumpScale = GetArgValue(splitLine, "-bm", 1.0f);

                        if (bumpTexture != null)
                        {
                            currentMaterial.SetTexture("_BumpMap", bumpTexture);
                            currentMaterial.SetFloat("_BumpScale", bumpScale);
                            currentMaterial.EnableKeyword("_NORMALMAP");
                        }
                    }
                    break;

                case "Ks":
                case "ks":
                    currentMaterial.SetColor("_SpecColor", OBJLoaderHelper.ColorFromStrArray(splitLine));
                    break;

                case "Ka":
                case "ka":
                    currentMaterial.SetColor("_EmissionColor", OBJLoaderHelper.ColorFromStrArray(splitLine, 0.05f));
                    currentMaterial.EnableKeyword("_EMISSION");
                    break;

                case "map_Ka":
                case "map_ka":
                    texturePath = GetTexPathFromMapStatement(processedLine, splitLine);
                    if (texturePath != null)
                    {
                        currentMaterial.SetTexture("_EmissionMap", TryLoadTexture(texturePath));
                    }
                    break;

                case "d":
                case "Tr":
                    float visibility = OBJLoaderHelper.FastFloatParse(splitLine[1]);

                    if (splitLine[0] == "Tr")
                        visibility = 1f - visibility;

                    if (visibility < (1f - Mathf.Epsilon))
                    {
                        var color = currentMaterial.GetColor("_Color");
                        color.a = visibility;
                        currentMaterial.SetColor("_Color", color);
                        OBJLoaderHelper.EnableMaterialTransparency(currentMaterial);
                    }
                    break;

                case "Ns":
                case "ns":
                    float Ns = OBJLoaderHelper.FastFloatParse(splitLine[1]);
                    Ns = (Ns / 1000f);
                    currentMaterial.SetFloat("_Glossiness", Ns);
                    break;

                default:
                    break;
            }

            // Perform garbage collection periodically to free up memory
            if (mtlDict.Count % 100 == 0)
            {
                System.GC.Collect();
            }
        }

        return mtlDict;
    }

    /// <summary>
    /// Loads a *.mtl file
    /// </summary>
    /// <param name="path">The path to the MTL file</param>
    /// <returns>Dictionary containing loaded materials</returns>
    public Dictionary<string, Material> Load(string path)
    {
        _objFileInfo = new FileInfo(path);
        SearchPaths.Add(_objFileInfo.Directory.FullName);

        using (var fs = new FileStream(path, FileMode.Open))
        {
            return Load(fs);
        }
    }
}
