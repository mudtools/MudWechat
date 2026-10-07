// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Hr;

/// <summary>
/// 获取员工花名册信息请求体（<c>/cgi-bin/hr/get_staff_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Hr")]
public class GetHrStaffInfoRequest
{
    /// <summary>获取或设置需要获取花名册信息的员工 userid（官方必填；该员工须在调用应用的可见范围内）。</summary>
    [JsonPropertyName("userid")]
    public string Userid { get; set; } = string.Empty;

    /// <summary>获取或设置是否获取全部字段（官方 get_all）：true - 拉取全部字段信息，忽略 fieldids 参数。</summary>
    [JsonPropertyName("get_all")]
    public bool? GetAll { get; set; }

    /// <summary>获取或设置需要获取的字段列表（官方 fieldids，get_all 为 false 或不填时生效）。</summary>
    [JsonPropertyName("fieldids")]
    public List<HrFieldQueryItem>? Fieldids { get; set; }
}
