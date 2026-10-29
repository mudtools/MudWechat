// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>获取视频号账号信息（<c>get_finder_attr_by_appid</c>）响应。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）；权限集 id：160。
/// <para><b>官方文档矛盾照录</b>：参数表把 <c>finder_attr</c> 标注为 objarray（数组），官方示例为单对象——按参数表（列表）建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetFinderAttrResponse : ChannelsResponse
{
    /// <summary>获取或设置视频号信息（官方 <c>finder_attr</c>，参数表标注为 objarray）。</summary>
    [JsonPropertyName("finder_attr")]
    public List<ChannelsFinderAttr>? FinderAttr { get; set; }
}
