// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 主持人列表对象（创建/修改预约会议请求与获取会议详情响应 <c>settings.hosts</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingHosts
{
    /// <summary>
    /// 获取或设置企业成员 userid 列表（主持人）。
    /// <para>最多 10 个；包含创建者 userid 会被自动过滤；userid 不合法直接报错；
    /// 仅购买了会议高级功能的企业可指定主持人。</para>
    /// </summary>
    [JsonPropertyName("userid")]
    public List<string>? Userid { get; set; }
}
