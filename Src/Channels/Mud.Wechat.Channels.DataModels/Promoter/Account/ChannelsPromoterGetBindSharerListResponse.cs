// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取机构绑定的推客信息（<c>promoter/get_bind_sharer_list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetBindSharerListResponse : ChannelsResponse
{
    /// <summary>获取或设置推客的绑定参数列表（官方 <c>sharer_info_list</c>）。</summary>
    [JsonPropertyName("sharer_info_list")]
    public List<ChannelsPromoterBindSharerInfo>? SharerInfoList { get; set; }

    /// <summary>获取或设置下一页翻页参数（官方 <c>next_key</c>）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
