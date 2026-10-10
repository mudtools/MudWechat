// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 设置家校通讯录自动同步模式请求体（<c>/cgi-bin/school/set_arch_sync_mode</c>，家校沟通）。
/// <para>
/// 官方业务限制：企业和第三方可通过此接口修改家校通讯录与班级标签之间的自动同步模式，
/// <b>一旦设置禁止自动同步，将无法再次开启</b>（不可逆操作）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolSetArchSyncModeRequest
{
    /// <summary>
    /// 获取或设置家校通讯录同步模式（官方必填）：
    /// 1-禁止将标签同步至家校通讯录，2-禁止将家校通讯录同步至标签，
    /// 3-禁止家校通讯录和标签相互同步。
    /// </summary>
    [JsonPropertyName("arch_sync_mode")]
    public int? ArchSyncMode { get; set; }
}
