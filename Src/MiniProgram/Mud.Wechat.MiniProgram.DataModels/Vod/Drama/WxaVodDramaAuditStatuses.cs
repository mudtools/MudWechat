// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧目审核状态取值（官方 <c>audit_detail.status</c> 说明列原文）。
/// </summary>
/// <remarks>
/// <para><b>与媒资审核状态分立的理由</b>：<c>2</c> 在剧目侧是「最终失败」、在媒资侧是「审核驳回」⇒ 见 <see cref="WxaVodMediaAuditStatuses"/>（守卫 DV5 锁定两侧值集）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdrama.html"/></para>
/// </remarks>
public static class WxaVodDramaAuditStatuses
{
    /// <summary>无效值（<c>0</c>）。</summary>
    public const int Invalid = 0;

    /// <summary>审核中（<c>1</c>）。</summary>
    public const int Auditing = 1;

    /// <summary>最终失败（<c>2</c>，剧目侧语义）。</summary>
    public const int FinalFailed = 2;

    /// <summary>审核通过（<c>3</c>）。</summary>
    public const int Approved = 3;

    /// <summary>驳回重填（<c>4</c>）。</summary>
    public const int RejectedRefill = 4;
}
