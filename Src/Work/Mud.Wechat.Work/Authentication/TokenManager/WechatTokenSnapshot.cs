// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// 持久化用令牌快照（SDK 自有 DTO，与框架 <see cref="CredentialToken"/> 互转）。
/// </summary>
public sealed class WechatTokenSnapshot
{
    /// <summary>创建令牌快照。</summary>
    public WechatTokenSnapshot(string accessToken, long expire, long issuedAt)
    {
        AccessToken = accessToken;
        Expire = expire;
        IssuedAt = issuedAt;
    }

    /// <summary>访问令牌。</summary>
    public string AccessToken { get; }

    /// <summary>过期时间（Unix 毫秒时间戳）。</summary>
    public long Expire { get; }

    /// <summary>签发时间（Unix 毫秒时间戳）。</summary>
    public long IssuedAt { get; }
}
