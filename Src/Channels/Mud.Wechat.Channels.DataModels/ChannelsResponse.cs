// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）统一响应基底。
/// </summary>
/// <remarks>
/// <para>
/// 实现公用层判错契约 <see cref="IWechatApiResponse"/>：<c>ErrorCode</c> 缺省 <c>0</c> ⇒
/// <c>IsSuccess = true</c>，天然覆盖官方「成功响应体不含 <c>errcode</c>」的形态
/// （<c>getAccessToken</c> / <c>getStableAccessToken</c>）。
/// </para>
/// <para>
/// <b>基底 DTO 为何留在产品线而不下沉公用层</b>：各产品线响应形态不一致（企微恒带 <c>errcode</c>；
/// 公众号/小店 token 签发成功时无 <c>errcode</c>；微信支付 APIv3 无 <c>errcode</c>），
/// 下沉的只能是「判错面」这一非序列化契约。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class ChannelsResponse : IWechatApiResponse
{
    /// <summary>
    /// 获取或设置微信小店 API 返回的错误码（0 表示成功；成功响应可能整体缺省本字段）。
    /// </summary>
    [JsonPropertyName("errcode")]
    public virtual int ErrorCode { get; set; }

    /// <summary>
    /// 获取或设置微信小店 API 返回的错误信息。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public virtual string? ErrorMessage { get; set; }

    /// <summary>
    /// 获取一个值，该值表示本次调用是否成功（<see cref="ErrorCode"/> == 0）。
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => ErrorCode == 0;
}