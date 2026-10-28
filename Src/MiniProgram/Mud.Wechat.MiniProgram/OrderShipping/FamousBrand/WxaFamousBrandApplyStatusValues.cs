// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 品牌申请状态（<c>progress.status</c> 与 <c>application.status</c> 同一取值表）。
/// </summary>
/// <remarks>
/// <para><b>官方枚举表只有 1/2/3，而返回示例两处均给 <c>0</c></b>（页面自相矛盾）⇒
/// 本表<b>不收录 0</b>，宿主判定须按「非 1/2/3 即视为未提交或官方未公布态」处理，
/// 不得把 0 解释成「无申请」这类业务语义（官方正文无此口径）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getfamousbrandapplystatus.html"/></para>
/// </remarks>
public static class WxaFamousBrandApplyStatusValues
{
    /// <summary>平台审核中（<c>1</c>）。</summary>
    public const int Auditing = 1;

    /// <summary>审核驳回（<c>2</c>），此时 <c>application.audit_info.audit_reason</c> 才有值。</summary>
    public const int Rejected = 2;

    /// <summary>审核通过（<c>3</c>）。</summary>
    public const int Approved = 3;
}
