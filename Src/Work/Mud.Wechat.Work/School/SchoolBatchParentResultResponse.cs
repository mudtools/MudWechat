// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 批量创建/更新/删除家长响应体（<c>/cgi-bin/school/user/batch_create_parent</c>、
/// <c>batch_update_parent</c>、<c>batch_delete_parent</c>，家校沟通）。
/// <para>
/// 官方形态：部分失败时顶层 <c>errcode</c> 非 0 并附 <see cref="ResultList"/> 逐条返回失败明细
/// （家长条目键为 <c>parent_userid</c>，区别于学生条目的 <c>student_userid</c>）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SchoolBatchParentResultResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置失败/处理明细的家长列表。
    /// </summary>
    [JsonPropertyName("result_list")]
    public List<SchoolParentBatchResultItem>? ResultList { get; set; }
}
