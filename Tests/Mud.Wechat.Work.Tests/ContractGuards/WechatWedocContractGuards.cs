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
/// （管理文档族 / 管理文档内容族 / 管理表格内容族 / 管理智能表格内容族四族均为三类应用公共面收敛父接口 + 空标记子接口；
/// 编辑文档内容与编辑表格内容为批量更新形态，单次操作数量官方分别限制 30 与 5；
/// 管理智能表格内容族为 20 个端点（子表 4 / 视图 4 / 字段 4 / 记录 4 / 编组 4），单表上限与批量建议官方另有约束；
/// 官方文档存在 verison / blod / property_ganttobect / typ / filed_id 等拼写陷阱，
/// 字段名照抄官方原文，守卫锁定防「顺手修正」）。
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

    private const string WedocSmartSheetParentImplementationClassName = "WechatWorkWedocSmartSheetService";

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
    /// 管理智能表格内容族官方路由表（父接口 20 条端点，官方即 POST，勿改 GET）。
    /// 按官方菜单结构分五组：子表 4 / 视图 4 / 字段 4 / 记录 4 / 编组 4。
    /// </summary>
    private static readonly (Type Interface, string Method, string Route)[] WedocSmartSheetRoutes =
    {
        // —— 子表 ——
        // 添加子表（自建 99896、第三方 100196、代开发 100214）：新建智能表不含视图/记录/字段。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.AddSheetAsync), "/cgi-bin/wedoc/smartsheet/add_sheet"),
        // 删除子表（自建 99899、第三方 100197、代开发 100215）。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.DeleteSheetAsync), "/cgi-bin/wedoc/smartsheet/delete_sheet"),
        // 更新子表（自建 99898、第三方 100198、代开发 100216）。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.UpdateSheetAsync), "/cgi-bin/wedoc/smartsheet/update_sheet"),
        // 查询子表（自建 101154、第三方 101182、代开发 101164）。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.GetSheetAsync), "/cgi-bin/wedoc/smartsheet/get_sheet"),
        // —— 视图（单表最多 200 个视图）——
        // 添加视图（自建 99900、第三方 100199、代开发 100217）：请求字段官方拼写为 property_ganttobect。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.AddViewAsync), "/cgi-bin/wedoc/smartsheet/add_view"),
        // 删除视图（自建 99901、第三方 100200、代开发 100218）：路由官方为复数 delete_views。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.DeleteViewsAsync), "/cgi-bin/wedoc/smartsheet/delete_views"),
        // 更新视图（自建 99902、第三方 100201、代开发 100219）。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.UpdateViewAsync), "/cgi-bin/wedoc/smartsheet/update_view"),
        // 查询视图（自建 101155、第三方 101183、代开发 101165）：路由官方为复数 get_views。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.GetViewsAsync), "/cgi-bin/wedoc/smartsheet/get_views"),
        // —— 字段（单表最多 150 个字段）——
        // 添加字段（自建 99904、第三方 100202、代开发 100220）：路由官方为复数 add_fields。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.AddFieldsAsync), "/cgi-bin/wedoc/smartsheet/add_fields"),
        // 删除字段（自建 99905、第三方 100203、代开发 100221）：路由官方为复数 delete_fields。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.DeleteFieldsAsync), "/cgi-bin/wedoc/smartsheet/delete_fields"),
        // 更新字段（自建 99906、第三方 100204、代开发 100222）：路由官方为复数 update_fields，仅可改字段名与字段属性。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.UpdateFieldsAsync), "/cgi-bin/wedoc/smartsheet/update_fields"),
        // 查询字段（自建 101157、第三方 100223、代开发 101166）：路由官方为复数 get_fields。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.GetFieldsAsync), "/cgi-bin/wedoc/smartsheet/get_fields"),
        // —— 记录（单表最多 100000 行 / 15000000 个单元格；增删改单次建议 500 行内）——
        // 添加记录（自建 99907、第三方 101184、代开发 100224）：路由官方为复数 add_records。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.AddRecordsAsync), "/cgi-bin/wedoc/smartsheet/add_records"),
        // 删除记录（自建 99908、第三方 100206、代开发 100225）：路由官方为复数 delete_records。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.DeleteRecordsAsync), "/cgi-bin/wedoc/smartsheet/delete_records"),
        // 更新记录（自建 99909、第三方 100207、代开发 100226）：路由官方为复数 update_records。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.UpdateRecordsAsync), "/cgi-bin/wedoc/smartsheet/update_records"),
        // 查询记录（自建 101158、第三方 101185、代开发 101167）：路由官方为复数 get_records。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.GetRecordsAsync), "/cgi-bin/wedoc/smartsheet/get_records"),
        // —— 编组（单表最多 150 个编组，每编组最多 150 个字段，字段只能同时存在于一个编组）——
        // 添加编组（自建 101100、第三方 101178、代开发 101174）：路由官方为单数 add_field_group。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.AddFieldGroupAsync), "/cgi-bin/wedoc/smartsheet/add_field_group"),
        // 删除编组（自建 101102、第三方 101179、代开发 101175）：路由官方为复数 delete_field_groups。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.DeleteFieldGroupsAsync), "/cgi-bin/wedoc/smartsheet/delete_field_groups"),
        // 更新编组（自建 101101、第三方 101180、代开发 101176）：路由官方为单数 update_field_group。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.UpdateFieldGroupAsync), "/cgi-bin/wedoc/smartsheet/update_field_group"),
        // 获取编组（自建 101103、第三方 101181、代开发 101177）：官方标题为「获取编组」（非「查询」），路由为复数 get_field_groups。
        (typeof(IWechatWorkWedocSmartSheetService),
            nameof(IWechatWorkWedocSmartSheetService.GetFieldGroupsAsync), "/cgi-bin/wedoc/smartsheet/get_field_groups"),
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
    /// 契约守卫 WD8：管理智能表格内容族全部端点路由必须与官方契约一致——
    /// 20 个端点为三类应用公共面（子表 4 / 视图 4 / 字段 4 / 记录 4 / 编组 4），全部收敛父接口；
    /// 官方路由单复数混用（add_field_group 单数 vs add_fields 复数、update_field_group 单数 vs update_field_groups 复数），
    /// 一律照抄官方原文，勿「顺手归一」。
    /// </summary>
    [Fact]
    public void WedocSmartSheetEndpoints_ShouldMatchOfficialRoutes()
    {
        WedocSmartSheetRoutes.Should().HaveCount(20,
            "管理智能表格内容族 20 个端点（子表 4 + 视图 4 + 字段 4 + 记录 4 + 编组 4）为三类应用公共面，全部收敛父接口");
        WedocSmartSheetRoutes.Select(r => r.Route).Distinct().Should().HaveCount(20, "各端点路由互不重复");
        WedocSmartSheetRoutes.Select(r => r.Route)
            .Should().OnlyContain(r => r.StartsWith("/cgi-bin/wedoc/smartsheet/"),
                "管理智能表格内容族路由必须落在 wedoc/smartsheet/ 下，"
                + "不得与消息推送域的 wedoc/smartsheet/groupchat/ 群聊族或表格内容域 wedoc/spreadsheet/ 混用");

        // 五组资源各 4 端点，官方菜单结构即为契约（官方路由单复数混用，逐条锁定）。
        var expectedByGroup = new Dictionary<string, string[]>
        {
            ["子表"] = new[]
            {
                "/cgi-bin/wedoc/smartsheet/add_sheet", "/cgi-bin/wedoc/smartsheet/delete_sheet",
                "/cgi-bin/wedoc/smartsheet/update_sheet", "/cgi-bin/wedoc/smartsheet/get_sheet",
            },
            ["视图"] = new[]
            {
                "/cgi-bin/wedoc/smartsheet/add_view", "/cgi-bin/wedoc/smartsheet/delete_views",
                "/cgi-bin/wedoc/smartsheet/update_view", "/cgi-bin/wedoc/smartsheet/get_views",
            },
            ["字段"] = new[]
            {
                "/cgi-bin/wedoc/smartsheet/add_fields", "/cgi-bin/wedoc/smartsheet/delete_fields",
                "/cgi-bin/wedoc/smartsheet/update_fields", "/cgi-bin/wedoc/smartsheet/get_fields",
            },
            ["记录"] = new[]
            {
                "/cgi-bin/wedoc/smartsheet/add_records", "/cgi-bin/wedoc/smartsheet/delete_records",
                "/cgi-bin/wedoc/smartsheet/update_records", "/cgi-bin/wedoc/smartsheet/get_records",
            },
            ["编组"] = new[]
            {
                "/cgi-bin/wedoc/smartsheet/add_field_group", "/cgi-bin/wedoc/smartsheet/delete_field_groups",
                "/cgi-bin/wedoc/smartsheet/update_field_group", "/cgi-bin/wedoc/smartsheet/get_field_groups",
            },
        };

        foreach (var (group, routes) in expectedByGroup)
        {
            routes.Should().HaveCount(4, $"{group}组官方为 4 个端点（增删改查），不得增删端点");
            WedocSmartSheetRoutes.Select(r => r.Route)
                .Should().Contain(routes, $"{group}组路由必须与官方契约逐条一致（单复数混用照抄官方原文）");
        }

        AssertRoutes(WedocSmartSheetRoutes);
    }

    /// <summary>
    /// 契约守卫 WD4：文档模块接口层级与生成器注册形态——四族公共端点收敛于 IsAbstract 父接口、
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

        // —— 管理智能表格内容族 ——
        var smartSheetChildren = new[]
        {
            typeof(IWechatWorkInternalWedocSmartSheetService),
            typeof(IWechatWorkThirdPartyWedocSmartSheetService),
            typeof(IWechatWorkProviderWedocSmartSheetService),
        };

        foreach (var child in smartSheetChildren)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkWedocSmartSheetService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkWedocSmartSheetService");
        }

        AssertFamilyHierarchy(
            typeof(IWechatWorkWedocSmartSheetService),
            WedocSmartSheetParentImplementationClassName,
            expectedDeclaredMethods: 20,
            new[] { (typeof(IWechatWorkInternalWedocSmartSheetService), 0),
                    (typeof(IWechatWorkThirdPartyWedocSmartSheetService), 0),
                    (typeof(IWechatWorkProviderWedocSmartSheetService), 0) });
    }

    /// <summary>
    /// 契约守卫 WD5：令牌绑定——文档模块全部 16 个接口统一消费 AccessToken 路由键并以 Query 注入
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
            typeof(IWechatWorkWedocSmartSheetService),
            typeof(IWechatWorkInternalWedocSmartSheetService),
            typeof(IWechatWorkThirdPartyWedocSmartSheetService),
            typeof(IWechatWorkProviderWedocSmartSheetService),
        };

        interfaces.Should().OnlyHaveUniqueItems("令牌守卫覆盖的接口清单不得重复");
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
    /// 契约守卫 WD9：管理智能表格内容族的官方拼写陷阱锁定——
    /// <list type="bullet">
    /// <item><c>property_ganttobect</c>：添加视图请求的甘特视图属性官方原文多一个 o，不得改写为 property_gantt_object。</item>
    /// <item><c>typ</c> / <c>type</c>：超链接与自动编号字段属性在「添加/更新字段」文档作 <c>typ</c>、
    /// 在「查询字段」文档作 <c>type</c>，两种拼写同时承载，不得合并为一种。</item>
    /// <item><c>field_id</c> / <c>filed_id</c>：关联字段属性在「添加/更新字段」文档作 <c>field_id</c>、
    /// 在「查询字段」文档作 <c>filed_id</c>（多一个 d），两种拼写同时承载，不得「顺手修正」。</item>
    /// <item><c>desc</c>：排序项官方参数表列名原文显示为 <c>sort_infoes.desc</c>（排版错误），
    /// 官方示例为 <c>desc</c>，以示例为准。</item>
    /// </list>
    /// </summary>
    [Fact]
    public void WedocSmartSheetOfficialSpellingTraps_ShouldBePreservedVerbatim()
    {
        var gantt = typeof(AddSmartSheetViewRequest).GetProperty(
            nameof(AddSmartSheetViewRequest.PropertyGanttObject),
            BindingFlags.Public | BindingFlags.Instance);
        gantt.Should().NotBeNull("添加视图请求必须承载甘特视图属性");
        gantt!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("property_ganttobect",
            "官方原文为 property_ganttobect（gantt + oject，多一个 o），照抄官方原文，不得改写");

        var urlTyp = typeof(SmartSheetUrlFieldProperty).GetProperty(
            nameof(SmartSheetUrlFieldProperty.Typ), BindingFlags.Public | BindingFlags.Instance);
        var urlType = typeof(SmartSheetUrlFieldProperty).GetProperty(
            nameof(SmartSheetUrlFieldProperty.Type), BindingFlags.Public | BindingFlags.Instance);
        urlTyp!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("typ",
            "超链接字段属性的添加/更新侧官方原文为 typ");
        urlType!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("type",
            "超链接字段属性的查询侧官方原文为 type；两种拼写并存，不得合并");

        var autoTyp = typeof(SmartSheetAutoNumberFieldProperty).GetProperty(
            nameof(SmartSheetAutoNumberFieldProperty.Typ), BindingFlags.Public | BindingFlags.Instance);
        var autoType = typeof(SmartSheetAutoNumberFieldProperty).GetProperty(
            nameof(SmartSheetAutoNumberFieldProperty.Type), BindingFlags.Public | BindingFlags.Instance);
        autoTyp!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("typ",
            "自动编号字段属性的添加/更新侧官方原文为 typ");
        autoType!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("type",
            "自动编号字段属性的查询侧官方原文为 type；两种拼写并存，不得合并");

        var refFieldId = typeof(SmartSheetReferenceFieldProperty).GetProperty(
            nameof(SmartSheetReferenceFieldProperty.FieldId), BindingFlags.Public | BindingFlags.Instance);
        var refFiledId = typeof(SmartSheetReferenceFieldProperty).GetProperty(
            nameof(SmartSheetReferenceFieldProperty.FiledId), BindingFlags.Public | BindingFlags.Instance);
        refFieldId!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("field_id",
            "关联字段属性的添加/更新侧官方原文为 field_id");
        refFiledId!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("filed_id",
            "关联字段属性的查询侧官方原文为 filed_id（多一个 d），照抄官方原文，不得改写为 field_id");

        var sortDesc = typeof(SmartSheetSortInfo).GetProperty(
            nameof(SmartSheetSortInfo.Desc), BindingFlags.Public | BindingFlags.Instance);
        sortDesc!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("desc",
            "排序项官方参数表列名原文显示为 sort_infoes.desc（排版错误），官方示例为 desc，以示例为准");
    }

    /// <summary>
    /// 契约守卫 WD7：文档模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 162 个契约面类型）。
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
            // 管理智能表格内容族——子表。
            typeof(AddSmartSheetSheetRequest), typeof(AddSmartSheetSheetResponse),
            typeof(DeleteSmartSheetSheetRequest),
            typeof(UpdateSmartSheetSheetRequest),
            typeof(GetSmartSheetSheetRequest), typeof(GetSmartSheetSheetResponse),
            typeof(SmartSheetSheetProperties), typeof(SmartSheetSheetInfo),
            // 管理智能表格内容族——视图。
            typeof(AddSmartSheetViewRequest), typeof(AddSmartSheetViewResponse),
            typeof(DeleteSmartSheetViewsRequest),
            typeof(UpdateSmartSheetViewRequest), typeof(UpdateSmartSheetViewResponse),
            typeof(GetSmartSheetViewsRequest), typeof(GetSmartSheetViewsResponse),
            typeof(SmartSheetView), typeof(SmartSheetViewProperty),
            typeof(SmartSheetGanttViewProperty), typeof(SmartSheetCalendarViewProperty),
            typeof(SmartSheetSortSpec), typeof(SmartSheetSortInfo),
            typeof(SmartSheetGroupSpec), typeof(SmartSheetGroupInfo),
            typeof(SmartSheetFilterSpec), typeof(SmartSheetCondition),
            typeof(SmartSheetFilterStringValue), typeof(SmartSheetFilterNumberValue),
            typeof(SmartSheetFilterBoolValue), typeof(SmartSheetFilterUserValue),
            typeof(SmartSheetFilterDateTimeValue),
            typeof(SmartSheetViewColorConfig), typeof(SmartSheetViewColorCondition),
            // 管理智能表格内容族——字段。
            typeof(AddSmartSheetFieldsRequest), typeof(AddSmartSheetFieldsResponse),
            typeof(DeleteSmartSheetFieldsRequest),
            typeof(UpdateSmartSheetFieldsRequest), typeof(UpdateSmartSheetFieldsResponse),
            typeof(GetSmartSheetFieldsRequest), typeof(GetSmartSheetFieldsResponse),
            typeof(SmartSheetField),
            typeof(SmartSheetNumberFieldProperty), typeof(SmartSheetCheckboxFieldProperty),
            typeof(SmartSheetDateTimeFieldProperty), typeof(SmartSheetAttachmentFieldProperty),
            typeof(SmartSheetUserFieldProperty), typeof(SmartSheetUrlFieldProperty),
            typeof(SmartSheetSelectFieldProperty), typeof(SmartSheetCreatedTimeFieldProperty),
            typeof(SmartSheetModifiedTimeFieldProperty), typeof(SmartSheetProgressFieldProperty),
            typeof(SmartSheetSingleSelectFieldProperty), typeof(SmartSheetReferenceFieldProperty),
            typeof(SmartSheetLocationFieldProperty), typeof(SmartSheetAutoNumberFieldProperty),
            typeof(SmartSheetCurrencyFieldProperty), typeof(SmartSheetWwGroupFieldProperty),
            typeof(SmartSheetPercentageFieldProperty), typeof(SmartSheetBarcodeFieldProperty),
            typeof(SmartSheetOption), typeof(SmartSheetNumberRule),
            // 管理智能表格内容族——记录。
            typeof(AddSmartSheetRecordsRequest), typeof(AddSmartSheetRecordsResponse),
            typeof(DeleteSmartSheetRecordsRequest),
            typeof(UpdateSmartSheetRecordsRequest), typeof(UpdateSmartSheetRecordsResponse),
            typeof(GetSmartSheetRecordsRequest), typeof(GetSmartSheetRecordsResponse),
            typeof(SmartSheetAddRecord), typeof(SmartSheetUpdateRecord),
            typeof(SmartSheetCommonRecord), typeof(SmartSheetRecord), typeof(SmartSheetRecordSort),
            // 管理智能表格内容族——编组。
            typeof(AddSmartSheetFieldGroupRequest), typeof(AddSmartSheetFieldGroupResponse),
            typeof(DeleteSmartSheetFieldGroupsRequest),
            typeof(UpdateSmartSheetFieldGroupRequest), typeof(UpdateSmartSheetFieldGroupResponse),
            typeof(GetSmartSheetFieldGroupsRequest), typeof(GetSmartSheetFieldGroupsResponse),
            typeof(SmartSheetFieldGroup), typeof(SmartSheetFieldGroupChild),
        };

        requiredTypes.Should().HaveCount(162,
            "文档模块契约面类型总数漂移须先核对 DTO 落位再同批调整本守卫"
            + "（管理文档族 10 + 管理文档内容族 34 + 管理表格内容族 37 + 管理智能表格内容族 81）");
        requiredTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

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
