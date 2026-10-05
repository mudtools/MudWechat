// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 高级功能账号任务执行结果对象（查询分配/取消高级功能账号结果响应 <c>job_result</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class PremiumAccountJobResult
{
    /// <summary>获取或设置操作成功的 userid 列表（分配场景下包括已经是高级功能账号的 userid）。</summary>
    [JsonPropertyName("succ_userid_list")]
    public List<string>? SuccUseridList { get; set; }

    /// <summary>获取或设置操作失败的 userid 列表。</summary>
    [JsonPropertyName("fail_userid_list")]
    public List<string>? FailUseridList { get; set; }
}
