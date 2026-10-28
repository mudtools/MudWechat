// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Resource;

/// <summary>
/// 上传图片（<c>img_upload</c>）图片 URL 模式（<c>upload_type=1</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + JSON 请求体（<c>img_url</c>）；<c>upload_type</c>/<c>resp_type</c> 为 URL 参数
/// （经主包 <c>IChannelsResourceService.UploadImageAsync</c> 的 <c>[Query]</c> 参数传入，不在本 DTO 内；
/// DataModels 为叶层不反向引用主包，接口名以代码体兜底）。
/// 二进制流模式（<c>upload_type=0</c>）官方要求 formdata 文件（<c>media</c>），由
/// <c>[MultipartForm]</c> 通道承载，不使用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Resource")]
public class ChannelsResourceImgUploadRequest
{
    /// <summary>获取或设置图片 URL（官方 <c>img_url</c>，<c>upload_type=1</c> 时必填；不支持 301/302 跳转，格式限 bmp/jpg(jpeg)/png/webp）。</summary>
    [JsonPropertyName("img_url")]
    public string ImgUrl { get; set; } = string.Empty;
}
