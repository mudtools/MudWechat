// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// Query 令牌注入白名单守卫（**全量集合的唯一持有者**）。
/// </summary>
/// <remarks>
/// <para>
/// 公众号官方契约强制令牌走 Query 参数（<c>access_token</c>，官方不支持 Header 注入）⇒
/// 这是 MUD005 的已知接受风险，必须由白名单锁定「哪些接口存在该风险面」。
/// </para>
/// <para>
/// <b>为何单点持有</b>：曾把全量集合分别写在基础 / 标签 / 用户 / 菜单 / 客服五个域的守卫里，
/// 结果是<b>每新增一个域就要同步改 N 处</b>（改漏即红、且属纯机械改动）。现改为：
/// 全量集合只在本守卫断言一次；各域守卫只断言「本域在内」（防「整个白名单为空」的静默空跑）。
/// </para>
/// </remarks>
public class MpQueryTokenWhitelistGuard
{
    /// <summary>契约守卫 QT1：Query 令牌注入接口白名单（新增须先评估、再显式扩展本表）。</summary>
    [Fact]
    public void QueryTokenInjectionInterfaces_ShouldMatchWhitelist()
    {
        var actual = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        actual.Should().BeEquivalentTo(new[]
        {
            nameof(IMpBasicService),              // 基础接口（3 端点）
            nameof(IMpTagService),                // 用户管理·标签管理（8 端点）
            nameof(IMpUserService),               // 用户管理·用户信息 + 转换 openid（8 端点）
            nameof(IMpMenuService),               // 自定义菜单（7 端点）
            nameof(IMpCustomerMessageService),    // 客服消息·客服消息（3 端点）
            nameof(IMpKfAccountService),          // 客服消息·客服管理（7 端点）
            nameof(IMpKfSessionService),          // 客服消息·会话控制（5 端点）
            nameof(IMpMediaService),              // 素材管理·上传通道（下载通道 IMpMediaDownloadService 无 [Token]，手工注入）
            nameof(IMpTemplateService),           // 模板消息（7 端点，服务号专属）
            nameof(IMpSubscriptionNoticeService), // 订阅通知（7 端点，服务号专属；bizsend + /wxaapi/newtmpl/*）
            nameof(IMpOpenApiService),            // openApi 管理（4 端点；clear_quota/v2 免令牌接口 IMpOpenApiTokenFreeService 无 [Token]，不入白名单）
            nameof(IMpMassMessageService),        // 群发消息（7 端点；uploadnews 废弃不建模、uploadimg 归素材域）
            nameof(IMpQrcodeService),             // 带参二维码（1 端点，服务号专属）
            nameof(IMpAutoReplyService),          // 自动回复（1 端点只读查询）
            nameof(IMpDraftService),              // 草稿管理（6 端点；draft/switch 废弃不实现）
            nameof(IMpFreePublishService),        // 发布能力（5 端点，仅认证）
            nameof(IMpProductCardService),        // 商品卡片（1 端点；/channels/ec/ 前缀）
            nameof(IMpCommentService),            // 留言管理（8 端点，仅认证 + 留言权限）
            nameof(IMpDataCubeService),           // 数据统计（21 端点单域承载，仅认证）
            nameof(IMpSmartApiService),           // 智能接口（12 端点：AI 语音 3 + OCR 7 + 图像处理 2）
            nameof(IMpQrcodeJumpService),         // 扫二维码打开小程序（4 端点，服务号专属）
            nameof(IMpShortLinkService),          // 长信息与短链（2 端点）
            nameof(IMpStoreService),              // 微信门店·门店小程序（12 端点；开放面仅电商类目）
            nameof(IMpOneCodeService),            // 微信「一物一码」（6 端点；服务号需申请）
            nameof(IMpInvoiceService),            // 微信发票（17 端点；全消费 access_token，不引入 api_ticket）
        }, "公众号官方契约强制 Query 注入（MUD005 已知接受风险）；新增 Query 注入接口须先评估再显式扩展本白名单");

        // 防静默空跑：白名单非空且每条均为 Query 注入（若发现机制失效，上面 BeEquivalentTo 会退化为真空断言）。
        actual.Should().NotBeEmpty();
    }

    /// <summary>
    /// 契约守卫 QT1（小程序线部分）：小程序 Query 令牌注入白名单。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为何在同一守卫内断言两条线</b>：本守卫是 Query 令牌注入白名单的<b>全量集合唯一持有者</b>
    /// （见类型 remarks）。小程序与公众号<b>同属微信公众平台、同一令牌域、同一 Query 注入契约</b>
    /// （设计方案 §3.2/§3.3），若把小程序白名单另放一处，就重新制造了「多处持有、改漏即红」的问题。
    /// </para>
    /// <para>
    /// <b>登录 <c>code2Session</c> 不在白名单内</b>：它<b>免令牌</b>（以 <c>appid</c> + <c>secret</c> 换用户级会话，
    /// 不消费应用级 <c>access_token</c>），刻意不声明 <c>[Token]</c> —— 声明之反而会把应用级令牌错误注入该请求。
    /// 同理，小程序码三端点走 <c>IWxaCodeService</c> 手工通道（无 <c>[Token]</c> 特性，手工拼 Query），
    /// 反馈图片走 <c>IWxaFeedbackMediaService</c> 手工通道（同形态），也不入白名单。
    /// </para>
    /// </remarks>
    [Fact]
    public void QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine()
    {
        var actual = typeof(Mud.Wechat.MiniProgram.Extensions.MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        actual.Should().BeEquivalentTo(new[]
        {
            nameof(Mud.Wechat.MiniProgram.IWxaAuthService),              // 登录与用户（7 端点；code2Session 免令牌独立接口）
            nameof(Mud.Wechat.MiniProgram.IWxaChargeService),            // 付费管理（2 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaDataAnalysisService),      // 数据分析（11 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaDynamicMessageService),    // 动态消息（3 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaFaceVerifyService),        // 微信人脸核身（2 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaHardwareDeviceService),    // 硬件设备（9 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaKfService),                // 客服（9 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaLaborUseService),          // 用工关系（2 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaNearbyPoiService),         // 附近小程序（4 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaOperationService),         // 运维中心（9 端点；反馈图片 IWxaFeedbackMediaService 手工通道不入白名单）
            nameof(Mud.Wechat.MiniProgram.IWxaPluginService),            // 插件管理（2 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaQrCodeLinkService),        // 二维码 / 链接 JSON 通道（6 端点；图片通道 IWxaCodeService 手工注入）
            nameof(Mud.Wechat.MiniProgram.IWxaRedPacketCoverService),    // 微信红包封面（1 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaSearchService),            // 微信搜一搜（1 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaSecurityService),          // 内容安全（3 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaServiceMarketService),     // 微信服务市场（2 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaSoterService),             // 生物认证（1 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaStudentService),           // 学生身份（1 端点）
            nameof(Mud.Wechat.MiniProgram.IWxaSubscribeMessageService),  // 订阅消息 / 服务卡片（4 端点）
        }, "小程序官方契约同样强制 Query 注入；新增 Query 注入接口须先评估再显式扩展本白名单");

        actual.Should().NotBeEmpty();
    }

    /// <summary>契约守卫 QT2：白名单接口必须全部声明 <c>[Token]</c> 的 <c>access_token</c> 参数名（官方契约）。</summary>
    [Fact]
    public void QueryTokenInterfaces_ShouldDeclareOfficialParameterName()
    {
        foreach (var iface in new[]
                 {
                     typeof(IMpBasicService), typeof(IMpTagService), typeof(IMpUserService),
                     typeof(IMpMenuService), typeof(IMpCustomerMessageService),
                 })
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
            token.TokenType.Should().Be(MpTokenTypes.AccessToken);
        }
    }
}
