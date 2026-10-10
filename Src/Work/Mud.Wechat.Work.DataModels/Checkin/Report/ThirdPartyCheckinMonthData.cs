// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡月报数据（获取打卡月报数据响应 <c>datas</c> 元素，第三方应用文档口径的旧字段结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinMonthData
{
    /// <summary>获取或设置统计基本信息（官方字段名为 baseinfo，区别于自建/代开发文档口径的 base_info，照抄）。</summary>
    [JsonPropertyName("baseinfo")]
    public ThirdPartyCheckinMonthBaseInfo? Baseinfo { get; set; }
}
