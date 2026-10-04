// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 文档模块（Wedoc 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定
/// （管理文档族 / 管理文档内容族 / 管理表格内容族三族均为三类应用公共面收敛父接口 + 空标记子接口；
/// 编辑文档内容与编辑表格内容为批量更新形态，单次操作数量官方分别限制 30 与 5；
/// 官方文档存在 verison / blod 拼写陷阱，字段名照抄官方原文，守卫锁定防「顺手修正」）。
/// </summary>
public class WechatWedocContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string WedocParentImplementationClassName = "WechatWorkWedocService";

    private const string WedocDocumentParentImplementationClassName = "WechatWorkWedocDocumentService";

    private const string WedocSpreadsheetParentImplementationClassName = "WechatWorkWedocSpreadsheetService";

    private const string WedocRegistryGroupName = "Wedoc";

    /// <summary>
    /// 管理文档族官方路由表（父接口 5 条端点，官方即 POST，勿改 GET）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] WedocRoutes =
    {
        // 新建文档（自建 97460、第三方 97464、代开发 97470）：docid 仅创建时返回，需妥善保存。
        (typeof(IWechatWorkWedocService),
            nameof(IWechatWorkWedocService.CreateDocumentAsync), "/cgi-bin/wedoc/create_doc"),
        // 重命名文档（自建 97736、第三方 97745、代开发 97740）：docid/formid 二选一，仅可修改应用自建文档。
        (typeof(IWechatWorkWedocService),
            nameof(IWechatWorkWedocService.RenameDocumentAsync), "/cgi-bin/wedoc/rename_doc"),
        // 删除文档（自建 97735、第三方 97746、代开发 97742）：路由为 del_doc（官方原文），勿改 delete_doc。
        (typeof(IWechatWorkWedocService),
            nameof(IWechatWorkWedocService.DeleteDocumentAsync), "/cgi-bin/wedoc/del_doc"),
        // 获取文档基础信息（自建 97734、第三方 97747、代开发 97743）。
        (typeof(IWechatWorkWedocService),
            nameof(IWechatWorkWedocService.GetDocumentBaseInfoAsync), "/cgi-bin/wedoc/get_doc_base_info"),
        // 分享文档（自建 97733、第三方 97748、代开发 97744）：路由为 doc_share，仅可分享应用自建文档。
        (typeof(IWechatWorkWedocService),
            nameof(IWechatWorkWedocService.ShareDocumentAsync), "/cgi-bin/wedoc/doc_share"),
    };

    /// <summary>
    /// 管理文档内容族官方路由表（父接口 2 条端点，官方即 POST，勿改 GET）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] WedocDocumentRoutes =
    {
        // 编辑文档内容（自建 97626、第三方 98027、代开发 98034）：单次批量更新操作数量 <= 30。
        (typeof(IWechatWorkWedocDocumentService),
            nameof(IWechatWorkWedocDocumentService.BatchUpdateDocumentAsync), "/cgi-bin/wedoc/document/batch_update"),
        // 获取文档数据（自建 101161、第三方 101188、代开发 101170）：返回文档 Node 树，编辑前须先获取节点位置。
        (typeof(IWechatWorkWedocDocumentService),
            nameof(IWechatWorkWedocDocumentService.GetDocumentDataAsync), "/cgi-bin/wedoc/document/get"),
    };

    /// <summary>
    /// 管理表格内容族官方路由表（父接口 3 条端点，官方即 POST，勿改 GET）。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] WedocSpreadsheetRoutes =
    {
        // 编辑表格内容（自建 101168、第三方 101190、代开发 101169）：单次批量更新操作数量 <= 5。
        (typeof(IWechatWorkWedocSpreadsheetService),
            nameof(IWechatWorkWedocSpreadsheetService.BatchUpdateSpreadsheetAsync), "/cgi-bin/wedoc/spreadsheet/batch_update"),
        // 获取表格数据（自建 97711、第三方 98031、代开发 98038）：range 遵循 A1 表示法。
        (typeof(IWechatWorkWedocSpreadsheetService),
            nameof(IWechatWorkWedocSpreadsheetService.GetSpreadsheetDataAsync), "/cgi-bin/wedoc/spreadsheet/get_sheet_range_data"),
        // 获取表格行列信息（自建 97661、第三方 98030、代开发 98037）。
        (typeof(IWechatWorkWedocSpreadsheetService),
            nameof(IWechatWorkWedocSpreadsheetService.GetSpreadsheetPropertiesAsync), "/cgi-bin/wedoc/spreadsheet/get_sheet_properties"),
    };

    /// <summary>
    /// 契约守卫 WD1：管理文档族全部端点路由必须与官方契约一致——
    /// 5 个端点为三类应用公共面，全部收敛父接口；del_doc / doc_share 为官方原文路由，勿「顺手改名」。
    /// </summary>
    [Fact]
    public void WedocEndpoints_ShouldMatchOfficialRoutes()
    {
        WedocRoutes.Should().HaveCount(5,
            "管理文档族 5 个端点（新建/重命名/删除/基础信息/分享）为三类应用公共面，全部收敛父接口");
        WedocRoutes.Select(r => r.Route).Distinct().Should().HaveCount(5, "各端点路由互不重复");

        AssertRoutes(WedocRoutes);
    }

    /// <summary>
    /// 契约守卫 WD2：管理文档内容族全部端点路由必须与官方契约一致——
    /// 2 个端点为三类应用公共面；batch_update 为批量更新形态（单次操作数量 &lt;= 30）。
    /// </summary>
    [Fact]
    public void WedocDocumentEndpoints_ShouldMatchOfficialRoutes()
    {
        WedocDocumentRoutes.Should().HaveCount(2,
            "管理文档内容族 2 个端点（编辑文档内容/获取文档数据）为三类应用公共面，全部收敛父接口");
        WedocDocumentRoutes.Select(r => r.Route).Distinct().Should().HaveCount(2, "各端点路由互不重复");

        AssertRoutes(WedocDocumentRoutes);
    }

    /// <summary>
    /// 契约守卫 WD3：管理表格内容族全部端点路由必须与官方契约一致——
    /// 3 个端点为三类应用公共面；batch_update 为批量更新形态（单次操作数量 &lt;= 5，区别于文档内容的 30，勿混用）。
    /// </summary>
    [Fact]
    public void WedocSpreadsheetEndpoints_ShouldMatchOfficialRoutes()
    {
        WedocSpreadsheetRoutes.Should().HaveCount(3,
            "管理表格内容族 3 个端点（编辑表格内容/获取表格数据/获取表格行列信息）为三类应用公共面，全部收敛父接口");
        WedocSpreadsheetRoutes.Select(r => r.Route)
            .Should().OnlyContain(r => r.StartsWith("/cgi-bin/wedoc/spreadsheet/"),
                "管理表格内容族路由必须落在 wedoc/spreadsheet/ 下，不得与文档内容域 document/ 混用");

        AssertRoutes(WedocSpreadsheetRoutes);
    }

    /// <summary>
    /// 契约守卫 WD4：文档模块接口层级与生成器注册形态——三族公共端点收敛于 IsAbstract 父接口、
    /// 三个应用类型子接口均为空标记（能力漂移守卫：子接口自身声明端点数必须为 0）。
    /// </summary>
    [Fact]
    public void WedocInterfaceHierarchy_ShouldConvergeOnAbstractParentWithWedocRegistry()
    {
        // —— 管理文档族 ——
        var wedocChildren = new[]
        {
            typeof(IWechatWorkInternalWedocService),
            typeof(IWechatWorkThirdPartyWedocService),
            typeof(IWechatWorkProviderWedocService),
        };

        foreach (var child in wedocChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkWedocService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkWedocService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkWedocService),
            WedocParentImplementationClassName,
            expectedDeclaredMethods: 5,
            new[] { (typeof(IWechatWorkInternalWedocService), 0),
                    (typeof(IWechatWorkThirdPartyWedocService), 0),
                    (typeof(IWechatWorkProviderWedocService), 0) });

        // —— 管理文档内容族 ——
        var documentChildren = new[]
        {
            typeof(IWechatWorkInternalWedocDocumentService),
            typeof(IWechatWorkThirdPartyWedocDocumentService),
            typeof(IWechatWorkProviderWedocDocumentService),
        };

        foreach (var child in documentChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkWedocDocumentService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkWedocDocumentService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkWedocDocumentService),
            WedocDocumentParentImplementationClassName,
            expectedDeclaredMethods: 2,
            new[] { (typeof(IWechatWorkInternalWedocDocumentService), 0),
                    (typeof(IWechatWorkThirdPartyWedocDocumentService), 0),
                    (typeof(IWechatWorkProviderWedocDocumentService), 0) });

        // —— 管理表格内容族 ——
        var spreadsheetChildren = new[]
        {
            typeof(IWechatWorkInternalWedocSpreadsheetService),
            typeof(IWechatWorkThirdPartyWedocSpreadsheetService),
            typeof(IWechatWorkProviderWedocSpreadsheetService),
        };

        foreach (var child in spreadsheetChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkWedocSpreadsheetService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkWedocSpreadsheetService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkWedocSpreadsheetService),
            WedocSpreadsheetParentImplementationClassName,
            expectedDeclaredMethods: 3,
            new[] { (typeof(IWechatWorkInternalWedocSpreadsheetService), 0),
                    (typeof(IWechatWorkThirdPartyWedocSpreadsheetService), 0),
                    (typeof(IWechatWorkProviderWedocSpreadsheetService), 0) });
    }

    /// <summary>
    /// 契约守卫 WD5：令牌绑定——文档模块全部 12 个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；第三方/代开发消费授权企业级令牌，scope = authCorpId）。
    /// </summary>
    [Fact]
    public void WedocTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkWedocService),
            typeof(IWechatWorkInternalWedocService),
            typeof(IWechatWorkThirdPartyWedocService),
            typeof(IWechatWorkProviderWedocService),
            typeof(IWechatWorkWedocDocumentService),
            typeof(IWechatWorkInternalWedocDocumentService),
            typeof(IWechatWorkThirdPartyWedocDocumentService),
            typeof(IWechatWorkProviderWedocDocumentService),
            typeof(IWechatWorkWedocSpreadsheetService),
            typeof(IWechatWorkInternalWedocSpreadsheetService),
            typeof(IWechatWorkThirdPartyWedocSpreadsheetService),
            typeof(IWechatWorkProviderWedocSpreadsheetService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 WD6：官方拼写陷阱锁定——编辑文档内容的顶层版本字段官方参数表作 <c>version</c>、
    /// 官方 JSON 示例作 <c>verison</c>，本仓以官方示例为准（同 universal_domian / cusor 口径）；
    /// 文本属性的加粗字段官方参数表原文作 <c>blod</c>（疑为 bold 笔误）。两者均照抄官方原文，
    /// 守卫锁定防「顺手修正」破坏线上契约。
    /// </summary>
    [Fact]
    public void WedocOfficialSpellingTraps_ShouldBePreservedVerbatim()
    {
        var verison = typeof(BatchUpdateWedocDocumentRequest).GetProperty(
            nameof(BatchUpdateWedocDocumentRequest.Verison),
            BindingFlags.Public | BindingFlags.Instance);
        verison.Should().NotBeNull("编辑文档内容请求必须承载官方版本字段");
        verison!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("verison",
            "官方 JSON 请求示例原文为 verison（参数表作 version，以示例为准），不得改写为 version");

        var blod = typeof(WedocDocumentTextProperty).GetProperty(
            nameof(WedocDocumentTextProperty.Blod),
            BindingFlags.Public | BindingFlags.Instance);
        blod.Should().NotBeNull("文本属性必须承载官方加粗字段");
        blod!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("blod",
            "官方参数表原文为 blod（疑为 bold 笔误），照抄官方原文，不得改写为 bold");

        // RunProperty 的加粗字段为正常拼写 bold（获取文档数据 Node 树），二者不可混淆。
        var bold = typeof(WedocDocumentRunProperty).GetProperty(
            nameof(WedocDocumentRunProperty.Bold),
            BindingFlags.Public | BindingFlags.Instance);
        bold.Should().NotBeNull("文档节点文本属性必须承载官方加粗字段");
        bold!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("bold",
            "获取文档数据 Node 树官方参数表原文为 bold");
    }

    /// <summary>
    /// 契约守卫 WD7：文档模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 81 个契约面类型）。
    /// </summary>
    [Fact]
    public void WedocDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = WedocJsonContext.Default;

        var requiredTypes = new Type[]
        {
            // 管理文档族。
            typeof(CreateWedocDocumentRequest), typeof(CreateWedocDocumentResponse),
            typeof(RenameWedocDocumentRequest), typeof(DeleteWedocDocumentRequest),
            typeof(GetWedocDocumentBaseInfoRequest), typeof(GetWedocDocumentBaseInfoResponse),
            typeof(WedocDocBaseInfo),
            typeof(ShareWedocDocumentRequest), typeof(ShareWedocDocumentResponse),
            // 管理文档内容族——端点请求/响应。
            typeof(BatchUpdateWedocDocumentRequest), typeof(GetWedocDocumentDataRequest),
            typeof(GetWedocDocumentDataResponse),
            // 管理文档内容族——批量更新操作。
            typeof(WedocDocumentUpdateOperation),
            typeof(WedocDocumentReplaceText), typeof(WedocDocumentRange), typeof(WedocDocumentLocation),
            typeof(WedocDocumentInsertText), typeof(WedocDocumentDeleteContent),
            typeof(WedocDocumentInsertImage), typeof(WedocDocumentInsertPageBreak),
            typeof(WedocDocumentInsertTable), typeof(WedocDocumentInsertParagraph),
            typeof(WedocDocumentTextProperty), typeof(WedocDocumentUpdateTextProperty),
            // 管理文档内容族——文档 Node 树（递归）。
            typeof(WedocDocumentNode), typeof(WedocDocumentProperty),
            typeof(WedocDocumentSectionProperty), typeof(WedocDocumentPageSize), typeof(WedocDocumentPageMargins),
            typeof(WedocDocumentParagraphProperty), typeof(WedocDocumentNumberProperty),
            typeof(WedocDocumentSpacing), typeof(WedocDocumentIndent),
            typeof(WedocDocumentRunProperty), typeof(WedocDocumentShading),
            typeof(WedocDocumentTableProperty), typeof(WedocDocumentTableWidth),
            typeof(WedocDocumentTableRowProperty), typeof(WedocDocumentTableCellProperty),
            typeof(WedocDocumentBorders), typeof(WedocDocumentBorderProperty),
            typeof(WedocDocumentDrawingProperty), typeof(WedocDocumentInline),
            typeof(WedocDocumentInlinePicture), typeof(WedocDocumentRelativeRect),
            typeof(WedocDocumentShapeProperties), typeof(WedocDocumentTransform2D),
            typeof(WedocDocumentPositiveSize2D), typeof(WedocDocumentInlineAddon),
            typeof(WedocDocumentAnchor), typeof(WedocDocumentAnchorPicture),
            typeof(WedocDocumentPositionHorizontal), typeof(WedocDocumentPositionVertical),
            typeof(WedocDocumentWrapSquare),
            // 管理表格内容族——编辑表格内容。
            typeof(BatchUpdateWedocSpreadsheetRequest), typeof(WedocSpreadsheetUpdateOperation),
            typeof(WedocSpreadsheetAddSheetRequest), typeof(WedocSpreadsheetDeleteSheetRequest),
            typeof(WedocSpreadsheetUpdateRangeRequest), typeof(WedocSpreadsheetDeleteDimensionRequest),
            typeof(BatchUpdateWedocSpreadsheetResponse), typeof(WedocSpreadsheetBatchUpdateData),
            typeof(WedocSpreadsheetUpdateResponse), typeof(WedocSpreadsheetAddSheetResponse),
            typeof(WedocSpreadsheetDeleteSheetResponse), typeof(WedocSpreadsheetUpdateRangeResponse),
            typeof(WedocSpreadsheetDeleteDimensionResponse), typeof(WedocSpreadsheetProperties),
            // 管理表格内容族——获取表格数据/行列信息。
            typeof(GetWedocSpreadsheetDataRequest), typeof(GetWedocSpreadsheetDataResponse),
            typeof(WedocSpreadsheetData),
            typeof(GetWedocSpreadsheetPropertiesRequest), typeof(GetWedocSpreadsheetPropertiesResponse),
            // 管理表格内容族——GridData 共享结构。
            typeof(WedocGridData), typeof(WedocRowData), typeof(WedocCellData),
            typeof(WedocCellValue), typeof(WedocLink), typeof(WedocCellFormat),
            typeof(WedocTextFormat), typeof(WedocColor),
        };

        requiredTypes.Should().HaveCount(81, "文档模块契约面类型总数漂移须先核对 DTO 落位再同批调整本守卫");

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是文档模块契约面类型，必须登记进 WedocJsonContext（AOT 源生成）");
        }
    }

    /// <summary>路由表断言：方法必须存在、必须声明 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, string Route)[] routes)
    {
        foreach (var (iface, method, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute<HttpMethodAttribute>();
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 HTTP 方法特性");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 接口层级断言：父接口 IsAbstract 且不进注册组；子接口挂 Wedoc 注册组、继承父实现类；
    /// 各子接口的自身声明端点数必须与官方开放面一致。
    /// </summary>
    private static void AssertFamilyHierarchy(
        Type parent,
        string parentImplementationClassName,
        int? expectedDeclaredMethods,
        (Type Child, int DeclaredMethods)[] children)
    {
        if (expectedDeclaredMethods.HasValue)
        {
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(expectedDeclaredMethods.Value, $"{parent.Name} 公共端点数漂移");
        }

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var (child, declaredMethods) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(WedocRegistryGroupName,
                $"{child.Name} 必须挂 {WedocRegistryGroupName} 注册组" +
                $"（Wedoc 模块共用 Add{WedocRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(declaredMethods,
                    $"{child.Name} 自身声明端点数必须与官方开放面一致（能力漂移须先核对官方文档再同批调整守卫）");
        }
    }
}
