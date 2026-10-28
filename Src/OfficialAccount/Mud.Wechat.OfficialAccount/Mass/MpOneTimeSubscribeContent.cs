// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>一次性订阅消息内容（官方 <c>data.content</c>；value + color 两字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpOneTimeSubscribeContent
{
    /// <summary>获取或设置消息文本（官方 <c>value</c>，<b>200 字内</b>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>获取或设置字体颜色（官方 <c>color</c>，如 #FF0000；本端点官方字段表<b>有</b> color——与模板消息 send 页不同，照各自页面原文）。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
}
