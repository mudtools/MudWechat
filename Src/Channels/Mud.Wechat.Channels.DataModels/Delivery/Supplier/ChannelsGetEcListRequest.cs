// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 获取关联的商家列表（<c>get_ec_list</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约（2026-10-17 逐页核验）：<b>POST</b>；<c>offset</c> / <c>limit</c> 必填；
/// <c>is_filter_type = true</c> 时 <c>type</c> 必填（官方条件必填，SDK 不做本地拦截，越界由错误码
/// <c>268580000</c> 表达）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetEcListRequest
{
    /// <summary>获取或设置分页游标起始位置（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public long Offset { get; set; }

    /// <summary>获取或设置分页数量（官方 <c>limit</c>，必填）。</summary>
    [JsonPropertyName("limit")]
    public long Limit { get; set; }

    /// <summary>获取或设置是否开启状态筛选（官方 <c>is_filter_type</c>，默认 <c>false</c>；为 <c>true</c> 时 <c>type</c> 必填）。</summary>
    [JsonPropertyName("is_filter_type")]
    public bool? IsFilterType { get; set; }

    /// <summary>
    /// 获取或设置筛选类型（官方 <c>type</c>；枚举 1 已关联 / 2 待审核 / 3 已取消·历史记录，
    /// 当且仅当 <c>is_filter_type = true</c> 时生效）。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }
}
