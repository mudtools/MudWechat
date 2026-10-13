// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧目状态取值（<c>drama_info.status</c>，与审核状态是两个维度）。
/// </summary>
/// <remarks>
/// <para><b>与 <see cref="WxaVodDramaAuditStatuses"/> 不可互换</b>：本枚举描述「当前可否播」，审核状态描述「最近一次提审的结论」；官方两侧值集与语义均不同（此处 2 = 审核失败、3 = 平台下架）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdrama.html"/></para>
/// </remarks>
public static class WxaVodDramaStatuses
{
    /// <summary>正常可播（<c>0</c>）。</summary>
    public const int Normal = 0;

    /// <summary>审核中（<c>1</c>）。</summary>
    public const int Auditing = 1;

    /// <summary>审核失败（<c>2</c>）。</summary>
    public const int AuditFailed = 2;

    /// <summary>平台下架（<c>3</c>）。</summary>
    public const int Offline = 3;
}
