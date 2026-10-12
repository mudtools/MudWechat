// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>解绑退货 ID 请求体（<c>POST /cgi-bin/express/delivery/return/unbind</c>，退货组件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-sale-return/api_unbindreturn"/>。</para>
/// <para>商家同意退货后与用户达成协商「无需退货」时解除绑定；仅当用户自主填写运单号（自行寄回）时才支持解绑。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUnbindReturnRequest
{
    /// <summary>退货 ID（<c>return_id</c>，必填）。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }
}
