// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡规则汇报对象信息（打卡规则 <c>reporterinfo</c> 字段，请求/响应共用形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinReporterInfo
{
    /// <summary>获取或设置汇报对象列表（每个汇报人用 userid 或 tagid 表示；若填写，vid 或 tagid 不可同时为空）。</summary>
    [JsonPropertyName("reporters")]
    public List<CheckinReporter>? Reporters { get; set; }

    /// <summary>获取或设置汇报对象更新时间（Unix 时间戳；仅获取企业所有打卡规则响应返回）。</summary>
    [JsonPropertyName("updatetime")]
    public long? Updatetime { get; set; }
}
