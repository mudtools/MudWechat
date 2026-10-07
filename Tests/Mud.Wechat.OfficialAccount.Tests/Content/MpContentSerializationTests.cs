// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Comment;
using Mud.Wechat.OfficialAccount.DataModels.Draft;
using Mud.Wechat.OfficialAccount.DataModels.FreePublish;
using Mud.Wechat.OfficialAccount.DataModels.ProductCard;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Content;

/// <summary>
/// P2 内容域（草稿 / 发布 / 商品卡片 / 留言）DTO 与官方报文的双向映射（官方页<b>原文示例</b>回放）。
/// </summary>
public class MpContentSerializationTests
{
    /// <summary>S1：草稿 add 官方示例形态回放（news + newspic 双条目、嵌套 cover_info/image_info/product_info）。</summary>
    [Fact]
    public void DraftAddRequest_ShouldSerializeOfficialShape()
    {
        var request = new MpDraftAddRequest
        {
            Articles = new List<MpDraftArticle>
            {
                new MpDraftArticle
                {
                    ArticleType = "news",
                    Title = "标题",
                    Author = "麦多多",
                    ThumbMediaId = "R7Ifp6ogGOmtr3u-THUMB",
                    Content = "<p>正文</p>",
                    CoverInfo = new MpDraftCoverInfo
                    {
                        CropPercentList = new List<MpDraftCropPercent>
                        {
                            new MpDraftCropPercent { Ratio = "2.35_1", X1 = "0.1945", Y1 = "0", X2 = "1", Y2 = "0.5236" },
                            new MpDraftCropPercent { Ratio = "1_1", X1 = "0", Y1 = "0", X2 = "1", Y2 = "1" },
                        },
                    },
                },
                new MpDraftArticle
                {
                    ArticleType = "newspic",
                    Title = "图片消息",
                    Content = "纯文本",
                    ImageInfo = new MpDraftImageInfo
                    {
                        ImageList = new List<MpDraftImageItem> { new MpDraftImageItem { ImageMediaId = "IMG-ID" } },
                    },
                    ProductInfo = new MpDraftProductInfo
                    {
                        FooterProductInfo = new MpDraftFooterProductInfo { ProductKey = "PRODUCT_KEY" },
                    },
                },
            },
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, DraftJsonContext.Default.MpDraftAddRequest));
        var articles = document.RootElement.GetProperty("articles");
        articles.GetArrayLength().Should().Be(2, "articles 为数组形态（add 页；update 页为单对象）");

        var news = articles[0];
        news.GetProperty("cover_info").GetProperty("crop_percent_list").GetArrayLength().Should().Be(2);
        news.GetProperty("cover_info").GetProperty("crop_percent_list")[0].GetProperty("ratio")
            .GetString().Should().Be("2.35_1", "官方示例坐标/比例为字符串形态");

        var newspic = articles[1];
        newspic.GetProperty("image_info").GetProperty("image_list")[0].GetProperty("image_media_id")
            .GetString().Should().Be("IMG-ID");
        newspic.GetProperty("product_info").GetProperty("footer_product_info").GetProperty("product_key")
            .GetString().Should().Be("PRODUCT_KEY");
    }

    /// <summary>S2：draft/update 的 articles 为单对象（官方两页形态不一致的落点）。</summary>
    [Fact]
    public void DraftUpdateRequest_ShouldSerializeSingleArticleObject()
    {
        var request = new MpDraftUpdateRequest
        {
            MediaId = "MEDIA_ID",
            Index = 0,
            Articles = new MpDraftArticle { Title = "T", Content = "C", ThumbMediaId = "THUMB" },
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, DraftJsonContext.Default.MpDraftUpdateRequest));
        var root = document.RootElement;

        root.GetProperty("media_id").GetString().Should().Be("MEDIA_ID");
        root.GetProperty("articles").ValueKind.Should().Be(JsonValueKind.Object,
            "draft_update 页 articles 为单对象（与 add 页数组不同，照各自页面）");
        root.GetProperty("articles").GetProperty("title").GetString().Should().Be("T");
    }

    /// <summary>S3：draft/get 官方响应形态回放（news_item 含临时链接 url）。</summary>
    [Fact]
    public void DraftGetResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"news_item":[{"article_type":"news","title":"TITLE","author":"AUTHOR","digest":"DIGEST",
            "content":"CONTENT","content_source_url":"SOURCE","thumb_media_id":"THUMB",
            "need_open_comment":0,"only_fans_can_comment":0,"url":"DRAFT_URL"}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, DraftJsonContext.Default.MpDraftGetResponse);

        response!.IsSuccess.Should().BeTrue();
        response.NewsItems.Should().HaveCount(1);
        response.NewsItems![0].Url.Should().Be("DRAFT_URL", "草稿的临时链接");
        response.NewsItems[0].ArticleType.Should().Be("news");
    }

    /// <summary>S4：draft/count 与 draft/batchget 官方响应形态。</summary>
    [Fact]
    public void DraftCountAndBatchGet_ShouldParseOfficialSamples()
    {
        var count = System.Text.Json.JsonSerializer.Deserialize(
            """{"total_count":15}""", DraftJsonContext.Default.MpDraftCountResponse);
        count!.TotalCount.Should().Be(15);

        var batchGet = System.Text.Json.JsonSerializer.Deserialize(
            """{"total_count":2,"item_count":1,"item":[{"media_id":"MEDIA_ID","content":{"news_item":[{"title":"T","url":"U"}]},"update_time":1627891234}]}""",
            DraftJsonContext.Default.MpDraftBatchGetResponse);
        batchGet!.Items.Should().HaveCount(1);
        batchGet.Items![0].MediaId.Should().Be("MEDIA_ID");
        batchGet.Items[0].Content!.NewsItems![0].Title.Should().Be("T");
        batchGet.Items[0].UpdateTime.Should().Be(1627891234);
    }

    /// <summary>S5：发布状态查询官方响应回放（publish_status 标量 + article_detail.item + fail_idx）。</summary>
    [Fact]
    public void FreePublishGetResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"publish_id":"100000001","publish_status":0,"article_id":"ARTICLE_ID",
            "article_detail":{"count":1,"item":[{"idx":1,"article_url":"ARTICLE_URL"}]},"fail_idx":[]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, FreePublishJsonContext.Default.MpFreePublishGetResponse);

        response!.IsSuccess.Should().BeTrue();
        response.PublishStatus.Should().Be(0, "publish_status 为标量（核验修正：非数组）");
        response.ArticleId.Should().Be("ARTICLE_ID");
        response.ArticleDetail!.Items!.Single().ArticleUrl.Should().Be("ARTICLE_URL");
        response.FailIdx.Should().BeEmpty();
    }

    /// <summary>S6：已发布图文（getarticle）官方响应回放（is_deleted boolean）。</summary>
    [Fact]
    public void FreePublishGetArticleResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"news_item":[{"title":"TITLE","author":"AUTHOR","digest":"DIGEST","content":"CONTENT",
            "content_source_url":"SOURCE","thumb_media_id":"THUMB","thumb_url":"THUMB_URL",
            "need_open_comment":1,"only_fans_can_comment":0,"url":"URL","is_deleted":false}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, FreePublishJsonContext.Default.MpFreePublishGetArticleResponse);

        response!.NewsItems.Should().HaveCount(1);
        response.NewsItems![0].IsDeleted.Should().BeFalse("is_deleted 为 boolean 形态");
        response.NewsItems[0].ThumbUrl.Should().Be("THUMB_URL");
    }

    /// <summary>S7：发布列表官方响应回放（条目键 article_id——核验修正：非 item_id）。</summary>
    [Fact]
    public void FreePublishBatchGetResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"total_count":100,"item_count":10,"item":[{"article_id":"ARTICLE_ID_1",
            "content":{"news_item":[{"title":"T","is_deleted":false}]},"update_time":1627891234}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, FreePublishJsonContext.Default.MpFreePublishBatchGetResponse);

        response!.Items.Should().HaveCount(1);
        response.Items![0].ArticleId.Should().Be("ARTICLE_ID_1", "条目键为 article_id（逐页核验）");
        response.Items[0].Content!.NewsItems![0].Title.Should().Be("T");
    }

    /// <summary>S8：商品卡片请求 / 响应（DOM 大写属性名锁定）。</summary>
    [Fact]
    public void ProductCard_ShouldRoundTripOfficialShape()
    {
        var request = new MpProductCardInfoRequest
        {
            ProductId = "1000000000",
            ArticleType = "newspic",
            CardType = 2,
        };

        using var document = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            request, ProductCardJsonContext.Default.MpProductCardInfoRequest));
        document.RootElement.GetProperty("product_id").GetString().Should().Be("1000000000");

        var response = System.Text.Json.JsonSerializer.Deserialize(
            """{"errcode":0,"errmsg":"ok","product_key":"PRODUCT_KEY","DOM":"<card/>"}""",
            ProductCardJsonContext.Default.MpProductCardInfoResponse);
        response!.Dom.Should().Be("<card/>", "官方响应键为大写 DOM");
        response.ProductKey.Should().Be("PRODUCT_KEY");
    }

    /// <summary>S9：留言列表官方响应回放（comment 数组 + 嵌套 reply）。</summary>
    [Fact]
    public void CommentListResponse_ShouldParseOfficialSample()
    {
        const string json = """
            {"errcode":0,"errmsg":"ok","total":1,"comment":[{"user_comment_id":1,
            "openid":"OPENID","create_time":1627891234,"content":"好文","comment_type":1,
            "reply":{"content":"感谢","create_time":1627891300}}]}
            """;

        var response = System.Text.Json.JsonSerializer.Deserialize(
            json, CommentJsonContext.Default.MpCommentListResponse);

        response!.Total.Should().Be(1);
        response.Comments.Should().HaveCount(1);
        response.Comments![0].UserCommentId.Should().Be(1);
        response.Comments[0].CommentType.Should().Be(1, "精选评论");
        response.Comments[0].Reply!.Content.Should().Be("感谢");
    }

    /// <summary>S10：留言共享请求（扁平三字段形态——无嵌套 user_comment 对象）与回复请求。</summary>
    [Fact]
    public void CommentRefRequests_ShouldSerializeFlatOfficialShape()
    {
        var elect = new MpCommentRefRequest { MsgDataId = 123456, Index = 0, UserCommentId = 789 };
        using var electDoc = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            elect, CommentJsonContext.Default.MpCommentRefRequest));
        electDoc.RootElement.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo(
            new[] { "msg_data_id", "index", "user_comment_id" },
            "markelect/unmarkelect/delete/reply-delete 为扁平三字段（核验修正：无嵌套 user_comment）");

        var reply = new MpCommentReplyAddRequest
        {
            MsgDataId = 123456, Index = 0, UserCommentId = 789, Content = "内容",
        };
        using var replyDoc = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(
            reply, CommentJsonContext.Default.MpCommentReplyAddRequest));
        replyDoc.RootElement.GetProperty("content").GetString().Should().Be("内容");
    }
}
