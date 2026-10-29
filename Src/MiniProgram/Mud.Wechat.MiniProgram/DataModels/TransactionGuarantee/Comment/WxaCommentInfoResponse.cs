// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>查询评价详情应答（<c>GET /wxaapi/comment/commentinfo/get</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/comment/api_getcommentinfo.html"/>。
/// </para>
/// <para>
/// <b>官方返回体参数表不含 <c>errcode</c>/<c>errmsg</c></b>（同域列表页却含）⇒ 错误信封走
/// <see cref="WxaResponse"/> 基类并按「成功响应可能整体缺省 <c>errcode</c>」的既定口径处理（缺省即成功）。
/// </para>
/// <para><b>条件字段</b>：<see cref="ProcessInfo"/> 官方原文「只有差评才会有」；<see cref="OldComment"/>
/// 官方原文「只有改评的新评价才有」⇒ 两者均可为 <c>null</c>，SDK 不做差评判定。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentInfoResponse : WxaResponse
{
    /// <summary>评价的信息（<c>info</c>），见 <see cref="WxaCommentInfo"/>。</summary>
    [JsonPropertyName("info")]
    public WxaCommentInfo? Info { get; set; }

    /// <summary>进度条的信息（<c>processInfo</c>，官方原文「只有差评才会有」），见 <see cref="WxaCommentProcessInfo"/>。</summary>
    [JsonPropertyName("processInfo")]
    public WxaCommentProcessInfo? ProcessInfo { get; set; }

    /// <summary>旧评价的信息（<c>oldComment</c>，官方原文「只有改评的新评价才有」），见 <see cref="WxaCommentOld"/>。</summary>
    [JsonPropertyName("oldComment")]
    public WxaCommentOld? OldComment { get; set; }
}
