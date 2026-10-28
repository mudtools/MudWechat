// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Kf;

/// <summary>
/// 上传多媒体资源（<c>cosupload</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：请求体为 <c>multipart/form-data</c>（msg_type / open_id / file 均为表单字段，SDK 走
/// <c>IFormContent</c> 声明式上传通路）；文件大小均不超过 30MB；媒体文件在微信服务器上保存 14 天。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class ChannelsKfCosUploadResponse : ChannelsResponse
{
    /// <summary>
    /// 获取或设置多媒体资源地址（官方 <c>cos_url</c>，形如
    /// channels.weixin.qq.com/shop/commkf/downloadmedia?encrypted_param=…，供发送消息接口引用）。
    /// </summary>
    [JsonPropertyName("cos_url")]
    public string? CosUrl { get; set; }
}
