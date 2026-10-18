// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 订单路径申请」子域 SDK（<b>1 端点</b>：申请设置订单页 path 信息）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：<c>component_access_token</c>（Query 参数名 <c>access_token</c>，2026-10-17 逐页取证官方「可使用 component_access_token」）。</para>
/// <para><b>与授权方 <see cref="IWechatOpenPlatformMpOrderPathService"/> 令牌归属相反</b>：同族 <c>getorderpathinfo</c> 用 <c>authorizer_access_token</c>、
/// <c>applysetorderpathinfo</c> 用 <c>component_access_token</c> ⇒ 拆两支接口/注册组（对齐 <c>Ams</c>/<c>AmsShare</c> 按令牌拆组先例）。</para>
/// <para>申请结果异步经 <c>order_path_audit_result_notify</c> 推送回授权事件接收 URL；本接口同步仅回 <c>errcode</c>/<c>errmsg</c>。一次 <c>appid_list</c> 不超过 100 个（超限报 <c>61042</c>）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramTemplate", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpOrderPathSetService
{
    /// <summary>
    /// 申请设置订单页 path 信息（<c>applysetorderpathinfo</c>，恒 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/applySetOrderPathInfo.html"/>。
    /// </summary>
    [Post("/wxa/security/applysetorderpathinfo")]
    Task<OpenPlatformResponse> ApplySetOrderPathInfoAsync(
        [Body] OpenPlatformMpOrderPathSetRequest request,
        CancellationToken cancellationToken = default);
}
