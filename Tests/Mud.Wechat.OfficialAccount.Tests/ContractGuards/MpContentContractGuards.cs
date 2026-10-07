// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Comment;
using Mud.Wechat.OfficialAccount.DataModels.Draft;
using Mud.Wechat.OfficialAccount.DataModels.FreePublish;
using Mud.Wechat.OfficialAccount.DataModels.ProductCard;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// P2 内容域契约守卫（草稿管理 6 + 发布能力 5 + 商品卡片 1 + 留言管理 8 = 20 端点）。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07，20 页全 subscription 域命中）。关键核验修正：
/// ①草稿「单篇 8 条上限」官方页面无原文（仅第三方转述）⇒ 不建模；②publish_status 为标量非数组；
/// ③freepublish/batchget count 上限官方原文 1~20（非 100）、条目键 article_id（非 item_id）；
/// ④留言列表参数为 msg_data_id/begin/count/type（非 article_id/begin/limit）；
/// ⑤留言共享请求为扁平三字段（无嵌套 user_comment 对象）。
/// </remarks>
public class MpContentContractGuards
{
    private const string DraftRegistryGroupName = "Draft";
    private const string FreePublishRegistryGroupName = "FreePublish";
    private const string ProductCardRegistryGroupName = "ProductCard";
    private const string CommentRegistryGroupName = "Comment";

    /// <summary>草稿域官方路由表（6 端点；draft/switch 废弃不实现）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] DraftRoutes =
    {
        (typeof(IMpDraftService), nameof(IMpDraftService.AddDraftAsync), typeof(PostAttribute), "/cgi-bin/draft/add"),
        (typeof(IMpDraftService), nameof(IMpDraftService.UpdateDraftAsync), typeof(PostAttribute), "/cgi-bin/draft/update"),
        (typeof(IMpDraftService), nameof(IMpDraftService.GetDraftAsync), typeof(PostAttribute), "/cgi-bin/draft/get"),
        (typeof(IMpDraftService), nameof(IMpDraftService.DeleteDraftAsync), typeof(PostAttribute), "/cgi-bin/draft/delete"),
        (typeof(IMpDraftService), nameof(IMpDraftService.GetDraftCountAsync), typeof(GetAttribute), "/cgi-bin/draft/count"),
        (typeof(IMpDraftService), nameof(IMpDraftService.BatchGetDraftsAsync), typeof(PostAttribute), "/cgi-bin/draft/batchget"),
    };

    /// <summary>发布域官方路由表（5 端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] FreePublishRoutes =
    {
        (typeof(IMpFreePublishService), nameof(IMpFreePublishService.SubmitAsync), typeof(PostAttribute), "/cgi-bin/freepublish/submit"),
        (typeof(IMpFreePublishService), nameof(IMpFreePublishService.GetPublishStatusAsync), typeof(PostAttribute), "/cgi-bin/freepublish/get"),
        (typeof(IMpFreePublishService), nameof(IMpFreePublishService.DeletePublishAsync), typeof(PostAttribute), "/cgi-bin/freepublish/delete"),
        (typeof(IMpFreePublishService), nameof(IMpFreePublishService.GetArticleAsync), typeof(PostAttribute), "/cgi-bin/freepublish/getarticle"),
        (typeof(IMpFreePublishService), nameof(IMpFreePublishService.BatchGetPublishedAsync), typeof(PostAttribute), "/cgi-bin/freepublish/batchget"),
    };

    /// <summary>留言域官方路由表（8 端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] CommentRoutes =
    {
        (typeof(IMpCommentService), nameof(IMpCommentService.OpenCommentAsync), typeof(PostAttribute), "/cgi-bin/comment/open"),
        (typeof(IMpCommentService), nameof(IMpCommentService.CloseCommentAsync), typeof(PostAttribute), "/cgi-bin/comment/close"),
        (typeof(IMpCommentService), nameof(IMpCommentService.ListCommentsAsync), typeof(PostAttribute), "/cgi-bin/comment/list"),
        (typeof(IMpCommentService), nameof(IMpCommentService.MarkCommentElectAsync), typeof(PostAttribute), "/cgi-bin/comment/markelect"),
        (typeof(IMpCommentService), nameof(IMpCommentService.UnmarkCommentElectAsync), typeof(PostAttribute), "/cgi-bin/comment/unmarkelect"),
        (typeof(IMpCommentService), nameof(IMpCommentService.DeleteCommentAsync), typeof(PostAttribute), "/cgi-bin/comment/delete"),
        (typeof(IMpCommentService), nameof(IMpCommentService.ReplyCommentAsync), typeof(PostAttribute), "/cgi-bin/comment/reply/add"),
        (typeof(IMpCommentService), nameof(IMpCommentService.DeleteCommentReplyAsync), typeof(PostAttribute), "/cgi-bin/comment/reply/delete"),
    };

    /// <summary>契约守卫 CT1：草稿 / 发布 / 留言三域路由与官方契约一致。</summary>
    [Fact]
    public void ContentEndpoints_ShouldMatchOfficialRoutes()
    {
        foreach (var routes in new[] { DraftRoutes, FreePublishRoutes, CommentRoutes })
        {
            routes.Select(r => r.Route).Distinct().Should().HaveCount(routes.Length, "各端点路由互不重复");
            foreach (var (iface, method, httpAttribute, route) in routes)
            {
                var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");
                var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
                attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
                attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
            }
        }

        // 草稿：仅 draft/count 为 GET；发布：5 端点全 POST；留言：8 端点全 POST。
        DraftRoutes.Count(r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(1, "仅 draft/count 为 GET");
        FreePublishRoutes.Should().OnlyContain(r => r.HttpAttribute == typeof(PostAttribute));
        CommentRoutes.Should().OnlyContain(r => r.HttpAttribute == typeof(PostAttribute));

        // draft/switch 官方已废弃 ⇒ 不得建模（总账 1.1 口径）。
        typeof(IMpDraftService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Should().NotContain("/cgi-bin/draft/switch", "draft/switch 官方已废弃，不实现");

        // 商品卡片：/channels/ec/ 特殊前缀（视频号小店域，非 /cgi-bin/——照抄官方路径）。
        var card = typeof(IMpProductCardService).GetMethod(nameof(IMpProductCardService.GetProductCardInfoAsync))!;
        card.GetCustomAttribute<PostAttribute>()!.RequestUri.Should().Be("/channels/ec/service/product/getcardinfo");
    }

    /// <summary>契约守卫 CT2：草稿双形态不一致（add 数组 vs update 单对象）与关键 DTO 字段锁定。</summary>
    [Fact]
    public void DraftShapes_ShouldLockArticleFieldsAndDualForms()
    {
        // add：articles 为数组；update：articles 为单对象（官方两页形态不一致，照各自页面）。
        typeof(MpDraftAddRequest).GetProperty(nameof(MpDraftAddRequest.Articles))!.PropertyType
            .Should().Be(typeof(List<MpDraftArticle>), "draft_add 页 articles 为 objarray（数组）");
        typeof(MpDraftUpdateRequest).GetProperty(nameof(MpDraftUpdateRequest.Articles))!.PropertyType
            .Should().Be(typeof(MpDraftArticle), "draft_update 页 articles 为 object（单对象）——官方两页不一致");

        AssertJsonProperty<MpDraftArticle>("article_type", "news / newspic（默认 news）");
        AssertJsonProperty<MpDraftArticle>("title", "标题 ≤32 字");
        AssertJsonProperty<MpDraftArticle>("digest", "摘要 ≤120 字（未填抓取正文前 54 字）");
        AssertJsonProperty<MpDraftArticle>("content", "正文（图片 url 须经 uploadimg 获取，外部被过滤）");
        AssertJsonProperty<MpDraftArticle>("thumb_media_id", "news 时必填（永久 MediaID）");
        AssertJsonProperty<MpDraftArticle>("image_info", "newspic 必填（≤20 张，首张为封面）");
        AssertJsonProperty<MpDraftArticle>("cover_info", "封面信息（news/newspic 均适用）");
        AssertJsonProperty<MpDraftArticle>("product_info", "商品信息（带货场景）");
        AssertJsonProperty<MpDraftImageItem>("image_media_id", "图片素材 id");
        AssertJsonProperty<MpDraftCropPercent>("ratio", "裁剪比例（字符串形态如 2.35_1）");
        AssertJsonProperty<MpDraftFooterProductInfo>("product_key", "商品 key");

        // 草稿 news_item 与永久素材 news_item 字段集不同（多 article_type/image_info/cover_info/product_info/url）⇒ 不共用。
        AssertJsonProperty<MpDraftNewsItem>("article_type", "草稿条目独有字段");
        AssertJsonProperty<MpDraftNewsItem>("url", "草稿的临时链接");
        AssertJsonProperty<MpDraftNewsItem>("image_info", "仅 newspic 携带");
        typeof(MpDraftNewsItem).Should().NotBe(typeof(Mud.Wechat.OfficialAccount.DataModels.Media.MpMaterialNewsItem),
            "草稿条目与永久素材条目字段集不同 ⇒ 不共用 DTO");

        // draft/get 与 draft/batchget 两页 news_item 同构 ⇒ 共用条目 DTO。
        typeof(MpDraftNewsContent).GetProperty(nameof(MpDraftNewsContent.NewsItems))!.PropertyType
            .Should().Be(typeof(List<MpDraftNewsItem>), "两页 news_item 同构 ⇒ 共用条目");

        AssertJsonProperty<MpDraftAddResponse>("media_id", "草稿 media_id（≤128 字符）");
        AssertJsonProperty<MpDraftGetRequest>("media_id", "按 media_id 寻址");
        AssertJsonProperty<MpDraftCountResponse>("total_count", "草稿总数");
        AssertJsonProperty<MpDraftBatchGetRequest>("no_content", "1 = 不返回 content（默认 0）");
        AssertJsonProperty<MpDraftBatchGetResponse>("item", "草稿条目列表");
    }

    /// <summary>契约守卫 CT3：发布域字段与核验修正锁定（标量 publish_status / count 1~20 / article_id 键）。</summary>
    [Fact]
    public void FreePublishShapes_ShouldLockVerifiedCorrections()
    {
        AssertJsonProperty<MpFreePublishSubmitRequest>("media_id", "要发布的草稿的 media_id");
        AssertJsonProperty<MpFreePublishSubmitResponse>("publish_id", "发布任务 id");
        AssertJsonProperty<MpFreePublishSubmitResponse>("msg_data_id", "官方返回表有、示例缺失（矛盾照录）");
        AssertJsonProperty<MpFreePublishGetResponse>("publish_status", "标量 0~6（方案预判的数组已被核验修正）");
        AssertJsonProperty<MpFreePublishGetResponse>("article_detail", "成功时的文章详情");
        AssertJsonProperty<MpFreePublishGetResponse>("fail_idx", "失败文章编号列表");
        AssertJsonProperty<MpFreePublishArticleUrlItem>("article_url", "图文的永久链接");

        AssertJsonProperty<MpFreePublishDeleteRequest>("article_id", "成功发布时返回的 article_id");
        AssertJsonProperty<MpFreePublishDeleteRequest>("index", "第一篇编号 1，不填或 0 删除全部");

        AssertJsonProperty<MpFreePublishNewsItem>("thumb_url", "封面图片 URL");
        AssertJsonProperty<MpFreePublishNewsItem>("is_deleted", "该图文是否被删除（boolean）");

        // 核验修正：batchget count 上限官方原文 1~20（非方案预判的 100）；条目键 article_id（非 item_id）。
        AssertJsonProperty<MpFreePublishBatchGetRequest>("count", "官方原文「取值在 1 到 20 之间」（已双验）");
        AssertJsonProperty<MpFreePublishListItem>("article_id", "成功发布的图文消息 id（非 item_id）");
        JsonNamesOf<MpFreePublishBatchGetResponse>().Should().NotContain("item_id",
            "逐页核验确认响应无 item_id 字段（方案预判已修正）");

        // getarticle 与 batchget 两页 news_item 同构 ⇒ 共用条目 DTO。
        typeof(MpFreePublishNewsContent).GetProperty(nameof(MpFreePublishNewsContent.NewsItems))!.PropertyType
            .Should().Be(typeof(List<MpFreePublishNewsItem>));
    }

    /// <summary>契约守卫 CT4：商品卡片字段与官方 DOM 属性名锁定。</summary>
    [Fact]
    public void ProductCardShapes_ShouldLockOfficialFields()
    {
        AssertJsonProperty<MpProductCardInfoRequest>("product_id", "商品 id");
        AssertJsonProperty<MpProductCardInfoRequest>("article_type", "newspic / news");
        AssertJsonProperty<MpProductCardInfoRequest>("card_type", "0 大卡 / 1 小卡 / 2 文字链接 / 3 条卡");
        AssertJsonProperty<MpProductCardInfoResponse>("product_key", "商品 key（按需返回）");
        AssertJsonProperty<MpProductCardInfoResponse>("DOM", "商品卡 DOM 结构（官方属性名大写形态）");
        typeof(MpProductCardInfoResponse).GetProperty(nameof(MpProductCardInfoResponse.Dom))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("DOM",
            "官方 JSON 键为大写 DOM——不得「规范化」为小写");
    }

    /// <summary>契约守卫 CT5：留言域寻址形态与共用 DTO 裁决锁定（扁平三字段，无嵌套 user_comment）。</summary>
    [Fact]
    public void CommentShapes_ShouldLockAddressingAndSharedDtos()
    {
        // 全域以 msg_data_id + index 寻址（核验修正：非 article_id/begin/limit 形态）。
        AssertJsonProperty<MpCommentOpenRequest>("msg_data_id", "群发返回的 msg_data_id");
        AssertJsonProperty<MpCommentOpenRequest>("index", "多图文序号（从 0 开始）");
        AssertJsonProperty<MpCommentListRequest>("begin", "起始位置");
        AssertJsonProperty<MpCommentListRequest>("count", "获取数目（50 以上被拒绝——88010）");
        AssertJsonProperty<MpCommentListRequest>("type", "0 全部 / 1 普通 / 2 精选");

        // 共用裁决（N4）：open/close 共用；markelect/unmarkelect/delete/reply-delete 共用扁平三字段。
        typeof(IMpCommentService).GetMethod(nameof(IMpCommentService.CloseCommentAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(MpCommentOpenRequest),
            "open/close 请求体字段集一致 ⇒ 共用 DTO");
        foreach (var method in new[]
                 {
                     nameof(IMpCommentService.MarkCommentElectAsync),
                     nameof(IMpCommentService.UnmarkCommentElectAsync),
                     nameof(IMpCommentService.DeleteCommentAsync),
                     nameof(IMpCommentService.DeleteCommentReplyAsync),
                 })
        {
            typeof(IMpCommentService).GetMethod(method)!.GetParameters()
                .Should().Contain(p => p.ParameterType == typeof(MpCommentRefRequest),
                    $"{method} 官方请求体为扁平 msg_data_id/index/user_comment_id 三字段 ⇒ 共用 DTO");
        }

        // 嵌套 user_comment 对象不存在（方案预判已被核验修正）——请求 DTO 不得出现该属性名。
        JsonNamesOf<MpCommentRefRequest>().Should().NotContain("user_comment",
            "官方为扁平 user_comment_id 三字段形态（无嵌套 user_comment 对象）");

        AssertJsonProperty<MpComment>("user_comment_id", "用户评论 id");
        AssertJsonProperty<MpComment>("comment_type", "0 非精选 / 1 精选（官方说明疑漏字，照录）");
        AssertJsonProperty<MpComment>("openid", "非微信身份评论不返回 openid");
        AssertJsonProperty<MpCommentReplyInfo>("content", "回复内容");
        AssertJsonProperty<MpCommentReplyAddRequest>("content", "回复内容（reply/add 独有第四字段）");
    }

    /// <summary>契约守卫 CT6：P2 四域 DTO 全量登记进各自 AOT JSON 上下文。</summary>
    [Fact]
    public void ContentDataModels_ShouldBeRegisteredInJsonContexts()
    {
        AssertContextGroup(typeof(MpDraftAddResponse).Assembly, "Mud.Wechat.OfficialAccount.DataModels.Draft",
            DraftJsonContext.Default, DraftRegistryGroupName, 19,
            "草稿域（article 条目 + image/cover/product 嵌套 + add/update/get/count/batchget 请求响应 + 列表条目/容器）");
        AssertContextGroup(typeof(MpFreePublishSubmitRequest).Assembly, "Mud.Wechat.OfficialAccount.DataModels.FreePublish",
            FreePublishJsonContext.Default, FreePublishRegistryGroupName, 14,
            "发布域（submit 请求响应 + get 请求响应/详情/条目 + delete 请求 + getarticle 请求响应/条目 + batchget 请求响应/条目/容器）");
        AssertContextGroup(typeof(MpProductCardInfoRequest).Assembly, "Mud.Wechat.OfficialAccount.DataModels.ProductCard",
            ProductCardJsonContext.Default, ProductCardRegistryGroupName, 2, "商品卡片域（请求 + 响应）");
        AssertContextGroup(typeof(MpCommentOpenRequest).Assembly, "Mud.Wechat.OfficialAccount.DataModels.Comment",
            CommentJsonContext.Default, CommentRegistryGroupName, 7,
            "留言域（open 共用请求 + list 请求响应/条目/回复 + ref 共用请求 + reply/add 请求）");
    }

    /// <summary>契约守卫 CT7：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void ContentModules_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("Draft").And.Contain("FreePublish")
            .And.Contain("ProductCard").And.Contain("Comment");
        typeof(MpServiceBuilder).GetMethod("AddDraftApi").Should().NotBeNull();
        typeof(MpServiceBuilder).GetMethod("AddFreePublishApi").Should().NotBeNull();
        typeof(MpServiceBuilder).GetMethod("AddProductCardApi").Should().NotBeNull();
        typeof(MpServiceBuilder).GetMethod("AddCommentApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpDraftService));
        queryInterfaces.Should().Contain(nameof(IMpFreePublishService));
        queryInterfaces.Should().Contain(nameof(IMpProductCardService));
        queryInterfaces.Should().Contain(nameof(IMpCommentService));
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>契约守卫 CT8：P2 内容域已核验错误码常量锁定。</summary>
    [Fact]
    public void ContentErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.DraftIndexInvalid.Should().Be(40114);
        MpErrorCodes.DraftContentSourceUrlInvalid.Should().Be(41039);
        MpErrorCodes.DraftContentInvalid.Should().Be(45166);
        MpErrorCodes.CommerceAbilityLimited.Should().Be(53404);
        MpErrorCodes.CommerceProductInfoInvalid.Should().Be(53405);
        MpErrorCodes.CommerceAbilityNotEnabled.Should().Be(53406);
        MpErrorCodes.PublishDraftCheckFailed.Should().Be(53503);
        MpErrorCodes.PublishDraftMpOnly.Should().Be(53504);
        MpErrorCodes.PublishDraftNotManuallySaved.Should().Be(53505);
        MpErrorCodes.ProductCardProductIdInvalid.Should().Be(10170001);
        MpErrorCodes.ProductCardArticleTypeUnsupported.Should().Be(10170002);
        MpErrorCodes.ProductCardCardTypeUnsupported.Should().Be(10170003);
        MpErrorCodes.CommentPrivilegeMissing.Should().Be(88000);
        MpErrorCodes.CommentMsgDataNotExists.Should().Be(88001);
        MpErrorCodes.CommentArticleSafetyLimited.Should().Be(88002);
        MpErrorCodes.CommentElectLimitReached.Should().Be(88003);
        MpErrorCodes.CommentDeletedByUser.Should().Be(88004);
        MpErrorCodes.CommentAlreadyReplied.Should().Be(88005);
        MpErrorCodes.CommentReplyContentInvalid.Should().Be(88007);
        MpErrorCodes.CommentNotExists.Should().Be(88008);
        MpErrorCodes.CommentCountOutOfRange.Should().Be(88010);
        MpErrorCodes.CommentReplySignatureInvalid.Should().Be(87009);
    }

    private static void AssertContextGroup(Type seedType, System.Text.Json.Serialization.JsonSerializerContext context,
        string groupName, int expectedCount, string because)
    {
        var domainTypes = seedType.Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == seedType.Namespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(expectedCount, because);

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull($"{type.Name} 必须登记进 {groupName}JsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(groupName, $"{type.Name} 的 SerializerClassName 必须为 {groupName}");
        }
    }

    private static void AssertContextGroup(System.Reflection.Assembly assembly, string ns,
        System.Text.Json.Serialization.JsonSerializerContext context,
        string groupName, int expectedCount, string because)
    {
        var domainTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == ns
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(expectedCount, because);

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull($"{type.Name} 必须登记进 {groupName}JsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(groupName, $"{type.Name} 的 SerializerClassName 必须为 {groupName}");
        }
    }

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
