// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 高级功能账号批量任务执行结果（<c>/cgi-bin/security/vip/batch_add_job_result</c>、
/// <c>/cgi-bin/security/vip/batch_del_job_result</c> 响应 job_result）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class VipJobResult
{
    /// <summary>
    /// 获取或设置执行成功的用户列表（分配任务含此前已分配过的用户）。
    /// </summary>
    [JsonPropertyName("succ_userid_list")]
    public List<string>? SuccUseridList { get; set; }

    /// <summary>
    /// 获取或设置执行失败的用户列表。
    /// </summary>
    [JsonPropertyName("fail_userid_list")]
    public List<string>? FailUseridList { get; set; }
}
