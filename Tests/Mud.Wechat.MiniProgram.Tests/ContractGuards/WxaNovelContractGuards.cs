// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.Novel;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「小说作品管理」域契约守卫（NV 系列：23 端点路由与动词 + 令牌绑定与注册组 +
/// 参数形态与裸应答裁决 + 官方字段名与包装形态 + DTO 登记计数与命名空间清单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方索引页 <c>platform-capabilities/industry/novel-api.html</c> 全集 23 支，2026-10-16 逐页取证）</b>：
/// 作品 6（<see cref="IWxaNovelBookService"/>）+ 章节 8（<see cref="IWxaNovelChapterService"/>）+
/// 账号-小说授权 3（<see cref="IWxaNovelAuthService"/>）+ 账号-账号授权 3（<see cref="IWxaNovelAccountAuthService"/>）+
/// 阅读器 3（<see cref="IWxaNovelReaderService"/>）。权限集 169，不支持云调用，支持第三方平台代商家调用。
/// </para>
/// <para>
/// <b>两路由族并存是本域的路由级反直觉点</b>：20 支落 <c>/wxa/book/*</c>，仅阅读器 3 支落
/// <c>/wxa/business/novelreader/*</c> ⇒ 不得整域归一成一个前缀断言。
/// </para>
/// <para>
/// <b>2026-10-16 逐页取证修出的四处契约偏差</b>（本守卫逐条锁定，防止按第三方转述回潮）：
/// ① <c>addbookauth</c> 官方请求体是 <c>{"books":[…]}</c> 数组包装，早期建模成了扁平单条；
/// ② <c>setpreviewsetting</c> 官方请求体是 <c>{"setting":{…}}</c> 嵌套单对象，早期建模成了四个扁平字段；
/// ③ <c>batchcreatechapter</c> 应答键名是 <c>chapter_id_list</c>（复数），早期写成 <c>chapter_id</c>；
/// ④ <c>delbookauthbyappid</c> 官方请求体只有 <c>grantee_appid</c> 一支键，早期的 <c>book_id</c> 系凭空补出。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：端点聚合计数（<c>ContractBaseline</c> 的 Novel 23）、跨线路由零重叠（MP-X1）、
/// 带令牌接口计数（MP-X2）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；本文件只锁本域精确契约。
/// </para>
/// </remarks>
public class WxaNovelContractGuards
{
    private const string RegistryGroupName = "Novel";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.Novel";

    /// <summary>作品、章节与两支授权族共用的官方路由前缀（20 支）。</summary>
    private const string BookRoutePrefix = "/wxa/book/";

    /// <summary>阅读器族前缀（3 支，本域唯一非 <c>/wxa/book/</c> 前缀）。</summary>
    private const string ReaderRoutePrefix = "/wxa/business/novelreader/";

    /// <summary>
    /// 小说作品管理域官方路由表（23 端点：作品 6 + 章节 8 + 授权 3 + 账号授权 3 + 阅读器 3；路由与动词照官方原文）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaNovelBookService), nameof(IWxaNovelBookService.CreateBookAsync), typeof(PostAttribute), "/wxa/book/createbook"),
        (typeof(IWxaNovelBookService), nameof(IWxaNovelBookService.UpdateBookAsync), typeof(PostAttribute), "/wxa/book/updatebook"),
        (typeof(IWxaNovelBookService), nameof(IWxaNovelBookService.DeleteBookAsync), typeof(PostAttribute), "/wxa/book/deletebook"),
        (typeof(IWxaNovelBookService), nameof(IWxaNovelBookService.ListBookAsync), typeof(PostAttribute), "/wxa/book/listbook"),
        (typeof(IWxaNovelBookService), nameof(IWxaNovelBookService.GetBookAsync), typeof(PostAttribute), "/wxa/book/getbook"),
        (typeof(IWxaNovelBookService), nameof(IWxaNovelBookService.AuditBookAsync), typeof(PostAttribute), "/wxa/book/auditbook"),

        (typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.CreateChapterAsync), typeof(PostAttribute), "/wxa/book/createchapter"),
        (typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.BatchCreateChapterAsync), typeof(PostAttribute), "/wxa/book/batchcreatechapter"),
        (typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.DeleteChapterAsync), typeof(PostAttribute), "/wxa/book/deletechapter"),
        (typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.ReplaceChapterAsync), typeof(PostAttribute), "/wxa/book/replacechapter"),
        (typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.ListChapterAsync), typeof(PostAttribute), "/wxa/book/listchapter"),
        (typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.GetChapterAsync), typeof(PostAttribute), "/wxa/book/getchapter"),
        (typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.ReorderChapterAsync), typeof(PostAttribute), "/wxa/book/reorderchapter"),
        (typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.UpdateChapterSeqAsync), typeof(PostAttribute), "/wxa/book/updatechapterseq"),

        (typeof(IWxaNovelAuthService), nameof(IWxaNovelAuthService.AddBookAuthAsync), typeof(PostAttribute), "/wxa/book/addbookauth"),
        (typeof(IWxaNovelAuthService), nameof(IWxaNovelAuthService.QueryBookAuthAsync), typeof(PostAttribute), "/wxa/book/querybookauth"),
        (typeof(IWxaNovelAuthService), nameof(IWxaNovelAuthService.DelBookAuthAsync), typeof(PostAttribute), "/wxa/book/delbookauth"),

        (typeof(IWxaNovelAccountAuthService), nameof(IWxaNovelAccountAuthService.AddBookAuthByAppIdAsync), typeof(PostAttribute), "/wxa/book/addbookauthbyappid"),
        (typeof(IWxaNovelAccountAuthService), nameof(IWxaNovelAccountAuthService.QueryBookAuthV2Async), typeof(PostAttribute), "/wxa/book/querybookauthv2"),
        (typeof(IWxaNovelAccountAuthService), nameof(IWxaNovelAccountAuthService.DelBookAuthByAppIdAsync), typeof(PostAttribute), "/wxa/book/delbookauthbyappid"),

        (typeof(IWxaNovelReaderService), nameof(IWxaNovelReaderService.SetPreviewSettingAsync), typeof(PostAttribute), "/wxa/business/novelreader/setpreviewsetting"),
        (typeof(IWxaNovelReaderService), nameof(IWxaNovelReaderService.GetPreviewSettingAsync), typeof(PostAttribute), "/wxa/business/novelreader/getpreviewsetting"),
        (typeof(IWxaNovelReaderService), nameof(IWxaNovelReaderService.SetRecmdNovelAsync), typeof(PostAttribute), "/wxa/business/novelreader/setrecmdnovel"),
    };

    private static readonly Type[] DomainInterfaces =
    {
        typeof(IWxaNovelBookService),
        typeof(IWxaNovelChapterService),
        typeof(IWxaNovelAuthService),
        typeof(IWxaNovelAccountAuthService),
        typeof(IWxaNovelReaderService),
    };

    /// <summary>官方返回体只有 errcode / errmsg 的端点（全仓口径：直接返回基类 <see cref="WxaResponse"/>，不建空壳应答）。</summary>
    private static readonly string[] BareResponseMethods =
    {
        nameof(IWxaNovelBookService.UpdateBookAsync),
        nameof(IWxaNovelBookService.DeleteBookAsync),
        nameof(IWxaNovelBookService.AuditBookAsync),
        nameof(IWxaNovelChapterService.DeleteChapterAsync),
        nameof(IWxaNovelChapterService.ReorderChapterAsync),
        nameof(IWxaNovelChapterService.UpdateChapterSeqAsync),
        nameof(IWxaNovelAuthService.DelBookAuthAsync),
        nameof(IWxaNovelAccountAuthService.DelBookAuthByAppIdAsync),
        nameof(IWxaNovelReaderService.SetPreviewSettingAsync),
        nameof(IWxaNovelReaderService.SetRecmdNovelAsync),
    };

    /// <summary>契约守卫 NV1：23 端点路由与动词必须与官方契约一致，且声明面与路由表全集相等。</summary>
    [Fact]
    public void NovelEndpoints_ShouldMatchOfficialRoutes()
    {
        // 23 端点 = 作品 6 + 章节 8 + 授权 3 + 账号授权 3 + 阅读器 3（官方「小说作品管理接口」索引页逐行取证全集）。
        Routes.Should().HaveCount(23,
            "小说作品管理域官方开放面为 23 端点（官方索引页 2026-10-16 逐行核验：book 20 + novelreader 3）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(23, "路由表不得出现重复条目");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（照官方原文拼写，禁止改写）");

            // 本域 23 支官方一律 POST（含六支「查询」与两支「列表」——POST 查询是本域的动词口径，勿顺手改 GET）。
            attr.Should().BeOfType<PostAttribute>($"{route} 官方调用方式为 POST");
            httpAttribute.Should().Be(typeof(PostAttribute), $"{route} 路由表的动词登记须与官方一致");
        }

        // 两路由族锁定：20 支 /wxa/book/* + 3 支 /wxa/business/novelreader/*，无第三种前缀混入。
        Routes.Count(static r => r.Route.StartsWith(BookRoutePrefix, StringComparison.Ordinal))
            .Should().Be(20, "作品 / 章节 / 两支授权共 20 支落在 /wxa/book/ 族内");
        Routes.Count(static r => r.Route.StartsWith(ReaderRoutePrefix, StringComparison.Ordinal))
            .Should().Be(3, "阅读器 3 支落在 /wxa/business/novelreader/ 族内");
        Routes.Should().OnlyContain(static r => r.Route == r.Route.ToLowerInvariant(),
            "本域官方路由一律全小写连写（不得顺手改成驼峰）");

        // 悬空宣称的正面封堵：/wxaapi/broadcast/* 是直播域路由族（IWxaLiveRoomService 在册），
        // 早期 README 曾把它记成小说域 3 支 ⇒ 本域声明面出现该前缀即为跨域错挂。
        Routes.Should().NotContain(static r => r.Route.StartsWith("/wxaapi/", StringComparison.Ordinal),
            "/wxaapi/broadcast/* 属直播域，小说域 23 支官方一律 /wxa/book/ 与 /wxa/business/novelreader/ 两族");

        // 声明面全集自证：Interfaces/Novel/ 五文件里出现的每一条路由字面量都必须在路由表内，
        // 且反向也成立（防「加了端点忘了进路由表」——方法数断言只能挡总量，挡不住错挂到别族）。
        var declared = DeclaredRoutesInNovelInterfaces();
        declared.Should().BeEquivalentTo(Routes.Select(static r => r.Route),
            "Interfaces/Novel/ 的声明面必须与 NV1 路由表逐条相等（多一条即漏登记，少一条即路由漂移）");
    }

    /// <summary>契约守卫 NV2：五接口令牌绑定、注册组与逐接口规模（权限集 169 的小程序 access_token 面）。</summary>
    [Fact]
    public void NovelInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup()
    {
        DomainInterfaces.Should().HaveCount(5, "小说作品管理域按官方子目录族分为五接口（合计 23 端点）");

        foreach (var iface in DomainInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方每页「调用方式」均以 access_token 为凭据，权限集 169）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（改绑小店或企微令牌会让单槽 Resolve 返回 null，errcode 自愈静默失效）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约把 access_token 强制放 Query（MP-X3 同口径）");
            token.Name.Should().Be("access_token");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 Novel 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序令牌底座（AddMpApp 硬前置）");
        }

        DomainInterfaces.Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .Distinct().Should().HaveCount(1, "本域五接口共用一个注册组（AddNovelWebApiHttpClient）");

        // 端点归属自证：五接口方法数之和须等于路由表规模。
        DomainInterfaces
            .Select(static i => i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length)
            .Sum().Should().Be(Routes.Length, "五接口声明方法总数须与 NV1 路由表一致");

        // 逐接口规模锁定（官方子目录族划分，防「把某一族的端点错挂到另一族」）。
        MethodCount(typeof(IWxaNovelBookService)).Should().Be(6, "作品族 6（创建 / 编辑 / 删除 / 列表 / 详情 / 提审）");
        MethodCount(typeof(IWxaNovelChapterService)).Should().Be(8, "章节族 8（上传 / 批量上传 / 删除 / 替换 / 列表 / 详情 / 调整顺序 / 调整相对顺序）");
        MethodCount(typeof(IWxaNovelAuthService)).Should().Be(3, "账号-小说授权族 3（新增 / 查询 / 删除）");
        MethodCount(typeof(IWxaNovelAccountAuthService)).Should().Be(3, "账号-账号授权族 3（新增 / 查询 V2 / 删除）");
        MethodCount(typeof(IWxaNovelReaderService)).Should().Be(3, "阅读器族 3（预览设置写 / 预览设置读 / 读后推荐）");

        static int MethodCount(Type type)
            => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;
    }

    /// <summary>
    /// 契约守卫 NV3：参数形态 —— 23 支官方一律「单 JSON 请求体 + 零业务 Query」，
    /// 十支裸应答端点直接返回 <see cref="WxaResponse"/>，十三支返回本域应答 DTO。
    /// </summary>
    [Fact]
    public void NovelEndpoints_ShouldUseSingleBodyAndZeroQuery()
    {
        foreach (var (iface, method, _, route) in Routes)
        {
            var target = iface.GetMethod(method)!;
            var parameters = target.GetParameters();

            // access_token 由接口级 [Token] 注入，不得落成方法参数（会与官方请求示例形态不符）。
            parameters.Should().NotContain(static p => p.GetCustomAttribute<QueryAttribute>() != null,
                $"{route} 本域除 access_token 外无官方 Query 参数，业务参数须走 [Body]");

            parameters.Count(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                .Should().Be(1, $"{route} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
            parameters.Should().NotContain(static p => p.GetCustomAttribute<MultipartFormAttribute>() != null,
                $"{route} 本域官方无 multipart 通路（章节正文走 content 字段内联，不是文件上传）");

            // 每个端点恰有两个参数：[Body] 请求体 + CancellationToken。
            parameters.Should().HaveCount(2, $"{route} 形态为 (request, cancellationToken = default)");
            parameters[1].ParameterType.Should().Be(typeof(CancellationToken), $"{route} 第二参数须为 CancellationToken");
            parameters[1].HasDefaultValue.Should().BeTrue($"{route} 的 CancellationToken 须有 default 缺省值（全仓异步约定）");

            // 裸应答裁决：官方返回体只有 errcode/errmsg 的十支直接返回基类，不得建空壳子类。
            if (BareResponseMethods.Contains(method))
            {
                ResultTypeOf(iface, method).Should().Be(typeof(WxaResponse),
                    $"{route} 官方返回体只有 errcode / errmsg ⇒ 复用基类 WxaResponse（全仓口径，不建空壳应答）");
            }
            else
            {
                ResultTypeOf(iface, method).Namespace.Should().Be(DtoNamespace,
                    $"{route} 有业务应答字段 ⇒ 必须返回本域应答 DTO");
            }
        }

        // 23 支里恰有十支裸应答（写操作与删除），余下十三支各返回一支本域应答 DTO。
        Routes.Count(static r => BareResponseMethods.Contains(r.Method)).Should().Be(10,
            "本域官方返回体只有 errcode/errmsg 的端点为 10（updatebook / deletebook / auditbook / deletechapter / "
            + "reorderchapter / updatechapterseq / delbookauth / delbookauthbyappid / setpreviewsetting / setrecmdnovel）");
        Routes.Count(static r => !BareResponseMethods.Contains(r.Method)).Should().Be(13,
            "其余 13 支返回本域应答 DTO（与 NV5 的应答 DTO 计数 13 互为印证）");

        // 请求体类型与本域 DTO 命名空间一致：不得用命名空间外的自建壳或 Dictionary 兜请求体。
        Routes.Select(static r => BodyTypeOf(r.Interface, r.Method)).Should()
            .OnlyContain(t => t.Namespace == DtoNamespace, "23 支的请求体类型全部落本域 DTO 命名空间");
    }

    /// <summary>
    /// 契约守卫 NV4：官方字段名原文 + 包装形态裁决（四处取证偏差的锁定面）+ 两套分页并存 + 同名不同型。
    /// </summary>
    [Fact]
    public void NovelDataModels_ShouldLockOfficialFieldsAndWrappers()
    {
        // 作品族：官方 createbook / updatebook 请求体一律平铺（与章节族 createchapter 的 chapter 包装相反）。
        AssertJsonProperty<WxaNovelCreateBookRequest>(nameof(WxaNovelCreateBookRequest.CoverMediaId), "cover_media_id",
            "官方原文拼写（封面走临时素材 media_id，不是 url）");
        AssertJsonProperty<WxaNovelCreateBookRequest>(nameof(WxaNovelCreateBookRequest.AwesomeParagraph), "awesome_paragraph", "官方原文拼写");
        typeof(WxaNovelCreateBookRequest).GetProperty("BookId", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("createbook 官方请求体无 book_id（作品 id 由应答给出，创建侧不得自拟）");
        typeof(WxaNovelCreateBookRequest).GetProperty("ChapterIdList", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("createbook 官方请求体无 chapter_id_list（章节只能由章节族端点写入，创建时作品尚不存在）");

        AssertJsonProperty<WxaNovelUpdateBookRequest>(nameof(WxaNovelUpdateBookRequest.ChapterIdList), "chapter_id_list", "官方原文拼写");
        AssertJsonProperty<WxaNovelUpdateBookRequest>(nameof(WxaNovelUpdateBookRequest.NeedVolume), "need_volume", "官方原文拼写（填了才按 volume_list 建分卷）");
        AssertJsonProperty<WxaNovelUpdateBookRequest>(nameof(WxaNovelUpdateBookRequest.UpdateKeyword), "update_keyword", "官方原文拼写");
        AssertJsonProperty<WxaNovelVolumeInfo>(nameof(WxaNovelVolumeInfo.VolumeTitle), "volume_title", "官方原文拼写");
        typeof(WxaNovelUpdateBookRequest).GetProperty("Book", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("updatebook 官方请求体是平铺字段，无 book 包装（勿与 createchapter 的 chapter 包装混记）");

        AssertJsonProperty<WxaNovelListBookRequest>(nameof(WxaNovelListBookRequest.LastId), "last_id", "官方原文拼写");
        AssertJsonProperty<WxaNovelListBookResponse>(nameof(WxaNovelListBookResponse.TotalCnt), "total_cnt", "官方原文拼写（非 totalCount）");
        AssertJsonProperty<WxaNovelBookSummary>(nameof(WxaNovelBookSummary.UploadScene), "upload_scene", "官方原文拼写");
        AssertJsonProperty<WxaNovelBookDetail>(nameof(WxaNovelBookDetail.FirstCategoryName), "first_category_name", "官方原文拼写（仅 getbook 详情侧有名称回显）");
        AssertJsonProperty<WxaNovelAuditInfo>(nameof(WxaNovelAuditInfo.AuditStatus), "audit_status", "官方原文拼写");
        AssertJsonProperty<WxaNovelAuditInfo>(nameof(WxaNovelAuditInfo.Suggestion), "suggestion", "官方原文拼写（审核驳回建议）");

        // 两套分页形态并存：listbook 官方同页给 limit/offset 与 last_id/limit 两种（示例注释「last_id + limit 推荐方式!」）
        // ⇒ 四个键都在册，不得只留一种。
        typeof(WxaNovelListBookRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.Name).Should().BeEquivalentTo(
                new[] { nameof(WxaNovelListBookRequest.Limit), nameof(WxaNovelListBookRequest.Offset),
                        nameof(WxaNovelListBookRequest.LastId), nameof(WxaNovelListBookRequest.NeedEditedData) },
                because: "listbook 官方请求体恰为 limit / offset / last_id / need_edited_data 四键（两种分页方式并存）");

        // 章节族：createchapter 与 batchcreatechapter 同为 chapter 元素类型，但包装形态不同（单对象 vs 数组）。
        AssertJsonProperty<WxaNovelCreateChapterRequest>(nameof(WxaNovelCreateChapterRequest.Chapter), "chapter",
            "官方原文拼写（单章上传是 chapter 对象包装，非平铺）");
        AssertJsonProperty<WxaNovelChapterInfo>(nameof(WxaNovelChapterInfo.ChapterTitle), "chapter_title", "官方原文拼写");
        AssertJsonProperty<WxaNovelBatchCreateChapterRequest>(nameof(WxaNovelBatchCreateChapterRequest.ChapterList), "chapter_list",
            "官方原文拼写（批量侧键名是 chapter_list，元素类型与单章侧同源）");
        AssertJsonProperty<WxaNovelBatchCreateChapterResponse>(nameof(WxaNovelBatchCreateChapterResponse.ChapterIdList), "chapter_id_list",
            "官方应答键为复数 chapter_id_list（NV4 偏差③裁决：早期误写单数 chapter_id，与 createchapter 的单值应答名相混）");
        typeof(WxaNovelBatchCreateChapterResponse).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.Name).Should().BeEquivalentTo(
                new[] { nameof(WxaNovelBatchCreateChapterResponse.ChapterIdList),
                        nameof(WxaNovelBatchCreateChapterResponse.ConflictOriginalIdList) },
                because: "batchcreatechapter 官方应答除 errcode/errmsg 外只有 chapter_id_list 与 conflict_original_id_list（部分成功语义）");
        AssertJsonProperty<WxaNovelCreateChapterResponse>(nameof(WxaNovelCreateChapterResponse.ChapterId), "chapter_id",
            "官方原文拼写（单章应答是单数 chapter_id，与批量侧复数不同源）");
        AssertJsonProperty<WxaNovelReplaceChapterRequest>(nameof(WxaNovelReplaceChapterRequest.NewChapterTitle), "new_chapter_title", "官方原文拼写");
        AssertJsonProperty<WxaNovelReplaceChapterResponse>(nameof(WxaNovelReplaceChapterResponse.NewChapterId), "new_chapter_id",
            "官方原文拼写（替换即生成新章节 id ⇒ 旧 id 作废，两键并存）");
        AssertJsonProperty<WxaNovelChapterDetail>(nameof(WxaNovelChapterDetail.WordCnt), "word_cnt", "官方原文拼写");
        AssertJsonProperty<WxaNovelChapterDetail>(nameof(WxaNovelChapterDetail.BanStatus), "ban_status", "官方原文拼写");
        AssertJsonProperty<WxaNovelReorderChapterRequest>(nameof(WxaNovelReorderChapterRequest.TargetChapterId), "target_chapter_id", "官方原文拼写");
        AssertJsonProperty<WxaNovelUpdateChapterSeqRequest>(nameof(WxaNovelUpdateChapterSeqRequest.ChapterSeqList), "chapter_seq_list", "官方原文拼写");
        AssertJsonProperty<WxaNovelListChapterRequest>(nameof(WxaNovelListChapterRequest.VolumeIndex), "volume_index", "官方原文拼写");

        // 两支排序端点形态不同（相对移动 vs 全量 seq 重排）⇒ 请求体类型不得合并。
        BodyTypeOf(typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.ReorderChapterAsync))
            .Should().Be(typeof(WxaNovelReorderChapterRequest));
        BodyTypeOf(typeof(IWxaNovelChapterService), nameof(IWxaNovelChapterService.UpdateChapterSeqAsync))
            .Should().Be(typeof(WxaNovelUpdateChapterSeqRequest),
            "reorderchapter 是 target_chapter_id + operation 的单章相对移动，updatechapterseq 是 chapter_seq_list 的全量重排 ⇒ 两类不共用");

        // 授权族：NV4 偏差①裁决 —— addbookauth 官方请求体是 books 数组包装。
        AssertJsonProperty<WxaNovelAddBookAuthRequest>(nameof(WxaNovelAddBookAuthRequest.Books), "books",
            "官方原文拼写（addbookauth 请求体为 {\"books\":[…]}，单批上限 20 条）");
        typeof(WxaNovelAddBookAuthRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().ContainSingle(static p => p.Name == nameof(WxaNovelAddBookAuthRequest.Books),
            "books 是 addbookauth 请求体的唯一键（早期扁平建模把 item 字段提到顶层，与官方示例不符，已改回包装形态）");
        AssertJsonProperty<WxaNovelBookAuthInfo>(nameof(WxaNovelBookAuthInfo.GranteeAppId), "grantee_appid", "官方原文拼写");
        AssertJsonProperty<WxaNovelBookAuthInfo>(nameof(WxaNovelBookAuthInfo.ExpireTime), "expire_time", "官方原文拼写（unix 秒，CST 时区）");

        // 同名 results、不同元素类型：写侧逐项结果是 {errcode, errmsg}，查侧结果是 {book_id, grantor…, sum}。
        AssertJsonProperty<WxaNovelAddBookAuthResponse>(nameof(WxaNovelAddBookAuthResponse.Results), "results", "官方原文拼写");
        AssertJsonProperty<WxaNovelQueryBookAuthResponse>(nameof(WxaNovelQueryBookAuthResponse.Results), "results", "官方原文拼写");
        AssertPropertyType<WxaNovelAddBookAuthResponse>(nameof(WxaNovelAddBookAuthResponse.Results), typeof(List<WxaNovelAuthResult>),
            "addbookauth 的 results 元素是逐条结果码（{errcode, errmsg}）");
        AssertPropertyType<WxaNovelQueryBookAuthResponse>(nameof(WxaNovelQueryBookAuthResponse.Results), typeof(List<WxaNovelAuthResultItem>),
            "querybookauth 的 results 元素是授权关系条目 ⇒ 与写侧同名不同型，两套元素类不得合并");
        AssertJsonProperty<WxaNovelAuthResult>(nameof(WxaNovelAuthResult.ErrorCode), "errcode",
            "官方原文拼写（逐条内层结果码，与外层 WxaResponse.errcode 不同层级）");
        AssertJsonProperty<WxaNovelAuthResultItem>(nameof(WxaNovelAuthResultItem.GrantorAppId), "grantor_appid", "官方原文拼写");
        AssertJsonProperty<WxaNovelAuthResultItem>(nameof(WxaNovelAuthResultItem.Sum), "sum", "官方原文拼写（is_sum=true 才回填）");

        // 两套分页形态在本域内三处各不同：querybookauth = offset/count，querybookauthv2 = cursor/count，listbook = limit/offset|last_id。
        AssertJsonProperty<WxaNovelQueryBookAuthRequest>(nameof(WxaNovelQueryBookAuthRequest.IsSum), "is_sum", "官方原文拼写");
        typeof(WxaNovelQueryBookAuthRequest).GetProperty("Cursor", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("querybookauth 官方无 cursor（游标分页只属于 v2）");
        AssertJsonProperty<WxaNovelQueryBookAuthV2Request>(nameof(WxaNovelQueryBookAuthV2Request.Cursor), "cursor", "官方原文拼写");
        AssertJsonProperty<WxaNovelQueryBookAuthV2Request>(nameof(WxaNovelQueryBookAuthV2Request.BookIds), "book_ids", "官方原文拼写");
        typeof(WxaNovelQueryBookAuthV2Request).GetProperty("Offset", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("querybookauthv2 官方无 offset（游标页与偏移页不得互抄）");
        AssertJsonProperty<WxaNovelQueryBookAuthV2Response>(nameof(WxaNovelQueryBookAuthV2Response.NextCursor), "next_cursor", "官方原文拼写");
        AssertJsonProperty<WxaNovelQueryBookAuthV2Response>(nameof(WxaNovelQueryBookAuthV2Response.AppIdResults), "appid_results", "官方原文拼写");

        // 账号授权族：infos 包装 + delbookauthbyappid 官方只有 grantee_appid 一支键（NV4 偏差④裁决）。
        AssertJsonProperty<WxaNovelAddBookAuthByAppIdRequest>(nameof(WxaNovelAddBookAuthByAppIdRequest.Infos), "infos",
            "官方原文拼写（addbookauthbyappid 请求体为 {\"infos\":[…]}，单批上限 20 条）");
        typeof(WxaNovelAddBookAuthByAppIdRequest).GetProperty("Books", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("addbookauthbyappid 官方包装名是 infos 而非 books（两视角请求体不同名，勿互相抄）");
        typeof(WxaNovelDelBookAuthByAppIdRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.Name).Should().BeEquivalentTo(
                new[] { nameof(WxaNovelDelBookAuthByAppIdRequest.GranteeAppId) },
                because: "delbookauthbyappid 官方请求体只有 grantee_appid（断开整个账号维度授权，无 book_id 侧键）");

        // 同族对照：delbookauth（作品维度）官方要求 book_id + grantee_appid 两键皆必填 ⇒ 与 byappid 侧形态不同。
        typeof(WxaNovelDelBookAuthRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.Name).Should().BeEquivalentTo(
                new[] { nameof(WxaNovelDelBookAuthRequest.BookId), nameof(WxaNovelDelBookAuthRequest.GranteeAppId) },
                because: "delbookauth 是「作品 + 被授权账号」双键定位的单条删除");

        // 阅读器族：NV4 偏差②裁决 —— setpreviewsetting 官方请求体是 setting 嵌套单对象，与 getpreviewsetting 应答同形。
        AssertJsonProperty<WxaNovelSetPreviewSettingRequest>(nameof(WxaNovelSetPreviewSettingRequest.Setting), "setting",
            "官方原文拼写（请求体为 {\"setting\":{…}}，读写共用 WxaNovelPreviewSetting 同形）");
        typeof(WxaNovelSetPreviewSettingRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().ContainSingle(static p => p.Name == nameof(WxaNovelSetPreviewSettingRequest.Setting),
            "setting 是 setpreviewsetting 请求体唯一键（早期扁平建模把 book_id / default_words / chapter_index / words 提到顶层，与官方示例不符）");
        AssertJsonProperty<WxaNovelPreviewSetting>(nameof(WxaNovelPreviewSetting.DefaultWords), "default_words", "官方原文拼写");
        AssertJsonProperty<WxaNovelPreviewSetting>(nameof(WxaNovelPreviewSetting.ChapterSetting), "chapter_setting", "官方原文拼写");
        AssertJsonProperty<WxaNovelChapterSetting>(nameof(WxaNovelChapterSetting.ChapterIndex), "chapter_index", "官方原文拼写（从 0 开始）");
        AssertJsonProperty<WxaNovelGetPreviewSettingResponse>(nameof(WxaNovelGetPreviewSettingResponse.Setting), "setting", "官方原文拼写");
        AssertJsonProperty<WxaNovelSetRecmdNovelRequest>(nameof(WxaNovelSetRecmdNovelRequest.RecmdType), "recmd_type",
            "官方原文拼写（1=android 付费小说，2=ios 全文免费小说）");
        AssertJsonProperty<WxaNovelSetRecmdNovelRequest>(nameof(WxaNovelSetRecmdNovelRequest.BookIdList), "book_id_list",
            "官方原文拼写（上限 300 条，空数组即清空该槽）");

        // 类型口径：官方的布尔开关落 bool?、计数与时间戳落 long?、id 列表落 List<string>?（不得数值化 id、不得把开关做成 string）。
        typeof(WxaNovelQueryBookAuthRequest).GetProperty(nameof(WxaNovelQueryBookAuthRequest.IsSum))!
            .PropertyType.Should().Be(typeof(bool?), "官方 is_sum 类型列为 boolean ⇒ 建为可空布尔");
        typeof(WxaNovelListBookRequest).GetProperty(nameof(WxaNovelListBookRequest.NeedEditedData))!
            .PropertyType.Should().Be(typeof(bool?), "官方 need_edited_data 类型列为 boolean");
        typeof(WxaNovelQueryBookAuthV2Request).GetProperty(nameof(WxaNovelQueryBookAuthV2Request.Count))!
            .PropertyType.Should().Be(typeof(long?), "官方 count 类型列为 number（单页上限 100）");
        typeof(WxaNovelBookAuthInfo).GetProperty(nameof(WxaNovelBookAuthInfo.ExpireTime))!
            .PropertyType.Should().Be(typeof(long?), "官方 expire_time 为 unix 秒时间戳（number）⇒ 不得建为 DateTimeOffset 改变线格式");
        typeof(WxaNovelQueryBookAuthV2Request).GetProperty(nameof(WxaNovelQueryBookAuthV2Request.BookIds))!
            .PropertyType.Should().Be(typeof(List<string>), "官方 book_ids 为字符串数组（作品 id 非定长，不得数值化）");
    }

    /// <summary>契约守卫 NV5：50 支 DTO 全量登记进 NovelJsonContext；命名空间清单锁死（零常量类）；应答 DTO 与端点返回类型一一对应。</summary>
    [Fact]
    public void NovelDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaNovelBookDetail).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("小说域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 50 DTO = 作品 13 + 章节 16 + 账号-小说授权 8 + 账号-账号授权 7 + 阅读器 6
        //（按官方逐页请求体 / 应答 / 嵌套子对象建模；2026-10-16 取证同批：删四支无人返回的空壳应答、
        //  addbookauth 补 books 元素类型、setpreviewsetting 改为复用 setting 同形类型）。
        dtoTypes.Should().HaveCount(50,
            "小说域 DTO 总数漂移须先核对官方文档再同批调整本守卫（book 13 + chapter 16 + auth 8 + accountauth 7 + reader 6 = 50）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Novel");
        }

        var contextType = assembly.GetType(DtoNamespace + ".NovelJsonContext");
        contextType.Should().NotBeNull(
            "NovelJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（后者必须显式带 -TargetProject / -OutputDir 指向本线，缺省只重生成企微线）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"Novel\")]，"
                + "必须登记进 NovelJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // 命名空间清单锁死：本域无常量类（官方取值全部写在 DTO 属性注释里），除 50 DTO 与生成上下文外不得出现新类型。
        var unregistered = domainTypes.Where(t => !dtoTypes.Contains(t) && t != contextType).ToList();
        unregistered.Should().BeEmpty(
            "小说域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 常量类或新 DTO 须同批更新本守卫");

        // 空壳应答不得回潮：官方只回 errcode/errmsg 的十支端点复用基类，四支早期同形空壳已删。
        var deletedEmptyResponses = new[]
        {
            "WxaNovelDelBookAuthResponse", "WxaNovelDelBookAuthByAppIdResponse",
            "WxaNovelSetPreviewSettingResponse", "WxaNovelSetRecmdNovelResponse",
        };
        dtoTypes.Should().NotContain(t => deletedEmptyResponses.Contains(t.Name),
            "裸应答端点不得建空壳 WxaResponse 子类（十支端点一律返回基类 WxaResponse，全仓同口径）");

        var responseDtos = domainTypes
            .Where(static t => t.IsClass && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToList();
        responseDtos.Should().HaveCount(13,
            "本域官方带业务字段的应答对象为 13（book 3 + chapter 5 + auth 2 + accountauth 2 + reader 1）");
        responseDtos.Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
            "小说域响应 DTO 必须继承 WxaResponse（errcode 判错与全线同源）");

        var endpointReturnTypes = Routes
            .Select(static r => ResultTypeOf(r.Interface, r.Method))
            .Where(static t => t.Namespace == DtoNamespace)
            .Distinct()
            .ToList();

        endpointReturnTypes.Should().BeEquivalentTo(responseDtos,
            "本域应答类型必须与端点返回类型一一对应（既不得留下无人返回的孤儿应答，也不得返回命名空间外的自建应答）");

        Routes.Count(static r => ResultTypeOf(r.Interface, r.Method) == typeof(WxaResponse))
            .Should().Be(10, "十支裸应答端点直接返回 WxaResponse（与 NV3 的 BareResponseMethods 同一清单）");
    }

    // ------------------------------------------------------------------
    // 内部辅助
    // ------------------------------------------------------------------

    /// <summary>取端点方法的应答类型（<c>Task&lt;T&gt;</c> 解包为 <c>T</c>），守卫比对的是应答对象本身而非 Task 壳。</summary>
    private static Type ResultTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        method.Should().NotBeNull($"{interfaceType.Name}.{methodName} 不存在（契约面漂移）");

        var returnType = method!.ReturnType;
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetGenericArguments()[0]
            : returnType;
    }

    /// <summary>取端点方法 <c>[Body]</c> 参数的声明类型。</summary>
    private static Type BodyTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var body = method!.GetParameters().Single(static p => p.GetCustomAttribute<BodyAttribute>() != null);
        return body.ParameterType;
    }

    /// <summary>
    /// 扫描 <c>Src/MiniProgram/Mud.Wechat.MiniProgram/Interfaces/Novel/</c> 剥离注释后的路由字面量集合
    /// （形态同 RD1 / NC3 的声明面自证：必须先证扫得出在册路由，零声明与全集断言才不是空转）。
    /// </summary>
    private static HashSet<string> DeclaredRoutesInNovelInterfaces()
    {
        var root = Path.Combine(GetSolutionRoot(), "Src", "MiniProgram", "Mud.Wechat.MiniProgram", "Interfaces", "Novel");
        Directory.Exists(root).Should().BeTrue($"扫描根必须存在（{root}）——目录漂移会让 NV1 的声明面自证静默恒真");

        var literalRegex = new Regex(@"""(/[A-Za-z0-9_\-./]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var set = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var code = string.Join("\n", Regex.Replace(File.ReadAllText(file), @"/\*[\s\S]*?\*/", " ")
                .Split('\n')
                .Where(static l => !l.TrimStart().StartsWith("//", StringComparison.Ordinal)));

            foreach (System.Text.RegularExpressions.Match m in literalRegex.Matches(code))
            {
                set.Add(m.Groups[1].Value);
            }
        }

        // 正对照：扫出的集合必须覆盖路由表全集，否则是扫描口径失效（而非「本域真的没有多余路由」）。
        foreach (var control in new[] { "/wxa/book/createbook", "/wxa/business/novelreader/setpreviewsetting" })
        {
            set.Should().Contain(control, $"NV1 的声明面扫描必须能捕获本域在册路由（{control}）——否则全集断言静默恒真");
        }

        return set;
    }

    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    private static void AssertPropertyType<T>(string propertyName, Type expectedType, string because = "")
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");
        property!.PropertyType.Should().Be(expectedType,
            $"{typeof(T).Name}.{propertyName} 的类型必须是 {expectedType.Name}（共用/元素类型裁决{(because.Length > 0 ? "：" + because : string.Empty)}）");
    }
}
