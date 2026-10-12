// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>通用印刷体识别条目（官方 <c>items[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrTextItem
{
    /// <summary>获取或设置位置信息（官方 <c>pos</c>）。</summary>
    [JsonPropertyName("pos")]
    public MpOcrRect? Position { get; set; }

    /// <summary>获取或设置识别文本（官方返回示例字段 <c>text</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
