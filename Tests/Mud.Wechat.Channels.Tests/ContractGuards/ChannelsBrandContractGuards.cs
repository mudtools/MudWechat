// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Brand;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「品牌资质」域契约守卫（CB 系列：Brand 域 8 端点路由 + 双前缀并存 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + brand_id 类型不一致照录 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由前缀并存（官方历史前缀，照抄原文不得改写）</b>：本域同时使用 <c>/shop/ec/brand/*</c>（3 端点）
/// 与 <c>/channels/ec/brand/*</c>（5 端点），由 CH-R2 锁定与其它线路由无交叠。
/// </para>
/// <para>
/// <b>brand_id 类型不一致（官方照录，勿归一）</b>：品牌库列表（<c>brand/all</c>）返回 string；
/// 新增 / 更新请求体 brand.brand_id 与详情响应 brand.brand_id 为 number；删除 / 撤回 / 详情请求体为 string。
/// </para>
/// </remarks>
public class ChannelsBrandContractGuards
{
    private const string BrandRegistryGroupName = "Brand";

    /// <summary>Brand 域官方路由表（8 端点：/shop/ec/brand/* 3 + /channels/ec/brand/* 5，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] BrandRoutes =
    {
        (typeof(IChannelsBrandService), nameof(IChannelsBrandService.AddBrandAsync), typeof(PostAttribute), "/shop/ec/brand/add"),
        (typeof(IChannelsBrandService), nameof(IChannelsBrandService.GetAllBrandsAsync), typeof(PostAttribute), "/shop/ec/brand/all"),
        (typeof(IChannelsBrandService), nameof(IChannelsBrandService.CancelBrandAuditAsync), typeof(PostAttribute), "/shop/ec/brand/audit/cancel"),
        (typeof(IChannelsBrandService), nameof(IChannelsBrandService.DeleteBrandAsync), typeof(PostAttribute), "/channels/ec/brand/delete"),
        (typeof(IChannelsBrandService), nameof(IChannelsBrandService.GetBrandAsync), typeof(PostAttribute), "/channels/ec/brand/get"),
        (typeof(IChannelsBrandService), nameof(IChannelsBrandService.GetBrandListAsync), typeof(PostAttribute), "/channels/ec/brand/list/get"),
        (typeof(IChannelsBrandService), nameof(IChannelsBrandService.UpdateBrandAsync), typeof(PostAttribute), "/channels/ec/brand/update"),
        (typeof(IChannelsBrandService), nameof(IChannelsBrandService.GetValidBrandListAsync), typeof(PostAttribute), "/channels/ec/brand/valid/list/get"),
    };

    /// <summary>契约守卫 CB1：Brand 域 8 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void BrandEndpoints_ShouldMatchOfficialRoutes()
    {
        // 8 端点 = 品牌资质写操作 4（新增/更新/删除/撤回）+ 品牌库 1 + 申请详情/列表 2 + 生效中列表 1。
        BrandRoutes.Should().HaveCount(8, "新增 1 + 品牌库 1 + 撤回 1 + 删除 1 + 详情 1 + 申请列表 1 + 更新 1 + 生效中列表 1 = 8 端点");
        BrandRoutes.Select(r => r.Route).Distinct().Should().HaveCount(8);

        BrandRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/shop/ec/brand/", StringComparison.Ordinal))
            .Should().Be(3, "3 端点走 /shop/ec/brand/* 官方历史前缀（新增 / 品牌库 / 撤回审核）");
        BrandRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/brand/", StringComparison.Ordinal))
            .Should().Be(5, "5 端点走 /channels/ec/brand/* 主干前缀（删除 / 详情 / 列表 / 更新 / 生效中列表）");

        foreach (var (iface, method, httpAttribute, route) in BrandRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CB2：Brand 域 8 端点全部为 POST，且全部携带 [Body] 请求体（本域无空请求体端点）。</summary>
    [Fact]
    public void BrandEndpoints_ShouldAllBePost_WithBodyRules()
    {
        foreach (var (iface, method, _, _) in BrandRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（品牌资质域无 GET 端点）");

            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().NotBeEmpty($"{iface.Name}.{method} 官方参数表均有请求体，必须声明 [Body]");
        }
    }

    /// <summary>契约守卫 CB3：令牌绑定与注册形态。</summary>
    [Fact]
    public void BrandInterface_ShouldBindChannelsToken_AndRegisterUnderBrandGroup()
    {
        var token = typeof(IChannelsBrandService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Brand 域 8 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsBrandService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(BrandRegistryGroupName, "必须挂 Brand 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Brand");
        typeof(ChannelsServiceBuilder).GetMethod("AddBrandApi").Should().NotBeNull("必须提供 AddBrandApi 链式注册方法");
    }

    /// <summary>契约守卫 CB4：Brand 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void BrandDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = BrandJsonContext.Default;

        var domainTypes = typeof(ChannelsBrandDetail).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Brand"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t)
                        && !t.IsAbstract)
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(22,
            "Brand 域 8 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（新增/更新共用 brand 明细 5 + 联系方式 1 + 提交请求体 2 + 共用审核单响应 1 + 品牌库 3 + " +
            "撤回/删除/详情请求体 3 + 详情响应 1 + 申请列表 2 + 生效中列表 2 + 共用申请单形态 2 = 22）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Brand 域命名空间，必须登记进 BrandJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(BrandRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Brand");
        }
    }

    /// <summary>契约守卫 CB5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void BrandDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 新增 / 更新品牌资质。
        AssertJsonProperty<ChannelsAddBrandRequest>("brand", "品牌详情");
        AssertJsonProperty<ChannelsBrandDetail>("brand_id", "品牌库中的品牌编号（提交侧 number 型）");
        AssertJsonProperty<ChannelsBrandDetail>("classification_no", "商标分类号（1-45）");
        AssertJsonProperty<ChannelsBrandDetail>("trade_mark_symbol", "商标类型（R/TM）");
        AssertJsonProperty<ChannelsBrandDetail>("register_details", "注册信息");
        AssertJsonProperty<ChannelsBrandDetail>("application_details", "申请信息");
        AssertJsonProperty<ChannelsBrandDetail>("grant_type", "商标授权类型");
        AssertJsonProperty<ChannelsBrandDetail>("grant_details", "品牌信息");
        AssertJsonProperty<ChannelsBrandRegisterDetails>("register_no", "商标注册号");
        AssertJsonProperty<ChannelsBrandRegisterDetails>("is_permanent", "是否长期有效");
        AssertJsonProperty<ChannelsBrandRegisterDetails>("register_certifications", "商标注册证 file_id");
        AssertJsonProperty<ChannelsBrandRegisterDetails>("renew_certifications", "变更/续展证明 file_id");
        AssertJsonProperty<ChannelsBrandApplicationDetails>("acceptance_time", "商标申请受理时间");
        AssertJsonProperty<ChannelsBrandApplicationDetails>("acceptance_no", "商标申请号");
        AssertJsonProperty<ChannelsBrandGrantDetails>("grant_level", "授权级数（1-3）");
        AssertJsonProperty<ChannelsBrandGrantDetails>("brand_owner_id_photos", "品牌权利人证件照 file_id");
        AssertJsonProperty<ChannelsBrandGrantDetails>("use_split_grant_info", "是否使用分级授权书结构");
        AssertJsonProperty<ChannelsBrandGrantDetails>("grant_info_lv1", "一级授权结构");
        AssertJsonProperty<ChannelsBrandGrantDetails>("grant_info_lv3", "三级授权结构");
        AssertJsonProperty<ChannelsBrandGrantInfo>("grant_certifications", "分级结构授权书 file_id");
        AssertJsonProperty<ChannelsBrandContactInfo>("key", "联系方式类型");
        AssertJsonProperty<ChannelsBrandAuditIdResponse>("audit_id", "审核单 ID");

        // 品牌库列表。
        AssertJsonProperty<ChannelsGetAllBrandsRequest>("page_size", "每页数量");
        AssertJsonProperty<ChannelsGetAllBrandsRequest>("next_key", "翻页上下文");
        AssertJsonProperty<ChannelsGetAllBrandsResponse>("brands", "品牌库中的品牌信息");
        AssertJsonProperty<ChannelsGetAllBrandsResponse>("continue_flag", "是否还有下一页内容");
        AssertJsonProperty<ChannelsBrandLibraryItem>("brand_id", "品牌库中的品牌编号（本端点为 string 型）");
        AssertJsonProperty<ChannelsBrandLibraryItem>("ch_name", "品牌商标中文名");

        // 撤回 / 删除 / 详情。
        AssertJsonProperty<ChannelsCancelBrandAuditRequest>("brand_id", "品牌库中的品牌编号");
        AssertJsonProperty<ChannelsCancelBrandAuditRequest>("audit_id", "要撤销的审核单 ID（string 型）");
        AssertJsonProperty<ChannelsDeleteBrandRequest>("brand_id", "品牌库中的品牌编号");
        AssertJsonProperty<ChannelsGetBrandRequest>("brand_id", "品牌库中的品牌编号");
        AssertJsonProperty<ChannelsGetBrandResponse>("brand", "品牌资质详情");

        // 申请列表 / 生效中列表。
        AssertJsonProperty<ChannelsGetBrandListRequest>("status", "状态筛选（0/1/2/3/4，与品牌资质状态语义不同）");
        AssertJsonProperty<ChannelsGetBrandListResponse>("total_num", "符合条件的申请单总数");
        AssertJsonProperty<ChannelsGetBrandListResponse>("next_key", "翻页游标");
        AssertJsonProperty<ChannelsGetValidBrandListRequest>("page_size", "每页数量");
        AssertJsonProperty<ChannelsGetValidBrandListResponse>("brands", "生效中的品牌资质列表");
        AssertJsonProperty<ChannelsBrandApplicationInfo>("status", "申请单状态");
        AssertJsonProperty<ChannelsBrandApplicationInfo>("create_time", "创建时间");
        AssertJsonProperty<ChannelsBrandAuditResult>("audit_id", "审核单 ID");
        AssertJsonProperty<ChannelsBrandAuditResult>("reject_reason", "审核不通过原因");

        // 共用 DTO 裁决 1：新增与更新请求体的 brand 对象字段集完全一致 ⇒ 共用 ChannelsBrandDetail。
        typeof(ChannelsAddBrandRequest).GetProperty(nameof(ChannelsAddBrandRequest.Brand))!
            .PropertyType.Should().Be(typeof(ChannelsBrandDetail));
        typeof(ChannelsUpdateBrandRequest).GetProperty(nameof(ChannelsUpdateBrandRequest.Brand))!
            .PropertyType.Should().Be(typeof(ChannelsBrandDetail));

        // 共用 DTO 裁决 2：新增与更新响应字段集一致（仅 audit_id）⇒ 共用 ChannelsBrandAuditIdResponse。
        typeof(IChannelsBrandService).GetMethod(nameof(IChannelsBrandService.AddBrandAsync))!.ReturnType
            .Should().Be(typeof(Task<ChannelsBrandAuditIdResponse>));
        typeof(IChannelsBrandService).GetMethod(nameof(IChannelsBrandService.UpdateBrandAsync))!.ReturnType
            .Should().Be(typeof(Task<ChannelsBrandAuditIdResponse>));

        // 共用 DTO 裁决 3：详情响应 brand 与两个列表接口 brands 元素字段集一致 ⇒ 共用 ChannelsBrandApplicationInfo。
        typeof(ChannelsGetBrandResponse).GetProperty(nameof(ChannelsGetBrandResponse.Brand))!
            .PropertyType.Should().Be(typeof(ChannelsBrandApplicationInfo));
        typeof(ChannelsGetBrandListResponse).GetProperty(nameof(ChannelsGetBrandListResponse.Brands))!
            .PropertyType.Should().Be(typeof(List<ChannelsBrandApplicationInfo>));
        typeof(ChannelsGetValidBrandListResponse).GetProperty(nameof(ChannelsGetValidBrandListResponse.Brands))!
            .PropertyType.Should().Be(typeof(List<ChannelsBrandApplicationInfo>));

        // 共用 DTO 裁决 4：register_details / application_details / grant_details 提交形态与响应形态字段集一致 ⇒ 共用。
        typeof(ChannelsBrandDetail).GetProperty(nameof(ChannelsBrandDetail.RegisterDetails))!
            .PropertyType.Should().Be(typeof(ChannelsBrandRegisterDetails));
        typeof(ChannelsBrandApplicationInfo).GetProperty(nameof(ChannelsBrandApplicationInfo.RegisterDetails))!
            .PropertyType.Should().Be(typeof(ChannelsBrandRegisterDetails));
        typeof(ChannelsBrandApplicationInfo).GetProperty(nameof(ChannelsBrandApplicationInfo.GrantDetails))!
            .PropertyType.Should().Be(typeof(ChannelsBrandGrantDetails));

        // 分建裁决：品牌库条目 brand_id 为 string 型，与申请单形态（number 型 + status 等扩展字段）不共用。
        typeof(ChannelsBrandLibraryItem).GetProperties().Should().HaveCount(3,
            "品牌库条目仅 brand_id/ch_name/en_name 三字段，且 brand_id 为 string 型——与申请单形态分建");
    }

    /// <summary>契约守卫 CB6：枚举常量字面量锁定。</summary>
    [Fact]
    public void BrandEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsBrandTradeMarkSymbols.Registered.Should().Be(1, "官方原文「1=R 标」");
        ChannelsBrandTradeMarkSymbols.Trademark.Should().Be(2, "官方原文「2=TM 标」");

        ChannelsBrandGrantTypes.Owned.Should().Be(1, "官方原文「1=自有品牌」");
        ChannelsBrandGrantTypes.Authorized.Should().Be(2, "官方原文「2=授权品牌」");

        ChannelsBrandStatuses.Auditing.Should().Be(1);
        ChannelsBrandStatuses.Rejected.Should().Be(2);
        ChannelsBrandStatuses.Effective.Should().Be(3);
        ChannelsBrandStatuses.Withdrawn.Should().Be(4);
        ChannelsBrandStatuses.ExpiringSoon.Should().Be(5, "官方原文「5=即将过期(不影响商品售卖)」");
        ChannelsBrandStatuses.Expired.Should().Be(6);

        ChannelsBrandListFilterStatuses.All.Should().Be(0);
        ChannelsBrandListFilterStatuses.Auditing.Should().Be(1);
        ChannelsBrandListFilterStatuses.Rejected.Should().Be(2);
        ChannelsBrandListFilterStatuses.Approved.Should().Be(3, "官方原文「3=审核通过(含即将过期/已过期)」——与品牌资质状态 3=已生效 语义不同");
        ChannelsBrandListFilterStatuses.Withdrawn.Should().Be(4);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
