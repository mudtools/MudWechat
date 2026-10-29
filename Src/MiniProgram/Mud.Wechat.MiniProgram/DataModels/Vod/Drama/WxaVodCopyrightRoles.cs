// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 提审主体身份取值（<c>copyright.copyright_role</c>）。
/// </summary>
/// <remarks>
/// <para><b>非制作方须补授权材料</b>（官方原文）：<c>2</c> 时须上传版权采买/播出授权证明材料。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
public static class WxaVodCopyrightRoles
{
    /// <summary>剧目制作方（<c>1</c>）。</summary>
    public const int Producer = 1;

    /// <summary>授权播出方或版权方（<c>2</c>）。</summary>
    public const int Licensee = 2;
}
