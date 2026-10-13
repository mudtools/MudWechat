// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 审核加急取值（<c>expedited</c>）。
/// </summary>
/// <remarks>
/// <para><b>每天 5 次加急机会；只在首次提审时生效</b>（官方原文）：重新提审沿用首次提审时的属性、不再扣次数。是否真为加急单以 <c>drama_info_list.expedited</c> 判断。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
public static class WxaVodExpedited
{
    /// <summary>不加急（<c>0</c>，或不填）。</summary>
    public const int Normal = 0;

    /// <summary>审核加急（<c>1</c>）。</summary>
    public const int Urgent = 1;
}
