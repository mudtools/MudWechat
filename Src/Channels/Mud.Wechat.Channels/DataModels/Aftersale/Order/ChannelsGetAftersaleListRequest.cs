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

/// <summary>获取售后单列表（<c>aftersale/getaftersalelist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGetAftersaleListRequest
{
    /// <summary>获取或设置售后单创建启始时间（官方 <c>begin_create_time</c>，与 end_create_time 成对，必选其一）。</summary>
    [JsonPropertyName("begin_create_time")]
    public long? BeginCreateTime { get; set; }

    /// <summary>获取或设置售后单创建结束时间（官方 <c>end_create_time</c>，减去 begin_create_time 不得大于 24 小时）。</summary>
    [JsonPropertyName("end_create_time")]
    public long? EndCreateTime { get; set; }

    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>，从第二页开始传，来源于上一页的返回值）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置售后单更新起始时间（官方 <c>begin_update_time</c>，与 end_update_time 成对，必选其一）。</summary>
    [JsonPropertyName("begin_update_time")]
    public long? BeginUpdateTime { get; set; }

    /// <summary>获取或设置售后单更新结束时间（官方 <c>end_update_time</c>，减去 begin_update_time 不得大于 24 小时）。</summary>
    [JsonPropertyName("end_update_time")]
    public long? EndUpdateTime { get; set; }
}
