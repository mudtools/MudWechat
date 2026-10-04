// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 批量创建家长请求体（<c>/cgi-bin/school/user/batch_create_parent</c>，家校沟通）。
/// <para>
/// 官方业务限制：<see cref="Parents"/> 每次最多 100 个家长；
/// 每个家长的孩子列表最多 10 个；
/// 参数字段超过长度限制时整个请求会被拦掉。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolBatchCreateParentRequest
{
    /// <summary>
    /// 获取或设置家长列表（官方必填，每次最多 100 个家长）。
    /// </summary>
    [JsonPropertyName("parents")]
    public List<SchoolCreateParentRequest>? Parents { get; set; }
}
