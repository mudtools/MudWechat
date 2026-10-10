// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 添加接待人员请求体（<c>/cgi-bin/kf/servicer/add</c>）。
/// <para>
/// <see cref="UserIdList"/> 与 <see cref="DepartmentIdList"/> 至少填其中一个；
/// 每个客服账号最多可添加 2000 个接待人员、20 个接待部门。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class AddKfServicerRequest
{
    /// <summary>
    /// 获取或设置客服账号 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置接待人员 userid 列表（可填 0~100 个，超过 100 个需分批调用）。
    /// <para>
    /// 第三方 / 代开发应用填密文 userid（即 open_userid）。
    /// </para>
    /// </summary>
    [JsonPropertyName("userid_list")]
    public List<string>? UserIdList { get; set; }

    /// <summary>
    /// 获取或设置接待人员部门 id 列表（可填 0~20 个）。
    /// </summary>
    [JsonPropertyName("department_id_list")]
    public List<int>? DepartmentIdList { get; set; }
}
