// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Enums;

/// <summary>
/// 企业微信业务错误码（HTTP 200 + errcode 通道）。
/// </summary>
/// <remarks>
/// 令牌失效码集合 <c>{40014, 42001, 42007, 42009, 42011}</c> 与
/// <c>WechatTokenInvalidationDetector</c> 的判定集合保持一致（Mud.HttpUtils v2.0.9
/// errcode 恢复链路）；其余非零 errcode 走业务异常（<see cref="Exceptions.WechatWorkException"/>）。
/// 实施时以官方错误码表为准（如 41001 官方语义为「缺少 access_token 参数」）。
/// </remarks>
public static class WechatErrorCodes
{
    /// <summary>成功。</summary>
    public const int Success = 0;

    /// <summary>access_token 失效 / 无效。</summary>
    public const int InvalidAccessToken = 40014;

    /// <summary>access_token 过期。</summary>
    public const int ExpiredAccessToken = 42001;

    /// <summary>相关 access_token 失效（部分接口）。</summary>
    public const int RelatedAccessTokenInvalid = 42007;

    /// <summary>suite_access_token 失效/过期。</summary>
    public const int InvalidSuiteAccessToken = 42009;

    /// <summary>provider_access_token 失效/过期。</summary>
    public const int InvalidProviderAccessToken = 42011;

    /// <summary>suite_ticket 不存在/无效。</summary>
    public const int InvalidSuiteTicket = 61028;

    /// <summary>缺少 access_token 参数 / 凭证缺失（永久授权码无效等场景）。</summary>
    public const int MissingAccessToken = 41001;
}
