// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「提交隐私接口审核」请求（<c>apply_privacy_interface</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-api-management/applyPrivacyInterface.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpPrivacyApplyRequest
{
    /// <summary>隐私接口名称（官方 <c>api_name</c>）。</summary>
    [JsonPropertyName("api_name")]
    public string? ApiName { get; set; }

    /// <summary>接口申请理由/使用说明（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>使用场景截图链接列表（官方 <c>url_list</c>，array）。</summary>
    [JsonPropertyName("url_list")]
    public List<string>? UrlList { get; set; }

    /// <summary>图片材料列表（官方 <c>pic_list</c>，array）。</summary>
    [JsonPropertyName("pic_list")]
    public List<string>? PicList { get; set; }

    /// <summary>视频材料列表（官方 <c>video_list</c>，array）。</summary>
    [JsonPropertyName("video_list")]
    public List<string>? VideoList { get; set; }
}
