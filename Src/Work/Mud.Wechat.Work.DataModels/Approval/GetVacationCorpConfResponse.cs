// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 获取企业假期管理配置响应体（<c>/cgi-bin/oa/vacation/getcorpconf</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class GetVacationCorpConfResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置假期列表，包括各个假期的id、名称、请假单位、时长计算方式、发放规则等。
    /// </summary>
    [JsonPropertyName("lists")]
    public List<VacationCorpConfItem>? Lists { get; set; }
}
