// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.DataCube;

/// <summary>
/// 发表内容每日阅读数据条目（官方 <c>getarticleread</c> 的 list 元素；新图文族 4 端点顶层均为
/// list + is_delay 形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataCube")]
public class MpDataArticleReadItem
{
    /// <summary>获取或设置统计日期（官方 <c>ref_date</c>）。</summary>
    [JsonPropertyName("ref_date")]
    public string? RefDate { get; set; }

    /// <summary>获取或设置图文消息标识（官方 <c>msgid</c>，由 msg_data_id 与 index 组成，如 "12003_3"）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>获取或设置每篇文章的详细数据（官方 <c>detail</c>）。</summary>
    [JsonPropertyName("detail")]
    public MpDataReadDetail? Detail { get; set; }
}
