using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "paging-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PagingModel { get; set; }
        public string PageAction { get; set; }
        public string PageClass { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClassSelected { get; set; }
        public string PageClassNormal { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            TagBuilder divTagBuilder = new TagBuilder("div");
            for (int i = 1; i <= PagingModel.totalPage; i++)
            {
                TagBuilder aTagBuilder = new TagBuilder("a");
                aTagBuilder.Attributes["href"] = urlHelper.Action(PageAction, new { productPage = i });
                if (PageClassesEnabled)   //If the page-classes-enabled=true in pagination div in view
                {
                    aTagBuilder.AddCssClass(PageClass); //Adds border to the anchor, see the pagination div part in view
                    aTagBuilder.AddCssClass(i == PagingModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                aTagBuilder.InnerHtml.Append(i.ToString());
                divTagBuilder.InnerHtml.AppendHtml(aTagBuilder);
            }
            output.Content.AppendHtml(divTagBuilder.InnerHtml);
        }
    }
}
