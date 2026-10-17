// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Category;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「类目管理」域契约守卫（CC 系列：Category 域 11 端点路由 + 双前缀并存 +
/// getcategoryrule 双语义同路由 + 联盟类目 GET 形态 + 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由前缀并存（官方历史前缀，照抄原文不得改写）</b>：本域同时使用 <c>/shop/ec/category/*</c>（9 端点）
/// 与 <c>/channels/ec/category/*</c>（2 端点：申请类目 + 联盟类目），由 CH-R2 锁定与其它线路由无交叠。
/// </para>
/// <para>
/// <b><c>getcategoryrule</c> 双语义同路由</b>：官方清单中该路由出现两次（保证金类目规则 / 发货方式类目规则），
/// 同路由只计 1 端点；请求体形态一致（仅 <c>category_id</c>），差异在 URL 参数 <c>rule_id</c>（1 / 2）
/// 与响应形态（<c>deposit</c> / <c>list</c>）——SDK 合并为一个方法，响应以可空超集承载。
/// </para>
/// <para>
/// <b>官方历史拼写照录（勿「顺手修正」）</b>：请求侧经营平台字段 <c>jingyingpintai</c>（少一个 g）与
/// 响应侧 <c>jingyingpingtai</c>（多一个 g）两处拼写不一致，字段名照抄官方原文。
/// </para>
/// </remarks>
public class ChannelsCategoryContractGuards
{
    private const string CategoryRegistryGroupName = "Category";

    /// <summary>Category 域官方路由表（11 端点：/shop/ec/category/* 9 + /channels/ec/category/* 2；10 POST + 1 GET）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] CategoryRoutes =
    {
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetAllCategoriesAsync), typeof(PostAttribute), "/shop/ec/category/all"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetLeagueAllCategoriesAsync), typeof(GetAttribute), "/channels/ec/category/league/all"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetCategoryDetailAsync), typeof(PostAttribute), "/shop/ec/category/detail"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.AddCategoryAsync), typeof(PostAttribute), "/channels/ec/category/add"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetBizCatFlowListAsync), typeof(PostAttribute), "/shop/ec/category/getbizcatflowlist"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetBizCatFlowDetailAsync), typeof(PostAttribute), "/shop/ec/category/getbizcatflowdetail"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetCategoryRelationListAsync), typeof(PostAttribute), "/shop/ec/category/get_category_relation_list"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetCategoryRelationDetailAsync), typeof(PostAttribute), "/shop/ec/category/get_category_relation_detail"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.CancelCategoryAuditAsync), typeof(PostAttribute), "/shop/ec/category/audit/cancel"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetCategoryRuleAsync), typeof(PostAttribute), "/shop/ec/category/getcategoryrule"),
        (typeof(IChannelsCategoryService), nameof(IChannelsCategoryService.GetCategoryProductRuleAsync), typeof(PostAttribute), "/shop/ec/category/getcategoryproductrule"),
    };

    /// <summary>契约守卫 CC1：Category 域 11 端点路由必须与官方契约一致（10 POST + 联盟类目 1 GET）。</summary>
    [Fact]
    public void CategoryEndpoints_ShouldMatchOfficialRoutes()
    {
        // 11 端点 = 类目管理 8 + 类目规则 2 + 联盟类目 1（getcategoryrule 官方双语义同路由只计 1；ChannelsModule 注释的
        // 「11 端点」为官方清单原始计数 = 类目 8 + 类目规则 3，其中 getcategoryrule 计两次，去重后为 10；
        // 2026-10-17 补联盟带货机构侧 category/league/all 后为 11）。
        CategoryRoutes.Should().HaveCount(11, "类目 8 + 类目规则 2（getcategoryrule 双语义同路由去重）+ 联盟类目 1 = 11 端点");
        CategoryRoutes.Select(r => r.Route).Distinct().Should().HaveCount(11);

        CategoryRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/shop/ec/category/", StringComparison.Ordinal))
            .Should().Be(9, "9 端点走 /shop/ec/category/* 官方历史前缀");
        CategoryRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/category/", StringComparison.Ordinal))
            .Should().Be(2, "申请类目 category/add 与联盟类目 category/league/all 2 端点走 /channels/ec/category/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in CategoryRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（含下划线路由 get_category_relation_* 照抄官方）");
        }
    }

    /// <summary>契约守卫 CC2：Category 域动词形态（10 POST + 联盟类目 1 GET）；空请求体端点不得携带 [Body]；getcategoryrule 须带 rule_id URL 参数。</summary>
    [Fact]
    public void CategoryEndpoints_ShouldMatchOfficialVerbs_WithBodyRules()
    {
        foreach (var (iface, method, httpAttribute, _) in CategoryRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetCustomAttribute(httpAttribute).Should().NotBeNull(
                $"{iface.Name}.{method} 动词必须逐页照官方原文（小店侧 getallcategory 官方变更日志 2026-09-15 由 GET 改 POST；"
                + "联盟侧 category/league/all 官方仍为 GET，两端点形态不同、不得互相对齐）");
        }

        CategoryRoutes.Count(static r => r.HttpAttribute == typeof(GetAttribute))
            .Should().Be(1, "本域唯一 GET 端点是联盟类目 category/league/all（逐页核验 2026-10-17）");

        // 官方请求体为空的端点（小店 getallcategory / 联盟 category/league/all）不得人为造请求体 DTO。
        var emptyBodyEndpoint = typeof(IChannelsCategoryService).GetMethod(nameof(IChannelsCategoryService.GetAllCategoriesAsync))!;
        emptyBodyEndpoint.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("getallcategory 官方规定空请求体——不得声明 [Body]");
        var leagueEmptyBodyEndpoint = typeof(IChannelsCategoryService).GetMethod(nameof(IChannelsCategoryService.GetLeagueAllCategoriesAsync))!;
        leagueEmptyBodyEndpoint.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("category/league/all 官方规定无请求体（Query 仅 access_token）——不得声明 [Body]");

        // getcategoryrule 的 rule_id 官方规定走 URL 参数（保证金填 1 / 发货方式填 2），不得塞进请求体。
        var ruleEndpoint = typeof(IChannelsCategoryService).GetMethod(nameof(IChannelsCategoryService.GetCategoryRuleAsync))!;
        ruleEndpoint.GetParameters()
            .Should().Contain(p => p.GetCustomAttribute<QueryAttribute>() != null && p.GetCustomAttribute<QueryAttribute>()!.Name == "rule_id",
                "getcategoryrule 官方契约 rule_id 走 URL 参数（1 保证金 / 2 发货方式）");
    }

    /// <summary>契约守卫 CC3：令牌绑定与注册形态。</summary>
    [Fact]
    public void CategoryInterface_ShouldBindChannelsToken_AndRegisterUnderCategoryGroup()
    {
        var token = typeof(IChannelsCategoryService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Category 域 11 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsCategoryService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(CategoryRegistryGroupName, "必须挂 Category 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Category");
        typeof(ChannelsServiceBuilder).GetMethod("AddCategoryApi").Should().NotBeNull("必须提供 AddCategoryApi 链式注册方法");
    }

    /// <summary>契约守卫 CC4：Category 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void CategoryDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = CategoryJsonContext.Default;

        var domainTypes = typeof(ChannelsGetAllCategoryResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Category"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t)
                        && !t.IsAbstract)
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(56,
            "Category 域 11 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（getallcategory 独有 7 + 联盟类目独有 1 + getcategorydetail 独有 6 + addcategory 独有 4 + 审核单 6 + 类目权限 5 + 撤销审核 1 + " +
            "getcategoryrule 3 + getcategoryproductrule 独有 14 + 跨端点共用 9" +
            "（类目信息 / 尺码表·编辑要求 / 品牌引用 / 资质条目 / 证照组三件套）= 56）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Category 域命名空间，必须登记进 CategoryJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(CategoryRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Category");
        }
    }

    /// <summary>契约守卫 CC5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void CategoryDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 获取所有类目。
        AssertJsonProperty<ChannelsGetAllCategoryResponse>("cats", "旧类目信息");
        AssertJsonProperty<ChannelsGetAllCategoryResponse>("cats_v2", "新类目信息");

        // 联盟类目（category/league/all）：应答树与小店侧逐字段同形 ⇒ 元素类型必须复用（防分叉复制一份漂移）。
        AssertJsonProperty<ChannelsLeagueAllCategoryResponse>("cats", "旧类目信息（联盟侧）");
        AssertJsonProperty<ChannelsLeagueAllCategoryResponse>("cats_v2", "新类目信息（联盟侧）");
        typeof(ChannelsLeagueAllCategoryResponse).GetProperty("Cats")!.PropertyType
            .Should().Be(typeof(List<ChannelsCategoryAndQua>),
                "联盟侧 cats 元素类型必须复用小店侧 ChannelsCategoryAndQua（官方两页表逐字段一致，单类型裁决）");
        AssertJsonProperty<ChannelsCategoryAndQua>("cat", "类目详情");
        AssertJsonProperty<ChannelsCategoryAndQua>("qua", "资质详情");
        AssertJsonProperty<ChannelsCategoryAndQua>("product_qua", "商品资质信息（将废弃）");
        AssertJsonProperty<ChannelsCategoryAndQua>("product_qua_list", "商品资质列表");
        AssertJsonProperty<ChannelsCategoryAndQua>("brand_qua", "品牌资质信息");
        AssertJsonProperty<ChannelsCategoryAndQua>("is_confidence_require_bad_must_pay", "放心买必须打开坏损包赔");
        AssertJsonProperty<ChannelsCategoryCat>("f_cat_id", "父类目ID");
        AssertJsonProperty<ChannelsCategoryCat>("leaf", "是否为叶子类目（仅 cats_v2 携带）");
        AssertJsonProperty<ChannelsCategoryQua>("cert_group_list", "证照信息（仅 cats 旧树携带）");
        AssertJsonProperty<ChannelsCategoryCertGroup>("license_group_id", "证照组id");
        AssertJsonProperty<ChannelsCategoryCertGroup>("busi_license_desc", "证照组说明");
        AssertJsonProperty<ChannelsCategoryLicenseInfo>("is_necessary", "是否必填");
        AssertJsonProperty<ChannelsCategoryLicenseField>("field_key", "填写字段类型（官方历史拼写 audit_license_validty_period 照录）");

        // 获取类目信息。
        AssertJsonProperty<ChannelsGetCategoryDetailRequest>("cat_id", "品类ID");
        AssertJsonProperty<ChannelsGetCategoryDetailResponse>("info", "类目信息");
        AssertJsonProperty<ChannelsGetCategoryDetailResponse>("attr", "属性信息");
        AssertJsonProperty<ChannelsCategoryAttr>("shop_no_shipment", "是否支持虚拟发货");
        AssertJsonProperty<ChannelsCategoryAttr>("access_permit_required", "是否定向准入");
        AssertJsonProperty<ChannelsCategoryAttr>("seven_day_return", "是否必须支持7天无理由退货");
        AssertJsonProperty<ChannelsCategoryAttr>("deposit", "类目关联的保证金（分）");
        AssertJsonProperty<ChannelsCategoryAttr>("confirm_receipt_days", "收货时间选项");
        AssertJsonProperty<ChannelsCategoryAttr>("is_limit_brand", "是否品牌定向准入");
        AssertJsonProperty<ChannelsCategoryAttrItem>("type_v2", "属性类型v2");
        AssertJsonProperty<ChannelsCategoryAttrItem>("append_allowed", "允许添加选项");
        AssertJsonProperty<ChannelsCategoryTransactionFeeInfo>("basis_point", "类目实收交易佣金比例（万分比）");
        AssertJsonProperty<ChannelsCategoryTransactionFeeInfo>("original_basis_point", "类目原始佣金比例");
        AssertJsonProperty<ChannelsCategoryCouponRule>("discount_ratio_limit", "最高折扣比例");
        AssertJsonProperty<ChannelsCategorySizeChartItem>("format", "尺码填写格式（官方标注 string）");

        // 申请类目。
        AssertJsonProperty<ChannelsAddCategoryRequest>("category_info", "申请的类目信息");
        AssertJsonProperty<ChannelsCategoryInfoApply>("level1", "一级类目ID");
        AssertJsonProperty<ChannelsCategoryInfoApply>("cats_v2", "类目树（新多级类目）");
        AssertJsonProperty<ChannelsCategoryInfoApply>("certificate", "资质材料（官方已废弃，改用证照组）");
        // 官方历史拼写照录：请求侧少一个 g（jingyingpintai），响应侧多一个 g（jingyingpingtai）——两者不得统一。
        AssertJsonProperty<ChannelsCategoryInfoApply>("jingyingpintai", "经营平台（请求侧官方拼写）");
        AssertJsonProperty<ChannelsCategoryAuditInfo>("jingyingpingtai", "经营平台（响应侧官方拼写）");
        AssertJsonProperty<ChannelsCategoryInfoApply>("license_group_list", "证照组");
        AssertJsonProperty<ChannelsCategoryInfoApply>("is_new_apply_cat", "是否为新版证照组申请");
        AssertJsonProperty<ChannelsCategoryLicenseGroup>("license_group_id", "证照组id");
        AssertJsonProperty<ChannelsCategoryLicense>("license_id", "证照id");
        AssertJsonProperty<ChannelsCategoryLicense>("file_id_list", "证照图片");
        AssertJsonProperty<ChannelsCategoryLicenseFieldPair>("key", "字段key");
        AssertJsonProperty<ChannelsAddCategoryResponse>("audit_id", "审核单id");

        // 审核单列表 / 详情。
        AssertJsonProperty<ChannelsGetBizCatFlowListRequest>("is_filter_status", "是否过滤状态");
        AssertJsonProperty<ChannelsGetBizCatFlowListRequest>("offset", "偏移量");
        AssertJsonProperty<ChannelsGetBizCatFlowListResponse>("list", "审核单列表");
        AssertJsonProperty<ChannelsCategoryAuditFlow>("audit_id", "审核单id");
        AssertJsonProperty<ChannelsCategoryAuditFlow>("audit_reason", "驳回原因");
        AssertJsonProperty<ChannelsCategoryAuditInfo>("commitment_letter_list", "承诺函");
        AssertJsonProperty<ChannelsCategoryAuditInfo>("account_link", "账号链接");
        AssertJsonProperty<ChannelsGetBizCatFlowDetailRequest>("audit_id", "审核单id（本端点为 number 型）");
        AssertJsonProperty<ChannelsGetBizCatFlowDetailResponse>("info", "类目审核单详情");

        // 类目权限列表 / 详情。
        AssertJsonProperty<ChannelsGetCategoryRelationListResponse>("list", "店铺类目权限列表");
        AssertJsonProperty<ChannelsCategoryRelation>("id", "类目id（权限列表元素字段名为 id）");
        AssertJsonProperty<ChannelsCategoryRelation>("uneffective_reason", "失效原因（官方历史拼写照录 uneffective）");
        AssertJsonProperty<ChannelsCategoryRelation>("uneffective_time", "失效时间");
        AssertJsonProperty<ChannelsGetCategoryRelationDetailRequest>("category_id", "类目 ID");
        AssertJsonProperty<ChannelsGetCategoryRelationDetailResponse>("category_id", "类目 ID");

        // 撤销类目审核（audit_id 本端点为 string 型，与详情接口 number 型不一致，照抄官方）。
        AssertJsonProperty<ChannelsCancelCategoryAuditRequest>("audit_id", "审核单id（本端点为 string 型）");

        // 类目规则（getcategoryrule 双语义合并）。
        AssertJsonProperty<ChannelsGetCategoryRuleRequest>("category_id", "类目id");
        AssertJsonProperty<ChannelsGetCategoryRuleResponse>("deposit", "保证金（分，rule_id=1 语义）");
        AssertJsonProperty<ChannelsGetCategoryRuleResponse>("list", "发货方式规则列表（rule_id=2 语义）");
        AssertJsonProperty<ChannelsCategoryDeliverRule>("deliver_type", "发货方式");
        AssertJsonProperty<ChannelsCategoryDeliverRule>("acct_type", "支持的账号类型（numarray）");

        // 类目下商品发布规则。
        AssertJsonProperty<ChannelsGetCategoryProductRuleRequest>("cat_id", "类目ID");
        AssertJsonProperty<ChannelsGetCategoryProductRuleRequest>("release_mode", "发布模式");
        AssertJsonProperty<ChannelsGetCategoryProductRuleRequest>("brand_id", "品牌ID");
        AssertJsonProperty<ChannelsGetCategoryProductRuleResponse>("product_attr_list", "产品属性");
        AssertJsonProperty<ChannelsGetCategoryProductRuleResponse>("sale_attr_list", "销售属性");
        AssertJsonProperty<ChannelsGetCategoryProductRuleResponse>("extra_service_list", "额外服务");
        AssertJsonProperty<ChannelsGetCategoryProductRuleResponse>("is_need_bar_code", "是否必填sku条形码");
        AssertJsonProperty<ChannelsGetCategoryProductRuleResponse>("presale_rule", "预售规则");
        AssertJsonProperty<ChannelsGetCategoryProductRuleResponse>("is_directed_category", "是否定向类目");
        AssertJsonProperty<ChannelsGetCategoryProductRuleResponse>("restricted_brand_list", "类目可售品牌范围");
        AssertJsonProperty<ChannelsCategoryPublishAttrItem>("required_rule", "填写规则（发品形态独有）");
        AssertJsonProperty<ChannelsCategoryRequiredRule>("or_combinators", "规则组合（or）");
        AssertJsonProperty<ChannelsCategoryRuleAndCombinator>("combine_type", "组合类型");
        AssertJsonProperty<ChannelsCategoryRuleTextCondition>("check_field", "检查的字段");
        AssertJsonProperty<ChannelsCategoryRuleTextCondition>("oper_type", "操作类型");
        AssertJsonProperty<ChannelsCategoryPublishQua>("required_rule", "资质填写规则（发品形态独有）");
        AssertJsonProperty<ChannelsCategoryExtraService>("extra_service_name", "额外服务参数名");
        AssertJsonProperty<ChannelsCategoryPresaleRule>("presale_switch", "是否支持预售");
        AssertJsonProperty<ChannelsCategoryPresaleSpan>("presale_cycle_span", "预售周期最大可设置天数");
        AssertJsonProperty<ChannelsCategoryBrandInfo>("is_in_category_brand_pool", "定向类目时品牌是否在品牌池内");

        // 共用 DTO 裁决 1：getcategorydetail 与 getcategoryproductrule 的 info 字段集一致（cat_id + name）⇒ 共用 ChannelsCategoryInfo。
        typeof(ChannelsGetCategoryDetailResponse).GetProperty(nameof(ChannelsGetCategoryDetailResponse.Info))!
            .PropertyType.Should().Be(typeof(ChannelsCategoryInfo));
        typeof(ChannelsGetCategoryProductRuleResponse).GetProperty(nameof(ChannelsGetCategoryProductRuleResponse.Info))!
            .PropertyType.Should().Be(typeof(ChannelsCategoryInfo));

        // 共用 DTO 裁决 2：审核单列表 list 元素与详情 info 字段集一致 ⇒ 共用 ChannelsCategoryAuditFlow。
        typeof(ChannelsGetBizCatFlowListResponse).GetProperty(nameof(ChannelsGetBizCatFlowListResponse.List))!
            .PropertyType.Should().Be(typeof(List<ChannelsCategoryAuditFlow>));
        typeof(ChannelsGetBizCatFlowDetailResponse).GetProperty(nameof(ChannelsGetBizCatFlowDetailResponse.Info))!
            .PropertyType.Should().Be(typeof(ChannelsCategoryAuditFlow));

        // 共用 DTO 裁决 3：audit_info 在「审核单列表 / 审核单详情 / 类目权限详情」三处字段集一致 ⇒ 共用 ChannelsCategoryAuditInfo。
        typeof(ChannelsCategoryAuditFlow).GetProperty(nameof(ChannelsCategoryAuditFlow.AuditInfo))!
            .PropertyType.Should().Be(typeof(ChannelsCategoryAuditInfo));
        typeof(ChannelsGetCategoryRelationDetailResponse).GetProperty(nameof(ChannelsGetCategoryRelationDetailResponse.AuditInfo))!
            .PropertyType.Should().Be(typeof(ChannelsCategoryAuditInfo));

        // 共用 DTO 裁决 4：证照组结构在「申请类目」请求体与「审核材料 audit_info」响应中字段集一致 ⇒ 共用三件套。
        typeof(ChannelsCategoryInfoApply).GetProperty(nameof(ChannelsCategoryInfoApply.LicenseGroupList))!
            .PropertyType.Should().Be(typeof(List<ChannelsCategoryLicenseGroup>));
        typeof(ChannelsCategoryAuditInfo).GetProperty(nameof(ChannelsCategoryAuditInfo.LicenseGroupList))!
            .PropertyType.Should().Be(typeof(List<ChannelsCategoryLicenseGroup>));

        // 共用 DTO 裁决 5：size_chart / product_requirement 在「获取类目信息」与「获取类目下商品发布规则」字段集一致 ⇒ 共用。
        typeof(ChannelsCategoryAttr).GetProperty(nameof(ChannelsCategoryAttr.SizeChart))!
            .PropertyType.Should().Be(typeof(ChannelsCategorySizeChart));
        typeof(ChannelsGetCategoryProductRuleResponse).GetProperty(nameof(ChannelsGetCategoryProductRuleResponse.SizeChart))!
            .PropertyType.Should().Be(typeof(ChannelsCategorySizeChart));
        typeof(ChannelsCategoryAttr).GetProperty(nameof(ChannelsCategoryAttr.ProductRequirement))!
            .PropertyType.Should().Be(typeof(ChannelsCategoryProductRequirement));
        typeof(ChannelsGetCategoryProductRuleResponse).GetProperty(nameof(ChannelsGetCategoryProductRuleResponse.ProductRequirement))!
            .PropertyType.Should().Be(typeof(ChannelsCategoryProductRequirement));

        // 共用 DTO 裁决 6：品牌引用 {brand_id} 在「类目 attr.brand_list / 申请类目 brand_list / audit_info.brand_list」三处一致 ⇒ 共用 ChannelsCategoryBrandRef。
        typeof(ChannelsCategoryAttr).GetProperty(nameof(ChannelsCategoryAttr.BrandList))!
            .PropertyType.Should().Be(typeof(List<ChannelsCategoryBrandRef>));
        typeof(ChannelsCategoryInfoApply).GetProperty(nameof(ChannelsCategoryInfoApply.BrandList))!
            .PropertyType.Should().Be(typeof(List<ChannelsCategoryBrandRef>));
        typeof(ChannelsCategoryAuditInfo).GetProperty(nameof(ChannelsCategoryAuditInfo.BrandList))!
            .PropertyType.Should().Be(typeof(List<ChannelsCategoryBrandRef>));

        // 分建裁决：getcategoryproductrule 的属性 / 资质形态与 getcategorydetail 不同（无 type/is_required、多 required_rule）⇒ 不共用。
        typeof(ChannelsCategoryPublishAttrItem).GetProperty("Type")!.Should().BeNull(
            "发品属性形态无 type 字段（以 type_v2 替代），与 getcategorydetail 形态分建");
        typeof(ChannelsCategoryPublishQua).GetProperty("QuaId")!.Should().BeNull(
            "发品资质形态以 id 为标识、无 qua_id/need_to_apply/mandatory，与 getallcategory 形态分建");
    }

    /// <summary>契约守卫 CC6：枚举常量字面量锁定。</summary>
    [Fact]
    public void CategoryEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsCategoryRuleIds.Deposit.Should().Be(1, "官方原文「保证金填 1」");
        ChannelsCategoryRuleIds.DeliveryMethod.Should().Be(2, "官方原文「发货方式为 2」");

        ChannelsCategoryAuditStatuses.Auditing.Should().Be(1);
        ChannelsCategoryAuditStatuses.Rejected.Should().Be(2);
        ChannelsCategoryAuditStatuses.Approved.Should().Be(3);
        ChannelsCategoryAuditStatuses.Canceled.Should().Be(4);

        ChannelsCategoryRelationStatuses.Effective.Should().Be(1);
        ChannelsCategoryRelationStatuses.Uneffective.Should().Be(2);

        ChannelsCategoryDeliverTypes.Express.Should().Be(0);
        ChannelsCategoryDeliverTypes.Education.Should().Be(1);
        ChannelsCategoryDeliverTypes.CardCoupon.Should().Be(3);
        ChannelsCategoryDeliverTypes.GameRole.Should().Be(4);
        ChannelsCategoryDeliverTypes.GameAccount.Should().Be(5);
        ChannelsCategoryDeliverTypes.IntraCityDelivery.Should().Be(6);

        ChannelsCategoryAcctTypes.Wechat.Should().Be(1);
        ChannelsCategoryAcctTypes.QQ.Should().Be(2);
        ChannelsCategoryAcctTypes.Phone.Should().Be(3);
        ChannelsCategoryAcctTypes.Email.Should().Be(4);

        ChannelsCategoryReleaseModes.Normal.Should().Be(0);
        ChannelsCategoryReleaseModes.Simple.Should().Be(1);

        ChannelsCategoryAttrTypes.String.Should().Be("string");
        ChannelsCategoryAttrTypes.SelectOne.Should().Be("select_one");
        ChannelsCategoryAttrTypes.SelectMany.Should().Be("select_many");
        ChannelsCategoryAttrTypes.Integer.Should().Be("integer");
        ChannelsCategoryAttrTypes.Decimal4.Should().Be("decimal4");
        ChannelsCategoryAttrTypes.IntegerUnit.Should().Be("integer_unit");
        ChannelsCategoryAttrTypes.Decimal4Unit.Should().Be("decimal4_unit");

        ChannelsCategoryIncentiveTypes.None.Should().Be(0);
        ChannelsCategoryIncentiveTypes.NewShopCommissionReduction.Should().Be(1);

        ChannelsCategoryRequiredRuleTypes.Required.Should().Be(1);
        ChannelsCategoryRequiredRuleTypes.Optional.Should().Be(2);
        ChannelsCategoryRequiredRuleTypes.ConditionalRequired.Should().Be(3);

        ChannelsCategoryCheckFields.ProductParamKey.Should().Be(1);
        ChannelsCategoryCheckFields.ProductParamValue.Should().Be(2);
        ChannelsCategoryCheckFields.ProductTitle.Should().Be(3);

        ChannelsCategoryOperTypes.Contains.Should().Be(1);
        ChannelsCategoryOperTypes.NotContains.Should().Be(2);
        ChannelsCategoryOperTypes.Regex.Should().Be(3);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
