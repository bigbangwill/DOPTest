using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        var catalogData = new Dictionary<string, object>
        {
            ["booksByIsbn"] = new Dictionary<string, object>
            {
                ["978-1779501127"] = new Dictionary<string, object>
                {
                    ["isbn"] = "978-1779501127",
                    ["title"] = "Watchmen",
                    ["publicationYear"] = 1987,
                    ["authorIds"] = new List<string> { "alan-moore", "dave-gibbons" },
                    ["bookItems"] = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["id"] = "book-item-1",
                    ["libId"] = "nyc-central-lib",
                    ["isLent"] = true
                },
                new Dictionary<string, object>
                {
                    ["id"] = "book-item-2",
                    ["libId"] = "nyc-central-lib",
                    ["isLent"] = false
                }
            }
                }
            },
            ["authorsById"] = new Dictionary<string, object>
            {
                ["alan-moore"] = new Dictionary<string, object>
                {
                    ["name"] = "Alan Moore",
                    ["bookIsbns"] = new List<string> { "978-1779501127" }
                },
                ["dave-gibbons"] = new Dictionary<string, object>
                {
                    ["name"] = "Dave Gibbons",
                    ["bookIsbns"] = new List<string> { "978-1779501127" }
                }
            }
        };

        var result = CatalogModule.SearchBooksByTitle(catalogData, "Wat");
        Debug.Log(JsonConvert.SerializeObject(result));
    }

    public List<TResult> Map<TSource, TResult>(List<TSource> collection, Func<TSource, TResult> f)
    {
        var result = new List<TResult>();
        foreach (var item in collection)
        {
            result.Add(f(item));
        }
        return result;
    }
}

public static class MyLinqExtension
{
    public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        foreach (var item in source)
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }
}


public static class CatalogModule
{
    public static List<Dictionary<string, object>> SearchBooksByTitle(Dictionary<string, object> catalogData, string query)
    {
        var booksByIsbn = (Dictionary<string, object>)catalogData["booksByIsbn"];

        var allBooks = booksByIsbn.Values.Cast<Dictionary<string, object>>().ToList();

        var matchingBooks = allBooks
            .Where(book =>
            {
                var title = book["title"].ToString();
                return title.Contains(query, StringComparison.OrdinalIgnoreCase);
            }).ToList();

        var bookInfos = matchingBooks.Select(book => BookInfo(catalogData, book)).ToList();

        return bookInfos;
    }

    // Stub function — you should implement this to return what you need
    private static Dictionary<string, object> BookInfo(Dictionary<string, object> catalogData, Dictionary<string, object> book)
    {
        return new Dictionary<string, object>
        {
            ["title"] = book["title"],
            ["isbn"] = book["isbn"],
            ["authorNames"] = GetAuthorNames(catalogData, book)
        };
    }

    private static List<string> GetAuthorNames(Dictionary<string, object> catalogData, Dictionary<string, object> book)
    {
        var authorIds = (List<string>)book["authorIds"];
        var authorsById = (Dictionary<string, object>)catalogData["authorsById"];

        return authorIds.Select(authorId =>
        {
            var author = (Dictionary<string, object>)authorsById[authorId];
            return author["name"].ToString();
        }).ToList();
    }
}
