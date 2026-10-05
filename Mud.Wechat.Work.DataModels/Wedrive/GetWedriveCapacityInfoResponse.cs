// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 获取盘容量信息响应体（<c>/cgi-bin/wedrive/mng_capacity</c>；官方请求包体为空对象，无请求 DTO）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class GetWedriveCapacityInfoResponse : WechatWorkResponse
{
    /// <summary>获取或设置全员容量总数（单位 B）。</summary>
    [JsonPropertyName("total_capacity_for_all")]
    public ulong? TotalCapacityForAll { get; set; }

    /// <summary>获取或设置专业容量总数（单位 B）。</summary>
    [JsonPropertyName("total_capacity_for_vip")]
    public ulong? TotalCapacityForVip { get; set; }
}
