using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Implementation;
using OrchardCore.DisplayManagement.Shapes;

namespace OrchardCore.Navigation
{
    public class PagerShapesTableProvider : IShapeTableProvider
    {
        private readonly IStringLocalizer S;

        public PagerShapesTableProvider(IStringLocalizer<PagerShapes> localizer)
        {
            S = localizer;
        }

        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("Pager")
                .OnCreated(created =>
                {
                    dynamic pager = created.Shape;

                    // Intializes the common properties of a Pager shape
                    // such that views can safely add values to them.
                    pager.ItemClasses = new List<string>();
                    pager.ItemAttributes = new Dictionary<string, string>();
                })
                .OnDisplaying(displaying =>
                {
                    dynamic pager = displaying.Shape;
                    string pagerId = pager.PagerId;
                    if (!String.IsNullOrEmpty(pagerId))
                    {
                        displaying.Shape.Metadata.Alternates.Add("Pager__" + EncodeAlternateElement(pagerId));
                    }
                });

            builder.Describe("PagerSlim")
                .OnCreated(created =>
                {
                    dynamic pager = created.Shape;

                    // Intializes the common properties of a Pager shape
                    // such that views can safely add values to them.
                    pager.ItemClasses = new List<string>();
                    pager.ItemAttributes = new Dictionary<string, string>();
                });


            builder.Describe("Pager_Gap")
                .OnDisplaying(displaying =>
                {
                    dynamic shape = displaying.Shape;
                    var pager = shape.Pager;
                    string pagerId = pager.PagerId;
                    if (!String.IsNullOrEmpty(pagerId))
                    {
                        pager.Metadata.Alternates.Add("Pager_Gap__" + EncodeAlternateElement(pagerId));
                    }
                });

            builder.Describe("Pager_First")
                .OnDisplaying(displaying =>
                {
                    dynamic shape = displaying.Shape;
                    var pager = shape.Pager;
                    string pagerId = pager.PagerId;
                    if (!String.IsNullOrEmpty(pagerId))
                    {
                        displaying.Shape.Metadata.Alternates.Add("Pager_First__" + EncodeAlternateElement(pagerId));
                    }
                });

            builder.Describe("Pager_Previous")
                .OnDisplaying(displaying =>
                {
                    dynamic shape = displaying.Shape;
                    var pager = shape.Pager;
                    string pagerId = pager.PagerId;
                    if (!String.IsNullOrEmpty(pagerId))
                    {
                        displaying.Shape.Metadata.Alternates.Add("Pager_Previous__" + EncodeAlternateElement(pagerId));
                    }
                });

            builder.Describe("Pager_Next")
                .OnDisplaying(displaying =>
                {
                    dynamic shape = displaying.Shape;
                    var pager = shape.Pager;
                    string pagerId = pager.PagerId;
                    if (!String.IsNullOrEmpty(pagerId))
                    {
                        displaying.Shape.Metadata.Alternates.Add("Pager_Next__" + EncodeAlternateElement(pagerId));
                    }
                });

            builder.Describe("Pager_Last")
                .OnDisplaying(displaying =>
                {
                    dynamic shape = displaying.Shape;
                    var pager = shape.Pager;
                    string pagerId = pager.PagerId;
                    if (!String.IsNullOrEmpty(pagerId))
                    {
                        displaying.Shape.Metadata.Alternates.Add("Pager_Last__" + EncodeAlternateElement(pagerId));
                    }
                });

            builder.Describe("Pager_CurrentPage")
                .OnDisplaying(displaying =>
                {
                    dynamic shape = displaying.Shape;
                    var pager = shape.Pager;
                    string pagerId = pager.PagerId;
                    if (!String.IsNullOrEmpty(pagerId))
                    {
                        displaying.Shape.Metadata.Alternates.Add("Pager_CurrentPage__" + EncodeAlternateElement(pagerId));
                    }
                });

            builder.Describe("Pager_Links")
                .OnDisplaying(displaying =>
                {
                    dynamic pager = displaying.Shape;
                    string pagerId = pager.PagerId;
                    if (!String.IsNullOrEmpty(pagerId))
                    {
                        displaying.Shape.Metadata.Alternates.Add("Pager_Links__" + EncodeAlternateElement(pagerId));
                    }
                });
        }

        private string EncodeAlternateElement(string alternateElement)
        {
            return alternateElement.Replace("-", "__").Replace('.', '_');
        }

    }

    public class PagerShapes : IShapeAttributeProvider
    {
        private readonly IStringLocalizer S;

        public PagerShapes(IStringLocalizer<PagerShapes> localizer)
        {
            S = localizer;
        }

        [Shape]
        public async Task<IHtmlContent> Pager_Links(dynamic Shape, dynamic DisplayAsync, dynamic New,
            IHtmlHelper Html,
            DisplayContext DisplayContext,
            int Page,
            int PageSize,
            double TotalItemCount,
            int? Quantity,
            object FirstText,
            object PreviousText,
            object NextText,
            object LastText,
            object GapText,
            string PagerId,
            bool ShowNext,
            string Tag,
            string ItemTag,
            IDictionary<string, string> Attributes,
            IDictionary<string, string> ItemAttributes
            // parameter omitted to workaround an issue where a NullRef is thrown
            // when an anonymous object is bound to an object shape parameter
            /*object RouteValues*/)
        {
            Shape.Tag = Tag;
            Shape.ItemTag = ItemTag;
            Shape.Attributes = Attributes;
            Shape.ItemAttributes = ItemAttributes;

            var currentPage = Page;
            if (currentPage < 1)
                currentPage = 1;

            var pageSize = PageSize;

            var numberOfPagesToShow = Quantity ?? 0;
            if (Quantity == null || Quantity < 0)
                numberOfPagesToShow = 7;

            var totalPageCount = pageSize > 0 ? (int)Math.Ceiling(TotalItemCount / pageSize) : 1;

            // return shape early if pager is not needed.
            if (totalPageCount < 2)
            {
                Shape.Metadata.Type = "List";
                return await DisplayAsync(Shape);
            }


            var firstText = FirstText ?? S["<<"];
            var previousText = PreviousText ?? S["<"];
            var nextText = NextText ?? S[">"];
            var lastText = LastText ?? S[">>"];
            var gapText = GapText ?? S["..."];

            var httpContextAccessor = DisplayContext.ServiceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var routeData = new RouteValueDictionary(Html.ViewContext.RouteData.Values);

            if (httpContext != null)
            {
                var queryString = httpContext.Request.Query;
                if (queryString != null)
                {
                    foreach (var key in from string key in queryString.Keys where key != null && !routeData.ContainsKey(key) let value = queryString[key] select key)
                    {
                        routeData[key] = queryString[key];
                    }
                }
            }

            // specific cross-requests route data can be passed to the shape directly (e.g., OrchardCore.Users)
            var shapeRoute = (object)((dynamic)Shape).RouteData;

            if (shapeRoute != null)
            {
                var shapeRouteData = shapeRoute as RouteValueDictionary;
                if (shapeRouteData == null)
                {
                    var route = shapeRoute as RouteData;
                    if (route != null)
                    {
                        shapeRouteData = new RouteValueDictionary(route.Values);
                    }
                }

                if (shapeRouteData != null)
                {
                    foreach (var rd in shapeRouteData)
                    {
                        routeData[rd.Key] = rd.Value;
                    }
                }
            }

            int firstPage = Math.Max(1, Page - (numberOfPagesToShow / 2));
            int lastPage = Math.Min(totalPageCount, Page + (int)(numberOfPagesToShow / 2));

            var pageKey = String.IsNullOrEmpty(PagerId) ? "page" : PagerId;

            Shape.Classes.Add("pager");
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "List";

            // first and previous pages
            if ((Page > 1) && (routeData.ContainsKey(pageKey)))
            {
                routeData.Remove(pageKey); // to keep from having "page=1" in the query string
            }
            // first
            Shape.Add(await New.Pager_First(Value: firstText, RouteValues: new RouteValueDictionary(routeData), Pager: Shape, Disabled: Page < 2));

            // previous
            if ((Page > 1) && (currentPage > 2))
            { // also to keep from having "page=1" in the query string
                routeData[pageKey] = currentPage - 1;
            }
            Shape.Add(await New.Pager_Previous(Value: previousText, RouteValues: new RouteValueDictionary(routeData), Pager: Shape, Disabled: Page < 2));


            // gap at the beginning of the pager
            if (firstPage > 1 && numberOfPagesToShow > 0)
            {
                Shape.Add(await New.Pager_Gap(Value: gapText, Pager: Shape));
            }

            // page numbers
            if (numberOfPagesToShow > 0 && lastPage > 1)
            {
                for (var p = firstPage; p <= lastPage; p++)
                {
                    if (p == currentPage)
                    {
                        routeData[pageKey] = currentPage;
                        Shape.Add(await New.Pager_CurrentPage(Value: p, RouteValues: new RouteValueDictionary(routeData), Pager: Shape));
                    }
                    else
                    {
                        if (p == 1)
                            routeData.Remove(pageKey);
                        else
                            routeData[pageKey] = p;
                        Shape.Add(await New.Pager_Link(Value: p, RouteValues: new RouteValueDictionary(routeData), Pager: Shape));
                    }
                }
            }

            // gap at the end of the pager
            if (lastPage < totalPageCount && numberOfPagesToShow > 0)
            {
                Shape.Add(await New.Pager_Gap(Value: gapText, Pager: Shape));
            }

            // Next
            routeData[pageKey] = Page + 1;
            Shape.Add(await New.Pager_Next(Value: nextText, RouteValues: new RouteValueDictionary(routeData), Pager: Shape, Disabled: Page >= totalPageCount && !ShowNext));

            // Last
            routeData[pageKey] = totalPageCount;
            Shape.Add(await New.Pager_Last(Value: lastText, RouteValues: new RouteValueDictionary(routeData), Pager: Shape, Disabled: Page >= totalPageCount));

            return await DisplayAsync(Shape);
        }

        [Shape]
        public Task<IHtmlContent> Pager(Shape Shape, dynamic DisplayAsync)
        {
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "Pager_Links";
            return DisplayAsync(Shape);
        }

        [Shape]
        public async Task<IHtmlContent> PagerSlim(dynamic Shape, dynamic DisplayAsync, dynamic New, IHtmlHelper Html,
            string Id,
            object PreviousText,
            object NextText,
            string PreviousClass,
            string NextClass,
            string Tag,
            string ItemTag,
            IDictionary<string, string> Attributes,
            IDictionary<string, string> ItemAttributes,
            Dictionary<string, object> UrlParams)
        {
            Shape.Classes.Add("pager");
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "List";
            Shape.Id = Id;
            Shape.Tag = Tag;
            Shape.ItemTag = ItemTag;
            Shape.Attributes = Attributes;
            Shape.ItemAttributes = ItemAttributes;

            var previousText = PreviousText ?? S["<"];
            var nextText = NextText ?? S[">"];

            var routeData = new RouteValueDictionary(Html.ViewContext.RouteData.Values);

            // Allows to pass custom url params to PagerSlim
            if (UrlParams != null)
            {
                foreach (var item in UrlParams)
                {
                    routeData.Add(item.Key, item.Value);
                }
            }

            string after = Shape.After;
            string before = Shape.Before;

            if (before != null)
            {
                var beforeRouteData = new RouteValueDictionary(routeData);
                beforeRouteData["before"] = (string)Shape.Before;
                Shape.Add(await New.Pager_Previous(Value: previousText, RouteValues: beforeRouteData, Pager: Shape));
                Shape.FirstClass = PreviousClass;
            }

            if (after != null)
            {
                var afterRouteData = new RouteValueDictionary(routeData);
                afterRouteData["after"] = (string)Shape.After;
                Shape.Add(await New.Pager_Next(Value: nextText, RouteValues: afterRouteData, Pager: Shape));
                Shape.LastClass = NextClass;
            }

            return await DisplayAsync(Shape);
        }

        [Shape]
        public Task<IHtmlContent> Pager_First(Shape Shape, dynamic DisplayAsync)
        {
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "Pager_Link";
            return DisplayAsync(Shape);
        }

        [Shape]
        public Task<IHtmlContent> Pager_Previous(Shape Shape, dynamic DisplayAsync)
        {
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "Pager_Link";
            return DisplayAsync(Shape);
        }

        [Shape]
        public Task<IHtmlContent> Pager_CurrentPage(Shape Shape, dynamic DisplayAsync)
        {
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "Pager_Link";
            ((dynamic)Shape).Tag.AddCssClass("active");
            return DisplayAsync(Shape);
        }

        [Shape]
        public Task<IHtmlContent> Pager_Next(Shape Shape, dynamic DisplayAsync)
        {
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "Pager_Link";
            return DisplayAsync(Shape);
        }

        [Shape]
        public Task<IHtmlContent> Pager_Last(Shape Shape, dynamic DisplayAsync)
        {
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "Pager_Link";
            return DisplayAsync(Shape);
        }

        [Shape]
        public Task<IHtmlContent> Pager_Link(IHtmlHelper Html, Shape Shape, dynamic DisplayAsync, object Value)
        {
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "ActionLink";
            return DisplayAsync(Shape);
        }

        [Shape]
        public IHtmlContent ActionLink(IUrlHelper Url, Shape Shape, object Value, bool Disabled = false)
        {
            if (Disabled)
            {
                TagBuilder parentLiTag = (TagBuilder)((dynamic)Shape).Tag;
                parentLiTag.AddCssClass("disabled");
            }

            var RouteValues = (object)((dynamic)Shape).RouteValues;
            RouteValueDictionary rvd;
            if (RouteValues == null)
            {
                rvd = new RouteValueDictionary();
            }
            else
            {
                rvd = RouteValues as RouteValueDictionary ?? new RouteValueDictionary(RouteValues);
            }

            var action = Url.Action((string)rvd["action"], (string)rvd["controller"], rvd);

            IEnumerable<string> classes = Shape.Classes;
            IDictionary<string, string> attributes = Shape.Attributes;
            attributes["href"] = action;
            string id = Shape.Id;
            var tag = Shape.GetTagBuilder("a", id, classes, attributes);

            tag.InnerHtml.AppendHtml(CoerceHtmlString(Value));
            return tag;
        }

        [Shape]
        public Task<IHtmlContent> Pager_Gap(Shape Shape, dynamic DisplayAsync)
        {
            Shape.Metadata.Alternates.Clear();
            Shape.Metadata.Type = "Pager_Link";
            ((dynamic)Shape).Tag.AddCssClass("disabled");
            return DisplayAsync(Shape);
        }

        private IHtmlContent CoerceHtmlString(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is IHtmlContent result)
            {
                return result;
            }

            return new StringHtmlContent(value.ToString());
        }
    }
}
