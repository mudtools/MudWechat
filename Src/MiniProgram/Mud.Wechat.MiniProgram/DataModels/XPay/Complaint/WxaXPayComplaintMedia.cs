// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 投诉资料凭证（官方子表：<c>media_type</c> + <c>media_url</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>media_url</c> 官方类型列是 <c>array</c> 而字段名是单数</b> ⇒ 线格式按类型落 <c>List&lt;string&gt;?</c>（示例优先于命名的判据，守卫 XP5 锁定）。</para>
/// <para><b>本类型三处共用</b>：列表侧、详情侧、协商历史侧的凭证子表字段名集相同 ⇒ 取值字典只落在 <see cref="WxaXPayComplaintMediaTypes"/>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_list.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayComplaintMedia
{
    /// <summary>media_type（官方类型 <c>string</c>）：媒体文件对应的业务类型，USER_COMPLAINT_IMAGE: 用户提交投诉时上传的图片凭证；OPERATION_IMAGE: 用户、商户、微信支付客服在协商解决投诉时，上传的图片凭证</summary>
    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    /// <summary>media_url（官方类型 <c>array</c>）：每一项的内容为string，媒体文件请求url</summary>
    [JsonPropertyName("media_url")]
    public List<string>? MediaUrl { get; set; }
}
