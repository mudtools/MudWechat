// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「售后单主文档」DTO（aftersale/getaftersaleorder、aftersale/getaftersalelist、
// aftersale/reason/get、aftersale/rejectreason/get）+ 售后单状态 / 原因 / 类型常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>售后原因（官方 <c>aftersale/reason/get</c> 的 <c>reason_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleReason
{
    /// <summary>获取或设置售后原因枚举（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置售后原因说明（官方 <c>reason_text</c>）。</summary>
    [JsonPropertyName("reason_text")]
    public string? ReasonText { get; set; }
}
