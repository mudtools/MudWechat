// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 品牌族（品牌资质事件通知）。
// 事件键：channels_ec_brand。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>品牌资质事件主体（官方 <c>BrandEvent</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsBrandEventInfo
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，number(uint64)；官方示例为字符串）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>
    /// 获取或设置品牌资质状态（官方 <c>status</c>，字符串枚举）：1 新增品牌；2 更新品牌；3 撤回品牌审核；
    /// 4 审核成功；5 审核失败；6 删除品牌；7 品牌资质被系统撤销；8 品牌资质过期。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置审核原因，非必填字段（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}
