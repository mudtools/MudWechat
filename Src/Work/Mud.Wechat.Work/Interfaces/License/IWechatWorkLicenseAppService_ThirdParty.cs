// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.License;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「接口调用许可」模块应用管理域第三方应用 / 服务商代开发 SDK（获取应用的接口许可状态，共 1 个端点）。
/// <para>官方在第三方应用开发与服务商代开发两棵文档树开放本族端点（共享同一端点页），
/// 唯一端点声明于本接口；企业自建应用官方不开放，不设自建子接口。</para>
/// </summary>
/// <remarks>
/// <para>消费服务商级 provider_access_token（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。</para>
/// <para>官方权限口径：企业必须已安装了该第三方应用或者代开发应用才允许调用。</para>
/// <para>官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。</para>
/// <para>MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "License",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkLicenseAppService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyLicenseAppService : IWechatWorkLicenseAppService
{
    /// <summary>
    /// 获取应用的接口许可状态
    /// <para>服务商可获取某个授权企业的应用接口许可试用期，免费试用期为企业首次安装应用后的 90 天。</para>
    /// <para>官方约束：企业必须已安装了该第三方应用或者代开发应用才允许调用；
    /// appid 为旧的多应用套件中的应用 id，新开发者请忽略。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetAppLicenseInfoRequest"/>：corpid 企业id /
    /// suite_id 套件id / appid 旧的多应用套件中的应用id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>license 检查开启状态（license_status）/ 应用 license 试用期信息（trail_info）/
    /// 接口开启拦截校验时间（license_check_time）。
    /// <para>官方口径：trail_info 仅当 license_status 为 1 且应用有试用期时返回该字段
    /// （服务商测试企业、历史迁移应用无试用期）；开始拦截校验后，无接口许可将会被拦截。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用 / 服务商代开发</b>SDK文档（官方两棵文档树共享同一端点页）：<see href="https://developer.work.weixin.qq.com/document/path/97194"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/license/get_app_license_info")]
    Task<GetAppLicenseInfoResponse> GetAppLicenseInfoAsync(
        [Body] GetAppLicenseInfoRequest request,
        CancellationToken cancellationToken = default);
}
