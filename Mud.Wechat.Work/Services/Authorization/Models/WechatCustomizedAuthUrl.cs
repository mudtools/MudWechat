// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Services.Authorization.Models;

/// <summary>代开发带参授权链接（官方二维码 URL）。</summary>
/// <remarks>纯进程内流转模型，不进 DataModels 的域 JsonContext（Generated/ 只覆盖官方传输 DTO）。</remarks>
public sealed class WechatCustomizedAuthUrl
{
    /// <summary>创建代开发带参授权链接。</summary>
    /// <param name="qrcodeUrl">二维码链接。</param>
    /// <param name="expiresIn">有效期（秒）。</param>
    /// <param name="expireAt">过期绝对时间。</param>
    public WechatCustomizedAuthUrl(string qrcodeUrl, int expiresIn, DateTimeOffset expireAt)
    {
        QrcodeUrl = qrcodeUrl ?? string.Empty;
        ExpiresIn = expiresIn;
        ExpireAt = expireAt;
    }

    /// <summary>二维码链接（宿主自行渲染为二维码图片）。</summary>
    public string QrcodeUrl { get; }

    /// <summary>二维码有效期（秒；官方默认 864000 = 10 天）。</summary>
    public int ExpiresIn { get; }

    /// <summary>过期绝对时间。</summary>
    public DateTimeOffset ExpireAt { get; }
}