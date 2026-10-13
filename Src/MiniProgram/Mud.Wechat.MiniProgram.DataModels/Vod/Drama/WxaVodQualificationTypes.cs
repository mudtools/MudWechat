// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 剧目资质取值（<c>qualification_type</c>）：决定后续哪一组材料必填。
/// </summary>
/// <remarks>
/// <para><b>必填面随本值切换（官方原文）</b>：<c>1</c> ⇒ <c>registration_number</c> 与 <c>qualification_certificate_material_id</c> 必填；<c>2</c> ⇒ <c>cost_of_production</c> 与 <c>cost_commitment_letter_material_id</c> 必填（成本填 1-99 的整数，非整数截断取整）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
public static class WxaVodQualificationTypes
{
    /// <summary>取得《网络剧片发行许可证》或重点节目备案号（<c>1</c>）。</summary>
    public const int Licensed = 1;

    /// <summary>未取得许可证且制作成本低于限额（<c>2</c>，2026-01-14 起限额为 100 万元）。</summary>
    public const int UnderCostLimit = 2;
}
