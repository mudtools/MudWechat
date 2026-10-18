// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「代小程序管理 · 直播能力开通」子域 SDK（<b>1 端点</b>：申请开通小程序直播）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌面</b>：官方明写「本接口支持「第三方平台」账号类型代调用……其他账号类型如无特殊说明，
/// 均不可调用」⇒ 以 <c>authorizer_access_token</c> 承载，Query 参数名 <c>access_token</c>；
/// 权限集 id 52，不得由小程序线声明（守卫 NC2 锁定两线归属）。
/// </para>
/// <para>
/// <b>结果是异步的</b>：接口成功仅表示申请已受理，审核结果经 <c>wxa_live_apply_event</c>
/// 消息推送下发；<c>errcode=2</c> 表示近 90 天无支付行为、暂不满足开通条件。
/// </para>
/// <para><b>频率与维护</b>：100 次/天；<c>268489001</c> 表示系统维护中，可稍后重试。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MiniProgramManage", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformMpLiveService
{
    /// <summary>
    /// 申请开通小程序直播（<c>POST /wxa/business/applyliveinfo</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/miniprogram-management/live-player/api_applylivelnfo.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformMpApplyLiveInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>受理结果与操作类型，见 <see cref="OpenPlatformMpApplyLiveInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>非幂等</b>：重复提交会消耗每日 100 次配额，调用方须自行去重。</para>
    /// </remarks>
    [Post("/wxa/business/applyliveinfo")]
    Task<OpenPlatformMpApplyLiveInfoResponse> ApplyLiveInfoAsync(
        [Body] OpenPlatformMpApplyLiveInfoRequest request,
        CancellationToken cancellationToken = default);
}
