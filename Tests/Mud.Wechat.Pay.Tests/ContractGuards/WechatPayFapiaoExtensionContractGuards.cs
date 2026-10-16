// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Fapiao;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「FapiaoExtension」契约守卫 —— 本批 <b>13 端点</b>（<b>追加</b>进既存接口，该接口实测 18 方法：本批 13 + 既存 5 由 WechatPayFapiaoContractGuards（FP1~FP4） 锁定）（注册组 <c>NewTaxControlFapiao</c>，接口 <c>IWechatPayFapiaoService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.Fapiao</c>）。
/// </summary>
/// <remarks>
/// <para><b>本守卫只覆盖 P1-5-B 新补的 13 支</b>：既存 5 支（开具 / 查询 / 冲红 / 下载信息 / 插卡）连同 <c>multipart</c> 上传通道由 <c>WechatPayFapiaoContractGuards</c>（FP1~FP4）锁定 ⇒ B1 的方法数断言写成 <c>13 + 5</c> 是<b>同一接口体</b> 的实测总数，不是本批规模；两支共用 <c>IWechatPayFapiaoService</c>、同一注册组 <c>NewTaxControlFapiao</c>、 同一 <c>NewTaxControlFapiaoJsonContext</c>，<b>不</b>另建接口或注册组。</para>
/// <para><b>一支路由多身份页 ⇒ 并集建模，一面一方法</b>：13 支中 <c>card-template</c>（商户 + 服务商 + 新服务商页）、 <c>base-information</c> / <c>development-config</c>（GET 与 PATCH 各两到三页）/ <c>tax-codes</c> / <c>user-title</c> / <c>user-title/title-url</c> 官方各印 <b>2–3 页</b> ⇒ 请求与应答字段取<b>并集</b>，仅某页印出的字段与「某页标必填」 口径写进 DTO 注释（B3 名册 + <c>onlyOn</c> / <c>reqNote</c>），<b>不得</b>按身份拆成两套方法或类型。</para>
/// <para><b>子商户定位键两名并存，不得互换</b>：多数支用 <c>sub_mchid</c>（query 或 path）， 但「查询开发选项」与「配置开发选项」两支官方印的是 <c>sub_mch_code</c> ⇒ 照页建模， 把 <c>sub_mch_code</c> 统一成 <c>sub_mchid</c> 即造出官方不存在的参数（B2 的 query 名册逐支锁定）。</para>
/// <para><b>三支「无应答包体」不建响应类型</b>：两支开具（<c>issue-general</c> / <c>real-estate-leasing</c>， 正常示例 <b>202 Accepted</b>）与「检查子商户电子发票功能状态」（<b>204 No Content</b>）⇒ 返回裸 <see cref="Task"/>； <b>202 只代表受理</b>，开票结果须经【查询电子发票】或开票回调判定。</para>
/// <para><b>清单↔官方页两处勘正已落实</b>：① <c>merchant/{sub_mchid}/check-status</c> 的清单动词原记 <c>POST</c>， 现行官方页为 <b>GET</b> ⇒ 照页取 GET；② 处置表把 <c>partner/4015792563</c>（配置开发选项·服务商）记在 <c>base-information</c> 行下，属<b>张冠李戴</b> ⇒ 该页实际归 <c>PATCH development-config</c>， 其独有字段 <c>support_vat_fapiao</c> 也只在该支出现。</para>
/// <para><b>服务商新面必填 query 不得给默认值</b>：<c>getspinviteurl</c> 的 <c>operation_type</c> / <c>fapiao_mode</c> / <c>invite_code</c>，<c>listspinvitemchinfo</c> 的 <c>query_time_start</c> / <c>query_time_end</c> / <c>offset</c> / <c>limit</c> / <c>mch_invite_status</c>，<c>tax-codes</c> 的 <c>offset</c> / <c>limit</c>，<c>user-title</c> 的 <c>fapiao_apply_id</c> / <c>scene</c>，<c>title-url</c> 的 <c>fapiao_apply_id</c> / <c>appid</c> / <c>openid</c> / <c>total_amount</c> / <c>source</c> 皆为官方<b>必填</b> ⇒ B2 的 <c>RequiredQueryOf</c> 逐支锁定，声明成可选即把 400 变成静默错参。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayFapiaoExtensionContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Fapiao";

    private static readonly Type[] Services = { typeof(IWechatPayFapiaoService) };

    /// <summary>FPX-B1：拓扑 —— 注册组 NewTaxControlFapiao、本批 13 方法（接口实测 18 = 既存 5 + 本批 13）、13 条 (动词, 路由)；动词集 Post / Get / Patch。</summary>
    [Fact]
    public void FapiaoExtensionTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayFapiaoService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("NewTaxControlFapiao", "组名即源生成注册入口名（改组名即改 AddNewTaxControlFapiaoWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(18, "FapiaoExtension 覆盖的接口声明式方法数（既存 5 支 + 本批 13 支）");
        DistinctVerbRoutes().Should().HaveCount(18, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[]{ "PostAttribute", "GetAttribute", "PatchAttribute" }, "动词集照官方页面逐支登记：本域写动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由为 APIv3 形态（官方文档树存在 v2 版式页，但请求 URL 仍印 /v3 前缀）");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayTransactionsService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>FPX-B2：本批 13 支端点的路由、path 键、query 名册、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void FapiaoExtensionEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 创建电子发票卡券模板｜官方页 v3/merchant/4012529382 + v3/partner/4012474026 + v3/partner/4015792562｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.SetFapiaoCardTemplateAsync)).Should().Be("/v3/new-tax-control-fapiao/card-template");
        PathKeysOf(nameof(IWechatPayFapiaoService.SetFapiaoCardTemplateAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.SetFapiaoCardTemplateAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayFapiaoService.SetFapiaoCardTemplateAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayFapiaoService.SetFapiaoCardTemplateAsync)).Should().Be(typeof(FapiaoCardTemplateRequest));
        ReturnTypeOf(nameof(IWechatPayFapiaoService.SetFapiaoCardTemplateAsync)).Should().Be(typeof(FapiaoCardTemplateResponse));

        // 开具通用行业电子发票｜官方页 v3/partner/4015792574｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.IssueGeneralFapiaoAsync)).Should().Be("/v3/new-tax-control-fapiao/fapiao-applications/issue-general");
        PathKeysOf(nameof(IWechatPayFapiaoService.IssueGeneralFapiaoAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.IssueGeneralFapiaoAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayFapiaoService.IssueGeneralFapiaoAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayFapiaoService.IssueGeneralFapiaoAsync)).Should().Be(typeof(FapiaoGeneralIssueRequest));
        ReturnTypeOf(nameof(IWechatPayFapiaoService.IssueGeneralFapiaoAsync)).Should().Be(typeof(Task), "开具通用行业电子发票官方应答体无业务字段（原文「无应答包体」/「无参数返回」，正常示例 204） ⇒ 裸 Task");

        // 开具不动产租赁行业电子发票｜官方页 v3/partner/4015941740｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.IssueRealEstateLeasingFapiaoAsync)).Should().Be("/v3/new-tax-control-fapiao/fapiao-applications/real-estate-leasing");
        PathKeysOf(nameof(IWechatPayFapiaoService.IssueRealEstateLeasingFapiaoAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.IssueRealEstateLeasingFapiaoAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayFapiaoService.IssueRealEstateLeasingFapiaoAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayFapiaoService.IssueRealEstateLeasingFapiaoAsync)).Should().Be(typeof(FapiaoRealEstateLeasingIssueRequest));
        ReturnTypeOf(nameof(IWechatPayFapiaoService.IssueRealEstateLeasingFapiaoAsync)).Should().Be(typeof(Task), "开具不动产租赁行业电子发票官方应答体无业务字段（原文「无应答包体」/「无参数返回」，正常示例 204） ⇒ 裸 Task");

        // 获取服务商电子发票能力邀请链接｜官方页 v3/partner/4015941495｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.GetFapiaoSpInviteUrlAsync)).Should().Be("/v3/new-tax-control-fapiao/fapiaomerchant/getspinviteurl");
        PathKeysOf(nameof(IWechatPayFapiaoService.GetFapiaoSpInviteUrlAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.GetFapiaoSpInviteUrlAsync)).Should().BeEquivalentTo(new[]{ "operation_type", "fapiao_mode", "fapiao_ability_type_list", "invite_channel", "operate_user", "invite_code", "sub_mchid", "taxpayer_id" });
        RequiredQueryOf(nameof(IWechatPayFapiaoService.GetFapiaoSpInviteUrlAsync)).Should().BeEquivalentTo(new[]{ "operation_type", "fapiao_mode", "invite_code" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFapiaoService.GetFapiaoSpInviteUrlAsync)).Should().BeNull("获取服务商电子发票能力邀请链接官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.GetFapiaoSpInviteUrlAsync)).Should().Be(typeof(FapiaoSpInviteUrlResponse));

        // 获取邀请开通的商户信息｜官方页 v3/partner/4015941524｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.ListFapiaoSpInviteMerchantInfoAsync)).Should().Be("/v3/new-tax-control-fapiao/fapiaomerchant/listspinvitemchinfo");
        PathKeysOf(nameof(IWechatPayFapiaoService.ListFapiaoSpInviteMerchantInfoAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.ListFapiaoSpInviteMerchantInfoAsync)).Should().BeEquivalentTo(new[]{ "query_time_start", "query_time_end", "offset", "limit", "invite_code", "mch_invite_status" });
        RequiredQueryOf(nameof(IWechatPayFapiaoService.ListFapiaoSpInviteMerchantInfoAsync)).Should().BeEquivalentTo(new[]{ "query_time_start", "query_time_end", "offset", "limit", "mch_invite_status" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFapiaoService.ListFapiaoSpInviteMerchantInfoAsync)).Should().BeNull("获取邀请开通的商户信息官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.ListFapiaoSpInviteMerchantInfoAsync)).Should().Be(typeof(FapiaoSpInviteMerchantListResponse));

        // 检查子商户电子发票功能状态｜官方页 v3/partner/4012474022｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.CheckFapiaoMerchantAsync)).Should().Be("/v3/new-tax-control-fapiao/merchant/{subMchid}/check");
        PathKeysOf(nameof(IWechatPayFapiaoService.CheckFapiaoMerchantAsync)).Should().BeEquivalentTo(new[]{ "subMchid" });
        QueryNamesOf(nameof(IWechatPayFapiaoService.CheckFapiaoMerchantAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayFapiaoService.CheckFapiaoMerchantAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayFapiaoService.CheckFapiaoMerchantAsync)).Should().BeNull("检查子商户电子发票功能状态官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.CheckFapiaoMerchantAsync)).Should().Be(typeof(Task), "检查子商户电子发票功能状态官方应答体无业务字段（原文「无应答包体」/「无参数返回」，正常示例 204） ⇒ 裸 Task");

        // 查询受邀商户开票能力开通状态｜官方页 v3/partner/4015792561｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantCheckStatusAsync)).Should().Be("/v3/new-tax-control-fapiao/merchant/{subMchid}/check-status");
        PathKeysOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantCheckStatusAsync)).Should().BeEquivalentTo(new[]{ "subMchid" });
        QueryNamesOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantCheckStatusAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantCheckStatusAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantCheckStatusAsync)).Should().BeNull("查询受邀商户开票能力开通状态官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantCheckStatusAsync)).Should().Be(typeof(FapiaoMerchantCheckStatusResponse));

        // 查询商户开票基础信息｜官方页 v3/merchant/4012538180 + v3/partner/4012474053｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantBaseInformationAsync)).Should().Be("/v3/new-tax-control-fapiao/merchant/base-information");
        PathKeysOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantBaseInformationAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantBaseInformationAsync)).Should().BeEquivalentTo(new[]{ "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantBaseInformationAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantBaseInformationAsync)).Should().BeNull("查询商户开票基础信息官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantBaseInformationAsync)).Should().Be(typeof(FapiaoMerchantBaseInformationResponse));

        // 查询商户开发选项｜官方页 v3/merchant/4012529526 + v3/partner/4012474033｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantDevelopmentConfigAsync)).Should().Be("/v3/new-tax-control-fapiao/merchant/development-config");
        PathKeysOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantDevelopmentConfigAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantDevelopmentConfigAsync)).Should().BeEquivalentTo(new[]{ "sub_mch_code" });
        RequiredQueryOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantDevelopmentConfigAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantDevelopmentConfigAsync)).Should().BeNull("查询商户开发选项官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoMerchantDevelopmentConfigAsync)).Should().Be(typeof(FapiaoMerchantDevelopmentConfigResponse));

        // 配置商户开发选项｜官方页 v3/merchant/4012529457 + v3/partner/4012474031 + v3/partner/4015792563｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.UpdateFapiaoMerchantDevelopmentConfigAsync)).Should().Be("/v3/new-tax-control-fapiao/merchant/development-config");
        PathKeysOf(nameof(IWechatPayFapiaoService.UpdateFapiaoMerchantDevelopmentConfigAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.UpdateFapiaoMerchantDevelopmentConfigAsync)).Should().BeEmpty();
        RequiredQueryOf(nameof(IWechatPayFapiaoService.UpdateFapiaoMerchantDevelopmentConfigAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayFapiaoService.UpdateFapiaoMerchantDevelopmentConfigAsync)).Should().Be(typeof(FapiaoMerchantDevelopmentConfigUpdateRequest));
        ReturnTypeOf(nameof(IWechatPayFapiaoService.UpdateFapiaoMerchantDevelopmentConfigAsync)).Should().Be(typeof(FapiaoMerchantDevelopmentConfigUpdateResponse));

        // 查询商户税收分类编码对照表｜官方页 v3/merchant/4012538236 + v3/partner/4012474057｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.QueryFapiaoTaxCodesAsync)).Should().Be("/v3/new-tax-control-fapiao/merchant/tax-codes");
        PathKeysOf(nameof(IWechatPayFapiaoService.QueryFapiaoTaxCodesAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.QueryFapiaoTaxCodesAsync)).Should().BeEquivalentTo(new[]{ "offset", "limit", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayFapiaoService.QueryFapiaoTaxCodesAsync)).Should().BeEquivalentTo(new[]{ "offset", "limit" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoTaxCodesAsync)).Should().BeNull("查询商户税收分类编码对照表官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoTaxCodesAsync)).Should().Be(typeof(FapiaoTaxCodeListResponse));

        // 获取用户填写抬头信息｜官方页 v3/merchant/4012538112 + v3/partner/4012474048 + v3/partner/4015784260｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.QueryFapiaoUserTitleAsync)).Should().Be("/v3/new-tax-control-fapiao/user-title");
        PathKeysOf(nameof(IWechatPayFapiaoService.QueryFapiaoUserTitleAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.QueryFapiaoUserTitleAsync)).Should().BeEquivalentTo(new[]{ "fapiao_apply_id", "scene", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayFapiaoService.QueryFapiaoUserTitleAsync)).Should().BeEquivalentTo(new[]{ "fapiao_apply_id", "scene" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoUserTitleAsync)).Should().BeNull("获取用户填写抬头信息官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.QueryFapiaoUserTitleAsync)).Should().Be(typeof(FapiaoUserTitleResponse));

        // 获取用户抬头填写链接｜官方页 v3/merchant/4012538106 + v3/partner/4012474040 + v3/partner/4015770776｜身份 【普通商户】【普通服务商】
        RouteOf(nameof(IWechatPayFapiaoService.GetFapiaoUserTitleUrlAsync)).Should().Be("/v3/new-tax-control-fapiao/user-title/title-url");
        PathKeysOf(nameof(IWechatPayFapiaoService.GetFapiaoUserTitleUrlAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayFapiaoService.GetFapiaoUserTitleUrlAsync)).Should().BeEquivalentTo(new[]{ "fapiao_apply_id", "appid", "openid", "total_amount", "seller_name", "show_phone_cell", "must_input_phone", "show_email_cell", "must_input_email", "source", "sub_mchid" });
        RequiredQueryOf(nameof(IWechatPayFapiaoService.GetFapiaoUserTitleUrlAsync)).Should().BeEquivalentTo(new[]{ "fapiao_apply_id", "appid", "openid", "total_amount", "source" }, "官方必填 query：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayFapiaoService.GetFapiaoUserTitleUrlAsync)).Should().BeNull("获取用户抬头填写链接官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayFapiaoService.GetFapiaoUserTitleUrlAsync)).Should().Be(typeof(FapiaoUserTitleUrlResponse));
    }

    /// <summary>FPX-B3：官方字段表顶层名册（请求体与应答，逐字照页；嵌套子树另立类型）。</summary>
    [Fact]
    public void FapiaoExtensionDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<FapiaoCardTemplateRequest>().Should().BeEquivalentTo(new[]{ "card_appid", "card_template_information", "sub_mchid" }, "创建电子发票卡券模板请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoGeneralIssueRequest>().Should().BeEquivalentTo(new[]{ "sub_mchid", "fapiao_apply_id", "buyer_information", "fapiao_information" }, "开具通用行业电子发票请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoRealEstateLeasingIssueRequest>().Should().BeEquivalentTo(new[]{ "sub_mchid", "fapiao_apply_id", "buyer_information", "fapiao_information" }, "开具不动产租赁行业电子发票请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoMerchantDevelopmentConfigUpdateRequest>().Should().BeEquivalentTo(new[]{ "callback_url", "show_fapiao_cell", "sub_mch_code", "support_vat_fapiao" }, "配置商户开发选项请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoCardTemplateResponse>().Should().BeEquivalentTo(new[]{ "card_appid", "card_id" }, "创建电子发票卡券模板应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoSpInviteUrlResponse>().Should().BeEquivalentTo(new[]{ "invite_url", "miniprogram_appid", "miniprogram_path" }, "获取服务商电子发票能力邀请链接应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoSpInviteMerchantListResponse>().Should().BeEquivalentTo(new[]{ "total_count", "offset", "limit", "links", "mch_invite_result_list" }, "获取邀请开通的商户信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoMerchantCheckStatusResponse>().Should().BeEquivalentTo(new[]{ "sub_mchid", "third_mode", "digital_tax_mode" }, "查询受邀商户开票能力开通状态应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoMerchantBaseInformationResponse>().Should().BeEquivalentTo(new[]{ "seller_information", "extra_information" }, "查询商户开票基础信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoMerchantDevelopmentConfigResponse>().Should().BeEquivalentTo(new[]{ "callback_url", "show_fapiao_cell" }, "查询商户开发选项应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoMerchantDevelopmentConfigUpdateResponse>().Should().BeEquivalentTo(new[]{ "callback_url", "show_fapiao_cell", "support_vat_fapiao" }, "配置商户开发选项应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoTaxCodeListResponse>().Should().BeEquivalentTo(new[]{ "offset", "limit", "links", "data", "total_count" }, "查询商户税收分类编码对照表应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoUserTitleResponse>().Should().BeEquivalentTo(new[]{ "type", "name", "taxpayer_id", "address", "telephone", "bank_name", "bank_account", "phone", "email", "amount", "out_trade_no", "fapiao_bill_type", "user_apply_message" }, "获取用户填写抬头信息应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<FapiaoUserTitleUrlResponse>().Should().BeEquivalentTo(new[]{ "miniprogram_appid", "miniprogram_path", "miniprogram_user_name" }, "获取用户抬头填写链接应答顶层字段名册（官方应答表逐字，嵌套子树另立类型）");
    }

    /// <summary>FPX-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void FapiaoExtensionDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoCardTemplateResponse)).Should().BeTrue("FapiaoCardTemplateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoSpInviteUrlResponse)).Should().BeTrue("FapiaoSpInviteUrlResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoSpInviteMerchantListResponse)).Should().BeTrue("FapiaoSpInviteMerchantListResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoMerchantCheckStatusResponse)).Should().BeTrue("FapiaoMerchantCheckStatusResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoMerchantBaseInformationResponse)).Should().BeTrue("FapiaoMerchantBaseInformationResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoMerchantDevelopmentConfigResponse)).Should().BeTrue("FapiaoMerchantDevelopmentConfigResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoMerchantDevelopmentConfigUpdateResponse)).Should().BeTrue("FapiaoMerchantDevelopmentConfigUpdateResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoTaxCodeListResponse)).Should().BeTrue("FapiaoTaxCodeListResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoUserTitleResponse)).Should().BeTrue("FapiaoUserTitleResponse 必须承载官方 code/message（判错面）");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoUserTitleUrlResponse)).Should().BeTrue("FapiaoUserTitleUrlResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoCardTemplateRequest)).Should().BeFalse("FapiaoCardTemplateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoGeneralIssueRequest)).Should().BeFalse("FapiaoGeneralIssueRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoRealEstateLeasingIssueRequest)).Should().BeFalse("FapiaoRealEstateLeasingIssueRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(FapiaoMerchantDevelopmentConfigUpdateRequest)).Should().BeFalse("FapiaoMerchantDevelopmentConfigUpdateRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>FPX-B5：本批 DTO 全量形态 —— 35 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void FapiaoExtensionAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = new[]
        {
            typeof(FapiaoCardTemplateRequestCardTemplateInformationCustomCell),
            typeof(FapiaoCardTemplateRequestCardTemplateInformation),
            typeof(FapiaoCardTemplateRequest),
            typeof(FapiaoCardTemplateResponse),
            typeof(FapiaoGeneralIssueRequestBuyerInformation),
            typeof(FapiaoGeneralIssueRequestFapiaoInformationItemsItem),
            typeof(FapiaoGeneralIssueRequestFapiaoInformationTransactionInformationItem),
            typeof(FapiaoGeneralIssueRequestFapiaoInformation),
            typeof(FapiaoGeneralIssueRequest),
            typeof(FapiaoRealEstateLeasingIssueRequestBuyerInformation),
            typeof(FapiaoRealEstateLeasingIssueRequestFapiaoInformationItemsItemRealEstateLeasingInformation),
            typeof(FapiaoRealEstateLeasingIssueRequestFapiaoInformationItemsItem),
            typeof(FapiaoRealEstateLeasingIssueRequestFapiaoInformationTransactionInformationItem),
            typeof(FapiaoRealEstateLeasingIssueRequestFapiaoInformation),
            typeof(FapiaoRealEstateLeasingIssueRequest),
            typeof(FapiaoMerchantCheckStatusResponseThirdMode),
            typeof(FapiaoMerchantCheckStatusResponseDigitalTaxModeBillingPersonInfoItem),
            typeof(FapiaoMerchantCheckStatusResponseDigitalTaxModeAbilityInfoItem),
            typeof(FapiaoMerchantCheckStatusResponseDigitalTaxMode),
            typeof(FapiaoMerchantCheckStatusResponse),
            typeof(FapiaoMerchantBaseInformationResponseSellerInformation),
            typeof(FapiaoMerchantBaseInformationResponseExtraInformation),
            typeof(FapiaoMerchantBaseInformationResponse),
            typeof(FapiaoMerchantDevelopmentConfigResponse),
            typeof(FapiaoMerchantDevelopmentConfigUpdateRequest),
            typeof(FapiaoMerchantDevelopmentConfigUpdateResponse),
            typeof(FapiaoTaxCodeListResponseLinks),
            typeof(FapiaoTaxCodeListResponseDataItem),
            typeof(FapiaoTaxCodeListResponse),
            typeof(FapiaoSpInviteUrlResponse),
            typeof(FapiaoSpInviteMerchantListResponseLinks),
            typeof(FapiaoSpInviteMerchantListResponseMchInviteResultListItem),
            typeof(FapiaoSpInviteMerchantListResponse),
            typeof(FapiaoUserTitleResponse),
            typeof(FapiaoUserTitleUrlResponse),
        };
        batchDtos.Should().HaveCount(35, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            NewTaxControlFapiaoJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 NewTaxControlFapiaoJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("NewTaxControlFapiao", type.Name + " 的 SerializerClassName 必须为 NewTaxControlFapiao");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    type.Name + " 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in batchDtos.SelectMany(JsonNamesOf))
            name.Should().NotContain(".", name + " 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        // 时间字段一律 string（rfc3339），不得用 DateTime / DateTimeOffset 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in batchDtos)
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
            }
    }

    /// <summary>FPX-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void FapiaoExtensionRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("NewTaxControlFapiao", "PayModule 须有 NewTaxControlFapiao 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddNewTaxControlFapiaoWebApiHttpClient()", "注册委托必须指向源生成方法（组名 NewTaxControlFapiao 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("NewTaxControlFapiaoJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- helpers -------------------------------------------------------------

    private static object? ApiProp(Type service, string name)
    {
        var api = service.GetCustomAttributes(false).Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        return api.GetType().GetProperty(name)!.GetValue(api);
    }

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException(methodName + " 必须存在于本域接口");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull(methodName + " 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static Type? BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Select(p => (p: p, attr: p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "BodyAttribute")))
            .Where(x => x.attr != null)
            .Select(x => x.p.ParameterType)
            .FirstOrDefault();

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
        => Services.SelectMany(static s => s.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .ToArray();

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
            .Select(static a => a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant() + " " + a.RequestUri))
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
        => Directory.EnumerateFiles(RepoRootDir, projectName + ".csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                               && !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string RepoRootDir
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
