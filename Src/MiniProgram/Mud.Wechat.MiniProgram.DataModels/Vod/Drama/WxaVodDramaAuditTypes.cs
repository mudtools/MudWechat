// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧目审核类型取值（<c>audit_detail.audit_type</c> 与 <c>getdramalatestauditinfo</c> 的入参 <c>audit_type</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdramalatestauditinfo.html"/></para>
/// </remarks>
public static class WxaVodDramaAuditTypes
{
    /// <summary>首次提审（<c>0</c>）。</summary>
    public const int FirstAudit = 0;

    /// <summary>再次提审（<c>1</c>）。</summary>
    public const int ReAudit = 1;

    /// <summary>替换剧集提审（<c>2</c>）。</summary>
    public const int ReplaceMediaAudit = 2;

    /// <summary>修改剧目基本信息审核（<c>3</c>）。</summary>
    public const int ModifyBasicInfoAudit = 3;
}
