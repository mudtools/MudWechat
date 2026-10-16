// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.BrandCardMember;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-4 品牌线「会员卡与会员」域契约守卫 —— <b>15 端点</b>：模板五支 + 预授权一支 + 用户卡六支 + 动态一支 + 积分两支。
/// </summary>
/// <remarks>
/// <para><b>模板族与用户卡族是两套生命周期</b>（<c>card_id</c> ↔ <c>user_card_code</c>）：模板四支（创建 / 查询 / 修改 / 作废）应答 49 行同形 ⇒ 共用一型；用户卡五支（查询 / 修改 / 确认 / 作废 / 导入）28 行同形 ⇒ 共用另一型；两族的<b>列表支各为独立形态</b>（模板列表 53 行、用户卡列表 32 行）⇒ 不得跨族套用（BCM-B3）。</para>
/// <para><b>共用只在应答侧</b>：模板创建（45 行 Body）与 PATCH 修改的<b>必填口径不同</b>、用户卡 PATCH / 确认 / 导入三支互异 ⇒请求类型一律各建（BCM-B1）。</para>
/// <para><b>写动词三分</b>：修改走 <b>PATCH</b>、作废与确认走 <b>POST 动词段</b>（<c>/invalidate</c>、<c>/confirm</c>）、创建与导入走 <b>POST 资源段</b> ⇒ 本域动词集为 GET / POST / PATCH，无 DELETE（BCM-B2）。</para>
/// <para><b>用户卡两支把定位键放在 query</b>：<c>GET …/user-cards/{user_card_code}</c> 的 <c>brand_id</c> / <c>card_id</c> /<c>openid</c> 官方<b>全部必填</b>（与 path 上的 <c>user_card_code</c> 共同四元定位）⇒ 缺任一即 400，不得给默认值。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayBrandCardMemberContractGuards
{
    private const string RegistryGroupName = "BrandCardMember";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.BrandCardMember";

    private static readonly Type Service = typeof(IWechatPayBrandCardMemberService);

    /// <summary>BCM-B1：拓扑 —— 一个注册组一个接口、15 个方法、15 条 (方法, 路由)、动词集 POST / GET / PATCH；路由前缀 /v3/brand/partner/card-member/。</summary>
    [Fact]
    public void BrandCardMemberTopology_ShouldBeOneInterfaceOnOneGroup()
    {
        var api = Service.GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api)
            .Should().Be(RegistryGroupName, "组名即源生成注册入口名（改组名即改 Add{组}WebApiHttpClient）");
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");

        Service.GetCustomAttributes(false)
            .Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
                "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        Service.GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线无 access_token（红线 PAY-B1）");
        Service.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(15, "品牌会员卡与会员声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(15, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute", "PatchAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/brand/partner/card-member/", StringComparison.Ordinal),
            "本域路由全在 /v3/brand/partner/card-member/ 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>BCM-B2：15 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void BrandCardMemberEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayBrandCardMemberService.CreateBrandMemberCardAsync)).Should().Be("/v3/brand/partner/card-member/cards");
        // 会员卡模板创建｜官方页 partner/4015336970｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberCardsAsync)).Should().Be("/v3/brand/partner/card-member/cards");
        QueryNamesOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberCardsAsync)).Should().BeEquivalentTo(new[] { "brand_id", "state", "offset", "limit" });
        RequiredQueryOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberCardsAsync)).Should().BeEquivalentTo(new[] { "brand_id", "offset", "limit" }, "会员卡模板列表的 brand_id / offset / limit 官方必填：给默认值即把 400 变成静默错参");
        // 会员卡模板列表｜官方页 partner/4015336976｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberCardAsync)).Should().Be("/v3/brand/partner/card-member/cards/{cardId}");
        PathKeysOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberCardAsync)).Should().BeEquivalentTo(new[] { "cardId" });
        QueryNamesOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberCardAsync)).Should().BeEquivalentTo(new[] { "brand_id" });
        RequiredQueryOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberCardAsync)).Should().BeEquivalentTo(new[] { "brand_id" }, "会员卡模板信息的 brand_id 官方必填：给默认值即把 400 变成静默错参");
        // 会员卡模板信息｜官方页 partner/4015336974｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.UpdateBrandMemberCardAsync)).Should().Be("/v3/brand/partner/card-member/cards/{cardId}");
        PathKeysOf(nameof(IWechatPayBrandCardMemberService.UpdateBrandMemberCardAsync)).Should().BeEquivalentTo(new[] { "cardId" });
        // 会员卡模板修改｜官方页 partner/4015336977｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.InvalidateBrandMemberCardAsync)).Should().Be("/v3/brand/partner/card-member/cards/{cardId}/invalidate");
        PathKeysOf(nameof(IWechatPayBrandCardMemberService.InvalidateBrandMemberCardAsync)).Should().BeEquivalentTo(new[] { "cardId" });
        // 会员卡模板作废｜官方页 partner/4015336972｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.CreateBrandMemberPreAuthTokenAsync)).Should().Be("/v3/brand/partner/card-member/pre-auth-tokens");
        // 会员卡预授权令牌发起｜官方页 partner/4015336986｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberUserCardAsync)).Should().Be("/v3/brand/partner/card-member/user-cards/{userCardCode}");
        PathKeysOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberUserCardAsync)).Should().BeEquivalentTo(new[] { "userCardCode" });
        QueryNamesOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberUserCardAsync)).Should().BeEquivalentTo(new[] { "brand_id", "card_id", "openid" });
        RequiredQueryOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberUserCardAsync)).Should().BeEquivalentTo(new[] { "brand_id", "card_id", "openid" }, "用户会员卡信息的 brand_id / card_id / openid 官方必填：给默认值即把 400 变成静默错参");
        // 用户会员卡信息｜官方页 partner/4015336980｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberUserCardsAsync)).Should().Be("/v3/brand/partner/card-member/user-cards");
        QueryNamesOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberUserCardsAsync)).Should().BeEquivalentTo(new[] { "brand_id", "openid", "user_card_state", "offset", "limit" });
        RequiredQueryOf(nameof(IWechatPayBrandCardMemberService.QueryBrandMemberUserCardsAsync)).Should().BeEquivalentTo(new[] { "brand_id", "openid", "offset", "limit" }, "用户会员卡列表的 brand_id / openid / offset / limit 官方必填：给默认值即把 400 变成静默错参");
        // 用户会员卡列表｜官方页 partner/4015336984｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.UpdateBrandMemberUserCardAsync)).Should().Be("/v3/brand/partner/card-member/user-cards/{userCardCode}");
        PathKeysOf(nameof(IWechatPayBrandCardMemberService.UpdateBrandMemberUserCardAsync)).Should().BeEquivalentTo(new[] { "userCardCode" });
        // 用户会员卡信息修改｜官方页 partner/4015336985｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.ConfirmBrandMemberUserCardAsync)).Should().Be("/v3/brand/partner/card-member/user-cards/{userCardCode}/confirm");
        PathKeysOf(nameof(IWechatPayBrandCardMemberService.ConfirmBrandMemberUserCardAsync)).Should().BeEquivalentTo(new[] { "userCardCode" });
        // 会员开通结果同步｜官方页 partner/4015336979｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.InvalidateBrandMemberUserCardAsync)).Should().Be("/v3/brand/partner/card-member/user-cards/{userCardCode}/invalidate");
        PathKeysOf(nameof(IWechatPayBrandCardMemberService.InvalidateBrandMemberUserCardAsync)).Should().BeEquivalentTo(new[] { "userCardCode" });
        // 用户会员卡作废｜官方页 partner/4015336983｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.ImportBrandMemberUserCardAsync)).Should().Be("/v3/brand/partner/card-member/user-cards/import-by-openid");
        // 按 OPENID 导入用户会员卡｜官方页 partner/4015336981｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.CreateBrandMemberUserFeedAsync)).Should().Be("/v3/brand/partner/card-member/user-feeds");
        // 用户动态信息创建｜官方页 partner/4015336987｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.ConfirmBrandMemberPointsCouponExchangeAsync)).Should().Be("/v3/brand/partner/card-member/user-points/exchange-coupon/confirm");
        // 积分兑券结果同步｜官方页 partner/4015897268｜身份 普通服务商

        RouteOf(nameof(IWechatPayBrandCardMemberService.SyncBrandMemberPointsAsync)).Should().Be("/v3/brand/partner/card-member/user-points/sync");
        // 积分余额同步｜官方页 partner/4015897280｜身份 普通服务商
    }

    /// <summary>BCM-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void BrandCardMemberDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<BrandMemberCardCreateRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "brand_id", "appid", "card_type", "card_title", "card_color", "card_picture_url", "code_mode", "code_type", "code_jump_information", "benefits", "notify_url", "need_pinned", "need_display_level", "init_level", "service_phone", "legal_agreement", "valid_date_information", "member_information", "points_information", "balance_information", "purchase_information", "user_information", "support_user_active_join", "purchase_membership" },
            "会员卡模板创建请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberCardResponse>().Should().BeEquivalentTo(new[] { "out_request_no", "card_id", "brand_id", "appid", "card_type", "card_title", "card_color", "card_picture_url", "code_mode", "code_type", "code_jump_information", "benefits", "notify_url", "need_pinned", "need_display_level", "init_level", "service_phone", "legal_agreement", "valid_date_information", "member_information", "points_information", "balance_information", "purchase_information", "user_information", "state", "create_time", "modify_time", "support_user_active_join", "purchase_membership" },
            "会员卡模板创建应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberCardListResponse>().Should().BeEquivalentTo(new[] { "data", "total_count", "offset", "limit" },
            "会员卡模板列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberCardResponse>().Should().BeEquivalentTo(new[] { "out_request_no", "card_id", "brand_id", "appid", "card_type", "card_title", "card_color", "card_picture_url", "code_mode", "code_type", "code_jump_information", "benefits", "notify_url", "need_pinned", "need_display_level", "init_level", "service_phone", "legal_agreement", "valid_date_information", "member_information", "points_information", "balance_information", "purchase_information", "user_information", "state", "create_time", "modify_time", "support_user_active_join", "purchase_membership" },
            "会员卡模板信息应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberCardUpdateRequest>().Should().BeEquivalentTo(new[] { "brand_id", "card_title", "card_color", "card_picture_url", "code_jump_information", "benefits", "notify_url", "need_pinned", "need_display_level", "service_phone", "valid_date_information", "member_information", "points_information", "balance_information", "purchase_information", "user_information", "legal_agreement", "init_level", "support_user_active_join", "purchase_membership" },
            "会员卡模板修改请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberCardResponse>().Should().BeEquivalentTo(new[] { "out_request_no", "card_id", "brand_id", "appid", "card_type", "card_title", "card_color", "card_picture_url", "code_mode", "code_type", "code_jump_information", "benefits", "notify_url", "need_pinned", "need_display_level", "init_level", "service_phone", "legal_agreement", "valid_date_information", "member_information", "points_information", "balance_information", "purchase_information", "user_information", "state", "create_time", "modify_time", "support_user_active_join", "purchase_membership" },
            "会员卡模板修改应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberCardInvalidateRequest>().Should().BeEquivalentTo(new[] { "brand_id" },
            "会员卡模板作废请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberCardResponse>().Should().BeEquivalentTo(new[] { "out_request_no", "card_id", "brand_id", "appid", "card_type", "card_title", "card_color", "card_picture_url", "code_mode", "code_type", "code_jump_information", "benefits", "notify_url", "need_pinned", "need_display_level", "init_level", "service_phone", "legal_agreement", "valid_date_information", "member_information", "points_information", "balance_information", "purchase_information", "user_information", "state", "create_time", "modify_time", "support_user_active_join", "purchase_membership" },
            "会员卡模板作废应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberPreAuthTokenRequest>().Should().BeEquivalentTo(new[] { "brand_id", "card_id", "openid" },
            "会员卡预授权令牌发起请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberPreAuthTokenResponse>().Should().BeEquivalentTo(new[] { "token", "expire_time" },
            "会员卡预授权令牌发起应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberUserCardResponse>().Should().BeEquivalentTo(new[] { "user_card_code", "card_id", "openid", "card_color", "card_picture_url", "brand_id", "card_type", "phone_number", "level", "valid_date_information", "pickup_time", "user_information", "attach", "user_card_state", "invalid_reason", "invalid_time", "create_time", "modify_time" },
            "用户会员卡信息应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberUserCardListResponse>().Should().BeEquivalentTo(new[] { "data", "total_count", "offset", "limit" },
            "用户会员卡列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberUserCardUpdateRequest>().Should().BeEquivalentTo(new[] { "brand_id", "card_id", "openid", "card_color", "card_picture_url", "phone_number", "level", "valid_date_information", "user_information", "attach" },
            "用户会员卡信息修改请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberUserCardResponse>().Should().BeEquivalentTo(new[] { "user_card_code", "card_id", "openid", "card_color", "card_picture_url", "brand_id", "card_type", "phone_number", "level", "valid_date_information", "pickup_time", "user_information", "attach", "user_card_state", "invalid_reason", "invalid_time", "create_time", "modify_time" },
            "用户会员卡信息修改应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberUserCardConfirmRequest>().Should().BeEquivalentTo(new[] { "brand_id", "card_id", "openid", "card_color", "card_picture_url", "phone_number", "level", "valid_date_information", "user_information", "attach", "user_card_confirm_state" },
            "会员开通结果同步请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberUserCardResponse>().Should().BeEquivalentTo(new[] { "user_card_code", "card_id", "openid", "card_color", "card_picture_url", "brand_id", "card_type", "phone_number", "level", "valid_date_information", "pickup_time", "user_information", "attach", "user_card_state", "invalid_reason", "invalid_time", "create_time", "modify_time" },
            "会员开通结果同步应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberUserCardInvalidateRequest>().Should().BeEquivalentTo(new[] { "brand_id", "card_id", "openid", "invalid_reason" },
            "用户会员卡作废请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberUserCardResponse>().Should().BeEquivalentTo(new[] { "user_card_code", "card_id", "openid", "card_color", "card_picture_url", "brand_id", "card_type", "phone_number", "level", "valid_date_information", "pickup_time", "user_information", "attach", "user_card_state", "invalid_reason", "invalid_time", "create_time", "modify_time" },
            "用户会员卡作废应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberUserCardImportRequest>().Should().BeEquivalentTo(new[] { "brand_id", "card_id", "openid", "user_card_code", "phone_number", "card_color", "card_picture_url", "level", "user_information", "valid_date_information", "pickup_time" },
            "按 OPENID 导入用户会员卡请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberUserCardResponse>().Should().BeEquivalentTo(new[] { "user_card_code", "card_id", "openid", "card_color", "card_picture_url", "brand_id", "card_type", "phone_number", "level", "valid_date_information", "pickup_time", "user_information", "attach", "user_card_state", "invalid_reason", "invalid_time", "create_time", "modify_time" },
            "按 OPENID 导入用户会员卡应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberUserFeedCreateRequest>().Should().BeEquivalentTo(new[] { "brand_id", "card_id", "user_card_code", "openid", "out_request_no", "cell" },
            "用户动态信息创建请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberUserFeedCreateResponse>().Should().BeEquivalentTo(new[] { "brand_id", "card_id", "user_card_code", "out_request_no", "cell" },
            "用户动态信息创建应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberPointsCouponConfirmRequest>().Should().BeEquivalentTo(new[] { "record_id", "exchange_coupon_template_id", "brand_id", "card_id", "openid", "user_card_code", "result", "reject_reason" },
            "积分兑券结果同步请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberPointsCouponConfirmResponse>().Should().BeEquivalentTo(new[] { "record_id", "exchange_coupon_template_id", "brand_id", "card_id", "openid", "user_card_code", "state", "product_coupon_id", "product_coupon_stock_type", "stock_id", "coupon_code", "reject_reason" },
            "积分兑券结果同步应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<BrandMemberPointsSyncRequest>().Should().BeEquivalentTo(new[] { "out_request_no", "brand_id", "card_id", "openid", "user_card_code", "point_balance" },
            "积分余额同步请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<BrandMemberPointsSyncResponse>().Should().BeEquivalentTo(new[] { "out_request_no", "brand_id", "card_id", "openid", "user_card_code", "point_balance" },
            "积分余额同步应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>BCM-B4：判错面 + AOT 上下文登记 + DI 与模块接线（11 请求型 / 15 应答型）。</summary>
    [Fact]
    public void BrandCardMemberDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(73, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            BrandCardMemberJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 BrandCardMemberJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 BrandCardMember");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(BrandMemberCardResponse), typeof(BrandMemberCardListResponse), typeof(BrandMemberCardResponse), typeof(BrandMemberCardResponse), typeof(BrandMemberCardResponse), typeof(BrandMemberPreAuthTokenResponse), typeof(BrandMemberUserCardResponse), typeof(BrandMemberUserCardListResponse), typeof(BrandMemberUserCardResponse), typeof(BrandMemberUserCardResponse), typeof(BrandMemberUserCardResponse), typeof(BrandMemberUserCardResponse), typeof(BrandMemberUserFeedCreateResponse), typeof(BrandMemberPointsCouponConfirmResponse), typeof(BrandMemberPointsSyncResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(BrandMemberCardCreateRequest), typeof(BrandMemberCardUpdateRequest), typeof(BrandMemberCardInvalidateRequest), typeof(BrandMemberPreAuthTokenRequest), typeof(BrandMemberUserCardUpdateRequest), typeof(BrandMemberUserCardConfirmRequest), typeof(BrandMemberUserCardInvalidateRequest), typeof(BrandMemberUserCardImportRequest), typeof(BrandMemberUserFeedCreateRequest), typeof(BrandMemberPointsCouponConfirmRequest), typeof(BrandMemberPointsSyncRequest) })
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("BrandCardMemberJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 BrandCardMember 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddBrandCardMemberWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 BrandCardMember 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayBrandCardMemberService.{methodName} 必须存在");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] RequiredQueryOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<QueryAttribute>() != null && !p.HasDefaultValue)
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name!)
            .ToArray();

    private static MethodInfo[] Methods()
        => Service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] Routes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] Verbs()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] DistinctVerbRoutes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => $"{a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant()} {a.RequestUri}"))
            .Distinct().ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(WechatPayResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null).Select(static n => n!).ToArray();

    private static string[] DeclaredJsonNamesOf<T>() => JsonNamesOf(typeof(T));

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(Root, $"{projectName}.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
