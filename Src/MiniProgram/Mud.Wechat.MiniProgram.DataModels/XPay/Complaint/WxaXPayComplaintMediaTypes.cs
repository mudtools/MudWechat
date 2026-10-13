// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 投诉凭证媒体类型取值（官方 <c>Res.history.complaint_media_list.media_type</c> 枚举表）。
/// </summary>
/// <remarks>
/// <para><b>官方只在协商历史侧给该枚举表</b>（列表/详情的同名字段无枚举表）⇒ 两处共用同一字典，来源锚点仍取协商历史页（守卫 XP5 锁定该唯一出处）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_negotiation_history.html"/></para>
/// </remarks>
public static class WxaXPayComplaintMediaTypes
{
    /// <summary>用户提交投诉时上传的图片凭证（<c>USER_COMPLAINT_IMAGE</c>）</summary>
    public const string UserComplaintImage = "USER_COMPLAINT_IMAGE";

    /// <summary>用户、商户、微信支付客服在协商解决投诉时，上传的图片凭证（<c>OPERATION_IMAGE</c>）</summary>
    public const string OperationImage = "OPERATION_IMAGE";
}
