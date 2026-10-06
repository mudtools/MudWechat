// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.License;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「接口调用许可」模块自动激活设置域第三方应用 / 服务商代开发 SDK
/// （设置企业的许可自动激活状态 / 查询企业的许可自动激活状态，共 2 个端点）。
/// <para>官方在第三方应用开发与服务商代开发两棵文档树开放本族端点（共享同一端点页），
/// 全部 2 个端点声明于本接口；企业自建应用官方不开放，不设自建子接口。</para>
/// </summary>
/// <remarks>
/// <para>消费服务商级 provider_access_token（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。</para>
/// <para>官方权限口径：操作对象要求服务商为企业购买过接口许可（购买指支付完成，购买并退款成功包括在内）。</para>
/// <para>官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。</para>
/// <para>MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "License",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkLicenseAutoActiveService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyLicenseAutoActiveService : IWechatWorkLicenseAutoActiveService
{
    /// <summary>
    /// 设置企业的许可自动激活状态
    /// <para>服务商可以调用该接口设置授权企业的许可自动激活状态。
    /// 设置为自动激活后，对应授权企业的员工使用服务商应用时，接口许可表现为自动激活。</para>
    /// <para>官方约束：corpid 要求服务商为企业购买过接口许可，
    /// 购买指支付完成，购买并退款成功包括在内。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="SetLicenseAutoActiveStatusRequest"/>：corpid 企业corpid /
    /// auto_active_status 许可自动激活状态）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 errcode / errmsg（官方本端点无业务负载）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97199"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/set_auto_active_status")]
    Task<WechatWorkResponse> SetAutoActiveStatusAsync(
        [Body] SetLicenseAutoActiveStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询企业的许可自动激活状态
    /// <para>服务商可以调用该接口查询授权企业的许可自动激活状态。</para>
    /// <para>官方约束：corpid 要求服务商为企业购买过接口许可才有查询结果。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetLicenseAutoActiveStatusRequest"/>：corpid 查询的企业corpid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>许可自动激活状态（auto_active_status：0 关闭 / 1 打开）。</returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97200"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/get_auto_active_status")]
    Task<GetLicenseAutoActiveStatusResponse> GetAutoActiveStatusAsync(
        [Body] GetLicenseAutoActiveStatusRequest request,
        CancellationToken cancellationToken = default);
}
