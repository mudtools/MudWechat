// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>
/// 罗盘人群数据查询请求体（<c>getshopsaleprofiledata</c> / <c>getfindersaleprofiledata</c>）。
/// </summary>
/// <remarks>共用裁决：两端点请求体字段集完全一致（<c>ds</c> + <c>type</c>）⇒ 共用本 DTO；用户类型枚举两侧不同，分建常量类。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassSaleProfileRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置用户类型（官方 <c>type</c>，必填；商家版见 <see cref="ChannelsCompassShopSaleProfileUserTypes"/>、达人版见 <see cref="ChannelsCompassFinderSaleProfileUserTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}
