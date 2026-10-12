// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>
/// 查询精选展示位（<c>getbanner</c>）响应。
/// </summary>
/// <remarks>官方契约：申请对象仅返回最近一次<b>流程中</b>的申请；不返回已生效或已撤销的申请。</remarks>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsGetBannerResponse : ChannelsResponse
{
    /// <summary>获取或设置当前生效的展示位（官方 <c>banner</c>）。</summary>
    [JsonPropertyName("banner")]
    public ChannelsBannerModule? Banner { get; set; }

    /// <summary>获取或设置最近一次流程中的申请（官方 <c>apply</c>；不返回已生效或已撤销的申请）。</summary>
    [JsonPropertyName("apply")]
    public ChannelsBannerApply? Apply { get; set; }
}
