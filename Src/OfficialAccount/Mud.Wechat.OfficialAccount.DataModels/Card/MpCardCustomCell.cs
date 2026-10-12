// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>自定义会员服务入口（官方 <c>custom_cell1..3</c>）—— 建卡与修改同表，共用一型。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCustomCell
{
    /// <summary>获取或设置入口名称（官方 <c>name</c>，5 字符以内）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置入口右侧提示语（官方 <c>tips</c>，5 字符以内）。</summary>
    [JsonPropertyName("tips")]
    public string? Tips { get; set; }

    /// <summary>获取或设置入口跳转链接（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
