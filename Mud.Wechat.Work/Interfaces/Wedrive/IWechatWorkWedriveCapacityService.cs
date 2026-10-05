// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedrive;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微盘」模块版本和容量管理域公共 SDK（获取盘专业版信息 + 获取盘容量信息）。
/// <para>
/// 官方单文档页承载 2 个端点（获取盘专业版信息 <c>mng_pro_info</c>、获取盘容量信息 <c>mng_capacity</c>），
/// 对三类应用开放一致，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedriveCapacityService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedriveCapacityService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedriveCapacityService"/>。
/// </para>
/// <para>官方契约陷阱：两端点请求包体官方均为空对象 <c>{}</c>，本 SDK 不声明请求 DTO 与 [Body] 参数
///（对齐 get_openid_migration 无请求体先例）。</para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)</c> 建立「应用 + 企业」作用域后再调用。
/// 官方权限口径：企业需要使用「微盘」secret 所获取的 accesstoken 来调用；自建应用需配置到「可调用应用」列表中；
/// 第三方应用与服务商代开发应用需具有「微盘」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedriveCapacityService
{
    /// <summary>
    /// 获取盘专业版信息
    /// <para>获取企业微盘专业版开通状态、专业版账号用量与到期时间。</para>
    /// <para>官方契约：请求包体为空对象 <c>{}</c>，无请求参数。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否专业版（is_pro）与专业版账号用量（total_vip_acct_num / use_vip_acct_num）、专业版到期时间（pro_expire_time，时间戳精确到秒）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95856"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95861"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96849"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/mng_pro_info")]
    Task<GetWedriveProInfoResponse> GetProInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取盘容量信息
    /// <para>获取企业微盘的全员容量与专业容量总数。</para>
    /// <para>官方契约：请求包体为空对象 <c>{}</c>，无请求参数。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>全员容量总数（total_capacity_for_all，单位 B）与专业容量总数（total_capacity_for_vip，单位 B）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95856"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95861"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96849"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/mng_capacity")]
    Task<GetWedriveCapacityInfoResponse> GetCapacityInfoAsync(
        CancellationToken cancellationToken = default);
}
