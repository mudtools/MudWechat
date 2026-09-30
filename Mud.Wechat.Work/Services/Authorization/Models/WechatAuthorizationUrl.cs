// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Services.Authorization.Models;

/// <summary>第三方应用授权安装链接（含预授权码与有效期）。</summary>
/// <remarks>纯进程内流转模型，不进 <c>WechatWorkJsonContext</c>。</remarks>
public sealed class WechatAuthorizationUrl
{
    /// <summary>创建授权安装链接。</summary>
    /// <param name="url">拼装完成的授权安装链接。</param>
    /// <param name="preAuthCode">本次使用的预授权码。</param>
    /// <param name="expiresIn">有效期（秒）。</param>
    /// <param name="expireAt">过期绝对时间。</param>
    public WechatAuthorizationUrl(string url, string preAuthCode, int expiresIn, DateTimeOffset expireAt)
    {
        Url = url ?? string.Empty;
        PreAuthCode = preAuthCode ?? string.Empty;
        ExpiresIn = expiresIn;
        ExpireAt = expireAt;
    }

    /// <summary>拼装完成的授权安装链接（<c>open.work.weixin.qq.com/3rdapp/install?...</c>）。</summary>
    public string Url { get; }

    /// <summary>本次使用的预授权码（pre_auth_code，有效期 1200 秒，可复用换取授权链接）。</summary>
    public string PreAuthCode { get; }

    /// <summary>邀请链接有效期（秒）。</summary>
    public int ExpiresIn { get; }

    /// <summary>过期绝对时间（由 ExpiresIn 与生成时刻推导，便于宿主缓存判断）。</summary>
    public DateTimeOffset ExpireAt { get; }
}