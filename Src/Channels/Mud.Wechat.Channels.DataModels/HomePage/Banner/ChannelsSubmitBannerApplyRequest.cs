// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>
/// 提交精选展示位申请（<c>submitbannerapply</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：请求体为嵌套形态 <c>{"banner":{…}}</c>（官方参数表与请求示例原文，照抄建模）；
/// 展示位最多 5 个，小图模式（scale=1）最少 3 个；模块标题最多 32 字。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsSubmitBannerApplyRequest
{
    /// <summary>获取或设置精选展示位模块（官方 <c>banner</c>，必填）。</summary>
    [JsonPropertyName("banner")]
    public ChannelsBannerModule Banner { get; set; } = new();
}
