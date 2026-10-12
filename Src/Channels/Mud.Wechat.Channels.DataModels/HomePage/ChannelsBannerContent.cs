// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>精选展示位内容（官方 <c>banner.banner[].banner</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerContent
{
    /// <summary>获取或设置图片 url（官方 <c>img_url</c>；提交时必填。须用「上传图片」接口 resp_type=1 上传，不接受其他任何格式的图片 url）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }

    /// <summary>获取或设置展示位标题（官方 <c>title</c>；提交时必填。scale=1 时最多 12 字，scale=2 时最多 40 字）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置描述（官方 <c>description</c>；scale=2 时可设，最多 100 字；scale=1 时不可设置）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
