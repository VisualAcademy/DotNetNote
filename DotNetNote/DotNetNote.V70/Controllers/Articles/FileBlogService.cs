#nullable enable

namespace DotNetNote.Controllers.Articles;

using MemoEngineCore.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class FileBlogService : IBlogService
{
    private const string FILES = "files";

    private const string POSTS = "Posts";

    private readonly List<Post> cache = new List<Post>();

    private readonly string folder;

    public FileBlogService(IWebHostEnvironment env)
    {
        if (env is null)
        {
            throw new ArgumentNullException(nameof(env));
        }

        // C# 10의 새로운 기능 
        ArgumentNullException.ThrowIfNull(env);

        folder = Path.Combine(env.WebRootPath, POSTS);

        Initialize();
    }

    public IEnumerable<Post> GetPosts()
    {
        var posts = cache
            .Where(p => p.PubDate <= DateTime.UtcNow && p.IsPublished);

        return posts;
    }

    protected void SortCache() => cache.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));

    private static void LoadCategories(Post post, XElement doc)
    {
        var categories = doc.Element("categories");
        if (categories is null)
        {
            return;
        }

        post.Categories.Clear();
        categories.Elements("category").Select(node => node.Value).ToList().ForEach(post.Categories.Add);
    }

    private static string ReadAttribute(XElement element, XName name, string defaultValue = "") =>
        element.Attribute(name) is null ? defaultValue : element.Attribute(name)?.Value ?? defaultValue;

    private static string ReadValue(XElement doc, XName name, string defaultValue = "") =>
        doc.Element(name) is null ? defaultValue : doc.Element(name)?.Value ?? defaultValue;

    private string GetFilePath(Post post) => Path.Combine(folder, $"{post.ID}.xml");

    private void Initialize()
    {
        LoadPosts();
        SortCache();
    }

    private void LoadPosts()
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        foreach (var file in Directory.EnumerateFiles(folder, "*.xml", SearchOption.TopDirectoryOnly))
        {
            var doc = XElement.Load(file);

            var post = new Post
            {
                ID = Path.GetFileNameWithoutExtension(file),
                Title = ReadValue(doc, "title"),
                Excerpt = ReadValue(doc, "excerpt"),
                Content = ReadValue(doc, "content"),
                Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                PubDate = DateTime.Parse(ReadValue(doc, "pubDate"), CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal),
                LastModified = DateTime.Parse(
                    ReadValue(
                        doc,
                        "lastModified",
                        DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
            };

            LoadCategories(post, doc);
            cache.Add(post);
        }
    }
}
