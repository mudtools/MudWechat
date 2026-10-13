// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.RealName;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 小程序线「实名认证」域接口（1 端点）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cityservice/basic/api_checkrealnameinfo.html"/>。</para>
/// <para>权限集未明确；不支持第三方平台代调用；官方未给出端点级频率数字。</para>
/// </remarks>
[Token(TokenType = MpTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
[HttpClientApi(RegistryGroupName = "RealName", TokenManage = nameof(IMpAppManager))]
public interface IWxaRealNameService
{
    /// <summary>校验实名信息（业务方页面获得 code 之后，通过该接口进行实名信息的校验）。</summary>
    /// <param name="request">请求体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>校验结果（含校验后的 openid 与真实姓名）。</returns>
    [Post("/intp/realname/checkrealnameinfo")]
    Task<WxaRealNameCheckResponse> CheckRealNameInfoAsync([Body] WxaRealNameCheckRequest request, CancellationToken cancellationToken = default);
}
